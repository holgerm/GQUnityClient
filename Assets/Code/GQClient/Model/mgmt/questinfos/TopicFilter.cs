using System.Collections.Generic;
using Code.GQClient.UI.Foyer;

namespace GQClient.Model
{
    public class TopicFilter : QuestInfoFilter.ActivatableFilter
    {
        private static TopicFilter _instance;

        public static TopicFilter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TopicFilter();

                return _instance;
            }
        }

        internal static void Disable()
        {
            _instance = null;
        }

        private TopicFilter()
        {
            //Topic.OnCursorChanged += RaiseFilterChangeEvent;
        }

        public override bool Accept(QuestInfo qi)
        {
            if (!IsActive)
            {
                return true;
            }
            
            if (Topic.Cursor == Topic.Root)
            {
                return true;
            }
            
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

        public override string ToString()
        {
            return "TopicFilter";
        } 
    }
}