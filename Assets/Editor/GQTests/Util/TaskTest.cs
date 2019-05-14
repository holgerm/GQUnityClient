using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using GQ.Client.Util;

namespace GQTests.Util
{

	public class TaskTest {

		[Test]
		public void TaskRunningAsCoroutine() {

			TestTaskCR t = new TestTaskCR ();
			Assert.IsTrue (t.RunsAsCoroutine);

			t.Start ();
			Assert.IsTrue (t.started);

		}

		public class TestTaskCR : Task {

			public bool started = false;

			public TestTaskCR() : base(true) {

			}

			protected override IEnumerator DoTheWork() {
				started = true;
				yield break;
			}
		}

	}

}
