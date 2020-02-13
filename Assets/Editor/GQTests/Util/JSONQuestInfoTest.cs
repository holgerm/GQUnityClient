using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using GQClient.Model;
using Code.GQClient.Util.tasks;
using GQ.Editor.Util;
using GQTests.Management;
using NUnit.Framework;

namespace GQTests.Util
{

    public class JSONQuestInfoTest
    {

        protected ImportQuestInfos importTask;
        protected QuestInfoManager qim;
        protected TestChangeListener testListener;

        [SetUp]
        public void SetupTask()
        {
            QuestInfoManager.Reset();
            qim = QuestInfoManager.Instance;

            testListener = new TestChangeListener();
            qim.OnDataChange += testListener.OnChange;

            importTask = new ImportServerQuestInfos();
        }


        protected const string JSON_QUEST_A_10557 = "jsonQuest_A_10557";
        protected const string JSON_QUEST_B_10558 = "jsonQuest_B_10558";

        protected void loadJSON(string jsonFileName)
        {
            string filePath = Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "JSON/QuestInfos", jsonFileName);
            string json = File.ReadAllText(filePath);
            StringProviderTask provideTestJSON = new StringProviderTask(json);
            TaskSequence seq = new TaskSequence(provideTestJSON, importTask);
            seq.Start();
        }

    }

    class StringProviderTask : Task
    {

        public StringProviderTask(string providedString) : base()
        {
            Result = providedString;
        }

        protected override IEnumerator DoTheWork()
        {
            RaiseTaskCompleted();
            yield break;
        }
    }

    public class TestChangeListener
    {

        public int added = 0;
        public int removed = 0;
        public int infoChanged = 0;
        public int listChanged = 0;

        public void OnChange(object sender, QuestInfoChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.AddedInfo:
                    added++;
                    break;
                case ChangeType.RemovedInfo:
                    removed++;
                    break;
                case ChangeType.ChangedInfo:
                    infoChanged++;
                    break;
                case ChangeType.ListChanged:
                    listChanged++;
                    break;
                default:
                    Assert.Fail("Unexpected Change Type: " + e.ChangeType.ToString());
                    break;
            }
        }
    }


}
