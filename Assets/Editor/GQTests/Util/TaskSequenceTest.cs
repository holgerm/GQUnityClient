using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using GQ.Client.Util;
using System;

namespace GQTests.Util
{

	public class TaskSequenceTest {

		[Test]
		public void SequencingInitiatedWithArray() {
			SucceedingTask t1 = new SucceedingTask ();
			SucceedingTask t2 = new SucceedingTask ();
			SucceedingTask t3 = new SucceedingTask ();
			TaskSequence ts = new TaskSequence (t1, t2, t3);

			ts.Start ();

			Assert.NotNull (t1.GetOnEndedInvocationList ());
			Assert.AreEqual (1, t1.GetOnEndedInvocationList ().Length);
			Assert.AreEqual (t1.GetOnEndedInvocationList() [0].Target, t2);
			Assert.AreEqual (t1.GetOnEndedInvocationList() [0].Method.Name, "StartCallback");

			Assert.NotNull (t2.GetOnEndedInvocationList ());
			Assert.AreEqual (1, t2.GetOnEndedInvocationList ().Length);
			Assert.AreEqual (t2.GetOnEndedInvocationList() [0].Target, t3);
			Assert.AreEqual (t2.GetOnEndedInvocationList() [0].Method.Name, "StartCallback");

			Assert.Null (t3.GetOnEndedInvocationList ());

			Assert.IsTrue (t1.started);
			Assert.IsTrue (t1.completed);
			Assert.IsTrue (t2.started);
			Assert.IsTrue (t2.completed);
			Assert.IsTrue (t3.started);
			Assert.IsTrue (t3.completed);
		}

		[Test]
		public void SequencingInitiatedWithAppend() {
			SucceedingTask t1 = new SucceedingTask ();
			SucceedingTask t2 = new SucceedingTask ();
			SucceedingTask t3 = new SucceedingTask ();
			TaskSequence ts = new TaskSequence ();
			ts.Append (t1);
			ts.Append (t2);
			ts.Append (t3);

			ts.Start ();

			Assert.NotNull (t1.GetOnEndedInvocationList ());
			Assert.AreEqual (1, t1.GetOnEndedInvocationList ().Length);
			Assert.AreEqual (t1.GetOnEndedInvocationList() [0].Target, t2);
			Assert.AreEqual (t1.GetOnEndedInvocationList() [0].Method.Name, "StartCallback");

			Assert.NotNull (t2.GetOnEndedInvocationList ());
			Assert.AreEqual (1, t2.GetOnEndedInvocationList ().Length);
			Assert.AreEqual (t2.GetOnEndedInvocationList() [0].Target, t3);
			Assert.AreEqual (t2.GetOnEndedInvocationList() [0].Method.Name, "StartCallback");

			Assert.Null (t3.GetOnEndedInvocationList ());

			Assert.IsTrue (t1.started);
			Assert.IsTrue (t2.started);
			Assert.IsTrue (t3.started);
		}

		[Test]
		public void SequencingInitiatedWithAppendOnlySuccess() {
			SucceedingTask t1 = new SucceedingTask ();
			SucceedingTask t2 = new SucceedingTask ();
			SucceedingTask t3 = new SucceedingTask ();
			TaskSequence ts = new TaskSequence (t1, t2, t3);

			ts.Start ();

			Assert.NotNull (t1.GetOnCompletedInvocationList ());
			Assert.AreEqual (1, t1.GetOnCompletedInvocationList ().Length);
			Assert.AreEqual (t1.GetOnCompletedInvocationList() [0].Target, t2);
			Assert.AreEqual (t1.GetOnCompletedInvocationList() [0].Method.Name, "StartCallback");

			Assert.NotNull (t2.GetOnCompletedInvocationList ());
			Assert.AreEqual (1, t2.GetOnCompletedInvocationList ().Length);
			Assert.AreEqual (t2.GetOnCompletedInvocationList() [0].Target, t3);
			Assert.AreEqual (t2.GetOnCompletedInvocationList() [0].Method.Name, "StartCallback");

			Assert.AreEqual (1, t3.GetOnCompletedInvocationList ().Length);
			Assert.AreEqual (t3.GetOnCompletedInvocationList() [0].Target, ts);
			Assert.AreEqual (t3.GetOnCompletedInvocationList() [0].Method.Name, "CompletedCallback");
			Assert.AreEqual (1, t3.GetOnFailedInvocationList ().Length);
			Assert.AreEqual (t3.GetOnFailedInvocationList() [0].Target, ts);
			Assert.AreEqual (t3.GetOnFailedInvocationList() [0].Method.Name, "FailedCallback");

			Assert.IsTrue (t1.started);
			Assert.IsTrue (t2.started);
			Assert.IsTrue (t3.started);
		}

