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
	public class PageImageWithText : PageNPCTalk
	{

		#region State

		private string text;

		public string Text { 
			get {
				return text;
			} 
			set {
				text = value;
				// adapt to NPCTalk: set the text as dialog item:
				DialogItem d = new DialogItem ();
				d.Id = -1; // not applicable
				d.IsBlocking = false;
				d.AudioURL = null;
				d.Speaker = null;
				d.Text = text;
				dialogItems.Clear (); // we have only this dialog item
				dialogItems.Add (d);
			} 
		}

		#endregion


		#region Runtime API

		public override void Start ()
		{
			base.Start ();
		}

		public override string PageSceneName {
			get {
				return GQML.PAGE_TYPE_NPCTALK;
			}
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

		#endregion

	}

}
