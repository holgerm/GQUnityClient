using UnityEngine;
using System.Collections;
using MonoMedia;
using System.IO;
using System.Collections.Generic;

namespace MediaKit.Processor
{

	class VideoDecoder
	{
		readonly MediaKitProcessor.OGVControl control_;
		readonly OGGStream oggStream_;
		
		public MediaKitProcessor.OGVControl Control { get { return control_; } }
		
		
		// OGG part
		Xiph.ogg_packet op;
		Xiph.ogg_page og;
		//Xiph.ogg_stream_state vo;
		Xiph.ogg_stream_state to;
		Xiph.ogg_sync_state oy;
		Xiph.th_info ti;
		Xiph.th_comment tc;
		Xiph.th_setup_info ts;
		Xiph.th_dec_ctx td;
//		Xiph.th_ycbcr_buffer ycbcr;
		Stream instream;
		long instream_pos = 0;
		int stateflag = 0;
		int theora_p=0;
		int theora_processing_headers=0;
		int videobuf_ready = 0;
		long videobuf_granulepos = -1;
		int rnd_frame = -1;
		int rdy_frame = -1;
		int cur_frame = -1;
		Texture2D y;
		int width;
		int height;

		Color32[] colors_y,colors_y_1,colors_y_2,colors_y_rnd;
		bool loaded = false;
		bool skip = true;
		float fps=0;
		float frame_in_sec = 0;
		float last_frame_at = 0;
		double videobuf_time = 0;
		float video_start_time = 0;
		string error = null;
		string info = null;

		const int ELAPSED_TIME_SAMPLES_MAX = 60;
		Queue<double> elapsedTimeSamples = new Queue<double>(ELAPSED_TIME_SAMPLES_MAX);
		Queue<long> elapsedTimeTimestamps = new Queue<long>(ELAPSED_TIME_SAMPLES_MAX);
		
		public VideoDecoder(OGGStream stream, MediaKitProcessor.OGVControl control)
		{
			control_ = control;
			oggStream_ = stream;
			oggStream_.RefCount ++;
			
			control_.Initializing = true;
		}
		
		~VideoDecoder()
		{
			oggStream_.RefCount --;
		}
		
