using UnityEngine;
using System.Collections;
using MonoMedia;
using System.IO;
using System.Collections.Generic;

namespace MediaKit.Processor
{
	
	class AudioDecoder
	{
		readonly MediaKitProcessor.OGVControl control_;
		readonly OGGStream oggStream_;
		
		public MediaKitProcessor.OGVControl Control { get { return control_; } }

		const int PCM_BUF_LENGHT = 44100*8; // 4 seconds buffer
		const int PCM_BUF_OVERLAP = PCM_BUF_LENGHT/2;
		const int PCM_BUF_SWITCH = PCM_BUF_OVERLAP/2;

		float[] Pcm;
		long PcmSampleOffset;
		long PcmSampleCount;
		long PcmRate;
		int PcmChannels;

		int curPcmOffset;
		float[] curPcm = new float[PCM_BUF_LENGHT];
		float[] rdyPcm = new float[PCM_BUF_LENGHT];
		float[] nxtPcm = new float[PCM_BUF_LENGHT];

		Xiph.ogg_sync_state oy; /* sync and verify incoming physical bitstream */
		Xiph.ogg_stream_state os; /* take physical pages, weld into a logical tream of packets */
		Xiph.ogg_page og; /* one Ogg bitstream page. Vorbis packets are inside */
		Xiph.ogg_packet op; /* one raw packet of data for decode */
		
		Xiph.vorbis_info vi; /* struct that stores all the static vorbis bitstream settings */
		Xiph.vorbis_comment vc; /* struct that stores all the bitstream user comments */
		Xiph.vorbis_dsp_state vd; /* central working state for the packet->PCM decoder */
		Xiph.vorbis_block vb; /* local working space for packet->PCM decode */

		Stream instream;
		long instream_pos = 0;

		bool vorbis_p;
		bool need_more_data;
		bool need_next_pcm;
		bool stateflag;
		bool comments_header;
		bool eos;
		bool last_buf;

		bool loaded = false;

		uint playSession = 0;

		int lastReadIdx = -1;
		int[] lastReadSamples = new int[10];
		long[] lastReadTime = new long[10];
		double readspeed = 44.1; // samples per millisecond

		float frame_in_sec = 0;
		float last_frame_at = 0;
		string error = null;
		string info = null;
		
		public AudioDecoder(OGGStream stream, MediaKitProcessor.OGVControl control)
		{
			control_ = control;
			oggStream_ = stream;
			oggStream_.RefCount ++;
		}
		
		~AudioDecoder()
		{
			oggStream_.RefCount --;
		}

		int buffer_data (Stream sin, Xiph.ogg_sync_state oy)
		{
			int bytes		= 0;
			byte[] buffer	= null;
			int offset		= 0;
			lock(sin) 
			{
				if (Xiph.ogg_sync_buffer (oy, 4096, out buffer, out offset)) 
				{
					sin.Position = instream_pos;
					bytes = sin.Read(buffer,(int)offset,4096);
					Xiph.ogg_sync_wrote (oy, bytes);
					instream_pos = sin.Position;
				}
			}
			return bytes;
		}

		/* dump the vorbis comment header */
		int dump_comments(Xiph.vorbis_info vi, Xiph.vorbis_comment vc){
			int   i;
			
			Debug.Log("Encoded by " + vc.vendor);
			if(vc.user_comments!=null)
			{
				Debug.Log("vorbis comment header:");
				for(i=0;i<vc.user_comments.Length;i++){
					Debug.Log("\t" + vc.user_comments[i]);
				}
			}

			Debug.Log("\nBitstream is " + vi.channels + " channel, " + vi.rate + "Hz\n");
			return 0;
		}	


