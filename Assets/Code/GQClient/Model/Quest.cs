using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using GQ.Geo;
using GQ.Util;
using UnitySlippyMap;
using GQ.Client.Conf;
using System.ComponentModel;
using System.Xml.Schema;
using System;

namespace GQ.Client.Model {

	[System.Serializable]
	[XmlRoot(GQML.QUEST)]
	public class Quest  : IComparable<Quest>, IXmlSerializable {

		#region Attributes

		public string Name { get; set; }

		public int Id { get; set; }

		[XmlAttribute("lastUpdate")]
		public long
			lastUpdate;

		[XmlAttribute("xmlformat")]
		public int
			xmlformat;

		#endregion

		#region Pages aka missions

		[Obsolete]
		public List<QuestPage>
			PageList = new List<QuestPage>();

		protected Dictionary<int, QuestPage> pageDict = new Dictionary<int, QuestPage>();

		public QuestPage GetPageWithID (int id) {
			QuestPage page;
			pageDict.TryGetValue(id, out page);
			return page;
		}

		#endregion


		#region Hotspots

		[Obsolete]
		public List<QuestHotspot>
			hotspotList = new List<QuestHotspot>();

		protected Dictionary<int, QuestHotspot> hotspotDict = new Dictionary<int, QuestHotspot>();

		public QuestHotspot GetHotspotWithID (int id) {
			QuestHotspot hotspot;
			hotspotDict.TryGetValue(id, out hotspot);
			return hotspot;
		}

		#endregion


		#region IXmlSerializable

		public System.Xml.Schema.XmlSchema GetSchema () {
			return null;
		}

		public void ReadXml (System.Xml.XmlReader reader) {
			Debug.Log("ReadXML called on " + GetType().Name);

			reader.MoveToContent();

			// Name:
			Name = reader.GetAttribute("name");

			// Id:
			int id;
			if ( !Int32.TryParse(reader.GetAttribute("id"), out id) ) {
				Debug.LogWarning("Id for quest " + Name + " could not be parsed, we find: " + reader.GetAttribute("id"));
			}
			else {
				Id = id;
			}

			// Content:
			XmlSerializer pageSerializer = new XmlSerializer(typeof(QuestPage));
			XmlSerializer hotspotSerializer = new XmlSerializer(typeof(QuestHotspot));

			bool read = false;
			while ( read || reader.Read() ) {
				read = false;
				Debug.Log("Node Type is: " + reader.NodeType.ToString());
				switch ( reader.NodeType ) {
					case XmlNodeType.Element:
						switch ( reader.LocalName ) {
							case GQML.PAGE:
								QuestPage page;
								string pageType = reader.GetAttribute(GQML.PAGE_TYPE);
								switch ( pageType ) {
									case GQML.PAGE_TYPE_NPCTALK:
										page = (QuestPage)pageSerializer.Deserialize(reader);
										read = true;
										pageDict.Add(page.id, page);
										Debug.Log("Added NPCTalk page id: " + page.id);

										// TODO: get rid:
										PageList.Add(page);
										break;
									default:
										Debug.LogWarning("Unknown page type found: " + pageType);
										break;
								}
								break;
							case GQML.HOTSPOT:
								QuestHotspot hotspot = (QuestHotspot)hotspotSerializer.Deserialize(reader);
								read = true;
								hotspotDict.Add(hotspot.id, hotspot);
								Debug.Log("Added hotspot id: " + hotspot.id);

								// TODO: get rid:
								hotspotList.Add(hotspot);
								break;
						}
						break;
					default:
						break;
				}
			}
		}

		public void WriteXml (System.Xml.XmlWriter writer) {
			Debug.LogWarning("WriteXML not implemented for " + GetType().Name);
		}

		#endregion


		#region Old Stuff TODO Reorganize

		public string filepath;



		[XmlAnyAttribute()]
		public XmlAttribute[]
			help_attributes;
		public List<QuestAttribute> attributes;
		public List<QuestMetaData> metadata;
		public bool hasData = false;
		public QuestPage currentpage;
		public List<QuestPage> previouspages = new List<QuestPage>();
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

		public long getLastUpdate () {
			if ( lastUpdate == 0 ) {
				long result;
				if ( PlayerPrefs.HasKey(Id + "_lastUpdate") && long.TryParse(PlayerPrefs.GetString(Id + "_lastUpdate"), out result) ) {
					return result;
				}
				else
					return 0;
			}
			else
				return lastUpdate;
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

				return this.Name.ToUpper().CompareTo(q.Name.ToUpper());
			}

		}

		/// <summary>
		/// redo equals localload in case a download of the quest has preceeded. In this case it is false. (hm)
		/// </summary>
		/// <returns>The from text.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="redo">If set to <c>true</c> redo.</param>
		public  Quest LoadFromText (int id, bool redo) {
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
			q.meta_Search_Combined += q.Name + "; ";
			q.meta_combined += q.Name;


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

			foreach ( QuestPage qp in q.PageList ) {
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

			if ( q.PageList != null &&
			     q.PageList.Count > 0 &&
			     q.PageList[0].onStart != null &&
			     q.PageList[0].onStart.actions != null &&
			     q.PageList[0].onStart.actions.Count > 0 ) {

				foreach ( QuestAction qameta in q.PageList[0].onStart.actions ) {

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

						//					int i = 0;
						//					while ( questdb.loadedfiles.Contains(filename) ) {
						//						i++;
						//						filename = "files/" + i + "_" + splitted[splitted.Length - 1];
						//						
						//					}
						//					
						//					questdb.loadedfiles.Add(filename);

						if ( !Application.isWebPlayer ) {

							if ( !redo ) {
								Debug.Log("GETTING Image for: " + Id + " in " + filename);
								questdb.downloadAsset(xmla.Value, Application.persistentDataPath + "/quests/" + Id + "/" + filename);
							}
							if ( splitted.Length > 3 ) {

								if ( predeployed ) {
									xmla.Value = questdb.PATH_2_PREDEPLOYED_QUESTS + "/" + Id + "/" + filename;

								}
								else {

									xmla.Value = Application.persistentDataPath + "/quests/" + Id + "/" + filename;

								}
								questdb.performSpriteConversion(xmla.Value);

							}
						}

					}	

					attributes.Add(new QuestAttribute(xmla.Name, xmla.Value));

				}
			}

			if ( PageList != null ) {
				foreach ( QuestPage qp in PageList ) {
					qp.deserializeAttributes(Id, redo);
				}
			}
			else {

				Debug.Log("no pages");
			}
			if ( hotspotList != null ) {

				foreach ( QuestHotspot qh in hotspotList ) {
					qh.deserializeAttributes(Id, redo);
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

		public bool getBoolAttribute (string attName) {
			string attValue = getAttribute(attName);
			if ( attValue.Trim().Equals("") || attValue.Equals("0") || attValue.ToLower().Equals("false") )
				return false;
			else
				return true;
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

			foreach ( QuestPage qp in PageList ) {
				if ( !b ) {
					if ( qp.hasActionInChildren(type1) ) {
						b = true;
					}
				}
			}
			foreach ( QuestHotspot qh in hotspotList ) {
				if ( !b ) {
					if ( qh.hasActionInChildren(type1) ) {
						b = true;
					}
				}
			}

			return b;

		}

		#endregion

	}

}

