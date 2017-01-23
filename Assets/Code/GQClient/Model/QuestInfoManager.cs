using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace GQ.Client.Model {

	/// <summary>
	/// Manages the meta data for all quests avalibale as well locally on the device as remotely on the server.
	/// </summary>
	public class QuestInfoManager {

		#region store & access data

		private Dictionary <int, QuestInfo> _questDict = new Dictionary <int, QuestInfo>();

		public Dictionary<int, QuestInfo> QuestDict {
			get {
				return _questDict;
			}
		}

		public int Count {
			get {
				return _questDict.Count;
			}
		}

		public QuestInfo GetQuestInfo (int id) {
			QuestInfo questInfo;
			return (_questDict.TryGetValue(id, out questInfo) ? questInfo : null);
		}

		#endregion


		#region quest info functions

		public void Import (QuestInfo[] quests) {
			if ( quests == null )
				return;
			
			foreach ( var q in quests ) {
				if ( q.id == null )
					continue;
				
				_questDict.Add((int)q.id, q);
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

		#endregion
	}

}
