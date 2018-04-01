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
			Mock.DeclareServerResponseByFile ("http://www.testserver.com", "Util/Downloader/hello.txt");

			Downloader d = new Downloader ("http://www.testserver.com");
			d.Start ();
			Assert.AreEqual ("Hello!", Convert.ToString(d.Result));
		}


		[Test]
		public void PersistentDataPath() {
			Mock.Use = true;

			Assert.AreEqual (Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "persistentDataPath"), Device.GetPersistentDatapath ());

			Mock.Use = false;
			Assert.AreEqual (Application.persistentDataPath, Device.GetPersistentDatapath ());
		}


		[Test]
		public void HTTPResponseHeaders() {
			Mock.Use = true;
			string url = "www.example.com/test/1";
			Mock.DeclareRequestResponse (url, HTTP.CONTENT_LENGTH, "1001");
			Mock.DeclareRequestResponse (url, HTTP.LAST_MODIFIED, "10101010");

			Dictionary<string, string> headers = HTTP.GetRequestHeaders (url);
			Assert.AreEqual ("1001", headers [HTTP.CONTENT_LENGTH]);
			Assert.AreEqual ("10101010", headers [HTTP.LAST_MODIFIED]);
		}
	}
}
