using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;
using System;

namespace GQ.Client.UI
{

	/// <summary>
	/// Configures the header layout based on the seetings in the current apps config data. Attach this script to all header game objects.
	/// </summary>
	[RequireComponent (typeof(Image)), RequireComponent (typeof(LayoutElement))]
	public class ContentLayoutConfig : LayoutConfig
	{

		public HeaderLayoutConfig header;
		public FooterLayoutConfig footer;

		protected override void layout ()
		{
			// set background color:
			Image image = GetComponent<Image> ();
			if (image != null) {
				image.color = ConfigurationManager.Current.contentBackgroundColor;
			}
	
			// set height as rest left over by header and footer:
			LayoutElement layElem = GetComponent<LayoutElement> ();
			if (layElem != null) {
				float headerPermill = 0f;
				if (header != null) {
					headerPermill = header.GetComponent<LayoutElement> ().flexibleHeight;
				} else {
					try {
						headerPermill = transform.parent.Find (LayoutConfig.HEADER).GetComponent<LayoutElement> ().flexibleHeight;
					} catch (Exception) {
					}
				}

				float footerPermill = 0f;
				if (footer != null) {
					footerPermill = footer.GetComponent<LayoutElement> ().flexibleHeight;
				} else {
					try {
						footerPermill = transform.parent.Find (LayoutConfig.FOOTER).GetComponent<LayoutElement> ().flexibleHeight;
					} catch (Exception) {
					}
				}

				layElem.flexibleHeight = 1000f - (headerPermill + footerPermill);
			}
		}
	}

}