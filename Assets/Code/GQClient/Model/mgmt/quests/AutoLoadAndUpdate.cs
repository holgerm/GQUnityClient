using System.Collections;
using System.Collections.Generic;
using Code.GQClient.Conf;
using GQClient.Model;
using Code.GQClient.Util.tasks;

namespace Code.GQClient.Model.mgmt.quests
{

    public class AutoLoadAndUpdate : Task
    {

        protected override IEnumerator DoTheWork()
        {
            var questInfoList = QuestInfoManager.Instance.GetListOfQuestInfos();

            var loadCounter = 0;
            var updateCounter = 0;

            foreach (var qi in questInfoList)
            {
                //if (qi.Name == "Neue Quest 2") {
                //    Debug.Log("Found");
                //}

                if (qi.IsHidden() || ConfigurationManager.Current.autoSynchQuestInfos)
                {
                    if (qi.ShowDownloadOption)
                    {
                        loadCounter++;
                        //Debug.Log("#### AUTOLOAD quest: " + qi.Id);
                        //qi.Download();
                        yield break;
                    }
                    else if (qi.ShowUpdateOption)
                    {
                        updateCounter++;
                        //Debug.Log("#### AUTOUPDATE quest: " + qi.Id);
                        //qi.Update();
                        yield break;
                    }
                }
            }

            //Debug.Log("#### AUTO HAS loaded: " + loadCounter + " and updated: " + updateCounter);
        }
    }

}
