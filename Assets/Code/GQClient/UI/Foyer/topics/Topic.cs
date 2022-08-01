using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Code.GQClient.Err;
using Code.QM.Util;
using GQClient.Model;
using UnityEngine;

namespace Code.GQClient.UI.Foyer
{
    public class Topic
    {
        static Topic()
        {
            Null = new NullTopic();
            Root = new Topic("Alle Themen", Null);
            CursorHome();
            CursorChangedEvent += QuestInfoManager.Instance.FilterChange.Invoke;
        }

        public static void ClearAll()
        {
            Root.Children.Clear();
            Topics.Clear();
            CursorHome();
        }

        public string FullName
        {
            get
            {
                if (Parent == null || Parent == Null || Parent == Root)
                    return Name;
                
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

        public int NumberOfOwnQuestInfos => QuestInfos.Count;

        public int NumberOfAllQuestInfos
        {
            get
            {
                var gatheredInfoIds = new List<int>();
                NumberOfAllQuestInfosRecursive(gatheredInfoIds);
                return gatheredInfoIds.Count;
            }
        }

        protected void NumberOfAllQuestInfosRecursive(List<int> gatheredQuestInfoIds)
        {
            foreach (var child in Children)
            {
                child.NumberOfAllQuestInfosRecursive(gatheredQuestInfoIds);
            }

            foreach (var info in QuestInfos.Where(info => !gatheredQuestInfoIds.Contains(info.Id)))
            {
                gatheredQuestInfoIds.Add(info.Id);
            }
        }

        /// <summary>
        /// Returns all QuestInfo assigned to this topic and subtopics. Each QuestInfo is contained only once.
        /// </summary>
        /// <returns></returns>
        public QuestInfo[] GetQuestInfos()
        {
            List<QuestInfo> gatheredInfos = new List<QuestInfo>(NumberOfAllQuestInfos);
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

        public void AddQuestInfo(QuestInfo questInfo)
        {
            if (!QuestInfos.Exists(info => info.Id == questInfo.Id))
                QuestInfos.Add(questInfo);
        }

        public static void InsertQuestInfo(QuestInfo questInfo)
        {
            foreach (var topicPath in questInfo.Topics)
            {
                Create(topicPath).AddQuestInfo(questInfo);
            }
        }

        public static bool RemoveQuestFromAllTopics(QuestInfo questInfo, bool clean = false)
        {
            object lockForSwitch = new object();

            var success = false;

            lock (lockForSwitch)
            {
                var tmp = Cursor;
                foreach (var topicPath in questInfo.Topics)
                {
                    if (CursorSetTo(topicPath))
                    {
                        success |= Cursor.QuestInfos.Remove(questInfo);

                        if (clean)
                        {
                            if (Cursor.QuestInfos.Count == 0 && Cursor.IsLeaf)
                            {
                                var tpToDelete = Cursor;
                                CursorMoveUp();
                                Cursor.Children.Remove(tpToDelete);
                            }
                        }
                    }
                }

                Cursor = tmp;
            }

            return success;
        }

        public bool RemoveQuest(QuestInfo questInfo)
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

                var result = true;
                foreach (var child in Children)
                    result &= child.IsEmpty;
                return result;
            }
        }
        
        private Topic(string name = "", Topic parent = null)
        {
            Name = name;
            Parent = parent;
            try
            {
                Topics.Add(FullName, this);
            }
            catch (ArgumentException exc)
            {
                Log.SignalErrorToDeveloper($"Topic with name '{FullName}' can not be added twice to _topics. Ignored.");
            }
        }

        private static readonly Dictionary<string, Topic> Topics = new();

        /// <summary>
        /// Creates a solitaire topic, without any leaves (quest infos) contained.
        /// </summary>
        /// <param name="topicPath"></param>
        /// <returns></returns>
        public static Topic Create(string topicPath)
        {
            if (Topics.TryGetValue(topicPath, out Topic result))
            {
                return result;
            }

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

            result = CreateRecursive(Root, new List<string>(trimmedSegments));
            return result;
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

        private static Topic _cursor;

        public static Topic Cursor
        {
            get => _cursor;
            private set
            {
                if (Cursor != value)
                {
                    _cursor = value;
                    //QuestInfoManager.Instance.FilterChange.Invoke();
                    //OnCursorChanged?.Invoke();
                }
            }
        }

        public static event Action CursorChangedEvent;

        //public static event VoidToVoid OnCursorChanged;

        public static bool CursorMoveDown(string childName)
        {
            var child = Cursor.Children.Find(ch => ch.Name == childName);
            if (child == null)
            {
                return false;
            }

            Cursor = child;
            CursorChangedEvent?.Invoke();
            return true;
        }

        public static bool CursorMoveUp()
        {
            if (Cursor == Root)
                return false;

            Cursor = Cursor.Parent;
            CursorChangedEvent?.Invoke();
            return true;
        }

        public static void CursorHome()
        {
            if (Cursor != Root)
            {
                Cursor = Root;
                CursorChangedEvent?.Invoke();
            }
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

            CursorChangedEvent?.Invoke();
            return true;
        }
    }
}