		bool LoadVideo( )
		{
			op = new Xiph.ogg_packet();
			og = new Xiph.ogg_page();
			//vo = new Xiph.ogg_stream_state();
			to = new Xiph.ogg_stream_state();
			oy = new Xiph.ogg_sync_state();
			ti = new Xiph.th_info();
			tc = new Xiph.th_comment();
			ts = null;
			
			fps = 0;
			frame_in_sec = 0;
			instream_pos = 0;
			stateflag = 0;
			theora_p=0;
			theora_processing_headers=0;
			videobuf_time = 0;
			videobuf_ready = 0;
			videobuf_granulepos = -1;
			rnd_frame = -1;
			rdy_frame = -1;
			cur_frame = -1;

			elapsedTimeSamples.Clear();
			elapsedTimeTimestamps.Clear();

			/* start up Ogg stream synchronization layer */
			Xiph.ogg_sync_init(oy);
			
			/* init supporting Theora structures needed in header parsing */
			Xiph.th_comment_init(tc);
			Xiph.th_info_init(ref ti);
			
			/*Ogg file open; parse the headers.
		    Theora (like Vorbis) depends on some initial header packets for decoder
		     setup and initialization.
		    We retrieve these first before entering the main decode loop.*/
			
			/* Only interested in Theora streams */
			while(stateflag==0){
				int ret=buffer_data(instream,oy);
				if(ret==0)break;
				while(Xiph.ogg_sync_pageout(oy,og)>0){
					int got_packet;
					Xiph.ogg_stream_state test = new Xiph.ogg_stream_state();
					
					/* is this a mandated initial header? If not, stop parsing */
					if(Xiph.ogg_page_bos(og) == 0){
						/* don't leak the page; get it into the appropriate stream */
						queue_page(og);
						stateflag=1;
						break;
					}
					
					Xiph.ogg_stream_init(test,Xiph.ogg_page_serialno(og));
					Xiph.ogg_stream_pagein(test,og);
					got_packet = Xiph.ogg_stream_packetpeek(test,op);
					
					/* identify the codec: try theora */
					if((got_packet==1) && theora_p==0 && (theora_processing_headers=
					                                      Xiph.th_decode_headerin(ref ti,tc,ref ts,op))>=0)
					{
						/* it is theora -- save this stream state */
						to = Xiph.ogg_stream_clone(test);
						
						theora_p=1;
						/*Advance past the successfully processed header.*/
						if(theora_processing_headers!=0)Xiph.ogg_stream_packetout(to,null);
					}else{
						/* whatever it is, we don't care about it */
						Xiph.ogg_stream_clear(test);
					}
				}
				/* fall through to non-bos page parsing */
			}
			
			/* we're expecting more header packets. */
			while(theora_p!=0 && theora_processing_headers!=0){
				int ret;
				
				/* look for further theora headers */
				while(theora_processing_headers!=0&&(ret=Xiph.ogg_stream_packetpeek(to,op))!=0){
					if(ret<0)continue;
					theora_processing_headers=Xiph.th_decode_headerin(ref ti,tc,ref ts,op);
					if(theora_processing_headers<0){
						error = "Error parsing Theora stream headers; corrupt stream?\n";
						return false;
					}
					else if(theora_processing_headers>0){
						/*Advance past the successfully processed header.*/
						Xiph.ogg_stream_packetout(to, null);
					}
					theora_p++;
				}
				
				/*Stop now so we don't fail if there aren't enough pages in a short
               	stream.*/
				if(!(theora_p!=0 && theora_processing_headers!=0))break;
				
				/* The header pages/packets will arrive before anything else we
               	care about, or the stream is not obeying spec */
				
				if (Xiph.ogg_sync_pageout(oy, og) > 0)
				{
					queue_page(og); /* demux into the appropriate stream */
				}else{
					int ret2=buffer_data(instream,oy); /* someone needs more data */
					if(ret2==0){
						error = "End of file while searching for codec headers.\n";
						return false;
					}
				}
			}
			
			/* and now we have it all.  initialize decoders */
			if(theora_p!=0){
				dump_comments(tc);
				td=Xiph.th_decode_alloc(ti,ts);
				info = string.Format("Ogg logical stream {0} is Theora {1}x{2} {3} fps video\nEncoded frame content is {4}x{5} with {6}x{7} offset\n",
				                     to.serialno,ti.frame_width,ti.frame_height,
				                     (double)ti.fps_numerator/ti.fps_denominator,
				                     ti.pic_width,ti.pic_height,ti.pic_x,ti.pic_y);
				fps = (float)ti.fps_numerator/ti.fps_denominator;
				frame_in_sec = 1.0f/fps;
			}else{
				/* tear down the partial theora setup */
				Xiph.th_info_clear(ref ti);
				Xiph.th_comment_clear(tc);
			}
			/*Either way, we're done with the codec setup data.*/
			Xiph.th_setup_free(ts);
			
			/* open video */
			if(theora_p!=0)open_video();
			
			/*Finally the main decode loop.

	        It's one Theora packet per frame, so this is pretty straightforward if
	         we're not trying to maintain sync with other multiplexed streams.

	        The videobuf_ready flag is used to maintain the input buffer in the libogg
	         stream state.
	        If there's no output frame available at the end of the decode step, we must
	         need more input data.
	        We could simplify this by just using the return code on
	         ogg_page_packetout(), but the flag system extends easily to the case where
	         you care about more than one multiplexed stream (like with audio
	         playback).
	        In that case, just maintain a flag for each decoder you care about, and
	         pull data when any one of them stalls.

	        videobuf_time holds the presentation time of the currently buffered video
	         frame.
	        We ignore this value.*/
			
			stateflag=0; /* playback has not begun */
			/* queue any remaining pages from data we buffered but that did not
           	contain headers */
			while(Xiph.ogg_sync_pageout(oy,og)>0){
				queue_page(og);
			}

            colors_y_1 = new Color32[width * height];
            colors_y_2 = new Color32[width * height];
            colors_y = colors_y_1;
            colors_y_rnd = colors_y_2;

			/* load the first frame */
			while(theora_p!=0 && videobuf_ready==0){
				/* theora is one in, one out... */
				if (Xiph.ogg_stream_packetout(to, op) > 0)
				{
					
					if (Xiph.th_decode_packetin(td, op, ref videobuf_granulepos) >= 0)
					{
						videobuf_time = Xiph.th_granule_time(td.state, videobuf_granulepos);
						//Debug.Log(videobuf_time);
						videobuf_ready=1;

					}
					
				}else
					break;
			}
			
			loaded = true;
			
			return true;
		}
		
