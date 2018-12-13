using System.Collections;
using System.Collections.Generic;
using GQ.Client.Conf;
using GQ.Client.Util;
using UnityEngine;

namespace GQ.Client.Model
{

    public class AutoLoadAndUpdate : Task
    {

        //public AutoLoadAndUpdate() : base() { }

        public override bool Run()
        {
            List<QuestInfo> questInfoList = QuestInfoManager.Instance.GetListOfQuestInfos();

            int loadCounter = 0;
            int updateCounter = 0;

            foreach (QuestInfo qi in questInfoList) {
                //if (qi.Name == "Neue Quest 2") {
                //    Debug.Log("Found");
                //}

                if (qi.IsHidden() || ConfigurationManager.Current.autoUpdateQuestInfos) {
                    if (qi.ShowDownloadOption) {
                        loadCounter++;
                       //Debug.Log("#### AUTOLOAD quest: " + qi.Id);
                        //qi.Download();
                    } else if (qi.ShowUpdateOption) {
                        updateCounter++;
                        //Debug.Log("#### AUTOUPDATE quest: " + qi.Id);
                        //qi.Update();
                    }
                }
            }

            //Debug.Log("#### AUTO HAS loaded: " + loadCounter + " and updated: " + updateCounter);

            return true;
        }
    }

}
