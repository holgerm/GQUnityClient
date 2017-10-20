using UnityEngine;
using System.Collections;

/*
	AudioRecorder class by Moodkie Interactive at http://www.moodkie.com.
	Provided with absolutely no warranty.
*/
using System.IO;

public class AudioRecorder : MonoBehaviour 
{
	// The name of the microphone we want to record from.
	private string microphoneName = "";
	// The name of the file we want to save our audio to.
	private string saveFile = "myAudio.txt";
	// An AudioSource which we'll use to play our audio.
	private AudioSource recordedAudio;
	
	IEnumerator Start() 
	{
		// Create our AudioSource.
		recordedAudio = gameObject.AddComponent<AudioSource>();
		// Request permission to use the microphone.
		yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
	}
	
	private void OnGUI() 
	{
		// If permission to use the microphone has been granted ...
		if(Application.HasUserAuthorization(UserAuthorization.Microphone))
		{
			// If a microphone has not been chosen, show list of microphones.
			if(microphoneName == "")
				ShowMicrophoneList();
			else
			// Else, show a record button to allow us to record from chosen microphone.
				ShowRecordButton();
		}
		
	
	}
	
	/*
		Shows a clickable list of microphones.
	*/
	private void ShowMicrophoneList()
	{
		// Store the device number so we can place buttons one after another.
		int deviceNo = 0;
		// For each microphone device detected ...
		foreach(string device in Microphone.devices)
		{
			// ... display a button.
			if(GUI.Button(new Rect(0, 50*deviceNo, Screen.width/2, 50), device))
			// When the button is pressed, set our microphone name to this microphone.
				microphoneName = device;
			deviceNo++;
		}
	}
	
	/*
		Shows a record button, or a stop button if we're already recording.
	*/
	private void ShowRecordButton()
	{
		// If microphone isn't recording ...
		if(!Microphone.IsRecording(microphoneName))
		{
			// Show a button which records when pressed.
			if(GUI.Button(new Rect(0, 0, Screen.width/2, 50), "Start Recording"))
				recordedAudio.clip = Microphone.Start(microphoneName, true, 3, 44100);
		}
		else
		{
			// Else, show a button which lets us stop recording.
			if(GUI.Button(new Rect(0, 0, Screen.width/2, 50), "Stop Recording"))
			{
				// Stop recording.
				Microphone.End(microphoneName);
				// Save the AudioClip containing our recording.
				SavWav.Save(saveFile,recordedAudio.clip);
				// Play the recording.
				recordedAudio.Play();
			}
		}
	}
	

}