		int buffer_data (Stream sin, Xiph.ogg_sync_state oy)
		{
			int bytes		= 0;
			byte[] buffer	= null;
			int offset		= 0;

			lock(sin) 
			{
				if (Xiph.ogg_sync_buffer (oy, 4096, out buffer, out offset)) {
					sin.Position = instream_pos;
					bytes = sin.Read(buffer,(int)offset,4096);
					Xiph.ogg_sync_wrote (oy, bytes);
					instream_pos = sin.Position;
				}
			}
			return bytes;
		}
		
		/* helper: push a page into the appropriate steam */
		/* this can be done blindly; a stream won't accept a page
            that doesn't belong to it */
		int queue_page(Xiph.ogg_page page){
			if(theora_p!=0)Xiph.ogg_stream_pagein(to,page);
			return 0;
		}
		
		/* dump the theora comment header */
		int dump_comments(Xiph.th_comment tc){
			int   i;
			
			Debug.Log("Encoded by " + tc.vendor);
			if(tc.user_comments!=null)
			{
				Debug.Log("theora comment header:");
				for(i=0;i<tc.user_comments.Length;i++){
					Debug.Log("\t" + tc.user_comments[i]);
				}
			}
			return 0;
		}	
		
		void open_video(){
			Xiph.th_stripe_callback cb;
			int                     pli;
			/*Here we allocate a buffer so we can use the striped decode feature.
	        There's no real reason to do this in this application, because we want to
	         write to the file top-down, but the frame gets decoded bottom up, so we
	         have to buffer it all anyway.
	        But this illustrates how the API works.*/
			/*for(pli=0;pli<3;pli++){
				int xshift;
				int yshift;
			xshift=(pli!=0&&(((int)ti.pixel_fmt)&1)==0)==true?1:0;
			yshift=(pli!=0&&(((int)ti.pixel_fmt)&2)==0)==true?1:0;

				switch (pli)
				{
				case 0:
					ycbcr.y.data = new Xiph.ogg_ptr(new byte[(ti.frame_width >> xshift) * (ti.frame_height >> yshift)], 0);//(unsigned char *)malloc((ti.frame_width>>xshift)*(ti.frame_height>>yshift)*sizeof(char));
					ycbcr.y.stride = (int)ti.frame_width >> xshift;
					ycbcr.y.width = (int)ti.frame_width >> xshift;
					ycbcr.y.height = (int)ti.frame_height >> yshift;
					break;
				case 1:
					ycbcr.cb.data = new Xiph.ogg_ptr(new byte[(ti.frame_width >> xshift) * (ti.frame_height >> yshift)], 0);//(unsigned char *)malloc((ti.frame_width>>xshift)*(ti.frame_height>>yshift)*sizeof(char));
					ycbcr.cb.stride = (int)ti.frame_width >> xshift;
					ycbcr.cb.width = (int)ti.frame_width >> xshift;
					ycbcr.cb.height = (int)ti.frame_height >> yshift;
					break;
				case 2:
					ycbcr.cr.data = new Xiph.ogg_ptr(new byte[(ti.frame_width >> xshift) * (ti.frame_height >> yshift)], 0);//(unsigned char *)malloc((ti.frame_width>>xshift)*(ti.frame_height>>yshift)*sizeof(char));
					ycbcr.cr.stride = (int)ti.frame_width >> xshift;
					ycbcr.cr.width = (int)ti.frame_width >> xshift;
					ycbcr.cr.height = (int)ti.frame_height >> yshift;
					break;
				}
			}*/

			width = (int)ti.frame_width;
			height = (int)ti.frame_height;


			/*Similarly, since ycbcr is a global, there's no real reason to pass it as
	         the context.
	        In a more object-oriented decoder, we could pass the "this" pointer
	         instead (though in C++, platform-dependent calling convention differences
	         prevent us from using a real member function pointer).*/
			cb.ctx = null;
			cb.stripe_decoded = stripe_decoded;
			Xiph.th_decode_ctl_opts opts = new Xiph.th_decode_ctl_opts();
			opts.stripe_callback = cb;
			Xiph.th_decode_ctl(td, Xiph.TH_DECCTL_SET_STRIPE_CB, opts);
		}
		
