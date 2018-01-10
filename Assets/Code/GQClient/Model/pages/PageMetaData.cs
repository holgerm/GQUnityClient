using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;
using GQ.Client.Err;
using System;

namespace GQ.Client.Model
{

	[XmlRoot (GQML.PAGE)]
	public class PageMetaData : Page
	{

		#region Runtime API

		public override void Start ()
		{
			base.Start ();
		}

		#endregion


		#region XML Serialization

		protected override void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			switch (reader.LocalName) {
			case GQML.PAGE_METADATA_STRINGMETA:
				xmlRootAttr.ElementName = GQML.PAGE_METADATA_STRINGMETA;
				XmlSerializer serializer = new XmlSerializer (typeof(StringMetaData), xmlRootAttr);
				StringMetaData smde = (StringMetaData)serializer.Deserialize (reader);
				if (smde.Key == null)
					break;
				if (QuestManager.CurrentlyParsingQuest.metadata.ContainsKey (smde.Key)) {
					QuestManager.CurrentlyParsingQuest.metadata [smde.Key] = smde.Value;
				}
				else {
					QuestManager.CurrentlyParsingQuest.metadata.Add (smde.Key, smde.Value);
				}
				break;
			default:
				base.ReadContent (reader, xmlRootAttr);
				break;
			}
		}

		#endregion

	}

	public class StringMetaData : IXmlSerializable
	{

		#region Runtime API

		public int Id {
			get;
			protected set;
		}

		public string Key {
			get;
			protected set;
		}

		public string Value {
			get;
			protected set;
		}

		#endregion


		#region XML Serialization

		public System.Xml.Schema.XmlSchema GetSchema ()
		{
			return null;
		}

		public void WriteXml (System.Xml.XmlWriter writer)
		{
			Debug.LogWarning ("WriteXML not implemented for " + GetType ().Name);
		}

		public void ReadXml (XmlReader reader)
		{
			GQML.AssertReaderAtStart (reader, GQML.PAGE_NPCTALK_DIALOGITEM);

			// Read Attributes:
			Id = GQML.GetIntAttribute (GQML.PAGE_ID, reader);
			Key = GQML.GetStringAttribute (GQML.PAGE_METADATA_STRINGMETA_KEY, reader);
			Value = GQML.GetStringAttribute (GQML.PAGE_METADATA_STRINGMETA_VALUE, reader);

			// consume the whole stringmeta tag:
			reader.ReadInnerXml ();
		}

		#endregion

		#region Null

		public static NullStringMetaData Null = new NullStringMetaData ();

		public class NullStringMetaData : StringMetaData
		{

			internal NullStringMetaData ()
			{

			}
		}

		#endregion

	}
}
