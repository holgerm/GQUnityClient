using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using GQ.Client.Err;

namespace GQ.Client.UI.Foyer
{
	/// <summary>
	/// Abstract super class of all viewer contorlers that show quest sets. E.g. the quest list and map in the foyer.
	/// </summary>
	public abstract class QuestContainerController : MonoBehaviour
	{

		#region Fields

		protected QuestInfoManager qim;

		private Dictionary<int, QuestInfoController> questInfoControllers;

		protected Dictionary<int, QuestInfoController> QuestInfoControllers {
			get {
				if (questInfoControllers == null) {
					questInfoControllers = new Dictionary<int, QuestInfoController> ();
				}
				return questInfoControllers;
			}
		}

		#endregion


		#region Lifecycle API

		protected void Start ()
		{
			qim = QuestInfoManager.Instance;
			qim.OnChange += OnQuestInfoChanged;
		}

		public abstract void OnQuestInfoChanged (object sender, QuestInfoChangedEvent e);

		public abstract void UpdateView ();


		void OnDestroy ()
		{
			qim.OnChange -= OnQuestInfoChanged;
		}

		#endregion

	}

}
