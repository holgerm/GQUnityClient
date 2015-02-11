using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MonoMedia;
using System;
using System.Runtime.InteropServices;
using System.Threading;
#if UNITY_ANDROID && !UNITY_EDITOR
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
#endif
using System.Text.RegularExpressions;
using System.IO.Compression;

using MediaKit.Processor;

public class MediaKitProcessor : MonoBehaviour {

	public static MediaKitProcessor Instance
	{
		get {
			var prc = GameObject.FindObjectOfType<MediaKitProcessor>();
			if( prc == null)
			{
				var go = new GameObject("MediaKitProcessor");
				DontDestroyOnLoad(go);
				prc = go.AddComponent<MediaKitProcessor>();
			}
			return prc;
		}
	}

	public class OGVControl
	{
		// initial unchangeable variables
		public readonly bool   	 Preload;
		public readonly string   Path;
		public readonly Material VideoOutput;

		// user control variables
		public bool Play { set { if(play_ != value)	{ if(value)	{ PlaySession ++; } }; play_ = value; } get { return play_; } }
		public uint PlaySession;
		public bool Loop;
		public float Scale = 1;

		// status variables
		public string Error;
		public string Info;
		public bool Initializing;
		public bool AudioAvailable;
		public bool Ready;
		public bool Playing { set {	playing_ = value; } get { return playing_; } }
		public double ElapsedTime;
		public DateTime LastFrameAt;
		public double RealScale;
		public double SamplesPerMillisecond;
		public float[] Pcm;
		public long PcmSampleOffset;
		public long PcmSampleCount;
		public long PcmRate;
		public int PcmChannels;

		bool play_;
		bool playing_;

		public OGVControl(string path, bool preload, Material videoOutput)
		{
			Preload = preload;
			Path = path;
			VideoOutput = videoOutput;
		}
	}

	public void Add(OGVControl ctrl)
	{
		lock(controls_)
		{
			controls_.Add(ctrl);
			AddNewVideo();
		}
	}

	public void Remove(OGVControl ctrl)
	{
		lock(controls_)
		{
			controls_.Remove(ctrl);
			RemoveOldVideo();
		}
	}


	#region Implementation

	List<OGVControl> controls_ = new List<OGVControl>();

	List<OGGStream> streams_ = new List<OGGStream>();

	List<VideoDecoder> videoDecoders_ = new List<VideoDecoder>();

	List<AudioDecoder> audioDecoders_ = new List<AudioDecoder>();

#if !MEDIAKIT_NOTHREADS
	Thread videoThread_;
	bool videoThreadContinue_ = true;
	Thread audioThread_;
	bool audioThreadContinue_ = true;
#endif

	Exception exception_ = null;

	// Use this for initialization
	void Start () {

		StartCoroutine(StreamLoader());

#if !MEDIAKIT_NOTHREADS
		videoThread_ = new Thread(new ThreadStart(VideoThreadFunc));
#if !UNITY_WP8
		videoThread_.Priority = System.Threading.ThreadPriority.Highest;
#endif
		videoThread_.Start();

		audioThread_ = new Thread(new ThreadStart(AudioThreadFunc));
#if !UNITY_WP8
		audioThread_.Priority = System.Threading.ThreadPriority.Highest;
#endif
		audioThread_.Start();
#endif
	}

	void OnDestroy() {

#if MEDIAKIT_MEMTRACE
		Debug.Log (Xiph.MemTrace.DumpTraceInfo());
#endif

#if !MEDIAKIT_NOTHREADS
		int trycount = 0;
		videoThreadContinue_ = false;
		if(videoThread_ != null)
		while(videoThread_.IsAlive)
		{
			trycount++;
			if(trycount > 10)
			{
				Debug.LogError("Video thread has stack!");
				break;
			}
			Thread.Sleep(100);
		}
		trycount = 0;
		audioThreadContinue_ = false;
		if(audioThread_ != null)
		while(audioThread_.IsAlive)
		{
			trycount++;
			if(trycount > 10)
			{
				Debug.LogError("Audio thread has stack!");
				break;
			}
			Thread.Sleep(100);
		}
#endif
	}


