using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using System;
using GQ.Client.Err;

namespace GQ.Client.Model
{

	public class DialogItem : IXmlSerializable
	{

		#region Runtime API

		public int Id {
			get;
			protected set;
		}

		public bool IsBlocking {
			get;
			protected set;
		}

		public string Speaker {
			get;
			protected set;
		}

		public string AudioURL {
			get;
			protected set;
		}

		public string Text {
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
			IsBlocking = GQML.GetBoolAttribute (GQML.PAGE_NPCTALK_DIALOGITEM_BLOCKING, reader);
			Speaker = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_DIALOGITEM_SPEAKER, reader);
			AudioURL = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_DIALOGITEM_AUDIOURL, reader);

			// Content: Read and implicitly proceed the reader so that this node is completely consumed:
			Text = reader.ReadInnerXml ();
		}

		#endregion
	
	}
}
