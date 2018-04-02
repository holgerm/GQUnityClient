using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using GQ.Client.Err;

namespace GQ.Client.UI.Foyer
{
	/// <summary>
	/// Abstract super class of all viewer controllers that show quest sets. E.g. the quest list and map in the foyer.
	/// </summary>
	public abstract class QuestContainerController : MonoBehaviour
	{

		#region Fields

		protected QuestInfoManager qim;

		protected Dictionary<int, QuestInfoUIC> QuestInfoControllers;

		#endregion


		#region Lifecycle API

		protected void Start ()
		{
			qim = QuestInfoManager.Instance;
			QuestInfoControllers = new Dictionary<int, QuestInfoUIC> ();
			qim.OnDataChange += OnQuestInfoChanged;
			qim.OnFilterChange += OnQuestInfoChanged;
		}

		public abstract void OnQuestInfoChanged (object sender, QuestInfoChangedEvent e);

		public abstract void UpdateView ();


		void OnDestroy ()
		{
			if (qim != null) {
				qim.OnDataChange -= OnQuestInfoChanged;
				qim.OnFilterChange -= OnQuestInfoChanged;
			}
		}

		#endregion

	}

}
