using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;

namespace GQ.Client.UI {

	public class MenuLayoutConfig : LayoutConfig {

		public GameObject MenuContent;

		protected override void layout() {
			MenuContent.GetComponent<Image> ().color = ConfigurationManager.Current.menuBGColor;
		}

	}
}
