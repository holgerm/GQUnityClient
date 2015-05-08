using UnityEngine;
using System.Collections;
using System;

public class Configuration : MonoBehaviour
{

	private static Configuration _instance;
	
	public int portalID = 1;
	public int autostartQuestID = 0;
	public int autostartQuestSize = 0;

	public string colorProfile = "default";


	private bool externalBuildProcess = false;
	
	public static Configuration instance {
		get {
//			Debug.Log ("SETTINGS: get()");

			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<Configuration> ();
				
				//Tell unity not to destroy this object when loading a new scene!
				DontDestroyOnLoad (_instance.gameObject);
			}
			
			return _instance;
		}
	}

	public void SetExternalBuildFlag (bool newValue)
	{
		externalBuildProcess = newValue;
	}
	
	void Awake ()
	{
//		Debug.Log ("SETTINGS: awake(): externalBuild = " + externalBuildProcess);

//		System.Object t = AssetDatabase.LoadAssetAtPath ("Assets/Config/configuration.json", Type.GetType ("System.Object"));

//		if (t == null || t.Equals (""))
//			Debug.Log ("CONFIG: NO configuration found => using standard editor config.");
//		else
//			Debug.Log ("Asset loaded. t = " + t);

		if (_instance == null) {
			//If I am the first instance, make me the Singleton
			_instance = this;
			DontDestroyOnLoad (this);
		} else {
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if (this != _instance)
				Destroy (this.gameObject);
		}
	}
	
}