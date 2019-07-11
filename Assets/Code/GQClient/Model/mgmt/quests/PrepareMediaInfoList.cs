using System.Collections;
using GQ.Client.Util;
using GQ.Client.Err;

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
            Quest quest = QuestManager.Instance.DeserializeQuest(gameXML);
            //Quest quest = QuestReader.DeserializeXML(gameXML);
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
