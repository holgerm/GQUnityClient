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
	public class PageStartAndExitScreen : Page
	{

		#region State

		// TODO: Add endbuttontext to this page type in Editor
		public string EndButtonText { get; set ; }

		public string ImageUrl { get; set; }

		public bool Loop { get; set; }

		public int Duration { get; set; }

		public int Fps { get; set; }

		#endregion


		#region Runtime API

		public override void Start (bool canReturnToPrevious = false)
		{
			base.Start (canReturnToPrevious);
		}

		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			EndButtonText = ">";

			ImageUrl = GQML.GetStringAttribute (GQML.PAGE_STARTANDEXITSCREEN_IMAGEURL, reader);
			if (ImageUrl != "")
				QuestManager.CurrentlyParsingQuest.AddMedia (ImageUrl, "StartAndExitScreen." + GQML.PAGE_STARTANDEXITSCREEN_IMAGEURL);

			Loop = GQML.GetOptionalBoolAttribute (GQML.PAGE_STARTANDEXITSCREEN_LOOP, reader);

			Duration = GQML.GetIntAttribute (GQML.PAGE_STARTANDEXITSCREEN_DURATION, reader);

			Fps = GQML.GetIntAttribute (GQML.PAGE_STARTANDEXITSCREEN_FPS, reader);
		}

		#endregion

	}

}
