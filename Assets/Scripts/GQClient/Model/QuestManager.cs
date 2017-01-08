using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace GQ.Client.Model {


	public class QuestManager {

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


		#region import functions

		public void ImportQuestInfo (QuestImporter_I importer) {
			
			QuestInfo[] quests = importer.import();

			foreach ( var q in quests ) {
				if ( q.id == null )
					continue;
				
				_questDict.Add((int)q.id, q);
			}
		}

		#endregion


		#region singleton

		private static QuestManager _instance = null;

		public static QuestManager Instance {
			get {
				if ( _instance == null ) {
					_instance = new QuestManager();
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
