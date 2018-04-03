using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using GQ.Client.Util;
using GQ.Client.Conf;
using GQ.Client.Model;
using Newtonsoft.Json;
using System.IO;
using GQ.Client.Err;
using GQ.Client.UI.Dialogs;
using System.Linq;
using QM.Util;
using GQ.Client.UI;


namespace GQ.Client.Model
{

	/// <summary>
	/// Manages the meta data for all quests available: locally on the device as well as remotely on the server.
	/// </summary>
	public class QuestInfoManager
	{

		#region store & access data

		public static string LocalQuestsPath {
			get {
				if (!Directory.Exists (Device.GetPersistentDatapath () + "/quests/")) {
					Directory.CreateDirectory (Device.GetPersistentDatapath () + "/quests/");
				}
				return Device.GetPersistentDatapath () + "/quests/";
			}
		}

		public static string LocalQuestInfoJSONPath {
			get {
				return LocalQuestsPath + "infos.json";
			}
		}

		public Dictionary<int, QuestInfo> QuestDict {
			get;
			set;
		}

		public List<QuestInfo> GetListOfQuestInfos ()
		{
			return QuestDict.Values.ToList<QuestInfo> ();
		}

		public IEnumerable<QuestInfo> GetFilteredQuestInfos ()
		{
			return QuestDict.Values.Where (x => Filter.Accept (x)).ToList ();
		}

		public bool ContainsQuestInfo (int id)
		{
			return QuestDict.ContainsKey (id);
		}

		public int Count {
			get {
				return QuestDict.Count;
			}
		}

		public QuestInfo GetQuestInfo (int id)
		{
			QuestInfo questInfo;
			return (QuestDict.TryGetValue (id, out questInfo) ? questInfo : null);
		}

		#endregion


		#region Filter

		private QuestInfoFilter _filter;

		public QuestInfoFilter Filter {
			get { 
				return _filter;
			}
			protected set {
				if (_filter != value) {
					_filter = value;
					// we register with later changes of the filter:
					_filter.filterChange += FilterChanged;
					// we use the new filter instantly:
					FilterChanged ();
				}
			}
		}

		public QuestInfoFilter.CategoryFilter CategoryFilter;

		public Dictionary<string, QuestInfoFilter.CategoryFilter> CategoryFilters;

		/// <summary>
		/// Adds the given andFilter in conjunction to the current filter(s).
		/// </summary>
		/// <param name="andFilter">And filter.</param>
		public void FilterAnd (QuestInfoFilter andFilter)
		{
			Filter = new QuestInfoFilter.And (Filter, andFilter);
		}

		public event ChangeCallback OnFilterChange;

		public void FilterChanged ()
		{
			if (OnFilterChange != null) {
				OnFilterChange (
					this, 					
					new QuestInfoChangedEvent (
						"Filter changed ...",
						ChangeType.FilterChanged,
						newQuestInfo: null,
						oldQuestInfo: null
					)
				);
			}
		}

		#endregion


		#region Quest Info Changes

		public void AddInfo (QuestInfo newInfo)
		{
			QuestInfo oldInfo = null;
			if (QuestDict.TryGetValue (newInfo.Id, out oldInfo)) {
				// A questInfo with this ID already exists: this is a CHANGE:
				if (newInfo.LastUpdateOnServer > oldInfo.LastUpdateOnServer) {
					// NEW INFO IS NEWER: hence we should replace the old one with the new:
					QuestDict.Remove (newInfo.Id);
					QuestDict.Add (newInfo.Id, newInfo);
					Debug.Log ("Change of QuestInfos in QIM, we found a server-side update and replaced the old info with the new one.");

					if (Filter.Accept (newInfo)) {
						// Run through filter and raise event if involved:
						raiseDataChange (
							new QuestInfoChangedEvent (
								String.Format ("Info for quest {0} changed.", newInfo.Name),
								ChangeType.ChangedInfo,
								newQuestInfo: newInfo,
								oldQuestInfo: oldInfo
							)
						);
					}
				}
			} else {
				// this is a NEW quest info:
				QuestDict.Add (newInfo.Id, newInfo);

				if (Filter.Accept (newInfo)) {
					// Run through filter and raise event if involved:
					raiseDataChange (
						new QuestInfoChangedEvent (
							String.Format ("Info for quest {0} added.", newInfo.Name),
							ChangeType.AddedInfo,
							newQuestInfo: newInfo
						)
					);
				}
			}
		}

