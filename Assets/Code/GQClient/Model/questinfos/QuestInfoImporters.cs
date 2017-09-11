using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using GQ.Client.Conf;
using System;
using GQ.Client.Util;


namespace GQ.Client.Model {


	public static class QuestInfoImportExtension {

		/// <summary>
		/// Imports quest info from server for the current portal. The complete import process takes three steps:
		/// 
		/// 1. Loading the Infos JSON file from the server or locally - which is done by the given loader.
		/// 2. Parsing the JSON string and creating the QuestInfo objects
		/// 3. Importing the QuestInfo objects into the QuestManager
		/// 
		/// The first is done asynchonously via the Start... method. The second and third steps are done in a callback 
		/// that is called on success of step 1.
		/// </summary>
		/// <param name="importer">Importer.</param>
		public static void StartImportQuestInfos (this QuestInfoImporter_I importer, QuestInfoLoader_I loader) {

			loader.StartLoadingJSON(importer);
		}

		/// <summary>
		/// The callback method that is called on success after loading the JSON string which is given here as parameter.
		/// </summary>
		/// <param name="importer">Importer.</param>
		/// <param name="questInfoJSON">Quest info JSO.</param>
		public static void QuestInfoLoaded (this QuestInfoImporter_I importer, string questInfoJSON) {
			
			QuestInfo[] quests = JsonConvert.DeserializeObject<QuestInfo[]>(questInfoJSON);
			QuestInfoManager.Instance.Import(quests);	
			importer.ImportQuestInfoDone();
		}

		public static QuestInfo[] ParseQuestInfoJSON (string jsonString) {
			
			return JsonConvert.DeserializeObject<QuestInfo[]>(jsonString);
		}
	}


	public interface QuestInfoImporter_I {
		
		/// <summary>
		/// Signals that the complete process of importing the list of quest infos has been finished.
		/// </summary>
		void ImportQuestInfoDone ();
	}


	public interface QuestInfoLoader_I {
	
		void StartLoadingJSON (QuestInfoImporter_I importer);
	}


	public class QuestInfoLoaderFromServer : QuestInfoLoader_I {

		public void StartLoadingJSON (QuestInfoImporter_I importer) {
			
			string url = 
				String.Format(
					"{0}/json/{1}/publicgamesinfo", 
					ConfigurationManager.GQ_SERVER_BASE_URL,
					ConfigurationManager.Current.portal
				);

			Downloader jsonDownload = new Downloader(url: url, timeout: 120000);

			jsonDownload.OnSuccess += (AbstractDownloader d, DownloadEvent e) => {
				importer.QuestInfoLoaded(e.Message);				
			};

			Base.Instance.StartCoroutine(jsonDownload.StartDownload());
		}
	}
}
