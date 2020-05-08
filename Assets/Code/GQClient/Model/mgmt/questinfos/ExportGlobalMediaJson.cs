using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Util.tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace GQClient.Model
{
    public class ExportGlobalMediaJson : Task
    {
        public ExportGlobalMediaJson() : base()
        {
        }

        protected override IEnumerator DoTheWork()
        {
            if (QuestManager.Instance.MediaStoreIsDirty)
            {
                var mediaList = QuestManager.Instance.GetListOfGlobalMediaInfos();

                try
                {
                    var mediaJson =
                        (mediaList.Count == 0)
                            ? "[]"
                            : JsonConvert.SerializeObject(mediaList, Newtonsoft.Json.Formatting.Indented);
                    Files.WriteAllText(QuestManager.GlobalMediaJsonPath, mediaJson);
                    QuestManager.Instance.MediaStoreIsDirty = false;
                }
                catch (Exception e)
                {
                    Log.SignalErrorToDeveloper("Error while trying to export quest info json file: " + e.Message);
                    RaiseTaskFailed();
                    yield break;
                }
            }

            RaiseTaskCompleted();
        }

        public override object Result
        {
            get => null;
            set { }
        }
    }
}