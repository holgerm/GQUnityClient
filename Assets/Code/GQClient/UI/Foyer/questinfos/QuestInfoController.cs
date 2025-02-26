﻿using System;
using Code.GQClient.Err;
using GQClient.Model;
using TMPro;

namespace Code.GQClient.UI.Foyer.questinfos
{
    /// <summary>
    /// Abstact class for all kinds of view controllers presenting quest infos. E.g. list elements in the foyer list, or markers on the foyer map.
    /// </summary>
    public abstract class QuestInfoUIC : PrefabController, IComparable<QuestInfoUIC> {

		public QuestInfo data;
		public TextMeshProUGUI Name;

		/// <summary>
		/// Returns a value greater than zero in case this object is considered greater than the given other. 
		/// A return value of 0 signals that both objects are equal and 
		/// a value less than zero means that this object is less than the given other one.
		/// </summary>
		/// <param name="otherCtrl">Other ctrl.</param>
		public int CompareTo (QuestInfoUIC otherCtrl)
		{
			return data.CompareTo (otherCtrl.data);
		}

		/// <summary>
		/// Performs an update of the controller data to the given newQuestInfo.
		/// </summary>
		public void UpdateData(QuestInfo newInfo) {
            // some values will be kept (until we really do the quest update, this here is only the quest-info update!):
            data = newInfo;
            data.OnChanged += UpdateView;
            UpdateView (newInfo);
		}

		public abstract void UpdateView (QuestInfo questInfo);

		public override void Destroy ()
		{
			base.Destroy ();
		}

		void OnDestroy ()
		{
			if (this == null) {
				Log.SignalErrorToDeveloper ("QICtrl: this == null in OnDestroy()".Red ());
			} else {
				if (data != null)
					data.OnChanged -= UpdateView;
			}
		}
			
	}

}
