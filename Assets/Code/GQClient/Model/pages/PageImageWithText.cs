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

		#region Runtime API

		public override void Start (bool canReturnToPrevious = false)
		{
			base.Start (canReturnToPrevious);
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
			QuestManager.CurrentlyParsingQuest.AddMedia (ImageUrl, "ImageWithText." + GQML.PAGE_IMAGEWITHTEXT_IMAGEURL);

			Text = GQML.GetStringAttribute (GQML.PAGE_IMAGEWITHTEXT_TEXT, reader);

			TextSize = GQML.GetIntAttribute (GQML.PAGE_IMAGEWITHTEXT_TEXTSIZE, reader);
		}

		#endregion

	}

}
