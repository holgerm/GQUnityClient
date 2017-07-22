using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using GQ.Util;
using GQ.Client.Conf;


namespace GQ.Client.Model {

	/// <summary>
	/// Manages the meta data for all quests available: locally on the device as well as remotely on the server.
	/// </summary>
	public class QuestInfoManager : IEnumerable<QuestInfo> {

		#region store & access data

		protected Dictionary<int, QuestInfo> QuestDict {
			get;
			set;
		}

		public IEnumerator<QuestInfo> GetEnumerator() {
			return QuestDict.Values.GetEnumerator();
		}

		IEnumerator<QuestInfo> IEnumerable<QuestInfo>.GetEnumerator() {
			return QuestDict.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return QuestDict.Values.GetEnumerator();
		}

		public int Count {
			get {
				return QuestDict.Count;
			}
		}

		public QuestInfo GetQuestInfo (int id) {
			QuestInfo questInfo;
			return (QuestDict.TryGetValue(id, out questInfo) ? questInfo : null);
		}

		public void Import (QuestInfo[] quests) {
			if ( quests == null || quests.Length == 0)
				return;
			
			foreach ( var q in quests ) {
				if ( q.Id == null )
					continue;

				if (QuestDict.ContainsKey(q.Id)) {
					// override:
					QuestDict [q.Id] = q;
				}
				QuestDict.Add((int)q.Id, q);
			}
		}

		#endregion


		#region Quest Info Changed Event

		public delegate void ChangeCallback (object sender, QuestInfoChangedEvent e);

		public event ChangeCallback OnChange;

		protected virtual void RaiseChange (QuestInfoChangedEvent e)
		{
			if (OnChange != null)
				OnChange (this, e);
		}

		#endregion


		#region Update Quest Infos

		public delegate void Callback (object sender, UpdateQuestInfoEventArgs e);

		public event Callback OnUpdateStart; 
		public event Callback OnUpdateProgress;
		public event Callback OnUpdateStep;
		public event Callback OnUpdateTimeout; // TODO replace by Error
		public event Callback OnUpdateSuccess;
		public event Callback OnUpdateError;

		/// <summary>
		/// Use this method to raise an event based on Callback delegate type, e.g. OnUpdateStart, OnUpdateProgress, etc.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="e">E.</param>
		protected virtual void Raise (Callback callback, UpdateQuestInfoEventArgs e)
		{
			if (callback != null)
				callback (this, e);
		}

		public void RaiseUpdateStart(UpdateQuestInfoEventArgs e) {
			Raise (OnUpdateStart, e); 
		}

		public void RaiseUpdateProgress(UpdateQuestInfoEventArgs e) {
			Raise (OnUpdateProgress, e);
		}

		public void RaiseUpdateStep(UpdateQuestInfoEventArgs e) {
			Raise (OnUpdateStep, e);
		}

		public void RaiseUpdateSuccess(UpdateQuestInfoEventArgs e) {
			Raise (OnUpdateSuccess, e);
		}

		public void RaiseUpdateError(UpdateQuestInfoEventArgs e) {
			Raise (OnUpdateError, e);
		}

		/// <summary>
		/// Updates the quest infos successively from different sources: 
		/// typically locally from the device and then from the connected server. 
		/// 
		/// This method is executed mostly as Coroutine, hence feedback is given
		/// in form of raised events in between the execution steps.
		/// </summary>
		public void UpdateQuestInfos(InfoLoader[] loaders) 
		{
			int totalSteps = loaders.Length + 1;
			int currentStep = 1;

			// new LocalInfoLoader ().Start(currentStep++, totalSteps));
			foreach(InfoLoader loader in loaders) 
			{
				loader.Start();
			}
		}
			
		#endregion


		#region singleton

		private static QuestInfoManager _instance = null;

		public static QuestInfoManager Instance {
			get {
				if ( _instance == null ) {
					_instance = new QuestInfoManager();
				}
				return _instance;
			}
			set {
				_instance = value;
			}
		}

		public static void Reset () {
			_instance = null;
		}

		public QuestInfoManager() {
			QuestDict = new Dictionary<int, QuestInfo> ();
		}

		#endregion
	}

	public class UpdateQuestInfoEventArgs : EventArgs 
	{
		public string Message { get; protected set; }
		public float Progress { get; protected set; }
		public int Step { get; protected set; }
		public int StepsTotal { get; protected set; }

		public UpdateQuestInfoEventArgs(
			string message = "", 
			float progress = 0f, 
			int step = 0, 
			int stepsTotal = 0)
		{
			Message = message;
			Progress = progress;
			Step = step;
			StepsTotal = stepsTotal;
		}
	}

	public class QuestInfoChangedEvent : EventArgs 
	{
		public string Message { get; protected set; }
		public ChangeEventType ChangeType { get; protected set; }
		public QuestInfo NewQuestInfo { get; protected set; }
		public QuestInfo OldQuestInfo { get; protected set; }

		public QuestInfoChangedEvent(
			string message = "", 
			ChangeEventType type = ChangeEventType.Changed, 
			QuestInfo NewQuestInfo = null, 
			QuestInfo OldQuestInfo = null
		)
		{
			Message = message;
			ChangeType = type;
		}

	}

	public enum ChangeEventType {
		Added, Removed, Changed
	}
		

}
