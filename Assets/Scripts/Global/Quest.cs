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
[XmlRoot("game")]
public class Quest  : IComparable<Quest> {
	
	[XmlAttribute("name")]
	public string
		name;
	[XmlAttribute("id")]
	public int
		id;
	[XmlAttribute("xmlformat")]
	public int
		xmlformat;
	public string filepath;
	[XmlElement("mission")]
	public List<QuestPage>
		pages;
	[XmlElement("hotspot")]
	public List<QuestHotspot>
		hotspots;
	[XmlAnyAttribute()]
	public XmlAttribute[]
		help_attributes;
	public List<QuestAttribute> attributes;
	public List<QuestMetaData> metadata;
	public bool hasData = false;
	public QuestPage currentpage;
	public List<QuestPage> previouspages;
	public string xmlcontent;
	public float start_longitude;
	public float start_latitude;
	public string meta_combined;
	public string meta_Search_Combined;

	public bool predeployed = false;
	public string version;
	public bool acceptedDS = false;

	[XmlIgnore]
	public WWW www;

	public string alternateDownloadLink;

	public Quest () {
		predeployed = false;

	}

	public static Quest CreateQuest (int id) {
		Quest q = new Quest();
		return q.LoadFromText(id, true);
	}

	public string getCategory () {

		string x = "";

		if ( hasMeta("category") ) {

			x = getMeta("category");

		}

		return x;

	}

	public int CompareTo (Quest q) {

		if ( q == null ) {
			return 1;
		}
		else {

			return this.name.ToUpper().CompareTo(q.name.ToUpper());
		}

	}

	/// <summary>
	/// redo equals localload in case a download of the quest has preceeded. In this case it is false. (hm)
	/// </summary>
	/// <returns>The from text.</returns>
	/// <param name="id">Identifier.</param>
	/// <param name="redo">If set to <c>true</c> redo.</param>
	public  Quest LoadFromText (int id, bool redo) {

		Debug.Log("XXX: LoadFromText id: " + id);
	
		string fp = filepath;
		string xmlfilepath = filepath;
		string xmlcontent_copy = xmlcontent;

		if ( xmlcontent_copy != null && xmlcontent_copy.StartsWith("<error>") ) {
			string errMsg = xmlcontent_copy;

			GameObject.Find("QuestDatabase").GetComponent<questdatabase>().showmessage(errMsg);
			return null;
		}

		if ( filepath == null ) {
			xmlfilepath = " ";

		}

		if ( xmlcontent_copy == null ) {

			xmlcontent_copy = " ";
		}

		Encoding enc = System.Text.Encoding.UTF8;
		
		TextReader txr = new StringReader(xmlcontent_copy);

//		Debug.Log ("XML:"+xmlcontent_copy);

		if ( !predeployed && xmlfilepath != null && xmlfilepath.Length > 9 ) {

//			Debug.Log(xmlfilepath);

			if ( !xmlfilepath.Contains("game.xml") ) {

				xmlfilepath = xmlfilepath + "game.xml";

			}
			txr = new StreamReader(xmlfilepath, enc);

		}
		XmlSerializer serializer = new XmlSerializer(typeof(Quest));

		Quest q = serializer.Deserialize(txr) as Quest; 
		q.xmlcontent = xmlcontent;
		q.predeployed = predeployed;

		q.filepath = fp;
		q.hasData = true;
	
		//q.id = id;
//		Debug.Log ("my id is " + id + " -> " + q.id);
		q.deserializeAttributes(redo);
		q.meta_Search_Combined += q.name + "; ";
		q.meta_combined += q.name;


		if ( metadata != null ) {

			metadata.Clear();
		}
		else {

			metadata = new List<QuestMetaData>();
		}

		if ( q.hasAttribute("author") ) {

			q.addMetaData(new QuestMetaData("author", q.getAttribute("author")));

		}

		if ( q.hasAttribute("version") ) {
			
			q.addMetaData(new QuestMetaData("version", q.getAttribute("version")));
			
		}

		foreach ( QuestPage qp in q.pages ) {
			if ( qp.type == "MetaData" ) {

				foreach ( QuestContent qc in qp.contents_stringmeta ) {
					if ( qc.hasAttribute("key") && qc.hasAttribute("value") ) {
						QuestMetaData newmeta = new QuestMetaData();
						newmeta.key = qc.getAttribute("key");
						newmeta.value = qc.getAttribute("value");
						q.addMetaData(newmeta);

					}

				}

			}
		}

		if ( q.pages != null &&
		     q.pages.Count > 0 &&
		     q.pages[0].onStart != null &&
		     q.pages[0].onStart.actions != null &&
		     q.pages[0].onStart.actions.Count > 0 ) {

			foreach ( QuestAction qameta in q.pages[0].onStart.actions ) {

				if ( qameta.type == "SetVariable" ) {
					//	Debug.Log ("found setVar");
					if ( qameta.hasAttribute("var") ) {
						
						QuestMetaData newmeta = new QuestMetaData();
						newmeta.key = qameta.getAttribute("var");
						if ( qameta.value != null && qameta.value.string_value != null && qameta.value.string_value.Count > 0 ) {
							newmeta.value = qameta.value.string_value[0];
						}
						else {
							continue;
						}

						q.addMetaData(newmeta);
					}

				}

			}
		
		}

		return q;
	}

