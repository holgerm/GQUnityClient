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
public class QuestPage {

	[XmlAttribute("id")]
	public int
		id;
	[XmlAttribute("type")]
	public string
		type;
	[XmlAnyAttribute()]
	public XmlAttribute[]
		help_attributes;
	public List<QuestAttribute> attributes;
	[XmlElement("dialogitem")]
	public List<QuestContent>
		contents_dialogitems;
	[XmlElement("expectedCode")]
	public List<QuestContent>
		contents_expectedcode;
	[XmlElement("answer")]
	public List<QuestContent>
		contents_answers;
	[XmlElement("question")]
	public QuestContent
		contents_question;
	[XmlElement("answers")]
	public List<QuestContent>
		contents_answersgroup;
	[XmlElement("stringmeta")]
	public List<QuestContent>
		contents_stringmeta;
	[XmlElement("onEnd")]
	public QuestTrigger
		onEnd;
	[XmlElement("onStart")]
	public QuestTrigger
		onStart;
	[XmlElement("onTap")]
	public QuestTrigger
		onTap;
	[XmlElement("onSuccess")]
	public QuestTrigger
		onSuccess;
	[XmlElement("onFail")]
	public QuestTrigger
		onFailure;
	[XmlElement("onRead")]
	public QuestTrigger
		onRead;
	public string state;
	public string result;

	public QuestPage () {

		state = "new";
		result = null;
	}

	public string getAttribute (string k) {
		
		foreach ( QuestAttribute qa in attributes ) {

			if ( qa.key.Equals(k) ) {
				return qa.value;
			}
			
		}

		return "";
		
	}

	public bool hasAttribute (string k) {

		bool h = false;
		foreach ( QuestAttribute qa in attributes ) {
			
			if ( qa.key.Equals(k) ) {
				h = true;
			}
			
		}
		
		return h;
		
	}

	public void deserializeAttributes (int id, bool redo) {

		questdatabase questdb = GameObject.Find("QuestDatabase").GetComponent<questdatabase>();

//		if ( questdb.currentquest != null ) {
		attributes = new List<QuestAttribute>();

		if ( help_attributes != null ) {
			foreach ( XmlAttribute xmla in help_attributes ) {
							
				if ( xmla.Value.StartsWith("http://") || xmla.Value.StartsWith("https://") && !(type == "WebPage" && xmla.Name.ToLower() == "url") ) {

					string[] splitted = xmla.Value.Split('/');

					string filename = "files/" + splitted[splitted.Length - 1];

					int i = 0;
					while ( questdb.loadedfiles.Contains(filename) ) {
						i++;
						filename = "files/" + i + "_" + splitted[splitted.Length - 1];
						
					}

					questdb.loadedfiles.Add(filename);

					if ( !Application.isWebPlayer ) {
				
						if ( !redo ) {
							questdb.downloadAsset(xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
						}
						if ( splitted.Length > 3 ) {

							if ( questdb.currentquest != null && questdb.currentquest.predeployed ) {
								Debug.Log("is predeployed file: " + filename);

								xmla.Value = questdb.PATH_2_PREDEPLOYED_QUESTS + "/" + id + "/" + filename;
								
							}
							else {
								
								xmla.Value = Application.persistentDataPath + "/quests/" + id + "/" + filename;
								
							}						
							questdb.performSpriteConversion(xmla.Value);

						}
					}

				}	
								
				attributes.Add(new QuestAttribute(xmla.Name, xmla.Value));
			
			}
		}

		foreach ( QuestContent qcdi in contents_dialogitems ) {
			qcdi.deserializeAttributes(id, redo);
		}

		foreach ( QuestContent qcdi in contents_answers ) {
			qcdi.deserializeAttributes(id, redo);
		}

		if ( contents_question != null ) {
			contents_question.deserializeAttributes(id, redo);
		}
		foreach ( QuestContent qcdi in contents_answersgroup ) {
			qcdi.deserializeAttributes(id, redo);
		}

		foreach ( QuestContent qcdi in contents_stringmeta ) {
			qcdi.deserializeAttributes(id, redo);
		}

		foreach ( QuestContent qcdi in contents_expectedcode ) {
			qcdi.deserializeAttributes(id, redo);
		}

		if ( onEnd != null ) {
			foreach ( QuestAction qa in onEnd.actions ) {
				qa.deserializeAttributes(id, redo);
			}
		}
		if ( onStart != null ) {
			foreach ( QuestAction qa in onStart.actions ) {
				qa.deserializeAttributes(id, redo);
			}
		}
		if ( onTap != null ) {
			foreach ( QuestAction qa in onTap.actions ) {
				qa.deserializeAttributes(id, redo);
			}
		}
		if ( onSuccess != null ) {
			foreach ( QuestAction qa in onSuccess.actions ) {
				qa.deserializeAttributes(id, redo);
			}
		}
		if ( onFailure != null ) {
			foreach ( QuestAction qa in onFailure.actions ) {
				qa.deserializeAttributes(id, redo);
			}
		}
		if ( onRead != null ) {
			foreach ( QuestAction qa in onRead.actions ) {
				qa.deserializeAttributes(id, redo);
			}
		}
//		}
	}

	public bool hasActionInChildren (string type1) {
		
		if ( onTap != null && onTap.hasActionInChildren(type1) ) {
			return true;
		}
		else
		if ( onEnd != null && onEnd.hasActionInChildren(type1) ) {
			return true;
		}
		else
		if ( onStart != null && onStart.hasActionInChildren(type1) ) {
			return true;
		}
		else
		if ( onSuccess != null && onSuccess.hasActionInChildren(type1) ) {
			return true;
		}
		else
		if ( onFailure != null && onFailure.hasActionInChildren(type1) ) {
			return true;
		}
		else
		if ( onRead != null && onRead.hasActionInChildren(type1) ) {
			return true;
		}
		
		return false;
	}

}
