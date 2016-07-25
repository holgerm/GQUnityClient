using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

namespace GQ.Client.UI.Pages {

	public abstract class PageController : MonoBehaviour {

		private const int DEFAULT_FONT_SIZE = 20;

		public QuestPage page;
		public questdatabase questdb;
		public Quest quest;
		public actions questactions;

		protected virtual void Start () { 

			if ( GameObject.Find("QuestDatabase") == null ) {
				Application.LoadLevel(0);
				return;
			}

			questdb = GameObject.Find("QuestDatabase").GetComponent<questdatabase>();
			quest = GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest;
			page = GameObject.Find("QuestDatabase").GetComponent<questdatabase>().currentquest.currentpage;
			questactions = GameObject.Find("QuestDatabase").GetComponent<actions>();

			InitBackButton(shouldShowBackButton);

			TriggerOnStart();
		}

		protected void TriggerOnStart () {
			if ( page.onStart != null ) {
				page.onStart.Invoke();
			}
		}

		/// <summary>
		/// Each page should show a back button with function to go back to the last page, if adequate. 
		/// Depends on global quest setting individualReturnDefinitions and action attribute allowReturn.
		/// </summary>
		private bool shouldShowBackButton {
			get {
				bool allowReturn = false;
				if ( questdb.individualReturnDefinitions ) {
					allowReturn = 
					questdb.allowReturn
					&& questdb.currentquest.previouspages.Count > 0
					&& questdb.currentquest.previouspages[questdb.currentquest.previouspages.Count - 1] != null;
				}
				else {
					allowReturn = 
					questdb.currentquest.previouspages.Count > 0
					&& questdb.currentquest.previouspages[questdb.currentquest.previouspages.Count - 1] != null
					&& !questdb.currentquest.previouspages[questdb.currentquest.previouspages.Count - 1].type.Equals("MultipleChoiceQuestion")
					&& !questdb.currentquest.previouspages[questdb.currentquest.previouspages.Count - 1].type.Equals("TextQuestion");
				}

				return allowReturn;
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
