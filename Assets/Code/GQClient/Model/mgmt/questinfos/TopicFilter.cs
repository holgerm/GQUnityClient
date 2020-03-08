using System.Collections.Generic;
using Code.GQClient.UI.Foyer;

namespace GQClient.Model
{
    public class TopicFilter : QuestInfoFilter
    {
        private static TopicFilter _instance;

        public static TopicFilter Instance => new TopicFilter();

        private TopicFilter()
        {
            Topic.OnCursorChanged += RaiseFilterChangeEvent;
        }

        public override bool Accept(QuestInfo qi)
        {
            // TODO Check wether qi fits to current cursor topic
            foreach (var topicString in qi.Topics)
            {
                if (topicString.StartsWith(Topic.Cursor.FullName))
                    return true; // TODO Update Event in Topic
            }

            return false;
        }

        public override List<string> AcceptedCategories(QuestInfo qi)
        {
            return qi.Categories;
        }
    }
}