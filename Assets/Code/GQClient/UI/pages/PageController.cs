using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnityEngine.SceneManagement;
using GQ.Client.Util;

namespace GQ.Client.UI
{

	public class PageController : UIController
	{

		protected Page page;

		// Use this for initialization
		public virtual void Start ()
		{
			QuestManager qm = QuestManager.Instance;
			if (qm.CurrentQuest == null || qm.CurrentPage == Page.Null) {
				SceneManager.LoadScene (Base.FOYER_SCENE);
				return;
			}

			page = qm.CurrentPage;
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}
	}
}
