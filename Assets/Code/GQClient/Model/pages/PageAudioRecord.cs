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
	public class PageAudioRecord : Page
	{
		
		#region State

		public string FileName { get; set; }
		public string PromptText { get; set; }

		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			FileName = GQML.GetStringAttribute (GQML.PAGE_AUDIORECORD_FILE, reader);
			PromptText = GQML.GetStringAttribute (GQML.PAGE_AUDIORECORD_TASK, reader);
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
