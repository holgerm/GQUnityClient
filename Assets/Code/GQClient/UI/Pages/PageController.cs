using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using GQ.Client.Model;

namespace GQ.Client.UI.Pages {

	public abstract class PageController : MonoBehaviour {

		private const int DEFAULT_FONT_SIZE = 20;

		public Page page;
		public questdatabase questdb;
		public Quest quest;
		public actions questactions;

		protected virtual void Start () { 

			if ( GameObject.Find("QuestDatabase") == null ) {
				SceneManager.LoadScene("questlist");
				return;
			}

			questdb = GameObject.Find("QuestDatabase").GetComponent<questdatabase>();
			quest = questdb.currentquest;
			page = quest.currentpage;
			questactions = GameObject.Find("QuestDatabase").GetComponent<actions>();

			InitBackButton(quest.AllowReturn);

			TriggerOnStart();
		}

		protected void TriggerOnStart () {
			if ( page.onStart != null ) {
				page.onStart.Invoke();
			}
		}

		protected abstract void InitBackButton (bool shouldBeShown);

		protected virtual int FontSize {
			get {
				return DEFAULT_FONT_SIZE;
			}
		}

		/// <summary>
		/// Default implementation for the page state on end. 
		/// Must be overridden by subclasses that do not always set the page state to "succeeded" on end!
		/// </summary>
		/// <value>The page state on end.</value>
		protected virtual string PageStateOnEnd { 
			get { 
				return "succeeded";
			}
		}

		protected void onEnd () {
			page.state = PageStateOnEnd;

			if ( page.onEnd != null && page.onEnd.actions != null && page.onEnd.actions.Count > 0 ) {

				page.onEnd.Invoke();

			}
			else {
				//Debug.Log ("ending");
				GameObject questDBGO = GameObject.Find("QuestDatabase");
				if ( questDBGO != null ) {
					questDBGO.GetComponent<questdatabase>().endQuest();
				}

			}
		}


	}

}