	public void addMetaData (QuestMetaData meta) {

		string key = meta.key;

		List<QuestMetaData> todelete = new List<QuestMetaData>();

		if ( metadata == null ) {

			metadata = new List<QuestMetaData>();

		}
		else {
			foreach ( QuestMetaData qmd in metadata ) {

				if ( qmd.key == key ) {
					todelete.Add(qmd);
				}

			}

			foreach ( QuestMetaData qmd in todelete ) {
				metadata.Remove(qmd);
			}

		}

		metadata.Add(meta);



		if ( Configuration.instance.metaCategoryIsSearchable(meta.key) ) {

			meta_Search_Combined += " " + meta.value;


		}
		if ( Configuration.instance.getMetaCategory(meta.key) != null ) {

			Configuration.instance.getMetaCategory(meta.key).addPossibleValues(meta.value);

		}


		meta_combined += ";" + meta.value;

		questdatabase questdb = GameObject.Find("QuestDatabase").GetComponent<questdatabase>();

		if ( !questdb.allmetakeys.Contains(meta.key) ) {

			questdb.allmetakeys.Add(meta.key);
		}

	}

	public void deserializeAttributes (bool redo) {

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
						
						if ( !redo ) {
							questdb.downloadAsset(xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
						}
						if ( splitted.Length > 3 ) {

							if ( predeployed ) {
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

		if ( pages != null ) {
			foreach ( QuestPage qp in pages ) {
				qp.deserializeAttributes(id, redo);
			}
		}
		else {

			Debug.Log("no pages");
		}
		if ( hotspots != null ) {

			foreach ( QuestHotspot qh in hotspots ) {
				qh.deserializeAttributes(id, redo);
			}
		}

	}

	public string getAttribute (string k) {
		
		foreach ( QuestAttribute qa in attributes ) {
			
			if ( qa.key.Equals(k) ) {
				return qa.value;
			}
			
		}
		
		return "";
		
	}

	public string getMeta (string k) {
		if ( metadata != null ) {
			foreach ( QuestMetaData qa in metadata ) {
			
				if ( qa.key.Equals(k) ) {
					return qa.value;
				}
			
			}
		}
		
		return "";
		
	}

	public string getMetaComparer (string k) {
		if ( metadata != null ) {
			foreach ( QuestMetaData qa in metadata ) {
				
				if ( qa.key.Equals(k) ) {
					return qa.value;
				}
				
			}
		}
		
		return ((char)0xFF).ToString();
		
	}

	public bool hasMeta (string k) {

		bool h = false;
		if ( metadata != null ) {
			foreach ( QuestMetaData qa in metadata ) {
			
				if ( qa.key != null ) {
					if ( qa.key.Equals(k) ) {
						h = true;
					}
				}
			}
		}
		
		return h;

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

	public bool hasActionInChildren (string type1) {
		
		bool b = false;
	
		foreach ( QuestPage qp in pages ) {
			if ( !b ) {
				if ( qp.hasActionInChildren(type1) ) {
					b = true;
				}
			}
		}
		foreach ( QuestHotspot qh in hotspots ) {
			if ( !b ) {
				if ( qh.hasActionInChildren(type1) ) {
					b = true;
				}
			}
		}
		
		return b;
		
	}
	
}
