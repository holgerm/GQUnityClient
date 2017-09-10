using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using GQ.Client.Err;
using System;

namespace GQ.Client.Util {

	public class HTTP {

		public const string CONTENT_LENGTH = "Content-Length";
		public const string LAST_MODIFIED = "Last-Modified";

		public static Dictionary<string, string> GetRequestHeaders (string url) {
			
			Dictionary<string, string> headers = new Dictionary<string, string>();

			WebRequest webRequest = null;
			try {
				webRequest = HttpWebRequest.Create(url);
			}
			catch (Exception e) {
				Log.SignalErrorToDeveloper ("HTTP.GetRequestHeaders could not create WebRequest. " + e.Message);
				return headers;
			}

			webRequest.Method = "HEAD";
			try {
				using ( WebResponse webResponse = webRequest.GetResponse() ) {
					foreach ( string header in webResponse.Headers ) {
						headers.Add(header, webResponse.Headers[header]);
					}
				}
			}
			catch (Exception e) {
				Log.SignalErrorToDeveloper ("HTTP.GetRequestHeaders did not get a response. " + e.Message);
			}
				
			return headers;
		}
	}

}