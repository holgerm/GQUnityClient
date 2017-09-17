using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;
using GQ.Client.Conf;
using Newtonsoft.Json;
using GQ.Client.Util;
using System;
using GQ.Client.UI;
using System.IO;
using GQ.Client.Err;

namespace GQ.Client.Model {

	/// <summary>
	/// Imports quest infos from JSON files. Either form the servers listing of all quest infos that are available, 
	/// or form the local json file which keeps track of the latest state of local and remote quest infos.
	/// 
	/// In order to import the server info, you need to use a downloader task before and 
	/// simply call the constructor of this class with 'true'). 
	/// 
	/// To load the local json file use 'false' as paraneter of the constructor. 
	/// In this case no download task is needed and if exitent its result will be ignored.
	/// </summary>
	public class ImportQuestInfosFromJSON : Task {

		/// <summary>
		/// Initializes a new instance of the <see cref="GQ.Client.Model.ImportQuestInfosFromJSON"/> class.
		/// </summary>
		/// <param name="importFromServer">If set to <c>true</c> import from server otherwise use the local infos.json file.</param>
		public ImportQuestInfosFromJSON(bool importFromServer) : base() { 
			this.importFromServer = importFromServer;

			if (!importFromServer) {
				// import from local quest json file:
				if (File.Exists (QuestInfoManager.LocalQuestInfoJSONPath)) {
					try {
						InputJSON = File.ReadAllText (QuestInfoManager.LocalQuestInfoJSONPath);
					}
					catch (Exception e) {
						Log.SignalErrorToDeveloper ("Error while trying to import local quest info json file: " + e.Message);
						InputJSON = "[]";
						return;
					}
				}
				else {
					InputJSON = "[]";
				}
			}
		}

		private bool importFromServer;

		private string InputJSON { get; set; }

		public override void InitAfterPreviousTask(object sender, TaskEventArgs e) {
			if (importFromServer) {
				if (e != null && e.Content != null && e.Content is string) {
					InputJSON = e.Content as string;
				}
			}
		}

		public override void Start(int step = 0) 
		{
			base.Start(step);

			QuestInfoManager.Instance.Update (InputJSON);
			RaiseTaskCompleted ();
		}

		public override object Result {
			get {
				return null;
			}
			protected set { }
		}
	}
}