		bool LoadAudio( )
		{

			oy = new Xiph.ogg_sync_state(); /* sync and verify incoming physical bitstream */
			os = new Xiph.ogg_stream_state(); /* take physical pages, weld into a logical tream of packets */
			og = new Xiph.ogg_page(); /* one Ogg bitstream page. Vorbis packets are inside */
			op = new Xiph.ogg_packet(); /* one raw packet of data for decode */
			
			vi = new Xiph.vorbis_info(); /* struct that stores all the static vorbis bitstream settings */
			vc = new Xiph.vorbis_comment(); /* struct that stores all the bitstream user comments */
			vd = new Xiph.vorbis_dsp_state(); /* central working state for the packet->PCM decoder */
			vb = new Xiph.vorbis_block(); /* local working space for packet->PCM decode */

			instream_pos = 0;

			eos = false;
			last_buf = false;
			need_more_data = false;
			need_next_pcm = true;
			vorbis_p = false;
			stateflag = false;
			comments_header = false;
			instream_pos = 0;
			curPcmOffset = 0;
			Pcm = null;
			PcmSampleCount = 0;
			PcmSampleOffset = 0;
			readspeed = control_.Scale + 44.1;
			lastReadIdx = -1;

			Xiph.ogg_sync_init(oy); /* Now we can read pages */
			
			
			Xiph.vorbis_info_init(vi);
			Xiph.vorbis_comment_init(vc);
			
			/*Ogg file open; parse the headers.
            Vorbis depends on some initial header packets for decoder
             setup and initialization.
            We retrieve these first before entering the main decode loop.*/
			
			/* Only interested in Vorbis streams */
			while (!stateflag)
			{
				
				int bytes = buffer_data(instream,oy);

				if (bytes == 0) break;
				
				while (Xiph.ogg_sync_pageout(oy, og) > 0)
				{
					int got_packet;
					Xiph.ogg_stream_state test = new Xiph.ogg_stream_state();
					
					/* is this a mandated initial header? If not, stop parsing */
					if (Xiph.ogg_page_bos(og) == 0)
					{
						stateflag = true;
						break;
					}
					
					Xiph.ogg_stream_init(test, Xiph.ogg_page_serialno(og));
					Xiph.ogg_stream_pagein(test, og);
					got_packet = Xiph.ogg_stream_packetpeek(test, op);
					
					/* identify the codec: try vorbis */
					if ((got_packet == 1) && !vorbis_p && Xiph.vorbis_synthesis_headerin(vi, vc, op) >= 0)
					{
						/* it is vorbis -- save this stream state */
						os = Xiph.ogg_stream_clone(test);
						//memcpy(&os,&test,sizeof(test));
						vorbis_p = true;
						/*Advance past the successfully processed header.*/
						Xiph.ogg_stream_packetout(os, null);
					}
					else
					{
						/* whatever it is, we don't care about it */
						Xiph.ogg_stream_clear(test);
					}
				}
				/* fall through to non-bos page parsing */
			}
			
			
			
			/* we're expecting more header packets. */
			while (vorbis_p && !comments_header)
			{
				int ret;
				
				/* look for further theora headers */
				while (0 != (ret = Xiph.ogg_stream_packetpeek(os, op)))
				{
					if (ret < 0) continue;
					if (Xiph.vorbis_synthesis_headerin(vi, vc, op) < 0)
					{
						error = "Error parsing Theora stream vorbis; corrupt stream?\n";
						return false;
					}
					else
					{
						/*Advance past the successfully processed header.*/
						Xiph.ogg_stream_packetout(os, null);
						comments_header = true;
					}
				}
				
				/* The header pages/packets will arrive before anything else we
                   care about, or the stream is not obeying spec */
				if (Xiph.ogg_sync_pageout(oy, og) > 0)
				{
					/* demux into the appropriate stream */
					Xiph.ogg_stream_pagein(os, og);
				}
				else
				{
					/* someone needs more data */
					int bytes = buffer_data(instream,oy);
					if (bytes == 0)
					{
						error = "End of file while searching for codec headers.\n";
						return false;
					}
				}
			}
			
			if(vorbis_p){
				/* no harm in not checking before adding more */
				int bytes = buffer_data(instream,oy);

				if (bytes == 0)
				{
					error = "End of file before finding all Vorbis headers!\n";
					return false;
				}

				PcmRate = vi.rate;
				PcmChannels = vi.channels;
				PcmSampleOffset = -(PCM_BUF_LENGHT - PCM_BUF_OVERLAP)/PcmChannels;
				PcmSampleCount = PCM_BUF_SWITCH/PcmChannels;
				curPcmOffset = PCM_BUF_SWITCH;

				control_.PcmChannels = PcmChannels;
				control_.PcmRate = PcmRate;
				control_.SamplesPerMillisecond = control_.Scale * 44.1;

				dump_comments(vi,vc);
				
				/* OK, got and parsed all three headers. Initialize the Vorbis
	               packet->PCM decoder. */
				if (Xiph.vorbis_synthesis_init(vd, vi) == 0)
				{ /* central decode state */
					Xiph.vorbis_block_init(vd, vb);          /* local state for most of the decode
	                                      so multiple block decodes can
	                                      proceed in parallel. We could init
	                                      multiple vorbis_block structures
	                                      for vd here */

				}
				else
				{
					error = "Wrong Vorbis stream!\n";
					return false;
				}
			}

			return vorbis_p;
		}
		

