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
public class QuestAction {

	[XmlAttribute("type")]
	public string
		type;
	[XmlElement("value")]
	public QuestVariableValue
		value;
	[XmlAnyAttribute()]
	public XmlAttribute[]
		help_attributes;
	public List<QuestAttribute> attributes;
	[System.NonSerialized()]
	[XmlArray("then"),XmlArrayItem("action")]
	public List<QuestAction>
		thenactions;
	[System.NonSerialized()]
	[XmlArray("else"),XmlArrayItem("action")]
	public List<QuestAction>
		elseactions;
	[XmlElement("condition")]
	public QuestConditionGrouper
		condition;

	public string getAttribute (string k) {
		
		foreach ( QuestAttribute qa in attributes ) {
			
			if ( qa.key.Equals(k) ) {
				return qa.value;
			}
			
		}
		
		return "";
		
	}

	public bool hasMissionAction () {

		bool b = false;

		if ( type == "StartMission" ) {
			b = true;
		}
		else
		if ( thenactions.Count > 0 || elseactions.Count > 0 ) {

			foreach ( QuestAction qa in thenactions ) {
				if ( qa.hasMissionAction() ) {
					b = true;
				} 
			}
			foreach ( QuestAction qa in elseactions ) {
				if ( qa.hasMissionAction() ) {
					b = true;
				} 
			}

		}

		return b;
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

	public void Invoke () {

		//Debug.Log (type);
		GameObject.Find("QuestDatabase").GetComponent<actions>().doAction(this);

	}

	public void InvokeThen () {
		
		if ( thenactions != null && thenactions.Count > 0 ) {
			foreach ( QuestAction qa in thenactions ) {
				
				qa.Invoke();
				
			}
		}
		
	}

	public void InvokeElse () {
		
		if ( elseactions != null && thenactions.Count > 0 ) {
			foreach ( QuestAction qa in elseactions ) {
				
				qa.Invoke();
				
			}
		}
		
	}

	public void deserializeAttributes (int id, bool redo) {

		attributes = new List<QuestAttribute>();
		
		if ( help_attributes != null ) {
			foreach ( XmlAttribute xmla in help_attributes ) {

				if ( xmla.Value.StartsWith("http://") || xmla.Value.StartsWith("https://") ) {
									
					string[] splitted = xmla.Value.Split('/');
									
					questdatabase questdb = GameObject.Find("QuestDatabase").GetComponent<questdatabase>();
									
					string filename = "files/" + splitted[splitted.Length - 1];
									
					int i = 0;
					while ( questdb.loadedfiles.Contains(filename) ) {
						i++;
						filename = "files/" + i + "_" + splitted[splitted.Length - 1];
										
					}
									
					questdb.loadedfiles.Add(filename);
									
					if ( !Application.isWebPlayer ) {
										
						if ( !redo || (questdb.currentquest.predeployed && filename.ToLower().Contains(".mp3")) ) {
							questdb.downloadAsset(xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
						}
						if ( splitted.Length > 3 ) {
											
							if ( questdb.currentquest.predeployed && !filename.ToLower().Contains(".mp3") ) {
								
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

		foreach ( QuestAction qa in thenactions ) {
			qa.deserializeAttributes(id, redo);
		}

		foreach ( QuestAction qa in elseactions ) {
			qa.deserializeAttributes(id, redo);
		}

	}

}
