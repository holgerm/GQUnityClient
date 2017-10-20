using UnityEngine;
using System.Collections;

public class webresponder : MonoBehaviour {





	public string xml;

	void Start(){

				if (GameObject.Find ("WebAccess") != gameObject) {
						Destroy (gameObject);		
				} else {
						DontDestroyOnLoad (gameObject);
						Debug.Log (Application.persistentDataPath);
				}
		}
}