		void DecodePage()
		{
			need_more_data = false;

			Xiph.ogg_stream_pagein(os, og); /* can safely ignore errors at
                                               this point */
			while (true)
			{
				int result = Xiph.ogg_stream_packetout(os, op);
				
				if (result == 0){
					need_more_data = true;
					break; /* need more data */
				}
				if (result < 0)
				{ /* missing or corrupt data at this page position */
					/* no reason to complain; already complained above */
					break;
				}
				else
				{
					/* we have a packet.  Decode it */
					float[][] pcm;
					int[] pcmret;
					int samples;
					
					if (Xiph.vorbis_synthesis(vb, op) == 0) /* test for success! */
						Xiph.vorbis_synthesis_blockin(vd, vb);
					/* 
                    **pcm is a multichannel float vector.  In stereo, for
                    example, pcm[0] is left, and pcm[1] is right.  samples is
                    the size of each channel.  Convert the float values
                    (-1.<=range<=1.) to whatever PCM format and write it out */
					
					while ((samples = Xiph.vorbis_synthesis_pcmout(vd, out pcm, out pcmret)) > 0)
					{
						
						int i,j;
						int bout = samples;//(samples < convsize ? samples : convsize);

						for (i = 0; i < vi.channels; i++)
						{
							int ptroff = i;
							
							float[] mono = pcm[i];
							int monoret = pcmret[i];

							for (j = 0; j < bout; j++)
							{
								float sample = mono[monoret+j];

								if(sample > 1.0f)
									sample = 1.0f;

								if(sample < -1.0f)
									sample = -1.0f;

								int curPcmIndex = curPcmOffset+ptroff;

								if(curPcmIndex < PCM_BUF_LENGHT)
									curPcm[curPcmIndex] = sample;

								int overlapIndex = curPcmIndex - PCM_BUF_LENGHT + PCM_BUF_OVERLAP;

								if(overlapIndex >= 0 && overlapIndex < PCM_BUF_LENGHT)
									nxtPcm[overlapIndex] = sample;

								ptroff += vi.channels;
							}
						}

						PcmSampleCount += bout;

						curPcmOffset += bout * vi.channels;

						Xiph.vorbis_synthesis_read(vd, bout); /* tell libvorbis how
			                                                      many samples we
			                                                      actually consumed */
					}

					if(need_more_data == false && curPcmOffset >= PCM_BUF_LENGHT)
					{
						need_next_pcm = false;
					}

				}
			}
			if (Xiph.ogg_page_eos(og) != 0){
				need_next_pcm = false;
				need_more_data = false;
				last_buf = true;
				eos = true;
			}

		}
		
		
		public void Update()
		{
			if(control_.Initializing)
			{
				if(oggStream_.Ready)
				{
					if(instream == null)
					{
						instream = oggStream_.InMem;
						if(instream == null)
						{
							instream = oggStream_.InFile;
						}
					}
					if(loaded)
					{
						if( info != null )
						{
							Debug.Log(info);
						}
					}
					
					if( error != null )
					{
						Debug.LogError(error);
						error = null;
					}
				}
			}
			else
			{
				if(!control_.AudioAvailable)
					return;

				if(eos)
					return;

			}
		}
		
		
		public void InBackground()
		{
			if(instream == null)
				return;

			if(playSession != control_.PlaySession)
			{
				playSession = control_.PlaySession;
				Pcm = null;
				control_.Pcm = Pcm;
				loaded = false;
			}

			if(loaded == false)
			{
				control_.AudioAvailable = LoadAudio();
				loaded = true;
				return;
			}
			else
			{
				if(control_.AudioAvailable == false)
					return;
			}
			
			if(!control_.Ready)
				return;
			
			if(!control_.Playing)
				return;

			//if(eos)
			//	return;

			while(!eos && vorbis_p && need_more_data && need_next_pcm){
				
				int bytes = buffer_data(instream,oy);
				
				if(bytes > 0)
				{
					int result = Xiph.ogg_sync_pageout(oy, og);
					if (result == 0) continue; /* need more data */
					if (result < 0)
					{ /* missing or corrupt data at this page position */
						error = "Corrupt or missing data in bitstream; continuing...\n";
						break;
					}
					else
					{
						need_more_data = false;
					}
				}
				else
				{
					need_next_pcm = false;
					need_more_data = false;
					eos = true;
					break;
				}
			}

			if(!eos && vorbis_p && !need_more_data && need_next_pcm)
			{
				DecodePage();
			}

			if( !need_next_pcm )
			{
				long currentSample = (long)(control_.ElapsedTime * control_.PcmRate) + PCM_BUF_SWITCH/2;

				if(Pcm != null && curPcmOffset < PCM_BUF_LENGHT && !eos)
				{
					if( currentSample > PcmSampleOffset + ((PCM_BUF_LENGHT - PCM_BUF_OVERLAP)/PcmChannels) )
					{
						control_.SamplesPerMillisecond = control_.Scale * 44.1;
						need_next_pcm = true;
					}
				}
				
				if((Pcm == null && curPcmOffset >= PCM_BUF_LENGHT)
				   ||
				   (last_buf && (currentSample > PcmSampleOffset + ((PCM_BUF_LENGHT - PCM_BUF_SWITCH)/PcmChannels)))
				   ||
				   ((need_next_pcm == false)
					&&((currentSample > PcmSampleOffset + ((PCM_BUF_LENGHT - PCM_BUF_SWITCH)/PcmChannels)))) )
				{
					last_buf = false;
					curPcmOffset -= (PCM_BUF_LENGHT - PCM_BUF_OVERLAP);
					float[] swp = curPcm;
					curPcm = nxtPcm;
					nxtPcm = rdyPcm;
					rdyPcm = swp;

					if(curPcmOffset > PCM_BUF_LENGHT - PCM_BUF_OVERLAP)
					{
						for(int i = 0; i < (curPcmOffset - PCM_BUF_LENGHT + PCM_BUF_OVERLAP); i++)
						{
							int srcind = i + PCM_BUF_LENGHT - PCM_BUF_OVERLAP;
							if(srcind >= PCM_BUF_LENGHT)
								Debug.LogError("(i > srcind)");
							nxtPcm[i] = curPcm[srcind];
						}
					}

					PcmSampleOffset += (PCM_BUF_LENGHT - PCM_BUF_OVERLAP) / PcmChannels;
					Pcm = rdyPcm;

					lastReadIdx ++;
					long milliseconds = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
					if(lastReadIdx > 9)
					{
						for(int i=1; i< 10; i++)
						{
							lastReadSamples[i-1] = lastReadSamples[i];
							lastReadTime[i-1] = lastReadTime[i];
						}
						lastReadIdx = 9;
					}
					
					lastReadSamples[lastReadIdx] = (PCM_BUF_LENGHT - PCM_BUF_OVERLAP) / PcmChannels;
					lastReadTime[lastReadIdx] = milliseconds;
					
					long totalLastSamples = 0;
					long totalLastMilliseconds = 0;
					for(int i = 1; i <= lastReadIdx; i++)
					{
						totalLastSamples += lastReadSamples[i-1];
						totalLastMilliseconds += lastReadTime[i] - lastReadTime[i-1];
					}

					if(totalLastSamples > 0)
						readspeed = (double)totalLastSamples / (double)totalLastMilliseconds;

					//Debug.Log("readspeed: " + readspeed + " RealScale: " + control_.RealScale + " ElapsedTime: " + control_.ElapsedTime + " Pcm switch, " + curPcmOffset / 2);

					control_.Pcm = Pcm;
					control_.PcmSampleOffset = PcmSampleOffset;
					control_.PcmSampleCount = PcmSampleCount;
					control_.SamplesPerMillisecond = readspeed;
				}

			}

			
		}
	}
	
}