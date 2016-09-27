using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GQ.Client.Conf {

	[System.Serializable]
	public class MarkerCategorySprite {

		public string category;
		public string anzeigename_de;
		public Sprite sprite;
		public bool showInList = true;
		/// <summary>
		/// Flag that determines whether this category's markers will be shown on the map. 
		/// This is currently also the only state used for category markers and also reflects the state of the buttons etc.
		/// </summary>
		public bool showOnMap = true;

	}

}