		public void RemoveInfo (int oldInfoID)
		{
			Debug.Log ("RemoveInfo(" + oldInfoID + ")");

			QuestInfo oldInfo = null;
			if (!QuestDict.TryGetValue (oldInfoID, out oldInfo)) {
				Log.SignalErrorToDeveloper (
					"Trying to remove quest info with ID {0} but it deos not exist in QuestInfoManager.", 
					oldInfoID
				);
				return;
			}

			oldInfo.Dispose ();
			QuestDict.Remove (oldInfoID);

			if (Filter.Accept (oldInfo)) {
				// Run through filter and raise event if involved

				raiseDataChange (
					new QuestInfoChangedEvent (
						String.Format ("Info for quest {0} removed.", oldInfo.Name),
						ChangeType.RemovedInfo,
						oldQuestInfo: oldInfo
					)
				);
			}
		}

		public void ChangeInfo (QuestInfo newInfo)
		{
			QuestInfo oldInfo;
			if (!QuestDict.TryGetValue (newInfo.Id, out oldInfo)) {
				Log.SignalErrorToDeveloper (
					"Trying to change quest info {0} but it deos not exist in QuestInfoManager.", 
					newInfo.Id.ToString ()
				);
				return;
			}

			if (ConfigurationManager.Current.autoUpdateQuestInfos || !oldInfo.IsOnDevice) {
				// preform the complete update, i.e. remove the old and add the new info:
				QuestDict.Remove (newInfo.Id);
				QuestDict.Add (newInfo.Id, newInfo);

				// TODO: update the quest itself:
//				newInfo.Download (); This should be done only when autoQuestSynch is on
			} else {
				// only update the quest info server timestamp so the views can figure out that this info is updatable 
				// and offer manual update to the user:
				oldInfo.LastUpdateOnServer = newInfo.LastUpdateOnServer;
				// store the new quest info to which this can manually be updated later ...
				oldInfo.NewVersionOnServer = newInfo;
			}

			// in case we update the info after updating the quest we have to erase the link to the new info within the new info:
			newInfo.NewVersionOnServer = null;

			if (Filter.Accept (oldInfo) || Filter.Accept (newInfo)) {
				// Run through filter and raise event if involved

				raiseDataChange (
					new QuestInfoChangedEvent (
						String.Format ("Info for quest {0} changed.", newInfo.Name),
						ChangeType.ChangedInfo,
						newQuestInfo: newInfo,
						oldQuestInfo: oldInfo
					)
				);
			}
		}

		/// <summary>
		/// Updates the quest infos from the server and intergrates the gathered data into the local data. 
		/// 
		/// Should be called in cases like the list is shown again (or first time), 
		/// the server connection is gained back, the last update is long ago or the user demands an update.
		/// </summary>
		public void UpdateQuestInfos ()
		{
			ImportQuestInfos importLocal = 
				new ImportLocalQuestInfos ();
			new SimpleDialogBehaviour (
				importLocal,
				string.Format ("Aktualisiere {0}", ConfigurationManager.Current.nameForQuestsPl),
				string.Format ("Lese lokale {0}", ConfigurationManager.Current.nameForQuestSg)
			);

			Downloader downloader = 
				new Downloader (
					url: ConfigurationManager.UrlPublicQuestsJSON, 
					timeout: ConfigurationManager.Current.timeoutMS);
			new DownloadDialogBehaviour (
				downloader, 
				string.Format ("Aktualisiere {0}", ConfigurationManager.Current.nameForQuestsPl)
			);

			ImportQuestInfos importFromServer = 
				new ImportServerQuestInfos ();
			new SimpleDialogBehaviour (
				importFromServer,
				string.Format ("Aktualisiere {0}", ConfigurationManager.Current.nameForQuestsPl),
				string.Format ("Neue {0} werden lokal gespeichert", ConfigurationManager.Current.nameForQuestsPl)
			);

			ExportQuestInfosToJSON exporter = 
				new ExportQuestInfosToJSON ();
			new SimpleDialogBehaviour (
				exporter,
				string.Format ("Aktualisiere {0}", ConfigurationManager.Current.nameForQuestsPl),
				string.Format ("{0}-Daten werden gespeichert", ConfigurationManager.Current.nameForQuestSg)
			);

			TaskSequence t = new TaskSequence (importLocal, downloader);
			t.AppendIfCompleted (importFromServer);
			t.Append (exporter);
			t.OnTaskCompleted += OnQuestInfosUpdateSucceeded;
			t.Start ();
		}

