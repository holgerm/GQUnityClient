using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace GQ.Client.Model {


	public interface QuestImporter_I {
		
		QuestInfo[] import ();
	}


	public class ServerQuestImporter : QuestImporter_I {
		
		public QuestInfo[] import () {
			
			return null;
		}

	}

}
