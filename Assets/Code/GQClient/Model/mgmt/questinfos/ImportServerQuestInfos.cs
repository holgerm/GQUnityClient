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

        protected override void updateQuestInfoManager(QuestInfo[] newQuests)
        {
            // we make a separate list of ids of all old quest infos:
            var oldIDsToBeRemoved = new List<int>(qim.QuestDict.Keys);

            // we create new qi elements and keep those we can reuse. We remove those from our helper list.
            foreach (var newInfo in newQuests)
            {
                if (newInfo.Id == 12902)
                {
                    Debug.Log($"ImportServerQI updateQuestInfoManager() newInfo: {newInfo}");
                }

                QuestInfo oldInfo = null;
                if (qim.QuestDict.TryGetValue(newInfo.Id, out oldInfo))
                {
                    // this new element was already there, hence we keep it (remove from the remove list) and update if newer:
                    oldIDsToBeRemoved.Remove(newInfo.Id);

                    if (oldInfo.TimeStamp == null || oldInfo.TimeStamp < newInfo.ServerTimeStamp)
                    {
                        if (newInfo.Id == 12902)
                        {
                            Debug.Log($"## 1");
                        }

                        qim.UpdateInfo(newInfo);
                    }
                    else
                    {
                        if (newInfo.Id == 12902)
                        {
                            Debug.Log($"## 2");
                        }
                    }
                }
                else
                {
                    if (newInfo.Id == 12902)
                    {
                        Debug.Log($"## 3");
                    }

                    qim.AddInfo(newInfo);
                }
            }

            // now in the helper list only the old elements that are not mentioned in the new list anymore are left. Hence we delete them:
            foreach (var oldID in oldIDsToBeRemoved)
            {
                if (ConfigurationManager.Current.autoSynchQuestInfos)
                {
                    if (oldID == 12902)
                    {
                        Debug.Log($"ImportServerQI removing: {oldID}");
                    }


                    // with autoSynch we automatically remove the local quest data:
                    qim.QuestDict[oldID].Delete();

                    // and also delete the quest infos from the list ...
                    qim.RemoveInfo(oldID);
                }
                else
                {
                    // when manually synching ...
                    if (qim.QuestDict[oldID].IsOnDevice)
                    {
                        if (oldID == 12902)
                        {
                            Debug.Log($"ImportServerQI deletedFromServer (-> OnChange should be triggered): {oldID}");
                        }

                        // if the quest exists local, we simply set the server-side update timestamp to null:
                        qim.QuestDict[oldID].DeletedFromServer();
                        // this will trigger an OnChanged event and update the according view of the list element
                    }
                    else
                    {
                        if (oldID == 12902)
                        {
                            Debug.Log($"ImportServerQI is not on device and server: remove info: {oldID}");
                        }

                        // if the quest has not been loaded yet, we remove the quest info:
                        qim.QuestDict[oldID].Delete(); // introduced newly without exact knowledge (hm)
                        qim.RemoveInfo(oldID);
                    }
                }
            }

            var qiX = newQuests.FirstOrDefault(info => info.Id == 12902);
            var newQiString = qiX != null ? qiX.ToString() : "no questInfos";
            Debug.Log($"ImportServerQI @END: {newQiString}");
        }
    }
}