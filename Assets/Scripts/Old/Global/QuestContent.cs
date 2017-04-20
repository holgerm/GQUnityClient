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

[System.Serializable]
public class QuestContent
{

	[XmlAttribute ("id")]
	public int
		id;
	[XmlText ()]
	public string
		content;
	[XmlAnyAttribute ()]
	public XmlAttribute[]
		help_attributes;
	public List<QuestAttribute> attributes;
	[System.NonSerialized ()]
	[XmlElement ("questiontext")]
	public QuestContent
		questiontext;
	[System.NonSerialized ()]
	[XmlElement ("answer")]
	public List<QuestContent>
		answers;

	public string getAttribute (string k)
	{
		
		foreach (QuestAttribute qa in attributes) {
			if (qa.key.Equals (k)) {
				return qa.value;
			}
		}
		return "";
	}

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

	public void deserializeAttributes (int id, bool redo)
	{

		foreach (QuestContent qcdi in answers) {
			qcdi.deserializeAttributes (id, redo);
		}

		if (questiontext != null) {
			questiontext.deserializeAttributes (id, redo);
		}
		
		attributes = new List<QuestAttribute> ();
		
		if (help_attributes != null) {
			foreach (XmlAttribute xmla in help_attributes) {

				if (xmla.Value.StartsWith ("http://") || xmla.Value.StartsWith ("https://")) {
									
					string[] splitted = xmla.Value.Split ('/');
									
					questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
									
					string filename = "files/" + splitted [splitted.Length - 1];
									
//					int i = 0;
//					while ( questdb.loadedfiles.Contains(filename) ) {
//						i++;
//						filename = "files/" + i + "_" + splitted[splitted.Length - 1];
//										
//					}
									
					questdb.loadedfiles.Add (filename);
									
					if (!Application.isWebPlayer) {

						if (!redo || (questdb.currentquest != null && questdb.currentquest.predeployed && filename.ToLower ().Contains (".mp3"))) {
							questdb.downloadAsset (xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
						}
						if (splitted.Length > 3) {
							
							if (questdb.currentquest != null && questdb.currentquest.predeployed && !filename.ToLower ().Contains (".mp3")) {
								
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
		
	}

}
