using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;

namespace GQ.Client.Model
{

	[XmlRoot (GQML.PAGE)]
	public class PageImageCapture : Page {

		#region State

		public string ButtonText { get; set ; }

		public string File { get; set ; }

		public string Task { get; set; }

		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			ButtonText = GQML.GetStringAttribute (GQML.PAGE_IMAGECAPTURE_BUTTONTEXT, reader);

			File = GQML.GetStringAttribute (GQML.PAGE_IMAGECAPTURE_FILE, reader);

			Task = GQML.GetStringAttribute (GQML.PAGE_IMAGECAPTURE_TASK, reader);
		}

		#endregion

	}

}
