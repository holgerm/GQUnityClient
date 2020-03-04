using System.Collections.Generic;
using System.Linq;
using Code.GQClient.Model.mgmt.quests;
using GQClient.Model;

namespace Code.GQClient.UI.Foyer
{
    public class Topic
    {
        static Topic()
        {
            Null = new NullTopic();
            Root = new Topic("", Null);
            CursorHome();
        }

        public static void ClearAll()
        {
            Roots.Clear();
            Root.Children.Clear();
            CursorHome();
        }

        public string FullName
        {
            get
            {
                if (Parent == null || Parent == Null)
                    return "";

                if (Parent == Root)
                    return Name
                        ;

                return Parent.FullName + "/" + Name;
            }
        }

        public string Name { get; private set; }

        public Topic Parent
        {
            get => _parent;
            protected set
            {
                _parent = value;
                if (_parent != Null && !_parent.Children.Contains((this)))
                    _parent.Children.Add(this);
            }
        }

        private static List<Topic> _rootTopics;

        protected static List<Topic> Roots
        {
            get
            {
                if (_rootTopics == null)
                {
                    _rootTopics = new List<Topic>();
                }

                return _rootTopics;
            }
        }

        private List<Topic> _children;
        private Topic _parent;

        public List<Topic> Children
        {
            get
            {
                if (_children == null)
                    _children = new List<Topic>();
                return _children;
            }
        }

        private List<QuestInfo> _questInfos;

        protected List<QuestInfo> QuestInfos
        {
            get
            {
                if (_questInfos == null)
                    _questInfos = new List<QuestInfo>();
                return _questInfos;
            }
        }

        public int NumberOfQuestInfos
        {
            get { return QuestInfos.Count + Children.Sum(topic => topic.NumberOfQuestInfos); }
        }

        /// <summary>
        /// Returns all QuestInfo assigned to this topic and subtopics. Each QuestInfo is contained only once.
        /// </summary>
        /// <returns></returns>
        public QuestInfo[] GetQuestInfos()
        {
            List<QuestInfo> gatheredInfos = new List<QuestInfo>(NumberOfQuestInfos);
            GetQuestInfosRecursive(gatheredInfos);
            return gatheredInfos.ToArray();
        }

        protected void GetQuestInfosRecursive(List<QuestInfo> questInfos)
        {
            foreach (var childTopic in Children)
            {
                childTopic.GetQuestInfosRecursive(questInfos);
            }

            foreach (var info in QuestInfos.Where(info => !questInfos.Contains(info)))
            {
                questInfos.Add(info);
            }
       }

        public void AddQuest(QuestInfo questInfo)
        {
            if (!QuestInfos.Exists(info => info.Id == questInfo.Id))
                QuestInfos.Add(questInfo);
        }

        public bool RemoveQuestFromTopic(QuestInfo questInfo)
        {
            var qi = QuestInfos.Find(info => info.Id == questInfo.Id);
            if (qi != null)
                return QuestInfos.Remove(qi);
            return false;
        }

        public bool IsEmpty
        {
            get
            {
                if (QuestInfos.Count > 0)
                    return false;

                return Children.Aggregate(true, (current, topic) => current & topic.IsEmpty);
            }
        }

        protected Topic() : this("")
        {
            Name = "";
        }

        private Topic(string name, Topic parent = null)
        {
            Name = name;
            Parent = parent;
        }

        /// <summary>
        /// Creates a solitaire topic, without any leaves (quest infos) contained.
        /// </summary>
        /// <param name="topicPath"></param>
        /// <returns></returns>
        public static Topic Create(string topicPath)
        {
            if (string.IsNullOrEmpty(topicPath))
                return Null;

            var segments = topicPath.Split('/');
            if (segments.Length == 0)
                return Null;

            var trimmedSegments = new string[segments.Length];
            for (var i = 0; i < segments.Length; i++)
            {
                trimmedSegments[i] = segments[i].Trim();
                if (trimmedSegments[i] == "")
                    return Null;
            }

            return CreateRecursive(Root, new List<string>(trimmedSegments));
        }

        private static Topic CreateRecursive(Topic baseTopic, List<string> nameSegments)
        {
            var firstPartTopic =
                baseTopic.Children.Find(topic => topic.Name == nameSegments.First())
                ?? new Topic(nameSegments.First(), baseTopic);

            if (nameSegments.Count > 1)
            {
                nameSegments.Remove(nameSegments.First());
                return CreateRecursive(firstPartTopic, nameSegments);
            }

            return firstPartTopic;
        }

        public bool IsRoot => Parent == Root;

        public bool IsLeaf => Children.Count == 0;

        public override string ToString()
        {
            return FullName;
        }

        /// <summary>
        /// Used as the implicitly given absolute root of any topic tree.
        /// </summary>
        public static readonly Topic Root;

        public static readonly Topic Null;

        private class NullTopic : Topic
        {
            protected internal NullTopic()
            {
                Name = "";
            }
        }

        public static Topic Cursor { get; set; }

        public static bool CursorMoveDown(string childName)
        {
            var child = Cursor.Children.Find(ch => ch.Name == childName);
            if (child == null)
            {
                return false;
            }

            Cursor = child;
            return true;
        }

        public static bool CursorMoveUp()
        {
            if (Cursor == Root)
                return false;

            Cursor = Cursor.Parent;
            return true;
        }

        public static void CursorHome()
        {
            Cursor = Root;
        }

        public static bool CursorSetTo(string topicPath)
        {
            var savedCursor = Cursor;
            CursorHome();
            var pathSegments = topicPath.Split('/');
            foreach (var segment in pathSegments)
            {
                if (!CursorMoveDown(segment))
                {
                    Cursor = savedCursor;
                    return false;
                }
            }

            return true;
        }
    }
}