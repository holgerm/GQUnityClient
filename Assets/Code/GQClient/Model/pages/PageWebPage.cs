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
	public class PageWebPage : Page
	{
		
		#region State

		public string File { get; set; }
		public string URL { get; set; }

		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			File = GQML.GetStringAttribute (GQML.PAGE_WEBPAGE_FILE, reader);
			URL = GQML.GetStringAttribute (GQML.PAGE_WEBPAGE_URL, reader);
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
