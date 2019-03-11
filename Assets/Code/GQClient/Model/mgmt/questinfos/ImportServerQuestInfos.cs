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
        public ImportServerQuestInfos() : base()
        {
        }

        public override void ReadInput(object sender, TaskEventArgs e)
        {
            if (e != null && e.Content != null && e.Content is string)
            {
                InputJSON = e.Content as string;
            }
            else
            {
                Log.SignalErrorToDeveloper("ImportFromInputString task should read from Input but got no input string.");
            }
        }

        protected override void updateQuestInfoManager(QuestInfo[] newQuests)
        {

            // we make a separate list of ids of all old quest infos:
            List<int> oldIDsToBeRemoved = new List<int>(qim.QuestDict.Keys);

            // we create new qi elements and keep those we can reuse. We remove those from our helper list.
            foreach (QuestInfo newInfo in newQuests)
            {
                if (newInfo.Id == 7244)
                {
                    int a = 0;
                    a++;
                }

                                QuestInfo oldInfo = null;
                if (qim.QuestDict.TryGetValue(newInfo.Id, out oldInfo))
                {
                    // this new element was already there, hence we keep it (remove from the remove list) and update if newer:
                    oldIDsToBeRemoved.Remove(newInfo.Id);

                    if (oldInfo.TimeStamp < newInfo.ServerTimeStamp)
                    {
                        qim.UpdateInfo(newInfo);
                    }
                }
                else
                {
                    qim.AddInfo(newInfo);
                }
            }

            // now in the helper list only the old elements that are not mentioned in the new list anymore are left. Hence we delete them:
            foreach (int oldID in oldIDsToBeRemoved)
            {
                if (ConfigurationManager.Current.autoSynchQuestInfos)
                {
                    // with autoSynch we automatically remove the local quest data:
                    qim.QuestDict[oldID].Delete();

                    // and also delete the quest infos from the list ...
                    qim.RemoveInfo(oldID);
                } else {
                    // when manually synching ...
                    if (qim.QuestDict[oldID].IsOnDevice)
                    {
                        // if the quest exists local, we simply set the server-side update timestamp to null:
                        qim.QuestDict[oldID].DeletedFromServer();
                        // this will trigger an OnChanged event and update the according view of the list element
                    }
                    else
                    {
                        // if the quest has not been loaded yet, we remove the quest info:
                        qim.RemoveInfo(oldID);
                    }
                }
            }
        }

    }
}
