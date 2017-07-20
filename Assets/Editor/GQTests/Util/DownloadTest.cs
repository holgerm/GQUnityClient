using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using GQTests;
using GQ.Editor.Util;
using GQ.Util;

namespace GQTests.Util {

	public class DownloadTest {

		[Test]
		public void FileAccessViaWWW() 
		{
			string filePath = Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "Util/Downloader/hello.txt");

			Assert.IsTrue (Files.ExistsFile (filePath), "File should exist at " + filePath);

			WWW www = new WWW (Files.LocalPath4WWW(filePath));

			Debug.Log ("text zu beginn: " + www.text);

			while (!www.isDone) {
				Debug.Log ("alive from " + www.url);
			}

			Debug.Log ("done.");

			if ( www.error != null && www.error != "" ) {
				string errMsg = www.error;
				www.Dispose();
				Debug.Log ("error: " + errMsg);
				Assert.Fail ();
			}
			else {
				Assert.AreEqual ("Hello!", www.text);
				Debug.Log ("Yeah!");
			}

		}

		[Test]
		public void FileAccess() {
			bool started = false;
			bool succeeded = false;

			string filePath = Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "Util/Downloader/hello.txt");

			Assert.IsTrue (Files.ExistsFile (filePath), "File should exist at " + filePath);

			Download downloader = new Download (Files.LocalPath4WWW(filePath));
			downloader.OnStart += (d, e) => {
				started = true;
			};
			downloader.OnSuccess += (d, e) => {
				succeeded = true;
			};
			downloader.OnError += (d, e) => {
				Assert.Fail("Download Error: " + e.Message);
			};

			IEnumerator enumerator = downloader.StartDownload ();
			while (enumerator.MoveNext()) {
				Debug.Log ("in while ... " + (enumerator.Current == null ? "null" : enumerator.Current.ToString()));
			}

			Assert.IsTrue (started , "Should have started the download.");
			Assert.IsTrue (succeeded , "Should have succeeded in downloading.");

			Assert.AreEqual ("Hello!", downloader.Response);

		}

	}

}
