using System;
using System.Collections.Generic;
using System.Net;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using UnityEngine;

namespace Code.GQClient.Util.http {

	public class HTTP {

		public const string CONTENT_LENGTH = "Content-Length";
		public const string LAST_MODIFIED = "Last-Modified";

		public static Dictionary<string, string> GetRequestHeaders (string url) {
			return RequestHeaderGetter (url);
		}

		public static Dictionary<string, string> internalGetRequestHeaders (string url) {
			
			Dictionary<string, string> headers = new Dictionary<string, string>();

			WebRequest webRequest = null;
			try {
				webRequest = HttpWebRequest.Create(url);
				webRequest.Timeout = (int) Config.Current.maxIdleTimeMS;
			}
			catch (Exception e) {
				Log.SignalErrorToDeveloper ("HTTP.GetRequestHeaders could not create WebRequest. " + e.Message);
				return headers;
			}

			webRequest.Method = "HEAD";
			try {
				using ( WebResponse webResponse = webRequest.GetResponse() ) {
					Debug.Log("Response Content-Length: " + webResponse.ContentLength);
					// use HTTPWebResponse instead:
					// extract timeModified like this: (long)(webResponse.TimeModified - new DateTime(1970, 1, 1)).TotalMilliseconds
					foreach ( string header in webResponse.Headers ) {
						headers.Add(header, webResponse.Headers[header]);
						Debug.Log("Header found: " + header);
					}
				}
			}
			catch (Exception e) {
				Log.SignalErrorToDeveloper ("HTTP.GetRequestHeaders did not get a response. " + e.Message);
			}
				
			return headers;
		}


		#region Delegation API for Tests

		/// <summary>
		/// Sets the default behaviour which is always used but in case of tests.
		/// </summary>
		static HTTP() {
			RequestHeaderGetter = internalGetRequestHeaders;
		}

		public delegate Dictionary<string, string> HTTPGetRequestHeadersMethod(string url);

		public static HTTPGetRequestHeadersMethod RequestHeaderGetter {
			get;
			set;
		}

		#endregion



	}

}