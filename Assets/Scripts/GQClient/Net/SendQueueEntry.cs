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

		public void serialize () {

			#if !UNITY_WEBPLAYER
			PlayerPrefs.SetInt("currentquestid", questid);
			StringBuilder sb = new StringBuilder();
			JsonWriter jsonWriter = new JsonWriter(sb);
			jsonWriter.PrettyPrint = true;
			JsonMapper.ToJson(this, jsonWriter);

			string dirPath = Application.persistentDataPath + "/quests/" + questid + "/sendqueue/";
			string filePath = dirPath + id + ".json";

			if ( !Directory.Exists(dirPath) ) {
				Directory.CreateDirectory(dirPath);
			}

			if ( File.Exists(filePath) ) {
				File.Delete(filePath);
			}

			File.WriteAllText(filePath, sb.ToString());
			#endif
		}


	}

}