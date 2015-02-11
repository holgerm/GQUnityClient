using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class page_audiorecord : MonoBehaviour {
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
	public QuestPage audiorecord;
	
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
	
	// Use this for initialization
	IEnumerator Start()
	{
		
		
		
		questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
		actioncontroller = GameObject.Find ("QuestDatabase").GetComponent<actions> ();
		quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;
		audiorecord = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest.currentpage;
		
		
		if(audiorecord.onStart != null){
			
			audiorecord.onStart.Invoke();
		}
		
		if (audiorecord.hasAttribute ("task") && audiorecord.getAttribute ("task").Length > 1) {
			text.text = audiorecord.getAttribute ("task");
		} else {
			
			text.enabled = false;
			textbg.enabled = false;
			
		}
		
	
		
		// init microphone;
		if (Application.platform == RuntimePlatform.OSXWebPlayer ||
		    Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
		}
		
	
		
	
		//Check if there is at least one microphone connected  
		if(Microphone.devices.Length <= 0)  
		{  
			//Throw a warning message at the console if there isn't  
			questdb.debug("Kein Mikrofon verbunden");
			onEnd();
		}  
		else //At least one microphone is present  
		{  
			//Set 'micConnected' to true  
			micConnected = true;  
			
			//Get the default microphone recording capabilities  
			Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);  
			
			//According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
			if(minFreq == 0 && maxFreq == 0)  
			{  
				//...meaning 44100 Hz can be used as the recording sampling rate  
				maxFreq = 44100;  
			}  
			
			//Get the attached AudioSource component  
			goAudioSource = this.GetComponent<AudioSource>();  
		}  
	}  


	void Update(){
		if (!Microphone.IsRecording (null)) { 

			length += Time.deltaTime;

				}
		if (playing) {


			if(!goAudioSource.isPlaying){

				playbutton.GetComponentInChildren<Text>().text = "Abspielen";
				
				playing = false;

			}

				}


	}



	public void play(){



		if (goAudioSource.isPlaying) {

						goAudioSource.Stop ();
			playbutton.GetComponentInChildren<Text>().text = "Abspielen";

			playing = false;
				} else {
			playbutton.GetComponentInChildren<Text>().text = "Stoppen";

			goAudioSource.Play();
			playing = true;

				}


	}
	public void retry(){
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

	public void record(){

		if (micConnected) {  

			//If the audio from any microphone isn't being captured  
			if(!Microphone.IsRecording(null))  
			{  

				goAudioSource.clip = Microphone.Start(null, true, 20, maxFreq);  
				recordbutton.GetComponentInChildren<Text>().text = "Aufnahme stoppen";



			} else if(!savedaudio) {


				Microphone.End(null); //Stop the audio recording  

				playbutton.enabled = true;
				playbutton.GetComponent<Image>().enabled = true;
				playbutton.GetComponentInChildren<Text>().enabled = true;

				retrybutton.enabled = true;
				retrybutton.GetComponent<Image>().enabled = true;

				retrybutton.GetComponentInChildren<Text>().enabled = true;

				recordbutton.GetComponentInChildren<Text>().text = "Abspeichern";
				savedaudio = true;

				AudioClip ac = goAudioSource.clip;
				float lengthL = ac.length;
				float samplesL = ac.samples;
				float samplesPerSec = (float)samplesL/lengthL;
				float[] samples = new float[(int)(samplesPerSec * length)];
				ac.GetData(samples,0);
				
				goAudioSource.clip = AudioClip.Create("RecordedSound",(int)(length*samplesPerSec),1,44100,false,false);
				
				goAudioSource.clip.SetData(samples,0);


			} else {

				// save to var



				onEnd ();


			}



				} else {

					onEnd ();

				}

	}
	void OnGUI()   
	{  
		//If there is a microphone  
		if(micConnected)  
		{  
			//If the audio from any microphone isn't being captured  
			if(!Microphone.IsRecording(null))  
			{  
				//Case the 'Record' button gets pressed  
				if(GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-25, 200, 50), "Record"))  
				{  
					//Start recording and store the audio captured from the microphone at the AudioClip in the AudioSource  
					goAudioSource.clip = Microphone.Start(null, true, 20, maxFreq);  
				}  
			}  
			else //Recording is in progress  
			{  
				//Case the 'Stop and Play' button gets pressed  
				if(GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-25, 200, 50), "Stop and Play!"))  
				{  
					Microphone.End(null); //Stop the audio recording  
					goAudioSource.Play(); //Playback the recorded audio  
				}  
				
				GUI.Label(new Rect(Screen.width/2-100, Screen.height/2+25, 200, 50), "Recording in progress...");  
			}  
		}  
		else // No microphone  
		{  
			//Print a red "Microphone not connected!" message at the center of the screen  
			GUI.contentColor = Color.red;  
			GUI.Label(new Rect(Screen.width/2-100, Screen.height/2-25, 200, 50), "Microphone not connected!");  
		}  
		
	}




	void onEnd(){
		
		audiorecord.state = "succeeded";
		
		
		
		if (audiorecord.onEnd != null && audiorecord.onEnd.actions.Count > 0) {
			Debug.Log ("onEnd");
			audiorecord.onEnd.Invoke ();
		} else {
			
			questdb.endQuest();
			
		}
		
		
	}
}
