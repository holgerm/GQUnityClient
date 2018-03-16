using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System;
using GQ.Client.Util;
using GQTests;
using GQ.Editor.Util;
using QM.Mocks;

namespace QM.Tests {

	public class MockTest {


		[Test]
		public void Downloader() {
			Mock.Use = true;
			Mock.MappedUris [new Uri("http://www.testserver.com")] = "Util/Downloader/hello.txt";

			Downloader d = new Downloader ("http://www.testserver.com");
			d.Start ();
			Assert.AreEqual ("Hello!", d.GetResultAsString());

			Mock.Use = false;
			Assert.AreEqual (0, Mock.MappedUris.Count);
		}

		[Test]
		public void PersistentDataPath() {
			Mock.Use = true;

			Assert.AreEqual (Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "persistentDataPath"), Device.GetPersistentDatapath ());

			Mock.Use = false;
			Assert.AreEqual (Application.persistentDataPath, Device.GetPersistentDatapath ());
		}

	}
}
