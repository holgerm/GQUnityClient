using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using UnityEngine.UI;
using System;
using GQ.Client.Util;
using GQ.Client.Err;
using GQ.Client.Event;
using UnityEngine.Events;
using UnityEditor.Events;
using GQ.Client.Conf;
using GQ.Client.UI.Dialogs;
using System.IO;
using GQ.Client.FileIO;

namespace GQ.Client.UI.Foyer {

	/// <summary>
	/// Represents one quest info object in a list within the foyer.
	/// </summary>
	public class QuestInfoController : PrefabController, IComparable<QuestInfoController> {

		#region Content and Structure

		protected static readonly string PREFAB = "QuestInfo";

		protected QuestInfo data;
		public Text Name;
		protected const string NAME_PATH = "Name";
		public Button DownloadButton;
		public Button StartButton;
		public Button DeleteButton;
		public Button DeleteWarnButton;
		public Button UpdateButton;
		public Button DowngradeButton;

		#endregion


		#region Internal UI Control Functions

		protected void HideAllButtons() {
			DownloadButton.gameObject.SetActive (false);
			StartButton.gameObject.SetActive (false);
			DeleteButton.gameObject.SetActive (false);
			UpdateButton.gameObject.SetActive (false);
			DowngradeButton.gameObject.SetActive (false);
		}

		/// <summary>
		/// Shows the button and add the given method to the onClick listener.
		/// </summary>
		/// <param name="button">Button.</param>
		/// <param name="actionCallback">Action callback.</param>
		protected void SetButtons(params Button[] buttons) {
			HideAllButtons ();
			foreach (Button button in buttons) {
				button.gameObject.SetActive (true);
				button.interactable = true;
			}
			// in case we can start this quest, we also allow clicks on the quest name to start it:
			Button.ButtonClickedEvent namebuttonEvent = Name.GetComponent<Button> ().onClick;
			if (StartButton.gameObject.activeInHierarchy) {
				namebuttonEvent.RemoveAllListeners ();
				namebuttonEvent.AddListener (Play);
			} else {
				namebuttonEvent.RemoveAllListeners ();
			}
		}

		/// <summary>
		/// Returns a value greater than zero in case this object is considered greater than the given other. 
		/// A return value of 0 signals that both objects are equal and 
		/// a value less than zero means that this object is less than the given other one.
		/// </summary>
		/// <param name="otherCtrl">Other ctrl.</param>
		public int CompareTo(QuestInfoController otherCtrl) {
			return data.CompareTo (otherCtrl.data);
		}

		#endregion


		#region Event Reaction Methods

