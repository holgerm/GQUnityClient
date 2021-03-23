using NUnit.Framework;
using GQClient.Model;

namespace GQTests.Management {

	public class QuestInfoManagerChangeEventsTest {

		[SetUp]
		public void ResetQMInstance () {
			QuestInfoManager.Reset();
		}

		[Test]
		public void InitQM () {
			// Arrange:
			QuestInfoManager qm = null;

			// Act:
			qm = QuestInfoManager.Instance;

			// Assert:
			Assert.NotNull(qm);
			Assert.AreEqual(0, qm.Count);
		}

	}




}
