using UnityEngine;
using System.Collections;

public class questaudio : MonoBehaviour
{



	public AudioSource audiosource;

	void Start ()
	{

		DontDestroyOnLoad (gameObject);
	}

	public void setLoop (bool b)
	{

//		Debug.Log (b);
		audiosource.loop = b;

	}

	public void setAudio (AudioClip ac)
	{
		if (audiosource != null) {
			audiosource.clip = ac;
		}

	}

	public void Play ()
	{
		if (audiosource != null) {
			audiosource.Play ();
		}

	}

	public void Stop ()
	{
		if (audiosource != null) 
			audiosource.Stop ();
	}



}
