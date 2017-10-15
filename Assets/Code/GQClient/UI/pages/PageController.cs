using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnityEngine.SceneManagement;
using GQ.Client.Util;
using GQ.Client.Err;

namespace GQ.Client.UI
{

	public abstract class PageController : UIController
	{

		protected Page page;
		protected QuestManager qm;

		#region MonoBehaviour

		public virtual void Start ()
		{
			bool resumingToFoyer = false;

			qm = QuestManager.Instance;
			if (qm.CurrentQuest == null || qm.CurrentPage == Page.Null) {
				SceneManager.LoadSceneAsync (Base.FOYER_SCENE);
				resumingToFoyer = true;
				return;
			}

			page = qm.CurrentPage;

			if (page == null) {
				if (!resumingToFoyer)
					Log.SignalErrorToDeveloper (
						"Page is null in quest {0}", 
						QuestManager.Instance.CurrentQuest.Id.ToString ()
					);
				return;
				// TODO What should we do now? End quest?
			}

			page.PageCtrl = this;

			Initialize ();
		}

		#endregion


		#region Runtime API

		public abstract void Initialize ();

		/// <summary>
		/// Override this method to react on Back Button CLick (or similar events).
		/// </summary>
		public virtual void OnBack ()
		{
			Debug.Log ("OnBack() not yet implemented for page controller " + GetType ().Name);
		}

		/// <summary>
		/// Override this method to react on Forward Button CLick (or similar events).
		/// </summary>
		public virtual void OnForward ()
		{
			page.End ();
		}

		#endregion

	}
}
