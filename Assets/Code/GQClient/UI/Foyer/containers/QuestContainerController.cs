using System.Collections.Generic;
using GQClient.Model;
using Code.GQClient.UI.author;
using Code.GQClient.UI.Foyer.questinfos;
using UnityEngine;

namespace Code.GQClient.UI.Foyer.containers
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
            ShowDeleteOption.DeleteOptionVisibilityChanged += UpdateElementViews;
        }

		public abstract void OnQuestInfoChanged (object sender, QuestInfoChangedEvent e);

		public abstract void RegenerateAll ();


		void OnDestroy ()
		{
			if (qim != null) {
				qim.OnDataChange -= OnQuestInfoChanged;
				qim.OnFilterChange -= OnQuestInfoChanged;
                ShowDeleteOption.DeleteOptionVisibilityChanged -= UpdateElementViews;
            }
		}

        public void Update()
        {
            //Debug.Log("QuestContainerController.Update()");
        }

        public abstract void UpdateElementViews();

        #endregion

    }

}
