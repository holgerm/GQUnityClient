using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnityEngine.SceneManagement;
using GQ.Client.Util;
using GQ.Client.Err;

namespace GQ.Client.UI
{

	public class PageController : UIController
	{

		protected Page page;

		protected bool resumingToFoyer = false;

		// Use this for initialization
		public virtual void Start ()
		{
			QuestManager qm = QuestManager.Instance;
			if (qm.CurrentQuest == null || qm.CurrentPage == Page.Null) {
				SceneManager.LoadSceneAsync (Base.FOYER_SCENE);
				resumingToFoyer = true;
				return;
			}

			page = qm.CurrentPage;
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}

		#region Back and Forward

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
