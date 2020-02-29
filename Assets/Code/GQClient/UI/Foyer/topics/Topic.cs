using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

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

        protected Topic() : this("")
        {
            Name = "";
        }

        private Topic(string name, Topic parent = null)
        {
            Name = name;
            Parent = parent;
        }

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
            protected internal NullTopic() : base()
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