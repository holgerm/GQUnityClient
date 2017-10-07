using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using GQTests;
using GQ.Editor.Util;
using GQ.Client.Util;

namespace GQTests.Util
{

	public class DownloadTest
	{

		[Test]
		public void FileAccessViaWWW ()
		{
			string filePath = Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "Util/Downloader/hello.txt");

			Assert.IsTrue (Files.ExistsFile (filePath), "File should exist at " + filePath);

			string url = Files.LocalPath4WWW (filePath);
			Debug.Log ("Files.LocalPath4WWW does:\n" + filePath + "\n" + url);
			WWW www = new WWW (url);

			Debug.Log ("text zu beginn: " + www.text);

			while (!www.isDone) {
				Debug.Log ("alive from " + www.url);
			}

			Debug.Log ("done.");

			if (www.error != null && www.error != "") {
				string errMsg = www.error;
				www.Dispose ();
				Debug.Log ("error: " + errMsg);
				Assert.Fail ();
			} else {
				Assert.AreEqual ("Hello!", www.text);
				Debug.Log ("Yeah!");
			}

		}

		[Test]
		public void FileAccess ()
		{
			bool started = false;
			bool succeeded = false;

			string filePath = Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "Util/Downloader/hello.txt");

			Assert.IsTrue (Files.ExistsFile (filePath), "File should exist at " + filePath);

			Downloader downloader = new Downloader (Files.LocalPath4WWW (filePath));
			downloader.OnStart += (d, e) => {
				started = true;
			};
			downloader.OnSuccess += (d, e) => {
				succeeded = true;
			};
			downloader.OnError += (d, e) => {
				Assert.Fail ("Download Error: " + e.Message);
			};

			IEnumerator enumerator = downloader.RunAsCoroutine ();
			while (enumerator.MoveNext ()) {
				Debug.Log ("in while ... " + (enumerator.Current == null ? "null" : enumerator.Current.ToString ()));
			}

			Assert.IsTrue (started, "Should have started the download.");
			Assert.IsTrue (succeeded, "Should have succeeded in downloading.");

			Assert.AreEqual ("Hello!", downloader.Result);
		}

		[Test]
		public void DownloaderUsesCoroutine ()
		{

			Downloader d = new Downloader ("some.url", 60000);

			Assert.IsTrue (d.RunsAsCoroutine);
		}

		[Test]
		public void MultiDownloaderUsesCoroutine ()
		{

			MultiDownloader md = new MultiDownloader ();

			Assert.IsTrue (md.RunsAsCoroutine);
		}

		[Test]
		public void LocalFileLoader ()
		{
			string filePath = Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "Util/Downloader/hello.txt");

			LocalFileLoader fileLoader = new LocalFileLoader (
				                             filePath: filePath
			                             );

			fileLoader.Start ();

			Assert.AreEqual ("Hello!", fileLoader.Result);
		}
	}

}
