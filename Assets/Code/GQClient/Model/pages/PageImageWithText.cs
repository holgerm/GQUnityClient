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

		public override string PageSceneName {
			get {
				return GQML.PAGE_TYPE_NPCTALK;
			}
		}

	}

}