	void AudioThreadFunc()
	{
		try {
			
			int i = 0;
			
#if !MEDIAKIT_NOTHREADS
			for(;audioThreadContinue_;)
#endif
			{
				
				AudioDecoder dec = null;
				lock(controls_)
				{
					if(i < audioDecoders_.Count)
					{
						dec = audioDecoders_[i++];
					}
					else
					{
						i = 0;
						if(i < audioDecoders_.Count)
						{
							dec = audioDecoders_[i++];
						}
					}
				}
				
				if(dec != null)
				{
					dec.InBackground();
				}
				else
				{
#if !MEDIAKIT_NOTHREADS
					Thread.Sleep(10);
#endif
				}
			}
			
		}
		catch(Exception e)
		{
			exception_ = e;
		}
	}

	void VideoThreadFunc()
	{
		try {

			int i = 0;

#if !MEDIAKIT_NOTHREADS
			for(;videoThreadContinue_;)
#endif
			{

				VideoDecoder dec = null;
				lock(controls_)
				{
					if(i < videoDecoders_.Count)
					{
						dec = videoDecoders_[i++];
					}
					else
					{
						i = 0;
						if(i < videoDecoders_.Count)
						{
							dec = videoDecoders_[i++];
						}
					}
				}

				if(dec != null)
				{
					dec.InBackground();
				}
				else
				{
#if !MEDIAKIT_NOTHREADS
					Thread.Sleep(10);
#endif
				}
			}

		}
		catch(Exception e)
		{
			exception_ = e;
		}
	}

	// Update is called once per frame
	void Update () {

#if MEDIAKIT_NOTHREADS
		VideoThreadFunc();
		AudioThreadFunc();
#endif

		for(int di = 0; di<audioDecoders_.Count;di++)
			audioDecoders_[di].Update ();

		for(int di = 0; di<videoDecoders_.Count;di++)
			videoDecoders_[di].Update ();

		// remove unneeded streams
		for(int si=0; si<streams_.Count;si++){
			var s = streams_[si];
			if(s.RefCount == 0)
			{
				streams_.Remove(s);
				break;
			}
		}

		if(exception_ != null)
		{
			Debug.LogException(exception_);
			exception_ = null;
		}

	}

