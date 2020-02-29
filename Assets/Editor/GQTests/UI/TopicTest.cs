using System;
using Code.GQClient.UI.Foyer;
using NUnit.Framework;
using UnityEngine;

namespace GQTests.UI
{
    
    public class TopicTest
    {
        [SetUp]
        public void SetUp()
        {
            Topic.ClearAll();
        }

        [Test]
        public void Null()
        {
            Assert.AreEqual("", Topic.Null.Name);
            Assert.AreEqual("", Topic.Null.FullName);
            Assert.IsNull(Topic.Null.Parent);
            Assert.That(Topic.Null.Children.Count == 0);
        }

        [Test]
        public void Root()
        {
            Assert.AreEqual("", Topic.Root.Name);
            Assert.AreEqual("", Topic.Root.FullName);
            Assert.AreSame(Topic.Null, Topic.Root.Parent);
            Assert.That(Topic.Root.Children.Count == 0);
        }

        [Test]
        public void ClearAllTopics()
        {
            Topic.Create("anyTopic1");
            Topic.Create("anyTopic2");
            Assert.AreEqual(2, Topic.Cursor.Children.Count);

            Topic.ClearAll();
            Assert.AreEqual(0, Topic.Cursor.Children.Count);
        }

        [Test]
        public void RootTopics()
        {
            // no topic at all => no root topic
            Assert.AreEqual(0, Topic.Cursor.Children.Count);

            // a first simple topic x => root topics {x}
            var simple = Topic.Create("simple");
            Assert.AreEqual(1, Topic.Cursor.Children.Count);
            Assert.That(Topic.Cursor.Children.Exists(x => x.Name == "simple"));
            Assert.That(simple.IsRoot);

            // a second simple topic x => root topics {x}
            var simple2 = Topic.Create("simple2");
            Assert.AreEqual(2, Topic.Cursor.Children.Count);
            Assert.That(Topic.Cursor.Children.Exists(x => x.Name == "simple2"));
            Assert.That(simple2.IsRoot);

            // same simple again will not double as root:
            simple2 = Topic.Create("simple2");
            Assert.AreEqual(2, Topic.Cursor.Children.Count);

            // a two step path topic x/y => root topics {x}
            var twoStepTopic = Topic.Create("root/sub");
            Assert.AreEqual(3, Topic.Cursor.Children.Count);
            Assert.That(Topic.Cursor.Children.Exists(x => x.Name.Equals("root")));
            Assert.That(!twoStepTopic.IsRoot);

            // same root in path will not double:
            Topic.Create("root/sub2");
            Assert.AreEqual(3, Topic.Cursor.Children.Count);

            // a three step path topic x/y => root topics {x}
            var threeStepTopic = Topic.Create("root2/sub3/sub4");
            Assert.AreEqual(4, Topic.Cursor.Children.Count);
            Assert.That(Topic.Cursor.Children.Exists(x => x.Name.Equals("root2")));
            Assert.That(!threeStepTopic.IsRoot);

            // same root in path will not double:
            Topic.Create("root2/sub5");
            Assert.AreEqual(4, Topic.Cursor.Children.Count);
        }

        [Test]
        public void CreateTopicWithCorrectProperties()
        {
            // invalid paths lead to Null Topic:
            var topic = Topic.Create("");
            Assert.AreSame(Topic.Null, topic);

            topic = Topic.Create("/");
            Assert.AreSame(Topic.Null, topic);

            // simple path:
            var sub = Topic.Create("root/sub");
            Assert.AreEqual("root/sub", sub.FullName);
            Assert.AreEqual("sub", sub.Name);
            var root = Topic.Cursor.Children.Find(t => t.FullName == "root");
            Assert.AreSame(root, sub.Parent);
        }

        [Test]
        public void TrimTopicNames()
        {
            Assert.AreEqual("name", Topic.Create("   name  ").Name);
            Assert.AreEqual("name", Topic.Create("   name  ").FullName);
            Assert.AreEqual("name", Topic.Create(" \t  name\n  ").FullName);
            Assert.AreEqual("a/b/c", Topic.Create("   a /  \t b /  c\n ").FullName);
        }

        [Test]
        public void Parents()
        {
            var sub3 = Topic.Create("root/sub1/sub2/sub3");

            var sub2 = sub3.Parent;
            Assert.AreEqual("root/sub1/sub2", sub2.FullName);
            Assert.That(!sub2.IsRoot);

            var sub1 = sub2.Parent;
            Assert.AreEqual("root/sub1", sub1.FullName);
            Assert.That(!sub1.IsRoot);

            var root = sub1.Parent;
            Assert.AreEqual("root", root.FullName);
            Assert.That(root.IsRoot);

            Assert.AreSame(Topic.Root, root.Parent);
        }

        [Test]
        public void CursorDown()
        {
            Assert.AreSame(Topic.Root, Topic.Cursor);

            Topic.Create("r1/s1/t1/u1");
            Assert.AreSame(Topic.Root, Topic.Cursor);

            Assert.IsTrue(Topic.CursorMoveDown("r1"));
            Assert.That(Topic.Cursor.FullName == "r1");

            Assert.IsTrue(Topic.CursorMoveDown("s1"));
            Assert.That(Topic.Cursor.FullName == "r1/s1");

            Assert.IsFalse(Topic.CursorMoveDown("t0"));

            Assert.IsTrue(Topic.CursorMoveDown("t1"));
            Assert.That(Topic.Cursor.FullName == "r1/s1/t1");

            Topic.CursorMoveDown("u1");
            Assert.That(Topic.Cursor.FullName == "r1/s1/t1/u1");
            Assert.That(Topic.Cursor.IsLeaf);

            Assert.IsFalse(Topic.CursorMoveDown("v1"));

            Topic.ClearAll();
            Assert.AreSame(Topic.Root, Topic.Cursor);
        }

