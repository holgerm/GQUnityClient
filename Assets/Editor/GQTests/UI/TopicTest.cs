using System;
using Code.GQClient.UI.Foyer;
using GQClient.Model;
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

        [Test]
        public void AddSameAgain()
        {
            Assert.AreEqual(0, Topic.Cursor.Children.Count);
            Topic.Create("r1");
            Assert.AreEqual(1, Topic.Cursor.Children.Count);
            Topic.Create("r1");
            Assert.AreEqual(1, Topic.Cursor.Children.Count);

            Topic.Create("r1/s1");
            Topic.CursorSetTo("r1");
            Assert.AreEqual(1, Topic.Cursor.Children.Count);
            Topic.Create("r1/s1");
            Assert.AreEqual(1, Topic.Cursor.Children.Count);
            Topic.Create("r1/s2");
            Assert.AreEqual(2, Topic.Cursor.Children.Count);
            Topic.Create("r1/s2");
            Assert.AreEqual(2, Topic.Cursor.Children.Count);
            Topic.Create("r1/s1");
            Topic.Create("r1/s1");
            Topic.Create("r1/s2");
            Topic.Create("r1/s2");
            Assert.AreEqual(2, Topic.Cursor.Children.Count);

            Topic.Create("r2/s1/t1/u1");
            Topic.Create("r2/s1/t1/u1");
            Topic.Create("r2/s1/t1/u1");
            Topic.Create("r2/s1/t1/u1");
            Topic.CursorSetTo("r2/s1/t1");
            Assert.AreEqual(1, Topic.Cursor.Children.Count);

            Topic.Create("r2/s1/t1/u2");
            Topic.Create("r2/s1/t1/u2");
            Topic.Create("r2/s1/t1/u2");
            Topic.Create("r2/s1/t1/u2");
            Assert.AreEqual(2, Topic.Cursor.Children.Count);
        }

        [Test]
        public void Leaves()
        {
            Topic.Create("r2/s1/t1/u2");
            Assert.That(Topic.Cursor.IsEmpty);
            Topic.CursorSetTo("r2/s1/t1/u2");
            Assert.That(Topic.Cursor.IsEmpty);

            Topic.Cursor.AddQuestInfo(new TestQuestInfo(1));
            Assert.IsFalse(Topic.Cursor.IsEmpty);
            Assert.AreEqual(1, Topic.Cursor.NumberOfAllQuestInfos);

            // add same questinfo again should not change number:
            Topic.Cursor.AddQuestInfo(new TestQuestInfo(1));
            Assert.IsFalse(Topic.Cursor.IsEmpty);
            Assert.AreEqual(1, Topic.Cursor.NumberOfAllQuestInfos);

            // add another questinfo changes number:
            var qi_2 = new TestQuestInfo(2).WithTopicPaths("r2/s1/t1/u2");
            Topic.Cursor.AddQuestInfo(qi_2);
            Topic.Cursor.AddQuestInfo(new TestQuestInfo(3));
            Assert.AreEqual(3, Topic.Cursor.NumberOfAllQuestInfos);
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 1));
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 2));
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 3));

            // removing the middle one:
            Assert.IsTrue(Topic.RemoveQuestInfo(qi_2));
            Assert.AreEqual(2, Topic.Cursor.NumberOfAllQuestInfos);
            Assert.IsFalse(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 2));

            // remove the other two:
            Assert.IsTrue(Topic.Cursor.RemoveQuestFromTopic(new TestQuestInfo(1)));
            Assert.IsTrue(Topic.Cursor.RemoveQuestFromTopic(new TestQuestInfo(3)));
            Assert.IsTrue(Topic.Cursor.IsEmpty);
        }

        [Test]
        public void CollectLeavesAlongPath()
        {
            // init wth no quest infos:
            Assert.AreEqual(0, Topic.Cursor.NumberOfAllQuestInfos);
            
            var a1b1 = Topic.Create("a1/b1");
            var a1b2 = Topic.Create("a1/b2");
            var a2 = Topic.Create("a2");
            a1b1.AddQuestInfo(new TestQuestInfo(1));
            a1b2.AddQuestInfo(new TestQuestInfo(2));
            a2.AddQuestInfo(new TestQuestInfo(3));

            // check leaves:
            Topic.CursorSetTo("a1/b1");
            Assert.AreEqual(1, Topic.Cursor.NumberOfAllQuestInfos);
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 1));

            Topic.CursorSetTo("a1/b2");
            Assert.AreEqual(1, Topic.Cursor.NumberOfAllQuestInfos);
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 2));

            Topic.CursorSetTo("a2");
            Assert.AreEqual(1, Topic.Cursor.NumberOfAllQuestInfos);
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 3));

            // check inner nodes:
            Topic.CursorSetTo("a1");
            Assert.AreEqual(2, Topic.Cursor.NumberOfAllQuestInfos);
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 1));
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 2));

            Topic.CursorHome();
            Assert.AreEqual(3, Topic.Cursor.NumberOfAllQuestInfos);
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 1));
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 2));
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 3));
            
            // do not count same quests twice even if they occure in different topics:
            a2.AddQuestInfo(new TestQuestInfo(2));
            Topic.CursorHome();
            Assert.AreEqual(3, Topic.Cursor.NumberOfAllQuestInfos);
        }

        [Test]
        public void Remove()
        {
            var qi_1311 = new TestQuestInfo(1311).WithTopicPaths("r1/s3/t1/u1");
            Topic.InsertQuestInfo(qi_1311);
            var qi_2111 = new TestQuestInfo(2111).WithTopicPaths("r2/s1/t1/u1");
            Topic.InsertQuestInfo(qi_2111);
            var qi_3111 = new TestQuestInfo(3111).WithTopicPaths("r3/s1/t1/u1");
            Topic.InsertQuestInfo(qi_3111);
            var qi_4111 = new TestQuestInfo(4111).WithTopicPaths("r4/s1/t1/u1");
            Topic.InsertQuestInfo(qi_4111);
            var qi_4211 = new TestQuestInfo(4211).WithTopicPaths("r4/s2/t1/u1");
            Topic.InsertQuestInfo(qi_4211);
            var qi_4311 = new TestQuestInfo(4311).WithTopicPaths("r4/s3/t1/u1");
            Topic.InsertQuestInfo(qi_4311);
            var qi_4312 = new TestQuestInfo(4411).WithTopicPaths("r4/s3/t1/u2");
            Topic.InsertQuestInfo(qi_4312);
            Assert.AreEqual(7, Topic.Cursor.NumberOfAllQuestInfos);
            Topic.CursorSetTo("r4/s2/t1/u1");
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 4211));

            // remove quest info from leave topic KEEPS topic node itself:
            Topic.RemoveQuestInfo(qi_4211);
            Assert.IsTrue(Topic.CursorSetTo("r4/s2/t1/u1"));
            Assert.IsFalse(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 4211));
            Assert.AreEqual(0, Topic.Cursor.NumberOfOwnQuestInfos);
            
            // remove one and only quest info from inner topic KEEPS the empty topic and its subtopics:
            Topic.CursorSetTo("r3");
            var qi_3 = new TestQuestInfo(3).WithTopicPaths("r3");
            Topic.InsertQuestInfo(qi_3);
            Assert.AreEqual(1, Topic.Cursor.NumberOfOwnQuestInfos);
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 3));
            // do
            Topic.RemoveQuestInfo(qi_3);
            // check
            Assert.That(Topic.CursorSetTo("r3"));
            Assert.AreEqual(0, Topic.Cursor.NumberOfOwnQuestInfos);
            Assert.IsFalse(Topic.Cursor.IsLeaf);
            Assert.That(Topic.CursorSetTo("r3/s1"));
            Assert.That(Topic.CursorSetTo("r3/s1/t1/u1"));
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 3111));

            // remove one of two questinfos from a topic KEEPS the other:
            var qi_4111_0 = new TestQuestInfo(41110).WithTopicPaths("r4/s1/t1/u1");
            Topic.InsertQuestInfo(qi_4111_0);
            Topic.CursorSetTo("r4/s1/t1/u1");
            Assert.AreEqual(2, Topic.Cursor.NumberOfOwnQuestInfos);
            // do
            Topic.RemoveQuestInfo(qi_4111);
            // check:
            Assert.That(Topic.CursorSetTo("r4/s1/t1/u1"));
            Assert.AreEqual(1, Topic.Cursor.NumberOfOwnQuestInfos);
            Assert.IsTrue(Array.Exists(Topic.Cursor.GetQuestInfos(), info => info.Id == 41110));
        }

        private class TestQuestInfo : QuestInfo
        {
            public TestQuestInfo(int id)
            {
                Id = id;
            }


            public TestQuestInfo WithTopicPaths(params string[] topicPaths)
            {
                foreach (var topicPath in topicPaths)
                {
                    Topics.Add(topicPath);
                }

                return this;
            }
        }
    }
}