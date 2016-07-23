using UnityEngine;
using System.Collections;
using GQ.Client.Conf;
using UnityEngine.UI;

namespace GQ.Client.UI.Menu {
	public class TopMenuButton : MonoBehaviour {

		private questdatabase qdb;

		//		public void OnEnable () {
		//			qdb = GameObject.Find("QuestDatabase").GetComponent<questdatabase>();
		//			Debug.Log("ENABLE " + gameObject.GetInstanceID());
		//			Debug.Log("currentQuest: " + qdb.currentquest.ToString() + " name: " + qdb.currentquest.name);
		//			if ( !Configuration.instance.hasMenuWithinQuests && qdb.currentquest != null ) {
		//				Debug.Log("ON");
		//				gameObject.GetComponent<Image>().enabled = true;
		//				gameObject.GetComponent<Button>().enabled = true;
		//			}
		//		}
		//
		//		public void OnDisable () {
		//			Debug.Log("DISABLE " + gameObject.GetInstanceID());
		//			Debug.Log("currentQuest: " + qdb.currentquest.ToString() + " name: " + qdb.currentquest.name);
		//			if ( !Configuration.instance.hasMenuWithinQuests && qdb.currentquest != null ) {
		//				Debug.Log("OFF");
		//				gameObject.GetComponent<Image>().enabled = false;
		//				gameObject.GetComponent<Button>().enabled = false;
		//			}
		//		}

		// Use this for initialization
		void Start () {
	
		}
	
		// Update is called once per frame
		void Update () {
	
		}
	}
}
