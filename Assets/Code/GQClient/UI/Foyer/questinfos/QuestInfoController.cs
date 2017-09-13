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
using GQ.Client.Util;
using GQ.Client.FileIO;

namespace GQ.Client.UI.Foyer {

	/// <summary>
	/// Represents one quest info object in a list within the foyer.
	/// </summary>
	public class QuestInfoController : PrefabController {

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

		public void Download() {
			Downloader downloader = 
				new Downloader (
					url: QuestManager.GetQuestURI(data.Id), 
					timeout: ConfigurationManager.Current.downloadTimeOutSeconds * 1000,
					targetPath: QuestManager.GetLocalQuestDirPath(data.Id) + QuestManager.QUEST_FILE_NAME
				);
			new DownloadDialogBehaviour (downloader, "Loading quest");

			PrepareMediaInfoList mediaInfoListPreparation = 
				new PrepareMediaInfoList ();
			new SimpleDialogBehaviour (
				mediaInfoListPreparation,
				"Synching Quest Data",
				"Preparing media information."
			);

			MultiDownloader mediaFileDownloader =
				new MultiDownloader (1);
			new SimpleDialogBehaviour (
				mediaFileDownloader,
				"Synching Quest Data",
				"Loading media files."
			);

			ExportMediaInfoList mediaInfoListExporter =
				new ExportMediaInfoList ();
			new SimpleDialogBehaviour (
				mediaInfoListExporter,
				"Synching Quest Data",
				"Saving updated media info."
			);

			TaskSequence t = 
				new TaskSequence(
					downloader, 
					mediaInfoListPreparation, 
					mediaFileDownloader,
					mediaInfoListExporter);
			t.Start ();

			CurrentMode = Mode.Deletable;
		}

		public void Delete() {
			// TODO in case we are in DeleteWithWarning state we show a dialog with awarning and two options: Delete and Cancel.

			Debug.Log ("Want to delete: " + QuestManager.GetLocalQuestDirPath (data.Id));
			Files.DeleteDirCompletely (QuestManager.GetLocalQuestDirPath (data.Id));
			CurrentMode = Mode.OnServer;
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
			CurrentMode = Mode.Predeployed;
		}


		#endregion


		#region States

		/// <summary>
		/// Mode that determines the UI as depcited here: @ref QuestsFromServerLifeCycle 
		/// and @ref QuestsPredeployedLifeCycle.
		/// </summary>
		protected enum Mode {
			OnServer,
			Deletable,
			Updatable,
			DeletableWithWarning,
			Predeployed,
			PredeployedUpdatabale,
			PredeployedDowngrade
		}

		private Mode currentMode;
		protected Mode CurrentMode {
			get {
				return currentMode;
			}
			set {
				Mode oldValue = currentMode;
				currentMode = value;
				if (oldValue != currentMode)
					UpdateView ();
			}
		}

		#endregion


		#region Runtime API

		public static GameObject Create(GameObject root) 
		{
			GameObject go = PrefabController.Create (PREFAB, root);
			go.GetComponent<QuestInfoController> ().CurrentMode = Mode.OnServer;
			return go;
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
			switch (CurrentMode) {
			case Mode.OnServer:
				HideAllButtons ();
				SetButtons (DownloadButton);
				break;
			case Mode.Deletable:
			case Mode.DeletableWithWarning:
				HideAllButtons ();
				SetButtons (StartButton, DeleteButton);
				break;
			case Mode.Updatable:
				HideAllButtons ();
				SetButtons (StartButton, UpdateButton, DeleteButton);
				break;
			case Mode.Predeployed:
				HideAllButtons ();
				SetButtons (StartButton);
				break;
			case Mode.PredeployedUpdatabale:
				HideAllButtons ();
				SetButtons (StartButton, UpdateButton);
				break;
			case Mode.PredeployedDowngrade:
				HideAllButtons ();
				SetButtons (StartButton, DowngradeButton);
				break;
			default:
				Log.SignalErrorToDeveloper ("QuestInfo Controller has undefined state: " + CurrentMode.ToString ());
				break;
			}

			ElipsifyOverflowingText elipsify = Name.GetComponent<ElipsifyOverflowingText> ();
			if (elipsify != null) {
				elipsify.ElipsifyText ();
			}
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