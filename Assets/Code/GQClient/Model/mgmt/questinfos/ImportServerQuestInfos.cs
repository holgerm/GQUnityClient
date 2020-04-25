using System.Collections.Generic;
using System.Linq;
using Code.GQClient.Conf;
using UnityEngine;

namespace GQClient.Model
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
        public ImportServerQuestInfos() : base()
        {
        }

        protected override void updateQuestInfoManager(IEnumerable<QuestInfo> newQuests)
        {
            // we make a separate list of ids of all old quest infos:
            var oldIDsToBeRemoved = new List<int>(qim.QuestDict.Keys);

            // we create new qi elements and keep those we can reuse. We remove those from our helper list.
            foreach (var newInfo in newQuests)
            {
                if (qim.QuestDict.TryGetValue(newInfo.Id, out var oldInfo))
                {
                    // this new element was already there, hence we keep it (remove from the remove list) and update if newer:
                    oldIDsToBeRemoved.Remove(newInfo.Id);

                    if (oldInfo.TimeStamp == null || oldInfo.TimeStamp < newInfo.ServerTimeStamp)
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
            foreach (var oldId in oldIDsToBeRemoved)
            {
                if (ConfigurationManager.Current.autoSyncQuestInfos)
                {
                    // with autoSync we automatically remove the local quest data:
                    qim.QuestDict[oldId].Delete();

                    // and also delete the quest infos from the list ...
                    qim.RemoveInfo(oldId);
                }
                else
                {
                    // when manually syncing ...
                    if (qim.QuestDict[oldId].IsOnDevice)
                    {
                        // if the quest exists local, we simply set the server-side update timestamp to null:
                        qim.QuestDict[oldId].DeletedFromServer();
                        // this will trigger an OnChanged event and update the according view of the list element
                    }
                    else
                    {
                         // if the quest has not been loaded yet, we remove the quest info:
                        qim.QuestDict[oldId].Delete(); // introduced newly without exact knowledge (hm)
                        qim.RemoveInfo(oldId);
                    }
                }
            }
        }
    }
}