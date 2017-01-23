using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System;
using System.Linq;
using System.Text;
using GQ.Geo;
using GQ.Util;
using UnitySlippyMap;

[System.Serializable]
public class QuestTrigger {

	[XmlArray("rule"),XmlArrayItem("action")]
	public List<QuestAction>
		actions;

	public void Invoke () {

		if ( actions != null ) {
			foreach ( QuestAction qa in actions ) {

				qa.Invoke();

			}
		}

	}

	public bool hasActionInChildren (string type1) {

		bool b = false;
			
		if (actions != null) {
			foreach (QuestAction a in actions) {
				if (a.hasActionInChildren (type1)) {
					b = true;
				}
			}
		}
			
		return b;

	}

	public bool hasMissionAction () {
		
		bool b = false;
		foreach ( QuestAction a in actions ) {

			if ( a.hasMissionAction() ) {

				b = true;

			}

		}

		return b;

	}
	
}