		void stripe_decoded(object ctx, ref Xiph.th_ycbcr_buffer src,
		                    int fragy0,int fragy_end)
		{
			skip = false;

			byte[] y_data = src.y.data.data;
			byte[] cr_data = src.cr.data.data;
			byte[] cb_data = src.cb.data.data;
			int y_data_offset = src.y.data.offset;
			int cb_data_offset = src.cb.data.offset;
			int cr_data_offset = src.cr.data.offset;
			int y_stride = src.y.stride;
			int cb_stride = src.cb.stride;
			int cr_stride = src.cr.stride;
			int y_width = src.y.width;
			int cb_width = src.cb.width;
			int cr_width = src.cr.width;



			int pli;
			for (pli = 0; pli < 3; pli++)
			{
				int yshift;
				int y_end;
				int y;
				yshift = (pli != 0 && (((int)ti.pixel_fmt) & 2)==0)?1:0;
				y_end = fragy_end << 3 - yshift;

				/*An implemention intending to display this data would need to check the
             	  crop rectangle before proceeding.*/
				for (y = fragy0 << 3 - yshift; y < y_end; y++)
				{/*
					//memcpy(_dst[pli].data + y * _dst[pli].stride,
					// _src[pli].data + y * _src[pli].stride, _src[pli].width);
					switch (pli)
					{
					case 0: Xiph.CopyArrays(src.y.data.data, src.y.data.offset + y * src.y.stride,
						                        ycbcr.y.data.data, ycbcr.y.data.offset + y * ycbcr.y.stride, src.y.width);
						break;
					case 1: Xiph.CopyArrays(src.cb.data.data, src.cb.data.offset + y * src.cb.stride,
						                        ycbcr.cb.data.data, ycbcr.cb.data.offset + y * ycbcr.cb.stride, src.cb.width);
						break;
					case 2: Xiph.CopyArrays(src.cr.data.data, src.cr.data.offset + y * src.cr.stride,
						                        ycbcr.cr.data.data, ycbcr.cr.data.offset + y * ycbcr.cr.stride, src.cr.width);
						break;
					}
                    */
                    switch (pli)
                    {
                        case 0: 
						for (int i = 0; i < y_width; i++)
                            colors_y_rnd[y * width + i].g = y_data[y_data_offset + y * y_stride + i];
                        break;
                        case 1: 
						for (int i = 0; i < cb_width; i++)
                        {
                                byte cb = cb_data[cb_data_offset + y * cb_stride + i];
							if(cb_stride == y_stride)
							{
								colors_y_rnd[y * width + i].b = cb;
							}
							else
							{
                                colors_y_rnd[y * 2 * y_width + i * 2].b = cb;
                                colors_y_rnd[y * 2 * y_width + i * 2 + 1].b = cb;
                                colors_y_rnd[(y * 2 + 1) * y_width + i * 2].b = cb;
                                colors_y_rnd[(y * 2 + 1) * y_width + i * 2 + 1].b = cb;
							}
	    	            }
						break;
                        case 2: 
						for (int i = 0; i < cr_width; i++)
                        {
							byte cr = cr_data[cr_data_offset + y * cr_stride + i];
							if(cb_stride == y_stride)
							{
								colors_y_rnd[y * width + i].r = cr;
							}
							else
							{

								colors_y_rnd[y * 2 * y_width + i * 2].r = cr;
								colors_y_rnd[y * 2 * y_width + i * 2 + 1].r = cr;
								colors_y_rnd[(y * 2 + 1) * y_width + i * 2].r = cr;
								colors_y_rnd[(y * 2 + 1) * y_width + i * 2 + 1].r = cr;
                            }
						}
                        break;
                    
					}
				}
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
						// create textures
						y = new Texture2D(width,height,TextureFormat.RGB24,false);
						/*
						// allocate buffers
						colors_y_1 = new Color32[width*height];
						colors_y_2 = new Color32[width*height];
						colors_y = colors_y_1;
						colors_y_rnd = colors_y_2;
						
						var strideY = ycbcr.y.stride;
						var stride2 = ycbcr.cb.stride;
						int rowIndex = 0;
						int row = 0;
						int x = 0, j;
						var cbData = ycbcr.cb.data.data;
						var crData = ycbcr.cr.data.data;
						byte cr = 0, cb = 0;
						foreach(byte b in ycbcr.y.data.data){
							if((x & 1) == 0)
							{
								j = (row * stride2) + (x >> 1);
								cr = crData[j];
								cb = cbData[j];
							}
							
							j = x + (strideY*rowIndex);
							
							colors_y[j].g = b;
							colors_y[j].r = cr;
							colors_y[j].b = cb;
							
							++x;
							if (x == strideY)
							{
								x = 0;
								++rowIndex;
								row = (rowIndex >> 1);
							}
						}
						*/
						// set initial frame number;
						cur_frame = rdy_frame = rnd_frame = 0;
						last_frame_at = Time.timeSinceLevelLoad;
						
						
						// setpixel & apply
						y.SetPixels32(colors_y);
						y.Apply();

						video_start_time = Time.timeSinceLevelLoad;

						// setup material
						control_.VideoOutput.SetTexture("_YTex",y);
						control_.Initializing = false;
						control_.RealScale = control_.Scale;
						control_.ElapsedTime = 0;
						control_.Ready = true;
						if(control_.Play)
						{
							control_.Playing = true;
						}
						
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
				
				if(rdy_frame == -1)
					return;

				float scaled_frame_in_sec = frame_in_sec;
				
				if(control_.Scale <= 0)
				{
					scaled_frame_in_sec = 999999;
				}
				else
				{
					scaled_frame_in_sec = frame_in_sec / control_.Scale;
				}
				
				if(cur_frame != rdy_frame && 
				   Time.timeSinceLevelLoad >= last_frame_at + scaled_frame_in_sec)
				{

					control_.ElapsedTime = videobuf_time;
					control_.LastFrameAt = System.DateTime.UtcNow;

					//Debug.Log("control_.ElapsedTime: " + control_.ElapsedTime + " control_.LastFrameAt: " + Time.realtimeSinceStartup);

					long lastFrameAt = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;

					if(elapsedTimeSamples.Count == ELAPSED_TIME_SAMPLES_MAX)
					{
						double frmdlt = control_.ElapsedTime - elapsedTimeSamples.Dequeue();
						double tsmdlt = (double)(lastFrameAt - elapsedTimeTimestamps.Dequeue())/1000.0; 
						
						control_.RealScale = frmdlt / tsmdlt;

						//Debug.Log("frmdlt: " + frmdlt + " tsmdlt: " + tsmdlt);
					}
					else
					{
						control_.RealScale = control_.Scale;
					}

					//Debug.Log(timeScale);

					elapsedTimeSamples.Enqueue(control_.ElapsedTime);
					elapsedTimeTimestamps.Enqueue(lastFrameAt);

					y.SetPixels32(colors_y);
					
					cur_frame = rdy_frame;
					last_frame_at += scaled_frame_in_sec;
					
					if( last_frame_at < Time.timeSinceLevelLoad - scaled_frame_in_sec)
						last_frame_at = Time.timeSinceLevelLoad - scaled_frame_in_sec;


					y.Apply();
					
					if(control_.Play)
					{
						control_.Playing = true;
					}
				}
				
			}
		}
		
		
		public void InBackground()
		{
			if(loaded == false && instream != null)
			{
				LoadVideo();
			}
			
			if(!control_.Ready)
				return;
			
			if(!control_.Play)
				return;
			
			if(rnd_frame != rdy_frame)
			{
				if(cur_frame == rdy_frame)
				{
					if(skip == false)
					{
						var colors_y_swap = colors_y_rnd;
						colors_y_rnd = colors_y;
						colors_y = colors_y_swap;
					}	
					rdy_frame = rnd_frame;
					
					skip = true;
				}
				return;
			}
			
			while(theora_p!=0 && videobuf_ready==0){
				/* theora is one in, one out... */
				if (Xiph.ogg_stream_packetout(to, op) > 0)
				{
					
					if (Xiph.th_decode_packetin(td, op, ref videobuf_granulepos) >= 0)
					{
						videobuf_time = Xiph.th_granule_time(td.state, videobuf_granulepos);
						//Debug.Log(videobuf_time);
						videobuf_ready=1;
					}
					
				}else
					break;
			}
			
			
			if (videobuf_ready==0 && instream.Length == instream_pos)
			{
				if(control_.Loop)
				{
					control_.PlaySession ++;
					LoadVideo();
				}
				else
				{
					control_.Playing = false;
				}
				return;
			}
			
			if (videobuf_ready == 0)
			{
				/* no data yet for somebody.  Grab another page */
				buffer_data(instream, oy);
				while (Xiph.ogg_sync_pageout(oy, og) > 0)
				{
					queue_page(og);
				}
			}
			/* dumpvideo frame, and get new one */
			else
			{
				rnd_frame++;
				/*
				var strideY = ycbcr.y.stride;
				var stride2 = ycbcr.cb.stride;
				int rowIndex = 0;
				int row = 0;
				int x = 0, j;
				var cbData = ycbcr.cb.data.data;
				var crData = ycbcr.cr.data.data;
				byte cr = 0, cb = 0;
				foreach(byte b in ycbcr.y.data.data){
					if((x & 1) == 0)
					{
						j = (row * stride2) + (x >> 1);
						cr = crData[j];
						cb = cbData[j];
					}
					
					j = x + (strideY*rowIndex);
					
					colors_y_rnd[j].g = b;
					colors_y_rnd[j].r = cr;
					colors_y_rnd[j].b = cb;
					
					++x;
					if (x == strideY)
					{
						x = 0;
						++rowIndex;
						row = (rowIndex >> 1);
					}
				}
				*/
				if(cur_frame == rdy_frame)
				{
					if(!skip)
					{
						var colors_y_swap = colors_y_rnd;
						colors_y_rnd = colors_y;
						colors_y = colors_y_swap;
					}
					rdy_frame = rnd_frame;
					skip = true;
				}
				
			}
			videobuf_ready=0;
			
		}
	}

}