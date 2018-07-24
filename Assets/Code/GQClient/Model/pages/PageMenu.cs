using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
using GQ.Client.Err;
using GQ.Client.Util;

namespace GQ.Client.Model
{

	[XmlRoot (GQML.PAGE)]
	public class PageMenu : Page
	{

		#region State

		public string Question { get; set ; }

		public bool Shuffle { get; set; }

		public List<MenuChoice> Choices = new List<MenuChoice> ();

		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			Question = GQML.GetStringAttribute (GQML.PAGE_QUESTION_QUESTION, reader);

			Shuffle = GQML.GetOptionalBoolAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_SHUFFLE, reader);
		}

		protected override void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			XmlSerializer serializer; 

			switch (reader.LocalName) {
			case GQML.PAGE_QUESTION_ANSWER:
				xmlRootAttr.ElementName = GQML.PAGE_QUESTION_ANSWER;
				serializer = new XmlSerializer (typeof(MenuChoice), xmlRootAttr);
				MenuChoice a = (MenuChoice)serializer.Deserialize (reader);
				Choices.Add (a);
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

	public class MenuChoice : IXmlSerializable
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
			GQML.AssertReaderAtStart (reader, GQML.PAGE_QUESTION_ANSWER);

			// Content: Read and implicitly proceed the reader so that this node is completely consumed:
			Text = reader.ReadInnerXml ().MakeReplacements();
		}

		#endregion

		#region Null

		public static NullChoice Null = new NullChoice ();

		public class NullChoice : MenuChoice
		{

			internal NullChoice ()
			{

			}
		}

		#endregion

	}
}
