using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.GQClient.Conf;
using Code.GQClient.UI.author;
using GQClient.Model;
using Code.GQClient.Util.tasks;
using UnityEngine;

namespace Code.GQClient.Model.mgmt.quests
{
    public class AutoLoadAndUpdate : Task
    {
        protected override IEnumerator DoTheWork()
        {
            var questInfoList = QuestInfoManager.Instance.GetListOfQuestInfos();

            if (ConfigurationManager.Current.autoSyncQuestInfos)
            {
                foreach (var qi in questInfoList.Where(qi => !qi.IsHidden()))
                {
                    if (qi.LoadOptionPossibleInTheory && !qi.LoadModeAllowsManualLoad && !Author.LoggedIn)
                    {
                        //          Debug.Log($"#### AUTOLOAD quest: {qi.Id}:{qi.Name}");
                        qi.Download();
                        yield return null;
                        continue;
                    }

                    if (qi.UpdateOptionPossibleInTheory && !qi.LoadModeAllowsManualUpdate && !Author.LoggedIn)
                    {
                        //           Debug.Log($"#### AUTOUPDATE quest: {qi.Id}:{qi.Name}");
                        qi.Update();
                        yield return null;
                    }
                }
            }

            //Debug.Log("#### AUTO HAS loaded: " + loadCounter + " and updated: " + updateCounter);
        }
    }
}