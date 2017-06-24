using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using GQ.Client.Model;
using UnityEngine.SceneManagement;

public class page_audiorecord : MonoBehaviour
{
	//A boolean that flags whether there's a connected microphone
	public bool micConnected = false;
	
	//The maximum and minimum available recording frequencies
	private int minFreq;
	private int maxFreq;
	
	//A handle to the attached AudioSource
	private AudioSource goAudioSource;
	
	
	public questdatabase questdb;
	public actions actioncontroller;
	
	public Quest quest;
	public Page audiorecord;
	
	public Text text;
	public Image textbg;
	

	public Button recordbutton;
	public Button playbutton;
	public Button retrybutton;
	
	
	public bool savedaudio = false;
	
	
	WebCamTexture cameraTexture;
	
	Material cameraMat;
	GameObject plane;

	private bool playing = false;

	private float length = 0f;
	private float playlength = 0f;
	
	// Use this for initialization
	IEnumerator Start ()
	{
		
		
		if (GameObject.Find ("QuestDatabase") == null) {

			SceneManager.LoadScene ("questlist", LoadSceneMode.Single);
			yield break;
		} 


		questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
		actioncontroller = GameObject.Find ("QuestDatabase").GetComponent<actions> ();
		quest = QuestManager.Instance.CurrentQuest;
		audiorecord = QuestManager.Instance.CurrentQuest.currentpage;
		
		
		if (audiorecord.onStart != null) {
			
			audiorecord.onStart.Invoke ();
		}
		
		if (audiorecord.hasAttribute ("task") && audiorecord.getAttribute ("task").Length > 1) {
			text.text = questdb.GetComponent<actions> ().formatString (audiorecord.getAttribute ("task"));
		} else {
			
			text.enabled = false;
			textbg.enabled = false;
			
		}
		
	
		
		// init microphone;
		if (Application.isWebPlayer) {
			yield return Application.RequestUserAuthorization (UserAuthorization.Microphone);
		}
		
	
		
	
		//Check if there is at least one microphone connected  
		if (Microphone.devices.Length <= 0) {  
			//Throw a warning message at the console if there isn't  
			questdb.debug ("Kein Mikrofon verbunden");
			onEnd ();
		} else { //At least one microphone is present  
			//Set 'micConnected' to true  
			micConnected = true;  
			
			//Get the default microphone recording capabilities  
			Microphone.GetDeviceCaps (null, out minFreq, out maxFreq);  
			
			//According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
			if (minFreq == 0 && maxFreq == 0) {  
				//...meaning 44100 Hz can be used as the recording sampling rate  
				maxFreq = 44100;  
			}  
			
			//Get the attached AudioSource component  
			goAudioSource = this.GetComponent<AudioSource> ();  
		}  
	}


	void Update ()
	{
		if (Microphone.IsRecording (null)) { 

			length += Time.deltaTime;

		}
		if (playing) {
			playlength += Time.deltaTime;

			if (!goAudioSource.isPlaying) {
				playbutton.GetComponentInChildren<Text> ().text = "Abspielen";
				
				playing = false;

			}

		}


	}



	public void play ()
	{



		if (goAudioSource.isPlaying) {
			playlength = 0f;
			goAudioSource.Stop ();
			playbutton.GetComponentInChildren<Text> ().text = "Abspielen";

			playing = false;
		} else {
			playbutton.GetComponentInChildren<Text> ().text = "Stoppen";

			goAudioSource.Play ();
			playing = true;

		}


	}

	public void retry ()
	{
		if (!Microphone.IsRecording (null)) { 
			savedaudio = false;
			recordbutton.GetComponentInChildren<Text> ().text = "Aufnahme stoppen";
			playbutton.enabled = false;
			playbutton.GetComponentInChildren<Text> ().enabled = false;
			playbutton.GetComponent<Image> ().enabled = false;

			length = 0f;

			retrybutton.enabled = false;
			retrybutton.GetComponentInChildren<Text> ().enabled = false;
			retrybutton.GetComponent<Image> ().enabled = false;

			goAudioSource.clip = Microphone.Start (null, true, 60, maxFreq);  
		}

	}

	public void record ()
	{

		if (micConnected) {  

			//If the audio from any microphone isn't being captured  
			if (!Microphone.IsRecording (null) && !savedaudio) {  

				goAudioSource.clip = Microphone.Start (null, true, 60, maxFreq);  
				recordbutton.GetComponentInChildren<Text> ().text = "Aufnahme stoppen";



			} else if (!savedaudio) {


				Microphone.End (null); //Stop the audio recording  

				playbutton.enabled = true;
				playbutton.GetComponent<Image> ().enabled = true;
				playbutton.GetComponentInChildren<Text> ().enabled = true;

				retrybutton.enabled = true;
				retrybutton.GetComponent<Image> ().enabled = true;

				retrybutton.GetComponentInChildren<Text> ().enabled = true;

				recordbutton.GetComponentInChildren<Text> ().text = "Abspeichern";
				savedaudio = true;

				AudioClip ac = goAudioSource.clip;

			

				float lengthL = ac.length;
				float samplesL = (float)ac.samples;
				float samplesPerSec = samplesL / lengthL;
				float[] samples = new float[(int)(samplesPerSec * length)];
				ac.GetData (samples, 0);
				goAudioSource.clip = AudioClip.Create ("RecordedSound", (int)(length * samplesPerSec), 1, (int)samplesPerSec, false, false);
				//Debug.Log("samples new:"+(length*samplesPerSec));
				goAudioSource.clip.SetData (samples, 0);
				//Debug.Log ((goAudioSource.clip.samples)/(goAudioSource.clip.length));



			} else {

				// save to var

				if (audiorecord.hasAttribute ("file")) {
					
					QuestRuntimeAsset qra = new QuestRuntimeAsset ("@_" + audiorecord.getAttribute ("file"), goAudioSource.clip);
					
					actioncontroller.audioclips.Add (qra);

				}
				onEnd ();


			}



		} else {

			onEnd ();

		}

	}




	void onEnd ()
	{
		
		audiorecord.stateOld = "succeeded";
		
		
		
		if (audiorecord.onEnd != null && audiorecord.onEnd.actions.Count > 0) {
			Debug.Log ("onEnd");
			audiorecord.onEnd.Invoke ();
		} else {
			
			questdb.endQuest ();
			
		}
		
		
	}
}
