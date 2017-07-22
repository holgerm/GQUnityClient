using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Util;
using GQ.Client.Conf;
using Newtonsoft.Json;

namespace GQ.Client.Model {

	public class ServerQuestInfoLoader : InfoLoader {

		QuestInfoManager qm = QuestInfoManager.Instance;

		public override void Start() {
			// Start the gathering:
			qm.RaiseUpdateStep(
				new UpdateQuestInfoEventArgs (
					message: "Gathering local Quests from Device")
			);

			// 1. Get locally stored quest infos

			// 2. Download Server-based quest infos
			Download jsonDownload = 
				new Download(
					ConfigurationManager.UrlPublicQuestsJSON, 
					120000
				);

			jsonDownload.OnProgress += 
				(Download downloader, DownloadEvent e) => 
			{
				qm.RaiseUpdateProgress(new UpdateQuestInfoEventArgs(progress: e.Progress));
			};

			jsonDownload.OnSuccess += 
				(Download downloader, DownloadEvent e) => 
			{
				// raise the update event:
				qm.RaiseUpdateSuccess(new UpdateQuestInfoEventArgs());

				// extract and import the loaded quests:
				extractQuestInfosFromJSON(downloader, e);
			};


			jsonDownload.OnError += 
				(Download downloader, DownloadEvent e) => 
			{
				qm.RaiseUpdateError(new UpdateQuestInfoEventArgs(message: e.Message));
			};

			Base.Instance.StartCoroutine(jsonDownload.StartDownload());
		}

		protected void extractQuestInfosFromJSON(Download d, DownloadEvent e) {
			QuestInfo[] quests = JsonConvert.DeserializeObject<QuestInfo[]>(d.Response);
			qm.Import (quests);
		}

	}
}
