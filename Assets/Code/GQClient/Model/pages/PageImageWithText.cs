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
	public class PageImageWithText : Page
	{

		#region State

		public string EndButtonText { get; set ; }

		public string ImageUrl { get; set; }

		public string Text { get; set; }

		public int TextSize { get; set; }

		#endregion


		#region Runtime API

		public override void Start ()
		{
			base.Start ();
		}

		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			EndButtonText = GQML.GetStringAttribute (GQML.PAGE_IMAGEWITHTEXT_ENDBUTTONTEXT, reader);

			ImageUrl = GQML.GetStringAttribute (GQML.PAGE_IMAGEWITHTEXT_IMAGEURL, reader);
			if (ImageUrl != "")
				QuestManager.CurrentlyParsingQuest.AddMedia (ImageUrl);

			Text = GQML.GetStringAttribute (GQML.PAGE_IMAGEWITHTEXT_TEXT, reader);

			TextSize = GQML.GetIntAttribute (GQML.PAGE_IMAGEWITHTEXT_TEXTSIZE, reader);
		}

		//		protected override void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		//		{
		//			switch (reader.LocalName) {
		//			case GQML.PAGE_NPCTALK_DIALOGITEM:
		//				xmlRootAttr.ElementName = GQML.PAGE_NPCTALK_DIALOGITEM;
		//				XmlSerializer serializer = new XmlSerializer (typeof(DialogItem), xmlRootAttr);
		//				DialogItem d = (DialogItem)serializer.Deserialize (reader);
		//				dialogItems.Add (d);
		//				break;
		//			default:
		//				base.ReadContent (reader, xmlRootAttr);
		//				break;
		//			}
		//		}

		#endregion

	}

}
