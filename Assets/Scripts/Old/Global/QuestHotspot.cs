using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System;
using System.Linq;
using System.Text;
using GQ.Geo;
using GQ.Util;
using UnitySlippyMap;
using GQ.Client.Model;

[System.Serializable]
[XmlRoot (GQML.HOTSPOT)]
public class QuestHotspot
{

	#region Attributes

	[XmlAttribute ("id")]
	public int id;

	[XmlAttribute ("iBeacon")]
	public int iBeacon;

	[XmlAttribute ("number")]
	public int number;

	[XmlAttribute ("qrcode")]
	public int qrcode;

	[XmlAttribute ("nfc")]
	public int nfc;

	[XmlAttribute ("initialActivity")]
	public bool initialActivity;

	[XmlAttribute ("initialVisibility")]
	public bool initialVisibility;

	// TODO: Do the loading of image files in the new version.
	//	[XmlAttribute("img")]
	//	public string imageURI;

	[XmlAttribute ("radius")]
	public double radius;

	[XmlAttribute ("latlong")]
	public string latlon;

	#endregion


	#region Old Stuff Needs Rework

	[XmlAnyAttribute (), Obsolete]
	public XmlAttribute[]
		help_attributes;

	[Obsolete]
	public List<QuestAttribute> attributes;

	[XmlElement ("onEnter")]
	public QuestTrigger
		onEnter;
	[XmlElement ("onLeave")]
	public QuestTrigger
		onLeave;
	[XmlElement ("onTap")]
	public QuestTrigger
		onTap;
	public int startquest = 0;

	[Obsolete]
	public string getAttribute (string k)
	{
		if (attributes != null) {
			foreach (QuestAttribute qa in attributes) {
			
				if (qa.key.Equals (k)) {
					return qa.value;
				}
			
			}
		}
		
		return "";
		
	}

	[Obsolete]
	public bool hasAttribute (string k)
	{
		
		bool h = false;
		foreach (QuestAttribute qa in attributes) {
			
			if (qa.key.Equals (k)) {
				h = true;
			}
			
		}
		
		return h;
		
	}

	public bool hasActionInChildren (string type1)
	{
		
		bool b = false;
		
		if (onTap != null && onTap.hasActionInChildren (type1)) {
			return true;
		} else if (onEnter != null && onEnter.hasActionInChildren (type1)) {
			return true;
		} else if (onLeave != null && onLeave.hasActionInChildren (type1)) {
			return true;
		} 
		
		return b;
		
	}

	public void deserializeAttributes (int id, bool redo)
	{
		
		attributes = new List<QuestAttribute> ();
		
		if (help_attributes != null) {
			foreach (XmlAttribute xmla in help_attributes) {

				if (xmla.Value.StartsWith ("http://") || xmla.Value.StartsWith ("https://")) {
								
					string[] splitted = xmla.Value.Split ('/');
								
					questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
								
					string filename = "files/" + splitted [splitted.Length - 1];
								
					questdb.loadedfiles.Add (filename);
								
					if (!Application.isWebPlayer) {
									
						if (!redo) {
							questdb.downloadAsset (xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
						}
						if (splitted.Length > 3) {
										
							if (questdb != null && QuestManager.Instance.CurrentQuest != null && QuestManager.Instance.CurrentQuest.predeployed) {
								xmla.Value = questdb.PATH_2_PREDEPLOYED_QUESTS + "/" + id + "/" + filename;
								
							} else {
								
								xmla.Value = Application.persistentDataPath + "/quests/" + id + "/" + filename;
								
							}
							questdb.performSpriteConversion (xmla.Value);

						}							

					}
								
				}

				attributes.Add (new QuestAttribute (xmla.Name, xmla.Value));
				
			}
		}
		if (onEnter != null) {
			foreach (QuestAction qa in onEnter.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
		if (onLeave != null) {
			foreach (QuestAction qa in onLeave.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
		if (onTap != null) {
			foreach (QuestAction qa in onTap.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
		
	}

	#endregion
	
}