		[Test]
		public void SequencingInitiatedWithAppendSecondTaskFails() {
			SucceedingTask t1 = new SucceedingTask ();
			FailingTask t2 = new FailingTask ();
			SucceedingTask t3 = new SucceedingTask ();
			TaskSequence ts = new TaskSequence (t1, t2, t3);
            ts.Start ();

			Assert.NotNull (t1.GetOnCompletedInvocationList ());
			Assert.AreEqual (1, t1.GetOnCompletedInvocationList ().Length);
			Assert.AreEqual (t1.GetOnCompletedInvocationList() [0].Target, t2);
			Assert.AreEqual (t1.GetOnCompletedInvocationList() [0].Method.Name, "StartCallback");

			Assert.NotNull (t2.GetOnCompletedInvocationList ());
			Assert.AreEqual (1, t2.GetOnCompletedInvocationList ().Length);
			Assert.AreEqual (t2.GetOnCompletedInvocationList() [0].Target, t3);
			Assert.AreEqual (t2.GetOnCompletedInvocationList() [0].Method.Name, "StartCallback");

			Assert.AreEqual (1, t3.GetOnCompletedInvocationList ().Length);
			Assert.AreEqual (t3.GetOnCompletedInvocationList() [0].Target, ts);
			Assert.AreEqual (t3.GetOnCompletedInvocationList() [0].Method.Name, "CompletedCallback");
			Assert.AreEqual (1, t3.GetOnFailedInvocationList ().Length);
			Assert.AreEqual (t3.GetOnFailedInvocationList() [0].Target, ts);
			Assert.AreEqual (t3.GetOnFailedInvocationList() [0].Method.Name, "FailedCallback");

			Assert.IsTrue (t1.started);
			Assert.IsTrue (t1.completed);
			Assert.IsTrue (t2.started);
			Assert.IsTrue (t2.failed);
			Assert.IsFalse (t3.started);
			Assert.IsFalse (t3.completed);
			Assert.IsFalse (t3.failed);
		}

		[Test]
		public void SucceedingTaskSequence() {
			TestSequence ts = new TestSequence ();
			ts.Append(new SucceedingTask());
			ts.Append(new SucceedingTask());
			ts.Append(new SucceedingTask());
				
			ts.Start ();

			Assert.IsTrue (ts.completed);
			Assert.IsFalse (ts.failed);
		}

		[Test]
		public void FailingTaskSequence() {
			TestSequence ts = new TestSequence ();
			ts.Append(new SucceedingTask());
			ts.Append(new SucceedingTask());
			ts.Append(new FailingTask());

			ts.Start ();

			Assert.IsFalse (ts.completed);
			Assert.IsTrue (ts.failed);
		}


		#region Helper Classes

		class SucceedingTask : Task {

			public bool started = false;

			protected override IEnumerator DoTheWork() {
				started = true;
                yield break;
			}

			public bool completed = false;
			public bool failed = false;

			protected override void BeforeCompleted() {
				completed = true;
			}
		}

		class FailingTask : Task {

			public bool started = false;

			protected override IEnumerator DoTheWork() {
				started = true;
                yield break;
			}

			public bool completed = false;
			public bool failed = false;

			protected override void BeforeFailed () {
				failed = true;
			}

		}

		class TestSequence : TaskSequence {

			public bool completed = false;
			public bool failed = false;

			protected override void BeforeFailed () {
				failed = true;
			}

			protected override void BeforeCompleted() {
				completed = true;
			}
		}

		#endregion
	}
}
