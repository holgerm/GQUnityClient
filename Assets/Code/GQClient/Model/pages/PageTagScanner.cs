using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
using System;
using GQ.Client.Err;

namespace GQ.Client.Model
{

	[XmlRoot (GQML.PAGE)]
	public class PageTagScanner : DecidablePage
	{
		
		#region State

		public bool ShowTagContent { get; set; }

		public string Prompt { get; set ; }

		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			ShowTagContent = GQML.GetRequiredBoolAttribute (GQML.PAGE_TYPE_QRTAGSCANNER_SHOWTAGCONTENT, reader);

			Prompt = GQML.GetStringAttribute (GQML.PAGE_TYPE_QRTAGSCANNER_PROMPT, reader);
		}

		protected override void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			XmlSerializer serializer; 

			switch (reader.LocalName) {
			case GQML.PAGE_TYPE_QRTAGSCANNER_EXPECTEDCODE:
				xmlRootAttr.ElementName = GQML.PAGE_TYPE_QRTAGSCANNER_EXPECTEDCODE;
				serializer = new XmlSerializer (typeof(ExpectedCode), xmlRootAttr);
				ExpectedCode a = (ExpectedCode)serializer.Deserialize (reader);
				ExpectedCodes.Add (a);
				break;
			default:
				base.ReadContent (reader, xmlRootAttr);
				break;
			}
		}

		#endregion


		#region Runtime API

		public override void Start ()
		{
			base.Start ();
		}

		#endregion

	}

	public class ExpectedCode : IXmlSerializable, IText
	{

		#region State

		public int Id {
			get;
			set;
		}

		public string Text {
			get;
			set;
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
			GQML.AssertReaderAtStart (reader, GQML.PAGE_TYPE_QRTAGSCANNER_EXPECTEDCODE);

			// Read Attributes:
			Id = GQML.GetIntAttribute (GQML.ID, reader);
			// Content: Read and implicitly proceed the reader so that this node is completely consumed:
			Text = reader.ReadInnerXml ();
		}

		#endregion

		#region Null

		public static NullExpectedCode Null = new NullExpectedCode ();

		public class NullExpectedCode : ExpectedCode
		{

			internal NullExpectedCode ()
			{

			}
		}

		#endregion

	}
}