        [Test]
        public void CursorUp()
        {
            Topic.Create("r1/s1/t1/u1");
            Topic.CursorMoveDown("r1");
            Topic.CursorMoveDown("s1");
            Topic.CursorMoveDown("t1");
            Topic.CursorMoveDown("u1");
            Assert.That(Topic.Cursor.FullName == "r1/s1/t1/u1");

            Assert.IsTrue(Topic.CursorMoveUp());
            Assert.That(Topic.Cursor.FullName == "r1/s1/t1");

            Assert.IsTrue(Topic.CursorMoveUp());
            Assert.That(Topic.Cursor.FullName == "r1/s1");

            Assert.IsTrue(Topic.CursorMoveUp());
            Assert.That(Topic.Cursor.FullName == "r1");

            Assert.IsTrue(Topic.CursorMoveUp());
            Assert.AreSame(Topic.Root, Topic.Cursor);
            Assert.That(Topic.Cursor.FullName == "");
        }
        
        [Test]
        public void CursorHome()
        {
            Topic.Create("r1/s1/t1/u1");
            Topic.CursorMoveDown("r1");
            Topic.CursorMoveDown("s1");
            Topic.CursorMoveDown("t1");
            Topic.CursorMoveDown("u1");
            Assert.That(Topic.Cursor.FullName == "r1/s1/t1/u1");

            Topic.CursorHome();
            
            Assert.AreSame(Topic.Root, Topic.Cursor);
            Assert.AreEqual("", Topic.Cursor.FullName);
            
            Topic.CursorMoveDown("r1");
            Assert.AreEqual("r1", Topic.Cursor.FullName);
        }


        [Test]
        public void CursorSetTo()
        {
            Topic.Create("r1/s1/t1/u1");
            Topic.Create("r4/s4/t1/u1");

            var r1 = Topic.Cursor.Children.Find(topic => topic.FullName == "r1");
            Assert.IsTrue(Topic.CursorSetTo("r1/s1/t1/u1"));
            Assert.AreEqual(Topic.Cursor.FullName, "r1/s1/t1/u1");
            
            Assert.IsFalse(Topic.CursorSetTo("non/existing/path"));
            Assert.AreEqual(Topic.Cursor.FullName, "r1/s1/t1/u1");
            
            Assert.IsFalse(Topic.CursorSetTo(""));
            Assert.AreEqual(Topic.Cursor.FullName, "r1/s1/t1/u1");
        }

        [Test]
        public void AddChildren()
        {
            Topic.Create("r1/s1/t1/u1");
            Assert.AreEqual(1, Topic.Cursor.Children.Count);
            Assert.IsTrue(Topic.Cursor.Children.Exists(topic => topic.Name == "r1"));

            Topic.Create("r2/s2");
            Assert.AreEqual(2, Topic.Cursor.Children.Count);
            Assert.IsTrue(Topic.Cursor.Children.Exists(topic => topic.Name == "r1"));
            Assert.IsTrue(Topic.Cursor.Children.Exists(topic => topic.Name == "r2"));
        }
        
        [Test]
        public void Tree()
        {
            Topic.Create("r1/s1/t1/u1");
            Topic.CursorSetTo("r1");
            Assert.AreEqual(1, Topic.Cursor.Children.Count);
            
            Topic.Create("r1/s1/t1/u2");
            Assert.AreEqual(1, Topic.Cursor.Children.Count);

            Topic.Create("r1/s1/t2/u1");
            Assert.AreEqual(1, Topic.Cursor.Children.Count);

            Topic.Create("r1/s2/t1/u1");
            Assert.AreEqual(2, Topic.Cursor.Children.Count);
            
            Topic.Create("r1/s3/t1/u1");
            Topic.Create("r2/s1/t1/u1");
            Topic.Create("r3/s1/t1/u1");
            Topic.Create("r4/s1/t1/u1");
            Topic.Create("r4/s2/t1/u1");
            Topic.Create("r4/s3/t1/u1");
            Topic.Create("r4/s4/t1/u1");

            Topic.CursorHome();
            Assert.AreEqual(4, Topic.Cursor.Children.Count);
            Assert.IsTrue(Topic.Cursor.Children.Exists(topic => topic.Name == "r1"));
            Assert.IsTrue(Topic.Cursor.Children.Exists(topic => topic.Name == "r2"));
            Assert.IsTrue(Topic.Cursor.Children.Exists(topic => topic.Name == "r3"));
            Assert.IsTrue(Topic.Cursor.Children.Exists(topic => topic.Name == "r4"));

            Topic.CursorSetTo("r1/s1/t1");
            Assert.AreEqual(2, Topic.Cursor.Children.Count);
            Assert.IsTrue(Topic.Cursor.Children.Exists(topic => topic.Name == "u1"));
            Assert.IsTrue(Topic.Cursor.Children.Exists(topic => topic.Name == "u2"));
        }
    }
}