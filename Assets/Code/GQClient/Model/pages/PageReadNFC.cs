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
	public class PageReadNFC : Page
	{
		
		#region State

		public string ImageUrl { get; set; }
		public string SaveToVar { get; set; }
		public string PromptText { get; set; }

		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			ImageUrl = GQML.GetStringAttribute (GQML.PAGE_READNFC_IMAGEURL, reader);
			QuestManager.CurrentlyParsingQuest.AddMedia (ImageUrl);
			SaveToVar = GQML.GetStringAttribute (GQML.PAGE_READNFC_SAVE2VAR, reader);
			PromptText = GQML.GetStringAttribute (GQML.PAGE_READNFC_TEXT, reader);
		}

		protected Trigger NFCReadTrigger = Trigger.Null;

		protected override void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			XmlSerializer serializer; 

			switch (reader.LocalName) {
			case GQML.ON_READ:
				xmlRootAttr.ElementName = GQML.ON_READ;
				serializer = new XmlSerializer (typeof(Trigger), xmlRootAttr);
				NFCReadTrigger = (Trigger)serializer.Deserialize (reader);
				NFCReadTrigger.Parent = this;
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

}
