using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;
using GQ.Client.Conf;
using Newtonsoft.Json;
using System;
using GQ.Client.UI;
using System.IO;
using GQ.Client.Err;
using Newtonsoft.Json.Converters;

namespace GQ.Client.Model
{

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
	public class ImportServerQuestInfos : ImportQuestInfos 
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="GQ.Client.Model.ImportQuestInfosFromJSON"/> class.
		/// </summary>
		/// <param name="importFromServer">If set to <c>true</c> import from server otherwise use the local infos.json file.</param>
		public ImportServerQuestInfos () : base ()
		{ 
		}

		public override void ReadInput (object sender, TaskEventArgs e)
		{
				if (e != null && e.Content != null && e.Content is string) {
					InputJSON = e.Content as string;
				} else {
					Log.SignalErrorToDeveloper ("ImportFromInputString task should read from Input but got no input string.");
				}
		}

		protected override void updateQuestInfoManager (QuestInfo[] newQuests) {

			// we make a separate list of ids of all old quest infos:
			List<int> oldIDsToBeRemoved = new List<int>(qim.QuestDict.Keys);

			// we create new qi elements and keep those we can reuse. We remove those from our helper list.
			foreach (QuestInfo newInfo in newQuests) {
				QuestInfo oldInfo = null;
				if (qim.QuestDict.TryGetValue(newInfo.Id, out oldInfo)) {
					// this new element was already there, hence we keep it (remove from the remove list) and update if newer:
					oldIDsToBeRemoved.Remove(newInfo.Id);
					if (oldInfo.LastUpdateOnServer < newInfo.LastUpdateOnServer) {
						qim.AddInfo (newInfo);
					}
				}
				else {
					qim.AddInfo(newInfo);
				}
			}

			// now in the helper list only the old elements that are not mentioned in the new list anymore are left. Hence we delete them:
			foreach (int oldID in oldIDsToBeRemoved) {
				qim.RemoveInfo (oldID);
			}
		}

	}
}
