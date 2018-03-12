using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;
using System;
using GQ.Editor.Util;
using GQTests;
using System.IO;

namespace Qm.Mocks {

	public class MockDownloader {

		public static bool Use {
			get {
				return _use;
			}
			set {
				_use = value;
				if (_use) {
					Downloader.CoroutineRunner = MockDownloadAsCoroutine;
				}
			}
		}
		private static bool _use = false;


		static public IEnumerator MockDownloadAsCoroutine (Downloader d)
		{
			Uri uri = new Uri (d.Url);
			string path = Files.CombinePath (GQAssert.TEST_DATA_SERVER_DIR, uri.Host, uri.AbsolutePath);
//			d.Result = File.ReadAllBytes (path);
			Debug.Log("MOCK DOWNLOADER would load from file: " + path);

			d.Raise (DownloadEventType.Success, new DownloadEvent (message: "Mock downlaod done."));
			d.RaiseTaskCompleted (d.Result);

			yield break;
		}

	}
}
