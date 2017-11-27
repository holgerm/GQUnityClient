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
				if (!Directory.Exists (Application.persistentDataPath + "/quests/")) {
					Directory.CreateDirectory (Application.persistentDataPath + "/quests/");
				}
				return Application.persistentDataPath + "/quests/";
			}
		}

		public static string LocalQuestInfoJSONPath {
			get {
				return LocalQuestsPath + "infos.json";
			}
		}

		protected Dictionary<int, QuestInfo> QuestDict {
			get;
			set;
		}

		public List<QuestInfo> GetListOfQuestInfos ()
		{
			return QuestDict.Values.ToList<QuestInfo> ();
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
					raiseChange (
						new QuestInfoChangedEvent (
							String.Format ("Quest Info Filter changed to {0}.", _filter.ToString ()),
							ChangeType.ListChanged
						)
					);

				}
			}
		}

		public string CurrentCategoryId(QuestInfo info) {
			return Filter.CategoryToShow (info);
		}

		#endregion


		#region Quest Info Changes

		public void AddInfo (QuestInfo newInfo)
		{
			if (QuestDict.ContainsKey (newInfo.Id)) {
				// A questInfo with this ID already exists: this is a CHANGE:
				// TODO
			} else {
				// this is a NEW quest info:
				QuestDict.Add (newInfo.Id, newInfo);

				if (Filter.Accept (newInfo)) {
					// Run through filter and raise event if involved:
					raiseChange (
						new QuestInfoChangedEvent (
							String.Format ("Info for quest {0} added.", newInfo.Name),
							ChangeType.AddedInfo,
							newQuestInfo: newInfo
						)
					);
				}
			}
		}

		public void RemoveInfo (QuestInfo oldInfo)
		{
			oldInfo.Dispose ();
			QuestDict.Remove (oldInfo.Id);

			if (Filter.Accept (oldInfo)) {
				// Run through filter and raise event if involved

				raiseChange (
					new QuestInfoChangedEvent (
						String.Format ("Info for quest {0} removed.", oldInfo.Name),
						ChangeType.RemovedInfo,
						oldQuestInfo: oldInfo
					)
				);
			}
		}

		public void ChangeInfo (QuestInfo info)
		{
			QuestInfo oldInfo;
			if (!QuestDict.TryGetValue (info.Id, out oldInfo)) {
				Log.SignalErrorToDeveloper (
					"Trying to change quest info {0} but it deos not exist in QuestInfoManager.", 
					info.Id.ToString ()
				);
				return;
			}

			QuestDict.Remove (info.Id);
			QuestDict.Add (info.Id, info);

			if (Filter.Accept (oldInfo) || Filter.Accept (info)) {
				// Run through filter and raise event if involved

				raiseChange (
					new QuestInfoChangedEvent (
						String.Format ("Info for quest {0} changed.", info.Name),
						ChangeType.ChangedInfo,
						newQuestInfo: info,
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
			ImportQuestInfosFromJSON importLocal = 
				new ImportQuestInfosFromJSON (false);
			new SimpleDialogBehaviour (
				importLocal,
				"Updating quests",
				"Reading local quests."
			);

			Downloader downloader = 
				new Downloader (
					url: ConfigurationManager.UrlPublicQuestsJSON, 
					timeout: ConfigurationManager.Current.downloadTimeOutSeconds * 1000);
			new DownloadDialogBehaviour (downloader, "Updating quests");

			ImportQuestInfosFromJSON importFromServer = 
				new ImportQuestInfosFromJSON (true);
			new SimpleDialogBehaviour (
				importFromServer,
				"Updating quests",
				"Reading all found quests into the local data store."
			);

			ExportQuestInfosToJSON exporter = 
				new ExportQuestInfosToJSON ();
			new SimpleDialogBehaviour (
				exporter,
				"Updating quests",
				"Saving Quest Data"
			);

			TaskSequence t = new TaskSequence (importLocal, downloader);
			t.AppendIfCompleted (importFromServer);
			t.Append (exporter);
			t.Start ();
		}

		public delegate void ChangeCallback (object sender,QuestInfoChangedEvent e);

		private event ChangeCallback onChange;

		public event ChangeCallback OnChange {
			add {
				onChange += value;
				value(
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
				onChange -= value;
			}
		}

		public int HowManyListerners ()
		{
			return onChange.GetInvocationList ().Length;
		}



		public virtual void raiseChange (QuestInfoChangedEvent e)
		{
			if (onChange != null)
				onChange (this, e);
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
			QuestDict = new Dictionary<int, QuestInfo> ();
			Filter = new QuestInfoFilter.All ();
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
		ListChanged
	}
		

}
