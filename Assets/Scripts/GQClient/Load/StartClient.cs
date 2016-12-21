using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace GQ.Client.Load {

	public class StartClient : MonoBehaviour {

		// Use this for initialization
		void Start () {
			// TODO load prod specs

			// TODO decide by prod specs which foyer scene to be used:

			SceneManager.LoadSceneAsync("questlist");
		}
	
		// Update is called once per frame
		void Update () {
	
		}
	}

}
