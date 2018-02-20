using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;
using System;
using GQ.Client.Err;

namespace GQ.Client.UI
{

	/// <summary>
	/// Configures the header layout based on the seetings in the current apps config data. Attach this script to all header game objects.
	/// </summary>
	[RequireComponent (typeof(Image)), RequireComponent (typeof(LayoutElement))]
	public class ScreenLayoutConfig : LayoutConfig
	{

		public GameObject Header;
		public GameObject TopMargin;
		public GameObject ContentArea;
		public GameObject Divider;
		public GameObject BottomMargin;
		public GameObject Footer;

		public override void layout ()
		{
			// set background color:
			Image image = GetComponent<Image> ();
			if (image != null) {
				image.color = ConfigurationManager.Current.contentBackgroundColor;
			}
	
			setHeader ();
			setContentHeight ();
			setTopMargin ();
			setDivider ();
			setBottomMargin ();
			setFooter ();
		}

		protected virtual void setHeader ()
		{
			if (Header == null)
				return;

			// set background color:
			Image image = GetComponent<Image> ();
			if (image != null) {
				image.color = ConfigurationManager.Current.headerBgColor;
			}

			// set MiddleTopLogo:
			try {
				Transform middleTopLogo = transform.Find ("ButtonPanel/MiddleTopLogo");
				if (middleTopLogo != null) {
					Image mtlImage = middleTopLogo.GetComponent<Image> ();
					if (mtlImage != null) {
						mtlImage.sprite = Resources.Load<Sprite> ("TopLogo");
					}
				}
			} catch (Exception e) {
				Log.SignalErrorToDeveloper ("Could not set Middle Top Logo Image. Exception occurred: " + e.Message);
			}

			LayoutElement layElem = Header.GetComponent<LayoutElement> ();
			if (layElem == null)
				return;

			layElem.flexibleHeight = LayoutConfig.HeaderHeightUnits;
		}

		protected virtual void setContentHeight ()
		{
			if (ContentArea == null)
				return;

			LayoutElement layElem = ContentArea.GetComponent<LayoutElement> ();
			if (layElem == null)
				return;

			layElem.flexibleHeight = LayoutConfig.ContentHeightUnits;
			if (Footer == null || !Footer.gameObject.activeSelf) {
				layElem.flexibleHeight += LayoutConfig.FooterHeightUnits;	
			}
		}

		protected virtual void setTopMargin ()
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

		protected virtual void setBottomMargin ()
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

		protected virtual void setDivider ()
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

		protected virtual void setFooter ()
		{
			if (Footer == null)
				return;

			// set background color:
			Image image = Footer.GetComponent<Image> ();
			if (image != null) {
				image.color = ConfigurationManager.Current.footerBgColor;
			}

			LayoutElement layElem = Footer.GetComponent<LayoutElement> ();
			if (layElem == null)
				return;

			layElem.flexibleHeight = LayoutConfig.FooterHeightUnits;
		}
	}

}