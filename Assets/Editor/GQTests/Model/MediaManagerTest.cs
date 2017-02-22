using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;

namespace GQTests.Model {

	public class MediaManagerTest {

		[SetUp]
		public void ResetQMInstance () {
			MediaManager.Reset();
		}


		[Test]
		public void InitMM () {
			// Arrange:
			MediaManager mm = null;

			// Act:
			mm = MediaManager.Instance;

			// Assert:
			Assert.NotNull(mm);
		}

	}
}
