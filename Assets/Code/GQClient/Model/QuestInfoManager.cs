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
			if ( quests == null )
				return;
			
			foreach ( var q in quests ) {
				if ( q.Id == null )
					continue;
				
				QuestDict.Add((int)q.Id, q);
			}
		}

		#endregion


		#region Quest Info Changed Event

		public delegate void ChangeCallback (object sender, QuestInfoChangedEvent e);

		public event ChangeCallback OnChange;

		#endregion


		#region Update Quest Infos

		public delegate void Callback (object sender, UpdateQuestInfoEventArgs e);

		public event Callback OnUpdateStart;
		public event Callback OnUpdateProgress;
		public event Callback OnUpdateStep;
		public event Callback OnUpdateTimeout;
		public event Callback OnUpdateSuccess;
		public event Callback OnUpdateError;

		public void UpdateQuestInfoList() {
			// TODO implement behavior of getting local and remote quest infos ...

			// Start the gathering:
			OnUpdateStart(this, new UpdateQuestInfoEventArgs("Loading Quest Information"));

			// 1. Get locally stored quest infos

			// 2. Download Server-based quest infos
			Download jsonDownload = 
				new Download(
					ConfigurationManager.UrlPublicQuestsJSON, 
					120000
				);

			jsonDownload.OnProgress += 
				(Download downloader, float percentLoaded) => 
				{
					OnUpdateProgress(this, new UpdateQuestInfoEventArgs(progress: percentLoaded));
				};

			jsonDownload.OnSuccess += 
				(Download downloader) => 
				{
					OnUpdateSuccess(this, new UpdateQuestInfoEventArgs());
				};


			jsonDownload.OnError += 
				(Download downloader, string msg) => 
				{
					OnUpdateError(this, new UpdateQuestInfoEventArgs(message: msg));
				};

			Base.Instance.StartCoroutine(jsonDownload.StartDownload());

			// 3. Mix both

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

		public UpdateQuestInfoEventArgs(string message = "", float progress = 0f)
		{
			Message = message;
			Progress = progress;
		}
	}

	public class QuestInfoChangedEvent : EventArgs 
	{
		public string Message { get; protected set; }
		public ChangeType ChangeType { get; protected set; }
		public QuestInfo NewQuestInfo { get; protected set; }
		public QuestInfo OldQuestInfo { get; protected set; }

		public QuestInfoChangedEvent(
			string message = "", 
			ChangeType type = ChangeType.Changed, 
			QuestInfo NewQuestInfo = null, 
			QuestInfo OldQuestInfo = null
		)
		{
			Message = message;
			ChangeType = type;
		}

	}

	public enum ChangeType {
		Added, Removed, Changed
	}
		

}
