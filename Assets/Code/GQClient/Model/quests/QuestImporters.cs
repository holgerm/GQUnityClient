using UnityEngine;
using System.Collections;
using System;

namespace GQ.Client.Model {

	[Obsolete]
	public static class QuestImportExtension {

		/// <summary>
		/// Starts the complete process of importing a quest, given by its id. 
		/// 
		/// This process includes:
		/// 1. downloading the quest xml
		/// 2. creating a runtime model of the quest
		/// 3. loading all media files needed to start the quest later
		/// 
		/// There are three callbacks called during this process:
		/// a. QuestXMLLoaded() 
		/// b. EnoughMediaFilesLoadedToStartQuest() 
		/// c. AllQuestMediaFilesLoaded()
		/// </summary>
		/// <param name="importer">Importer.</param>
		/// <param name="questID">Quest I.</param>
		/// <param name="loader">Loader.</param>
		public static void StartImportQuest (this QuestImporter_I importer, int questID, QuestLoader_I loader) {

			loader.StartLoadingQuestXML(importer, questID);
		}

		public static void QuestXMLLoaded (this QuestImporter_I importer, string questXML) {

			// first: create all model objects that represent this quest at runtime
//			Quest quest = CreateQuestModel(string questXML);

			// thereby: determine and collect all media files that have to be loaded

			// second: download all media files and call another callback: AllQuestMediaFilesLoaded()

			// TODO: we could even call another callback before finish: EnoughMediaFilesLoadedToStartQuest()
			// then we could start the quest and load the rest of the media files in the background.

		}

		public static void AllMediaFilesLoaded () {
			
		}


	}


	public interface QuestImporter_I {

		/// <summary>
		/// Signals that enough files have been loaded so that we can start the quest (and show the first page e.g.)
		/// </summary>
		void EnoughMediaFilesLoadedToStartQuest ();

		/// <summary>
		/// Signals that the complete import process has been finished.
		/// </summary>
		void ImportQuestDone ();
	}


	public interface QuestLoader_I {

		void StartLoadingQuestXML (QuestImporter_I importer, int questID);
	}

}
