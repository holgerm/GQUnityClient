using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using GQ.Editor.Util;
using GQTests;

namespace QM.Mocks {

	public class AbstractMockTest {

		[TearDown]
		public void CleanUpAfterMocking() {
			Files.DeleteDirCompletely (Mock.GetMockPersistentPath ());
			Files.ClearDir(Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, GQAssert.TEST_DATA_TEMP_DIR));
		}

	}
}
