using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine.Networking;
using GQ.Client.Net;
using GQ.Client.Conf;
using QM.NFC;
using GQ.Client.Util;
using GQ.Client.Model;

[System.Serializable]
public class QuestRuntimeAsset {

	public Texture2D texture;
	public AudioClip clip;
	public string key;

	public QuestRuntimeAsset (string k, Texture2D t2d) {
		key = k;
		texture = t2d;
	}

	public QuestRuntimeAsset (string k, AudioClip aclip) {
		key = k;
		clip = aclip;
	}

}