		public static event Task.TaskCallback OnQuestInfosUpdateSucceeded;

		public delegate void ChangeCallback (object sender,QuestInfoChangedEvent e);

		private event ChangeCallback onDataChange;

		public event ChangeCallback OnDataChange {
			add {
				onDataChange += value;
				value (
					this, 					
					new QuestInfoChangedEvent (
						"Initializing listener ...",
						ChangeType.ListChanged,
						newQuestInfo: null,
						oldQuestInfo: null
					)
				);
			}
			remove {
				onDataChange -= value;
			}
		}

		public int HowManyListerners ()
		{
			return onDataChange.GetInvocationList ().Length;
		}



		public virtual void raiseDataChange (QuestInfoChangedEvent e)
		{
			if (onDataChange != null)
				onDataChange (this, e);
		}

		#endregion


		#region singleton

		private static QuestInfoManager _instance = null;

		public static QuestInfoManager Instance {
			get {
				if (_instance == null) {
					_instance = new QuestInfoManager ();
				}
				return _instance;
			}
			set {
				_instance = value;
			}
		}

		public static void Reset ()
		{
			_instance = null;
		}

		public QuestInfoManager ()
		{
			// init quest info store:
			QuestDict = new Dictionary<int, QuestInfo> ();

			initViews ();
			initFilters ();
		}

		void initViews ()
		{
			if (ConfigurationManager.Current.questInfoViews == null || ConfigurationManager.Current.questInfoViews.Length == 0) {
				Log.SignalErrorToDeveloper ("No quest info views defined for this app. Fix that!");
				return;
			}

			string startView = ConfigurationManager.Current.questInfoViews [0];
			Base.Instance.ListCanvas.gameObject.SetActive (startView == QuestInfoView.List.ToString ());
			Base.Instance.MapCanvas.gameObject.SetActive (startView == QuestInfoView.Map.ToString ());
			Base.Instance.MapHolder.gameObject.SetActive (startView == QuestInfoView.Map.ToString ());

			// check whether we have alternative views to offer:
			if (ConfigurationManager.Current.questInfoViews.Length <= 1)
				return;

			// Create the multitoggle View for the view alternatives currently not displayed, i.e. 2 to n:
			GameObject menuContent = Base.Instance.GetComponent<MenuAccessPoint> ().MenuTopLeftContent;
			ViewToggleController.Create (menuContent);
		}

		void initFilters ()
		{
			// init filters
			Filter = new QuestInfoFilter.All ();
			// init category filters:
			CategoryFilters = new Dictionary<string, QuestInfoFilter.CategoryFilter> ();
			List<CategorySet> catSets = ConfigurationManager.Current.categorySets;
			foreach (CategorySet catSet in catSets) {
				CategoryFilters [catSet.name] = new QuestInfoFilter.CategoryFilter (catSet);
				FilterAnd (CategoryFilters [catSet.name]);
			}

			// create UI for Category Filters:
			GameObject menuContent = Base.Instance.GetComponent<MenuAccessPoint> ().MenuTopLeftContent;
			foreach (CategorySet catSet in ConfigurationManager.Current.categorySets) {
				CategoryTreeCtrl.Create (menuContent, CategoryFilters [catSet.name], catSet.categories);
			}
		}

		#endregion
	}

	public class QuestInfoChangedEvent : EventArgs
	{
		public string Message { get; protected set; }

		public ChangeType ChangeType { get; protected set; }

		public QuestInfo NewQuestInfo { get; protected set; }

		public QuestInfo OldQuestInfo { get; protected set; }

		public QuestInfoChangedEvent (
			string message = "", 
			ChangeType type = ChangeType.ChangedInfo, 
			QuestInfo newQuestInfo = null, 
			QuestInfo oldQuestInfo = null
		)
		{
			Message = message;
			ChangeType = type;
			NewQuestInfo = newQuestInfo;
			OldQuestInfo = oldQuestInfo;
		}

	}

	public enum ChangeType
	{
		AddedInfo,
		RemovedInfo,
		ChangedInfo,
		ListChanged,
		FilterChanged
	}
		

}