	IEnumerator StreamLoader()
	{
		for(;;)
		{
			for(int si=0; si<streams_.Count;si++)
			{
				var s = streams_[si];

				if(s.Initializing)
				{
					s.RefCount ++;

					if(s.Preload)
					{



						#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
						string path = "file://" + Application.persistentDataPath + "/" + s.Path; 
						#elif UNITY_WEBPLAYER
						string path = Application.persistentDataPath + s.Path;
						#elif UNITY_IPHONE
						string path = Application.persistentDataPath + s.Path;
						#else
						string path = Application.persistentDataPath + s.Path;
						#endif		

						if(s.Path.StartsWith("http:") || s.Path.StartsWith("https:")){
							path= s.Path;
							
						} 
						WWW www = new WWW(path);
						yield return www;

						if(www.error != null)
						{
							Debug.LogError(www.error);
							s.Error = www.error;
						}
						else
						{
							s.InMem = new MemoryStream(www.bytes);
							s.Initializing = false;
							s.Ready = true;
						}
					}
					else
					{
						/* not supported yet
						string path = Application.persistentDataPath + "/MediaKit/" + s.Path;
						Directory.CreateDirectory(Path.GetDirectoryName(path));
						if(!File.Exists(path))
						{
							string assetpath = Application.streamingAssetsPath + "/" + s.Path;
							WWW www = new WWW(assetpath);
							yield return www;
							File.WriteAllBytes(path,www.bytes);
						}*/

						#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
						string path = Application.streamingAssetsPath + "/" + s.Path; 
						#elif UNITY_WEBPLAYER
						string path = "StreamingAssets/" + s.Path;
						#elif UNITY_IPHONE
						string path = Application.dataPath + "/Raw/" + s.Path;
						#elif UNITY_ANDROID
						string path = Application.persistentDataPath + "/MediaKitCache/Assets";

						Directory.CreateDirectory(path);

						//	"/jar:file:/data/app/..../....apk!/assets"

						Match match = Regex.Match(Application.streamingAssetsPath, @"(?<path>/data/app/.+)!/(?<assets>.+)", RegexOptions.IgnoreCase);
						
						if (match.Success)
						{
							// Finally, we get the Group value and display it.
							string zpath = match.Groups[@"path"].Value;
							string assets = match.Groups[@"assets"].Value;
                            

							ZipFile zf = null;
							try {
								FileStream fs = File.OpenRead(zpath);
								zf = new ZipFile(fs);
								ZipEntry zipEntry = zf.GetEntry(assets+'/'+s.Path);
								if (zipEntry != null && zipEntry.IsFile) {

									path += "/" + s.Path + "_" + zipEntry.Crc;
									
									
									if( File.Exists(path) )
									{
										FileInfo f = new FileInfo(path);
										if( zipEntry.Size != f.Length )
										{
											File.Delete(path);
										}
									}
									
									if( !File.Exists(path) )
									{
										var delMask = Application.persistentDataPath + "/MediaKitCache/Assets/" + s.Path;
										
										var assetDir = Path.GetDirectoryName(delMask);
										
										Directory.CreateDirectory(assetDir);
										
										foreach(FileInfo f in new DirectoryInfo(assetDir).GetFiles(Path.GetFileName(s.Path)+"_*")) {
                                            f.Delete();
                                        }

										byte[] buffer = new byte[4096];     // 4K is optimum
										Stream zipStream = zf.GetInputStream(zipEntry);
										
		                                // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
		                                // of the file, but does not waste memory.
		                                // The "using" will close the stream even if an exception occurs.
										using (FileStream streamWriter = File.Create(path)) {
											StreamUtils.Copy(zipStream, streamWriter, buffer);
		                                }
									}
	                            }
								else
								{
									s.Error = "Can't find: " + s.Path;
									Debug.LogError(s.Error);
								}
	                        } finally {
	                            if (zf != null) {
	                                zf.IsStreamOwner = true; // Makes close also shut the underlying stream
	                                zf.Close(); // Ensure we release resources
	                            }
							}

							if(s.Error != null)
							{
                            	yield break;
							}
                            
						}
						else
						{
							s.Error = "Unknown error";
							Debug.LogError(s.Error + ": " + Application.streamingAssetsPath);
							yield break;
						}

						#else
						string path = Application.streamingAssetsPath + "/" + s.Path;
						#endif		

						s.InFile = new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read);
						s.Initializing = false;
						s.Ready = true;

					}
					s.RefCount --;
				}
			}

			yield return 0;
		}
	}


	void AddNewVideo()
	{
		for(int ci=0;ci<controls_.Count;ci++)
		{
			var ctrl = controls_[ci];
			
			VideoDecoder dec = null;
			
			for(int di = 0; di<videoDecoders_.Count;di++)
			{
				if(videoDecoders_[di].Control == ctrl){
					dec = videoDecoders_[di];
					break;
				}
			}
			
			if(dec == null)
			{
				OGGStream s = null;

				for(int si=0; si<streams_.Count;si++)
				{
					if(streams_[si].Path==ctrl.Path && 
					   streams_[si].Preload==ctrl.Preload )
					{
						s = streams_[si];
						break;
					}
				}

				if(s == null)
				{
					streams_.Add(s = new OGGStream(ctrl.Path,ctrl.Preload));
				}

				videoDecoders_.Add(new VideoDecoder(s,ctrl));
				audioDecoders_.Add(new AudioDecoder(s,ctrl));
			}
		}
	}

	void RemoveOldVideo()
	{
		for(int di = 0; di<videoDecoders_.Count;di++)
		{
			var d = videoDecoders_[di];

			for(int ci=0;ci<controls_.Count;ci++)
			{
				var ctrl = controls_[ci];
				if(d.Control == ctrl){
					d = null;
					break;
				}
			}

			if(d != null)
			{

				for(int ai = 0; ai < audioDecoders_.Count; ai++)
				{
					var a = audioDecoders_[ai];
					if(a.Control == d.Control)
					{
						audioDecoders_.Remove(a);
					}
				}

				videoDecoders_.Remove(d);
				break;
			}
		}
	}

	#endregion
}