		public void Download() {
			// Load quest data: game.xml
			Downloader downloadGameXML = 
				new Downloader (
					url: QuestManager.GetQuestURI(data.Id), 
					timeout: ConfigurationManager.Current.downloadTimeOutSeconds * 1000,
					targetPath: QuestManager.GetLocalPath4Quest(data.Id) + QuestManager.QUEST_FILE_NAME
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
				QuestInfo i = QuestInfoManager.Instance.GetQuestInfo(data.Id);
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

			t.OnTaskCompleted += (object sender, TaskEventArgs e) => {
				data.CurrentMode = QuestInfo.Mode.Deletable;
			};

			t.Start ();

		}

		public void Delete() {
			// TODO in case we are in DeleteWithWarning state we show a dialog with awarning and two options: Delete and Cancel.

			Debug.Log ("Want to delete: " + QuestManager.GetLocalPath4Quest (data.Id));
			Files.DeleteDirCompletely (QuestManager.GetLocalPath4Quest (data.Id));
			data.CurrentMode = QuestInfo.Mode.OnServer;
		}

		public void Play() {
			// TODO
			Debug.Log("TODO: Implement play method! Trying to start quest " + data.Name);
		}


		public void UpdateQuest() {
			// TODO
			Debug.Log("TODO: Implement update method! Trying to update quest " + data.Name);
		}

		public void Downgrade() {
			// TODO
			Debug.Log("TODO: Implement downgrade method! Trying to downgrade quest " + data.Name);
			data.CurrentMode = QuestInfo.Mode.Predeployed;
		}


		#endregion


		#region Runtime API

		public static GameObject Create(GameObject root, QuestInfo qInfo) 
		{
			// CReate the view object for this controller:
			GameObject go = PrefabController.Create (PREFAB, root);
			QuestInfoController ctrl = go.GetComponent<QuestInfoController> ();
			ctrl.data = qInfo;
			ctrl.data.OnStateChanged += ctrl.UpdateView;
			ctrl.data.CurrentMode = QuestInfo.Mode.OnServer;
			return go;
		}

		public override void Destroy() {
			data.OnStateChanged -= UpdateView;
			base.Destroy ();
		}

		public void SetContent(QuestInfo q) 
		{
			data = q;

			Name.text = q.Name;

			// TODO: enable Info dialog

			UpdateView ();
		}

		void UpdateView ()
		{
			// TODO: enable Info dialog

			// Show Name:
			Name.text = data.Name;

			// Show Buttons:
			switch (data.CurrentMode) {
			case QuestInfo.Mode.OnServer:
				HideAllButtons ();
				SetButtons (DownloadButton);
				break;
			case QuestInfo.Mode.Deletable:
			case QuestInfo.Mode.DeletableWithWarning:
				HideAllButtons ();
				SetButtons (StartButton, DeleteButton);
				break;
			case QuestInfo.Mode.Updatable:
				HideAllButtons ();
				SetButtons (StartButton, UpdateButton, DeleteButton);
				break;
			case QuestInfo.Mode.Predeployed:
				HideAllButtons ();
				SetButtons (StartButton);
				break;
			case QuestInfo.Mode.PredeployedUpdatabale:
				HideAllButtons ();
				SetButtons (StartButton, UpdateButton);
				break;
			case QuestInfo.Mode.PredeployedDowngrade:
				HideAllButtons ();
				SetButtons (StartButton, DowngradeButton);
				break;
			default:
				Log.SignalErrorToDeveloper ("QuestInfo Controller has undefined state: " + data.CurrentMode.ToString ());
				break;
			}

			ElipsifyOverflowingText elipsify = Name.GetComponent<ElipsifyOverflowingText> ();
			if (elipsify != null) {
				elipsify.ElipsifyText ();
			}
			// TODO make elipsify automatic when content of name text changes....???!!!
		}

		#endregion


		#region Initialization in Editor

		public virtual void Reset()
		{
			Name = EnsurePrefabVariableIsSet<Text> (Name, "Name", NAME_PATH);

			DownloadButton = EnsurePrefabVariableIsSet<Button> (DownloadButton, "Download Button", "DownloadButton");
			if (DownloadButton.onClick.GetPersistentEventCount() == 0)
				UnityEventTools.AddPersistentListener (DownloadButton.onClick, Download);
			
			StartButton = EnsurePrefabVariableIsSet<Button> (StartButton, "Start Button", "StartButton");
			if (StartButton.onClick.GetPersistentEventCount() == 0)
				UnityEventTools.AddPersistentListener (StartButton.onClick, Play);
			
			DeleteButton = EnsurePrefabVariableIsSet<Button> (DeleteButton, "Delete Button", "DeleteButton");
			if (DeleteButton.onClick.GetPersistentEventCount() == 0)
				UnityEventTools.AddPersistentListener (DeleteButton.onClick, Delete);

			UpdateButton = EnsurePrefabVariableIsSet<Button> (UpdateButton, "Update Button", "UpdateButton");
			if (UpdateButton.onClick.GetPersistentEventCount() == 0)
				UnityEventTools.AddPersistentListener (UpdateButton.onClick, UpdateQuest);
			
			DowngradeButton = EnsurePrefabVariableIsSet<Button> (DowngradeButton, "Downgrade Button", "DowngradeButton");
			if (DowngradeButton.onClick.GetPersistentEventCount() == 0)
				UnityEventTools.AddPersistentListener (DowngradeButton.onClick, Downgrade);
		}

		#endregion
	}

}