using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;

namespace GQ.Client.UI.Foyer
{
	/// <summary>
	/// Abstract super class of all viewer contorlers that show quest sets. E.g. the quest list and map in the foyer.
	/// </summary>
	public abstract class QuestSetViewerController : MonoBehaviour {

		#region Fields

		public Transform InfoList;

		protected QuestInfoManager qim;

		protected Dictionary<int, QuestInfoController> questInfoControllers;

		#endregion


		#region Lifecycle API

		void Start ()
		{
			qim = QuestInfoManager.Instance;

			qim.OnChange += OnQuestInfoChanged;

			if (questInfoControllers == null) {
				questInfoControllers = new Dictionary<int, QuestInfoController> ();
			}

			// TODO soll wirklich der ListController hier dem Manager sagen, dass er ein update braucht?
			// sollte doch eher passieren, wenn wieder online, oder user interaktion, oder letztes updates lange her...
			// aber vielleicht eben doch auch hier beim Start des Controllers, d.h. bei neuer Anzeige der Liste.
			if (!ListAlreadyShownBefore) {
				qim.UpdateQuestInfos ();
				ListAlreadyShownBefore = true;
			} else {
				UpdateView ();
			}
		}

		public abstract void OnQuestInfoChanged (object sender, QuestInfoChangedEvent e);

		public abstract void UpdateView ();

		static bool _listAlreadyShownBefore = false;

		public static bool ListAlreadyShownBefore {
			get {
				return _listAlreadyShownBefore;
			}
			set {
				_listAlreadyShownBefore = value;
			}
		}


		void OnDestroy ()
		{
			qim.OnChange -= OnQuestInfoChanged;
		}

		#endregion

	}

}
