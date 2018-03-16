using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;
using System;
using GQ.Editor.Util;
using GQTests;
using System.IO;
using GQ.Client.Conf;

namespace QM.Mocks {

	public class Mock {

		#region Initialize and Reset Usage

		public static bool Use {
			get {
				return _use;
			}
			set {
				_use = value;
				MappedUris.Clear ();
				if (_use) {
					// set Downloader:
					Downloader.CoroutineRunner = MockDownloadAsCoroutine;

					// set Persistent Path:
					Device.SetPersistentDataPathMethod(GetMockPersistentPath);
				} else {
					Downloader.CoroutineRunner = Downloader.DownloadAsCoroutine;
					Device.SetPersistentDataPathMethod (() => {
						return Application.persistentDataPath;
					});
				}
			}
		}
		private static bool _use = false;

		#endregion


		#region Download

		/// <summary>
		/// The mock download method put as delegate into the abstract downloader when mocking is used.
		/// </summary>
		static public IEnumerator MockDownloadAsCoroutine (Downloader d)
		{
			Uri uri = new Uri (d.Url);
			string path = MapUri2Path (uri);
			d.Result = File.ReadAllText (path);

			d.Raise (DownloadEventType.Success, new DownloadEvent (message: "Mock downlaod done."));
			d.RaiseTaskCompleted (d.Result);

			yield break;
		}

		static public Dictionary<Uri, string> MappedUris = new Dictionary<Uri, string>();

		public static void DeclareServerResponseByFile (string serverPath, string mockPath) {
			MappedUris[new Uri(ConfigurationManager.GQ_SERVER_BASE_URL + "/" + serverPath)] = mockPath;
		}

		public static void DeclareServerResponseByString (string serverPath, string responseText) {
			string tmpFilePath = Files.CombinePath(GQAssert.TEST_DATA_TEMP_DIR, "server_response.json");
			File.WriteAllText(Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, tmpFilePath), responseText);
			DeclareServerResponseByFile(serverPath, tmpFilePath);
		}

		static string MapUri2Path(Uri uri) {
			string path = null;
			if (MappedUris.TryGetValue (uri, out path)) {
				return Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, path);
			} else {
				return Files.CombinePath (GQAssert.TEST_DATA_SERVER_DIR, uri.Host, uri.AbsolutePath);
			}
		}

		#endregion


		#region Persistent Path

		public static string GetMockPersistentPath () {
			return Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "persistentDataPath");
		}

		#endregion

	}
}
