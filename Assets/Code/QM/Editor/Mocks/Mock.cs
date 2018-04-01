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
				setDownloader (_use);
				setPersistentPath (_use);
				setHTTPGetRequestHeadersMethod (_use);
			}
		}
		private static bool _use = false;

		private static void setDownloader(bool mocked) {
			MappedUris.Clear ();
			if (mocked) {
				Downloader.CoroutineRunner = MockDownloadAsCoroutine;
			} else {
				Downloader.CoroutineRunner = Downloader.DownloadAsCoroutine;
			}
		}

		private static void setPersistentPath(bool mocked) {
			if (mocked) {
				Device.SetPersistentDataPathMethod(GetMockPersistentPath);
			} else {
				Device.SetPersistentDataPathMethod (() => {
					return Application.persistentDataPath;
				});
			}
		}

		private static void setHTTPGetRequestHeadersMethod(bool mocked) {
			MappedHTTPRequestHeaders.Clear ();
			if (mocked) {
				HTTP.RequestHeaderGetter = MockGetRequestHeaders;
			} else {
				HTTP.RequestHeaderGetter = HTTP.internalGetRequestHeaders;
			}
		}

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

			d.Raise (DownloadEventType.Success, new DownloadEvent (message: "Mock download done."));
			d.RaiseTaskCompleted (d.Result);

			yield break;
		}

		static Dictionary<Uri, string> MappedUris = new Dictionary<Uri, string>();

		public static void DeclareServerResponseByFile(string serverURI, string mockPath) {
			MappedUris [new Uri (serverURI)] = mockPath;
		}

		public static void DeclareGQServerResponseByFile (string serverPath, string mockPath) {
			DeclareServerResponseByFile(ConfigurationManager.GQ_SERVER_BASE_URL + "/" + serverPath, mockPath);
		}

		public static void DeclareGQServerResponseByString (string serverPath, string responseText) {
			string tmpFilePath = Files.CombinePath(GQAssert.TEST_DATA_TEMP_DIR, "server_response.json");
			File.WriteAllText(Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, tmpFilePath), responseText);
			DeclareGQServerResponseByFile(serverPath, tmpFilePath);
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


		#region HTTP

		public static Dictionary<string, string> MockGetRequestHeaders (string url) {
			Dictionary<string, string> headers = null;
			MappedHTTPRequestHeaders.TryGetValue (url, out headers);
			return headers;
		}

		static Dictionary<string, Dictionary<string, string>> MappedHTTPRequestHeaders = new Dictionary<string, Dictionary<string, string>>();

		static Dictionary<string, string> MapUrl2ResponseHeaders(string url) {
			Dictionary<string, string> headers = null;
			MappedHTTPRequestHeaders.TryGetValue (url, out headers);
			return headers;
		}

		public static void DeclareRequestResponse(string url, string headerName, string headerValue) {
			Dictionary<string, string> headers = null;
			if (!MappedHTTPRequestHeaders.TryGetValue(url, out headers)) {
				// for this url we miss a repsonse header collection up to now, hence we create one:
				headers = new Dictionary<string, string> ();
				MappedHTTPRequestHeaders [url] = headers;
			}
			headers [headerName] = headerValue;
		}

		public static void DeclareGQRequestResponse(string serverPath, string headerName, string headerValue) {
			DeclareRequestResponse (ConfigurationManager.GQ_SERVER_BASE_URL + "/" + serverPath, headerName, headerValue);
		}

		#endregion
	}
}
