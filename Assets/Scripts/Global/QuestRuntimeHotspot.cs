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
using GQ.Client.Conf;

[System.Serializable]
public class QuestRuntimeHotspot {
	
	public QuestHotspot hotspot;
	public bool active;
	public bool visible;
	public float lon;
	public float lat;
	public MeshRenderer renderer;
	public bool entered = false;
	public Quest startquest;
	public string category;

	public QuestRuntimeHotspot (QuestHotspot hp, bool a, bool v, string ll) {
		
		hotspot = hp;
		active = a;
		visible = v;

		if ( ll.Contains(",") ) {
			
			char[] splitter = ",".ToCharArray();
			
			string[] splitted = ll.Split(splitter);

			foreach ( string x in splitted ) {

				if ( lon == null || lon == 0.0f ) {

					lon = float.Parse(x);

				}
				else {

					lat = float.Parse(x);

				}

			}

		}
		else {

			lon = 0.0f;
			lat = 0.0f;

		}
		
	}

	public Sprite getMarkerImage () {
		Sprite s = Configuration.instance.defaultmarker;
		foreach ( MarkerCategorySprite mcs in Configuration.instance.categoryMarker ) {

			if ( mcs.category == category ) {
				s = mcs.sprite;
			}

		}

		return s;
	}
	
}
