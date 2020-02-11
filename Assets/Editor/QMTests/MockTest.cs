using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System;
using Code.GQClient.Conf;
using Code.GQClient.Util.http;
using Code.QM.Util;
using GQTests;
using GQ.Editor.Util;
using QM.Mocks;

namespace QM.Tests
{

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

			Assert.AreEqual (
				Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, Mock.MOCK_PERSISTENT_DATA_PATH), 
				Device.GetPersistentDatapath ()
			);

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


		[Test]
		public void OverwriteServerResponseMapping() {
			Mock.Use = true;
			Mock.DeclareGQServerResponseByString ("test", "Originalwert");

			Downloader d = new Downloader (ConfigurationManager.GQ_SERVER_BASE_URL + "/" + "test");
			d.Start ();
			Assert.AreEqual ("Originalwert", Convert.ToString(d.Result));

			Mock.DeclareGQServerResponseByString ("test", "Neuer Wert");

			d.Start ();
			Assert.AreEqual ("Neuer Wert", Convert.ToString(d.Result));
		}


		[Test]
		public void MultipleServerResponseMappingsByString() {
			Mock.Use = true;
			Mock.DeclareGQServerResponseByString ("target1", "Wert 1");
			Mock.DeclareGQServerResponseByString ("target2", "Wert 2");

			Downloader d = new Downloader (ConfigurationManager.GQ_SERVER_BASE_URL + "/" + "target1");
			d.Start ();
			Assert.AreEqual ("Wert 1", Convert.ToString(d.Result));

			d = new Downloader (ConfigurationManager.GQ_SERVER_BASE_URL + "/" + "target2");
			d.Start ();
			Assert.AreEqual ("Wert 2", Convert.ToString(d.Result));}
	}
}
