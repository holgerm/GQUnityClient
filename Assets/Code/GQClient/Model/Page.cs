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
using GQ.Client.Err;

namespace GQ.Client.Model
{
	

	[System.Serializable]
	[XmlRoot (GQML.PAGE)]
	public abstract class Page : IPage
	{

		#region XML Serialization

		public System.Xml.Schema.XmlSchema GetSchema ()
		{
			return null;
		}

		public void WriteXml (System.Xml.XmlWriter writer)
		{
			Debug.LogWarning ("WriteXML not implemented for " + GetType ().Name);
		}

		/// <summary>
		/// Reader must be at the page element (start). When it returns the reader is position behind the page end element. 
		/// 
		/// This is a template method. Subtypes should only override the ReadAttributes() and ReadContent() methods 
		/// and extend them by calling their base versions.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public void ReadXml (XmlReader reader)
		{
			GQML.AssertReaderAtStart (reader, GQML.PAGE);

			ReadAttributes (reader);

			if (reader.IsEmptyElement) {
				reader.Read ();
				Log.SignalErrorToDeveloper ("Empty ml page element found.");
				return;
			}

			// consume the Begin Action Element:
			reader.Read (); 

			XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
			xmlRootAttr.IsNullable = true;

			while (!GQML.IsReaderAtEnd (reader, GQML.PAGE)) {

				if (reader.NodeType == XmlNodeType.Element)
					ReadContent (reader, xmlRootAttr);
			}

			// consume the closing action tag (if not empty page element)
			if (reader.NodeType == XmlNodeType.EndElement)
				reader.Read ();
		}

		protected virtual void ReadAttributes (XmlReader reader)
		{
			// Id:
			int id;
			if (Int32.TryParse (reader.GetAttribute (GQML.PAGE_ID), out id)) {
				Id = id;
			} else {
				Log.SignalErrorToDeveloper ("Id for a page could not be parsed. We found: " + reader.GetAttribute (GQML.QUEST_ID));
			}
		}

		protected virtual void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			XmlSerializer serializer;

			switch (reader.LocalName) {
			case GQML.ON_START:
				xmlRootAttr.ElementName = GQML.ON_START;
				serializer = new XmlSerializer (typeof(Trigger), xmlRootAttr);
				StartTrigger = (Trigger)serializer.Deserialize (reader);
				break;
			case GQML.ON_END:
				xmlRootAttr.ElementName = GQML.ON_END;
				serializer = new XmlSerializer (typeof(Trigger), xmlRootAttr);
				EndTrigger = (Trigger)serializer.Deserialize (reader);
				break;
			// UNKOWN CASE:
			default:
				Log.WarnDeveloper ("Page has additional unknown {0} element. (Ignored)", reader.LocalName);
				reader.Skip ();
				break;
			}
		}

		protected Trigger StartTrigger = Trigger.Null;
		protected Trigger EndTrigger = Trigger.Null;

		#endregion


		#region State

		public Page ()
		{
			State = GQML.STATE_NEW;

			stateOld = GQML.STATE_NEW;

			result = null;
		}

		Quest quest;

		public Quest Quest {
			get {
				return quest;
			}
			protected set {
				quest = value;
			}
		}

		[XmlAttribute ("id")]
		public int id;

		public int Id {
			get {
				return id;
			}
			protected set {
				id = value;
			}
		}

		[XmlAttribute ("type"), Obsolete]
		public string
			type;

		[Obsolete]
		public string stateOld;

		private string state;

		public string State {
			get {
				return state;
			}
			protected set {
				state = value;
			}
		}

		public string result;

		public string Result {
			get {
				return result;
			}
		}

		#endregion


		#region Runtime API

		public virtual void Start (Quest quest)
		{
			Quest = quest;
			Quest.CurrentPage = this;
			State = GQML.STATE_RUNNING;
			StartTrigger.Initiate ();
		}

		public virtual void End ()
		{
			State = GQML.STATE_SUCCEEDED;
			EndTrigger.Initiate ();
		}

		#endregion


		#region Old Stuff needs Rework

		[XmlAnyAttribute ()]
		public XmlAttribute[]
			help_attributes;

		public List<QuestAttribute> attributes;

		[XmlElement ("dialogitem")]
		public List<QuestContent>
			contents_dialogitems;



