using UnityEngine;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.IO;
using System;
using GQ.Client.Net;

namespace GQ.Client.Net {

	[System.Serializable]
	public class SendQueueEntry {

		public const string MODE_VALUE = "value";
		public const string MODE_FILE_START = "file_start";
		public const string MODE_FILE_MID = "file_mid";
		public const string MODE_FILE_FINISH = "file_finish";


		public int id;
		public int questid;
		public string ip;
		public float timeout;
		public string mode;
		public string var;
		public string value;
		public string filetype;
		public byte[] file;
		public bool resetid = false;

	}

}