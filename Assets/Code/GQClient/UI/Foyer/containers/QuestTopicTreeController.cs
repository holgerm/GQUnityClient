using GQClient.Model;

namespace Code.GQClient.UI.Foyer.containers
{
    class QuestTopicTreeController : QuestContainerController
    {
        protected override void SorterChanged()
        {
            // TODO maybe we could sort for alphabet, numbers, date, grades etc.
        }

        protected override void FilterChanged()
        {
            // ignore filter changes
        }

        protected override void ListChanged()
        {
            foreach (var info in qim.GetFilteredQuestInfos())
            {
                foreach (var topic in info.Topics)
                {
                    Topic.Create(topic).AddQuest(info);
                }
            }
            
            var t = Topic.Cursor;
        }

        protected override void RemovedInfo(QuestInfoChangedEvent e)
        {
            throw new System.NotImplementedException();
        }

        protected override void ChangedInfo(QuestInfoChangedEvent e)
        {
            throw new System.NotImplementedException();
        }

        protected override void AddedInfo(QuestInfoChangedEvent e)
        {
            throw new System.NotImplementedException();
        }
    }
}