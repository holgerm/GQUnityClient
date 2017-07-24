using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Util;
using GQ.Client.Conf;
using Newtonsoft.Json;
using GQ.Client.Util;

namespace GQ.Client.Model {

	public class ServerQuestInfoLoader : Task {

		QuestInfoManager qm = QuestInfoManager.Instance;

		public override void Start(int step = 0) {
			// Start the gathering:
			qm.RaiseUpdateStart(
				new UpdateQuestInfoEventArgs (
					message: "Gathering Quests from Server",
					step: step
				)
			);

			Download jsonDownload = 
				new Download(
					ConfigurationManager.UrlPublicQuestsJSON, 
					120000
				);

			//ON PROGRESS:
			jsonDownload.OnProgress += 
				(Download downloader, DownloadEvent e) => 
			{
				qm.RaiseUpdateProgress(new UpdateQuestInfoEventArgs(progress: e.Progress));
			};

			// ON SUCCESS:
			jsonDownload.OnSuccess += 
				(Download downloader, DownloadEvent e) => 
			{
				// raise the update event:
				qm.RaiseUpdateSuccess(new UpdateQuestInfoEventArgs());

				// extract and import the loaded quests:
				extractQuestInfosFromJSON(downloader, e);

				// Task completed:
				RaiseTaskCompleted();
			};

			// ON ERROR:
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
