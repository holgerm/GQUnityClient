using UnityEngine;
using System.Collections;
using System.IO;
using MonoMedia;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class VideoPlayer : MonoBehaviour {

	public string Video;

	public Material VideoOutput;

	public float[] Pcm;

	public long PcmSampleOffset;

	public long PcmSampleCount;

	public long PcmRate;

	public int PcmChannels;

	public double ElapsedTime;

	public DateTime LastFrameAt;

	public double RealScale;

	public double SamplesPerMillisecond;

	public bool Preload = true;
	
	public bool Play = false;

	public uint PlaySession = 0;

	public bool Loop = true;

	public bool Transparent = false;

	public bool Additive = false;

	public float Scale = 1;

	public List<GameObject> OnReady;

	public List<GameObject> OnStart;

	public List<GameObject> OnStop;

	public bool IsPlaying {

		get {
			return isPlaying_;
		}

		private set {

			if(value && isPlaying_ == false)
			{
				isPlaying_ = value;
				for(int i = 0; i < OnStart.Count; i++) {
					if(OnStart[i] != null)
						OnStart[i].SendMessage("OnVideoStart");
				}
			}
			else if(value == false && isPlaying_ == true)
			{
				isPlaying_ = value;
				for(int i = 0; i < OnStop.Count; i++) {
					if(OnStop[i] != null)
						OnStop[i].SendMessage("OnVideoStop");
				}
			}
			else
				isPlaying_ = value;
		}
	}
	
	// Video control part
	string video_ = string.Empty; // current video
	bool runOnce_ = true;
	bool isPlaying_ = false;
	MediaKitProcessor.OGVControl control_;

	// helper
	MediaKitProcessor processor_ = null;
	MediaKitProcessor Processor { get { if(processor_ == null) processor_ = MediaKitProcessor.Instance; return processor_; } }

	// Use this for initialization
	void Start () {

		#if UNITY_WEBPLAYER
		Preload = true;
		#endif

		Shader shader = Shader.Find(Transparent ? Additive ? "MediaKit/AdditiveTransparentVideoOutput" : "MediaKit/TransparentVideoOutput" : "MediaKit/VideoOutput");
		
		VideoOutput = new Material(shader);

		RefreshVideoControl();
	}
	
	void OnDestroy()
	{
		if( control_ != null && processor_ != null )
			processor_.Remove(control_);
	}

	void RefreshVideoControl()
	{

		if( string.IsNullOrEmpty(Video) )
		{
			video_ = string.Empty;

			if( control_ != null )
				Processor.Remove(control_);

			control_ = null;
		}
		else
		{
			if( !video_.Equals(Video) )
			{
				if( control_ != null )
					Processor.Remove(control_);

				video_ = Video;

				control_ = new MediaKitProcessor.OGVControl( Video, Preload, VideoOutput );
				control_.Scale = Scale;
				Processor.Add(control_);
			}
		}
	}


	// Update is called once per frame
	void Update () {

		RefreshVideoControl();

		if(control_ != null)
		{

			control_.Scale = Scale;

			if(control_.Ready)
			{
				if(runOnce_){
					runOnce_ = false;
					for(int i = 0; i < OnReady.Count; i++) {
						if(OnReady[i] != null)
							OnReady[i].SendMessage("OnVideoReady");
					}
				}
				control_.Play = Play;
				control_.Loop = Loop;
				IsPlaying = control_.Playing;
				PlaySession = control_.PlaySession;
				ElapsedTime = control_.ElapsedTime;
				LastFrameAt = control_.LastFrameAt;
				RealScale = control_.RealScale;
				SamplesPerMillisecond = control_.SamplesPerMillisecond;
				Pcm = control_.Pcm;
				PcmRate = control_.PcmRate;
				PcmChannels = control_.PcmChannels;
				PcmSampleOffset = control_.PcmSampleOffset;
				PcmSampleCount = control_.PcmSampleCount;
			}
		}
	}
	
}