using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;
using GQ.Client.Conf;
using Newtonsoft.Json;
using System;
using GQ.Client.UI;
using GQ.Client.Err;
using System.IO;
using GQ.Client.FileIO;

namespace GQ.Client.Model
{

    public class PrepareMediaInfoList : Task
    {

        public PrepareMediaInfoList() : base() { }

        private string gameXML { get; set; }

        protected override void ReadInput(object input)
        {
            if (input == null)
            {
                RaiseTaskFailed();
                return;
            }

            if (input is string)
            {
                gameXML = input as string;
            }
            else
            {
                Log.SignalErrorToDeveloper(
                    "Improper TaskEventArg received in SyncQuestData Task. Should be of type string but was " +
                    input.GetType().Name);
                RaiseTaskFailed();
                return;
            }
        }

        protected override IEnumerator DoTheWork()
        {
            // step 1 deserialize game.xml:
            //QuestManager.Instance.SetCurrentQuestFromXML(gameXML);
            Quest quest = QuestManager.Instance.DeserializeQuest(gameXML);
            yield return null;

            // step 2 import local media info:
            quest.ImportLocalMediaInfo();
            yield return null;

            // step 3 include remote media info:
            Result = quest.GetListOfFilesNeedDownload();

            // TODO BAD HACK:
            QuestManager.Instance.CurrentQuest = quest;
        }
    }
}
