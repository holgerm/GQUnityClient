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
		public GameObject TopMargin;
		public GameObject ContentArea;
		public GameObject Divider;
		public GameObject BottomMargin;
		public FooterLayoutConfig footer;

		protected override void layout ()
		{
			// set background color:
			Image image = GetComponent<Image> ();
			if (image != null) {
				image.color = ConfigurationManager.Current.contentBackgroundColor;
			}
	
			setContentHeight ();
			setTopMargin ();
			setDividerHeight ();
			setBottomMargin ();
		}

		private void setContentHeight ()
		{
			if (ContentArea == null)
				return;

			LayoutElement layElem = ContentArea.GetComponent<LayoutElement> ();
			if (layElem == null)
				return;

			layElem.flexibleHeight = PageController.ContentHeightUnits;
		}

		private void setTopMargin ()
		{
			if (TopMargin == null)
				return;

			// set background color:
			Image image = TopMargin.GetComponent<Image> ();
			if (image != null) {
				image.color = ConfigurationManager.Current.contentBackgroundColor;
			}

			LayoutElement layElem = TopMargin.GetComponent<LayoutElement> ();
			if (layElem == null)
				return;
			layElem.flexibleHeight = PageController.ContentTopMarginUnits;
		}

		private void setBottomMargin ()
		{
			if (BottomMargin == null)
				return;

			// set background color:
			Image image = BottomMargin.GetComponent<Image> ();
			if (image != null) {
				image.color = ConfigurationManager.Current.contentBackgroundColor;
			}

			LayoutElement layElem = BottomMargin.GetComponent<LayoutElement> ();
			if (layElem == null)
				return;
			layElem.flexibleHeight = PageController.ContentBottomMarginUnits;
		}

		private void setDividerHeight ()
		{
			if (Divider == null)
				return;

			// set background color:
			Image image = Divider.GetComponent<Image> ();
			if (image != null) {
				image.color = ConfigurationManager.Current.contentBackgroundColor;
			}

			LayoutElement layElem = Divider.GetComponent<LayoutElement> ();
			if (layElem == null)
				return;

			layElem.flexibleHeight = PageController.ContentDividerUnits;
		}
	}

}