		[XmlElement ("expectedCode")]
		public List<QuestContent>
			contents_expectedcode;

		[XmlElement ("answer")]
		public List<QuestContent>
			contents_answers;

		[XmlElement ("question")]
		public QuestContent
			contents_question;

		[XmlElement ("answers")]
		public List<QuestContent>
			contents_answersgroup;

		[XmlElement ("stringmeta")]
		public List<QuestContent>
			contents_stringmeta;

		[XmlElement ("onEnd")]
		public QuestTrigger
			onEnd;

		[XmlElement ("onStart")]
		public QuestTrigger
			onStart;

		[XmlElement ("onTap")]
		public QuestTrigger
			onTap;

		[XmlElement ("onSuccess")]
		public QuestTrigger
			onSuccess;

		[XmlElement ("onFail")]
		public QuestTrigger
			onFailure;

		[XmlElement ("onRead")]
		public QuestTrigger
			onRead;

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

			questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();

//		if ( QuestManager.Instance.CurrentQuest != null ) {
			attributes = new List<QuestAttribute> ();

			if (help_attributes != null) {
				foreach (XmlAttribute xmla in help_attributes) {

					if (xmla.Name.Equals ("file") && xmla.Value.StartsWith (page_videoplay.YOUTUBE_URL_PREFIX)) {
						attributes.Add (new QuestAttribute (xmla.Name, xmla.Value));

						return;
					}
							
					if ((xmla.Value.StartsWith ("http://") || xmla.Value.StartsWith ("https://")) && !(type == "WebPage" && xmla.Name.ToLower () == "url")) {

						string[] splitted = xmla.Value.Split ('/');

						string filename = "files/" + splitted [splitted.Length - 1];

						if (!Application.isWebPlayer) {
				
							if (!redo) {
								questdb.downloadAsset (xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
							}
							if (splitted.Length > 3) {

								if (QuestManager.Instance.CurrentQuest != null && QuestManager.Instance.CurrentQuest.predeployed) {

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

			foreach (QuestContent qcdi in contents_dialogitems) {
				qcdi.deserializeAttributes (id, redo);
			}

			foreach (QuestContent qcdi in contents_answers) {
				qcdi.deserializeAttributes (id, redo);
			}

			if (contents_question != null) {
				contents_question.deserializeAttributes (id, redo);
			}
			foreach (QuestContent qcdi in contents_answersgroup) {
				qcdi.deserializeAttributes (id, redo);
			}

			foreach (QuestContent qcdi in contents_stringmeta) {
				qcdi.deserializeAttributes (id, redo);
			}

			foreach (QuestContent qcdi in contents_expectedcode) {
				qcdi.deserializeAttributes (id, redo);
			}

			if (onEnd != null) {
				foreach (QuestAction qa in onEnd.actions) {
					qa.deserializeAttributes (id, redo);
				}
			}
			if (onStart != null) {
				foreach (QuestAction qa in onStart.actions) {
					qa.deserializeAttributes (id, redo);
				}
			}
			if (onTap != null) {
				foreach (QuestAction qa in onTap.actions) {
					qa.deserializeAttributes (id, redo);
				}
			}
			if (onSuccess != null) {
				foreach (QuestAction qa in onSuccess.actions) {
					qa.deserializeAttributes (id, redo);
				}
			}
			if (onFailure != null) {
				foreach (QuestAction qa in onFailure.actions) {
					qa.deserializeAttributes (id, redo);
				}
			}
			if (onRead != null) {
				foreach (QuestAction qa in onRead.actions) {
					qa.deserializeAttributes (id, redo);
				}
			}
		}

		public bool hasActionInChildren (string type1)
		{
		
			if (onTap != null && onTap.hasActionInChildren (type1)) {
				return true;
			} else if (onEnd != null && onEnd.hasActionInChildren (type1)) {
				return true;
			} else if (onStart != null && onStart.hasActionInChildren (type1)) {
				return true;
			} else if (onSuccess != null && onSuccess.hasActionInChildren (type1)) {
				return true;
			} else if (onFailure != null && onFailure.hasActionInChildren (type1)) {
				return true;
			} else if (onRead != null && onRead.hasActionInChildren (type1)) {
				return true;
			}
		
			return false;
		}

		#endregion

	}
}
