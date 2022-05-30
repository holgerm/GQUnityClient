using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code.GQClient.Conf;
using Code.QM.Util;
using UnityEngine;

namespace GQClient.Model
{
    /// <summary>
    /// Imports quest infos from JSON files. Either from the servers listing of all quest infos that are available, 
    /// or from the local json file which keeps track of the latest state of local and remote quest infos.
    /// 
    /// In order to import the server info, you need to use a downloader task before and 
    /// simply call the constructor of this class with 'true'). 
    /// 
    /// To load the local json file use 'false' as parameter of the constructor. 
    /// In this case no download task is needed and if existent its result will be ignored.
    /// </summary>
    public class ImportServerQuestInfos : ImportQuestInfos
    {
        public ImportServerQuestInfos() : base()
        {
        }

        protected override void updateQuestInfoManager(IEnumerable<QuestInfo> quests)
        {
            qim.AddInfosFromServer(quests);
            RaiseTaskCompleted();
        }
    }
}