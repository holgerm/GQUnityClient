using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using GQ.Client.Util;
using GQ.Client.Conf;
using GQ.Client.Model;
using GQ.Client.Util;
using Newtonsoft.Json;
using System.IO;
using GQ.Client.Err;


namespace GQ.Client.Model {

	/// <summary>
	/// Manages the meta data for all quests available: locally on the device as well as remotely on the server.
	/// </summary>
	public class QuestInfoManager : IEnumerable<QuestInfo> {

		#region store & access data

		public static string LocalQuestsPath {
			get {
				if (!Directory.Exists(Application.persistentDataPath + "/quests/")) {
					Directory.CreateDirectory(Application.persistentDataPath + "/quests/");
				}
				return Application.persistentDataPath + "/quests/";
			}
		}

		public static string LocalQuestInfoJSONPath {
			get {
				return LocalQuestsPath + "infos.json";
			}
		}

		public Dictionary<int, QuestInfo> QuestDict {
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

		public void Update (string json) {
			QuestInfo[] quests;

			try {
				quests = JsonConvert.DeserializeObject<QuestInfo[]>(json);
			}
			catch (Exception e) {
				Log.SignalErrorToDeveloper(
					"Error in JSON while trying to update quest infos: {0}\nJSON:\n{1}",
					e.Message,
					json
				);
				return;
			}

			if ( quests == null || quests.Length == 0)
				return;
			
			foreach ( var q in quests ) {
				if ( q.Id <= 0 )
					continue;

				if (QuestDict.ContainsKey (q.Id)) {
					// override:
					QuestInfo oldQ = QuestDict [q.Id];
					QuestDict [q.Id] = q;
					RaiseChange (
						new QuestInfoChangedEvent (
							String.Format ("Info for quest {0} changed.", q.Name),
							ChangeType.Changed,
							newQuestInfo: q,
							oldQuestInfo: oldQ
						)
					);
				}
				else {
					QuestDict.Add ((int)q.Id, q);
					RaiseChange (
						new QuestInfoChangedEvent (
							String.Format ("Info for quest {0} added.", q.Name),
							ChangeType.Added,
							newQuestInfo: q,
							oldQuestInfo: null
						)
					);
				}
			}

			// persist the quest infos from the update QuestDict:
			List<QuestInfo> questInfoList = new List<QuestInfo>(QuestDict.Values);
			string questInfosJSON = 
				(questInfoList.Count == 0) 
				? "[]"
				: JsonConvert.SerializeObject(questInfoList, Newtonsoft.Json.Formatting.Indented);
			File.WriteAllText(QuestInfoManager.LocalQuestInfoJSONPath, questInfosJSON);

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

	public class QuestInfoChangedEvent : EventArgs 
	{
		public string Message { get; protected set; }
		public ChangeType ChangeType { get; protected set; }
		public QuestInfo NewQuestInfo { get; protected set; }
		public QuestInfo OldQuestInfo { get; protected set; }

		public QuestInfoChangedEvent(
			string message = "", 
			ChangeType type = ChangeType.Changed, 
			QuestInfo newQuestInfo = null, 
			QuestInfo oldQuestInfo = null
		)
		{
			Message = message;
			ChangeType = type;
			NewQuestInfo = newQuestInfo;
			OldQuestInfo = oldQuestInfo;
		}

	}

	public enum ChangeType {
		Added, Removed, Changed
	}
		

}
