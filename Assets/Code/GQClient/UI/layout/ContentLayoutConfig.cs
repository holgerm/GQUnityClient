using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;
using System;

/// <summary>
/// Configures the header layout based on the seetings in the current apps config data. Attach this script to all header game objects.
/// </summary>
[RequireComponent (typeof(Image)), RequireComponent (typeof(LayoutElement))]
public class ContentLayoutConfig : LayoutConfig {

	protected override void layout() {
		// set background color:
//		Image image = GetComponent<Image> ();
//		if (image != null) {
//			image.color = ConfigurationManager.Current.footerBackgroundColor;
//		}
//
		// set height as rest left over by header and footer:
		LayoutElement layElem = GetComponent<LayoutElement>();
		if (layElem != null) {
			float headerPermill = 0f;
			try {
				headerPermill = transform.parent.Find (LayoutConfig.HEADER).GetComponent<LayoutElement>().flexibleHeight;
			}
			catch (Exception) {
				headerPermill = 0f;
			}

			float footerPermill = 0f;
			try {
				footerPermill = transform.parent.Find (LayoutConfig.FOOTER).GetComponent<LayoutElement>().flexibleHeight;
			}
			catch (Exception) {
				footerPermill = 0f;
			}

			layElem.flexibleHeight = 1000f - (headerPermill + footerPermill);
		}
	}
}
