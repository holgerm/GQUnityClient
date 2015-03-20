using UnityEngine;
using System.Collections;


[RequireComponent (typeof (AudioSource))]
public class AudioTransmitter: MonoBehaviour {

	public VideoPlayer VideoPlayer;

	AudioSource audio_;
	uint playSession_;
	long sampleCount_;
	long lastOffset_;
	long swapOffset_;
	bool playing_ = false;

	double scaleCorrection_ = 1;

	int lastWriteIdx = -1;
	int[] lastWritingSamples = new int[SPEEDACC];
	long[] lastWritingTime = new long[SPEEDACC];
	double writespeed = 44.1; // samples per millisecond
    
	const int SPEEDACC = 20;

	// Use this for initialization
	void Start () {
		// just cashed it.
		audio_ = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

		if(VideoPlayer != null)
		{
			playing_ = VideoPlayer.IsPlaying;
			if(playing_)
			{
				if(playSession_ != VideoPlayer.PlaySession)
				{
					scaleCorrection_ = 1.1;
					playSession_ = VideoPlayer.PlaySession;
					//sampleCount_ = (long)((VideoPlayer.ElapsedTime /*+ (((System.DateTime.UtcNow - VideoPlayer.LastFrameAt).TotalSeconds) * VideoPlayer.Scale)*/) * VideoPlayer.PcmRate);
					lastOffset_ = VideoPlayer.PcmSampleOffset;
					swapOffset_ = 44100;
					sampleCount_ = swapOffset_;
				}
			}
			else
			{
				scaleCorrection_ = 1.1;
			}
		}

	}

	public void OnAudioFilterRead(float[] data, int channels) 
	{
		lastWriteIdx ++;
		long milliseconds = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
		if(lastWriteIdx > SPEEDACC -1)
		{
			for(int i=1; i< SPEEDACC; i++)
			{
				lastWritingSamples[i-1] = lastWritingSamples[i];
				lastWritingTime[i-1] = lastWritingTime[i];
			}
			lastWriteIdx = SPEEDACC -1;
		}

		lastWritingSamples[lastWriteIdx] = data.Length / channels;
		lastWritingTime[lastWriteIdx] = milliseconds;
        
		long totalLastSamples = 0;
		long totalLastMilliseconds = 0;
		for(int i = 1; i <= lastWriteIdx; i++)
		{
			totalLastSamples += lastWritingSamples[i-1];
			totalLastMilliseconds += lastWritingTime[i] - lastWritingTime[i-1];
		}
		if( lastWriteIdx == SPEEDACC - 1 )
			writespeed = (double)totalLastSamples / (double)totalLastMilliseconds;
        else
			writespeed = 44.1;

		//Debug.Log("writespeed: " + writespeed);

        if(!playing_)
			return;

		if(VideoPlayer.Pcm == null)
			return;
		//long curSample = (long)((VideoPlayer.ElapsedTime + (((System.DateTime.UtcNow - VideoPlayer.LastFrameAt).TotalSeconds * 0.5) * VideoPlayer.RealScale)) * VideoPlayer.PcmRate);
		float[] pcm = VideoPlayer.Pcm;
		int bufChannels = VideoPlayer.PcmChannels;
		int bufSize = VideoPlayer.Pcm.Length;
		int bufSamples = VideoPlayer.Pcm.Length / bufChannels;
		long bufOffset = VideoPlayer.PcmSampleOffset;
		long bufBegin = sampleCount_ - bufOffset;
		long bufLast = VideoPlayer.PcmSampleCount - bufOffset;

		if(lastOffset_ != VideoPlayer.PcmSampleOffset)
		{
			lastOffset_ = VideoPlayer.PcmSampleOffset;
			scaleCorrection_ = 1.0-((double)(bufBegin - swapOffset_))/(double)(44100*4);
		}

		//Debug.Log ("--- " + VideoPlayer.PcmSampleOffset +  " -- " +bufPtr + " --- " + data.Length + " --- " + sampleCount_ + "==" + callCount);

		//long curIndex = curSample - bufOffset;

		//scaleCorrection_ = ((double)(curIndex - bufBegin))/(double)(44100);

		int samplesRequested = data.Length / channels;

		double rwscale = VideoPlayer.RealScale * scaleCorrection_;//44.1 / writespeed;

		long srcSamples = (long)((double)samplesRequested * rwscale /*(scaleCorrection_ + VideoPlayer.RealScale)*/);

		long bufEnd = bufBegin + srcSamples;

		sampleCount_ += srcSamples;

		//Debug.Log ("srcSamples: " + srcSamples + " scaleCorrection_: " + scaleCorrection_ + " rwscale: " + rwscale + " RealScale: " + VideoPlayer.RealScale +" bufBegin: " + bufBegin + " bufEnd: " + bufEnd + "  offset: " + bufOffset + " sampleCount_: " + sampleCount_ );

		//if(!scaleCorrected)
		//	scaleCorrection_ = 1;

		double step = (double)(bufEnd - bufBegin)/(double)samplesRequested;


		for(int ch = 0; ch < bufChannels;ch++)
		{
			for(int i = 0; i < samplesRequested; i++)
			{

				double srcIdb = step * i;
				long lid = bufBegin + (long)srcIdb;
				
				if(lid < 0){
					//Debug.LogWarning("lid < 0");
					lid = 0;
				}
				
				long rid = lid + 1;

				double scale = srcIdb - (double)(lid-bufBegin);
				
				long lastId = bufSamples-1;


				if(lastId >= bufLast)
					lastId = bufLast;
				
				if(lid > lastId){
					lid = lastId;
					//Debug.LogWarning("lid > lastId");
					//break;
				}
				
				if(rid > lastId){
					rid = lastId;
					//Debug.LogWarning("rid > lastId");
					//break;
				}

				float lf = pcm[(lid*bufChannels) + ch];
				float rg = pcm[(rid*bufChannels) + ch];
				
				float v = (float)(lf * (1-scale) + rg * scale);
				
				if(v > 1)
					v = 1;
				if(v < -1) 
					v = -1;

				data[i*channels + ch] = v;
			}
		}
	}
}
