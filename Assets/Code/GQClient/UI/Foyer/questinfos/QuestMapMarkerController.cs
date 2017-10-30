using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using UnityEngine.UI;
using System;
using GQ.Client.Util;
using GQ.Client.Err;
using GQ.Client.GQEvents;
using GQ.Client.Conf;
using GQ.Client.UI.Dialogs;
using System.IO;
using GQ.Client.FileIO;

//using UnityEngine.Events;


namespace GQ.Client.UI.Foyer
{

	/// <summary>
	/// Represents one quest info object on the map within the foyer.
	/// </summary>
	public class QuestMapMarkerController : QuestInfoController
	{

		#region Content and Structure

		protected static readonly string PREFAB = "QuestInfoMapMarker";

		protected const string NAME_PATH = "Name";

		#endregion


		#region Event Reaction Methods


		public void Download ()
		{
			// Load quest data: game.xml
			Downloader downloadGameXML = 
				new Downloader (
					url: QuestManager.GetQuestURI (data.Id), 
					timeout: ConfigurationManager.Current.downloadTimeOutSeconds * 1000,
					targetPath: QuestManager.GetLocalPath4Quest (data.Id) + QuestManager.QUEST_FILE_NAME
				);
			new DownloadDialogBehaviour (downloadGameXML, "Loading quest");

			// analyze game.xml, gather all media info compare to local media info and detect missing media
			PrepareMediaInfoList prepareMediaInfosToDownload = 
				new PrepareMediaInfoList ();
			new SimpleDialogBehaviour (
				prepareMediaInfosToDownload,
				"Synching Quest Data",
				"Preparing media information."
			);

			// download all missing media info
			MultiDownloader downloadMediaFiles =
				new MultiDownloader (1);
			new SimpleDialogBehaviour (
				downloadMediaFiles,
				"Synching Quest Data",
				"Loading media files."
			);
			downloadMediaFiles.OnTaskCompleted += (object sender, TaskEventArgs e) => {
				data.LastUpdateOnDevice = data.LastUpdateOnServer;
			};

			// store current media info locally
			ExportMediaInfoList exportLocalMediaInfo =
				new ExportMediaInfoList ();
			new SimpleDialogBehaviour (
				exportLocalMediaInfo,
				"Synching Quest Data",
				"Saving updated media info."
			);

			ExportQuestInfosToJSON exportQuestsInfoJSON = 
				new ExportQuestInfosToJSON ();
			new SimpleDialogBehaviour (
				exportQuestsInfoJSON,
				"Updating quests",
				"Saving Quest Data"
			);

			TaskSequence t = 
				new TaskSequence (downloadGameXML);
			t.AppendIfCompleted (prepareMediaInfosToDownload);
			t.Append (downloadMediaFiles);
			t.AppendIfCompleted (exportLocalMediaInfo);
			t.Append (exportQuestsInfoJSON);

			t.Start ();
		}

		public void Play ()
		{
			// Load quest data: game.xml
			LocalFileLoader loadGameXML = 
				new LocalFileLoader (
					filePath: QuestManager.GetLocalPath4Quest (data.Id) + QuestManager.QUEST_FILE_NAME
				);
			new DownloadDialogBehaviour (loadGameXML, "Loading quest");

			QuestStarter questStarter = new QuestStarter ();

			TaskSequence t = 
				new TaskSequence (loadGameXML, questStarter);

			t.Start ();
		}

		#endregion


		#region Runtime API

		public static GameObject Create (GameObject root, QuestInfo qInfo)
		{
			// CReate the view object for this controller:
			GameObject go = PrefabController.Create (PREFAB, root);
			go.name = PREFAB + " (" + qInfo.Name + ")";
			QuestMapMarkerController ctrl = go.GetComponent<QuestMapMarkerController> ();
			ctrl.data = qInfo;
			ctrl.data.OnChanged += ctrl.UpdateView;
			ctrl.UpdateView ();
			return go;
		}

		public override void UpdateView ()
		{
			// Update Info-Icon:
			// TODO: enable Info dialog

			// Update Name:
			Name.text = data.Name;
			// Set Name button for download or play or nothing:
			Button nameButton = Name.gameObject.GetComponent<Button> ();
			Button.ButtonClickedEvent namebuttonEvent = nameButton.onClick;
			namebuttonEvent.RemoveAllListeners ();
			if (data.IsOnServer && !data.IsOnDevice) {
				namebuttonEvent.AddListener (() => {
					Download ();
				});
			}
			if (data.IsOnDevice) {
				namebuttonEvent.AddListener (() => {
					Play ();
				});
			}

		}

		#endregion
	}

}