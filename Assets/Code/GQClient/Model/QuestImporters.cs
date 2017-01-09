using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using GQ.Client.Conf;
using System;
using GQ.Util;


namespace GQ.Client.Model {


	public static class QuestImportExtension {

		public static void StartGetQuestInfosFromServer (this QuestImporter_I importer) {

			if ( !IsSubclassOfOrSame(importer.GetType(), typeof(MonoBehaviour)) ) {
				
				throw new ArgumentException(
					"This method can only be called from a Unity component (MonoBehaviour or derived), but you called it from an object of type " + importer.GetType().FullName);
			}
			
			string url = 
				String.Format(
					"{0}/json/{1}/publicgamesinfo", 
					ConfigurationManager.GQ_SERVER_BASE_URL,
					ConfigurationManager.Current.portal
				);

			Download jsonDownload = new Download(url, 120000);

			jsonDownload.OnSuccess += (Download.SuccessCallback)((Download d) => {
				QuestInfo[] quests = JsonConvert.DeserializeObject<QuestInfo[]>(d.Www.text);
				importer.ImportDone(quests);				
			});
				
			((MonoBehaviour)importer).StartCoroutine(jsonDownload.startDownload());
		}


		public static void StartExtractQuestInfosFromFile (this QuestImporter_I importer, string path) {
			
			string json = File.ReadAllText(path);
			QuestInfo[] quests = JsonConvert.DeserializeObject<QuestInfo[]>(json);
			importer.ImportDone(quests);
		}


		private static bool IsSubclassOfOrSame (Type potentialDescendant, Type potentialBase) {
			
			return (
			    potentialDescendant.IsSubclassOf(potentialBase)
			    || potentialDescendant == potentialBase
			);
		}
	}


	public interface QuestImporter_I {
		
		void ImportDone (QuestInfo[] importedQuests);
	}


}
