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
			setMainBackgroundColor ();

			setHeader ();
			setContentHeight ();
			setTopMargin ();
			setDivider ();
			setBottomMargin ();
			setFooter ();
		}

		protected virtual void setMainBackgroundColor ()
		{
			// set background color:
			Image image = GetComponent<Image> ();
			if (image != null) {
				image.color = ConfigurationManager.Current.contentBackgroundColor;
			}
		}

		protected virtual void setHeader ()
		{
			if (Header == null)
				return;

			// set background color:
			Image image = Header.GetComponent<Image> ();
			if (image != null) {
				image.color = ConfigurationManager.Current.headerBgColor;
			}

			// set MiddleTopLogo:
			try {
				Transform middleTopLogo = Header.transform.Find ("ButtonPanel/MiddleTopLogo");
				if (middleTopLogo != null) {
					Image mtlImage = middleTopLogo.GetComponent<Image> ();
					if (mtlImage != null) {
						mtlImage.sprite = Resources.Load<Sprite> (ConfigurationManager.Current.topLogo.path);
					}
				}
			} catch (Exception e) {
				Log.SignalErrorToDeveloper ("Could not set Middle Top Logo Image. Exception occurred: " + e.Message);
			}

			LayoutElement layElem = Header.GetComponent<LayoutElement> ();
			if (layElem == null)
				return;

			layElem.flexibleHeight = Units2Pixels (LayoutConfig.HeaderHeightUnits);
		}

		protected virtual void setContentHeight ()
		{
			if (ContentArea == null)
				return;

			LayoutElement layElem = ContentArea.GetComponent<LayoutElement> ();
			if (layElem == null)
				return;

			layElem.flexibleHeight = Units2Pixels (LayoutConfig.ContentHeightUnits);
			if (Footer == null || !Footer.gameObject.activeSelf) {
				layElem.flexibleHeight += Units2Pixels (LayoutConfig.FooterHeightUnits);	
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
			TopMargin.SetActive (PageController.ContentTopMarginUnits > 0f);
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
			BottomMargin.SetActive (PageController.ContentBottomMarginUnits > 0f);
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
			Divider.SetActive (PageController.ContentDividerUnits > 0f);
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

			layElem.flexibleHeight = LayoutConfig.Units2Pixels (LayoutConfig.FooterHeightUnits);
		}

		protected static void SetEntryHeight (float heightUnits, GameObject menuEntry, string gameObjectPath = null, float sizeScaleFactor = 1f)
		{
			// set layout height:
			Transform transf = (gameObjectPath == null ? menuEntry.transform : menuEntry.transform.Find (gameObjectPath));
			if (transf != null) {
				LayoutElement layElem = transf.GetComponent<LayoutElement> ();
				if (layElem != null) {
					layElem.minHeight = Units2Pixels (heightUnits) * sizeScaleFactor;
					layElem.preferredHeight = layElem.minHeight;

					// for images we set the width too:
					if (transf.GetComponent<Image> () != null) {
						layElem.minWidth = layElem.minHeight;
						layElem.preferredWidth = layElem.minHeight;
					}

					// for texts we adapt the font size to be at most two thirds of the container element height:
					Text text = transf.GetComponent<Text> ();
					if (text != null) {
						text.fontSize = (int)Math.Floor (layElem.minHeight * 0.66f * sizeScaleFactor); 
					}
				}
			} else {
				Log.SignalErrorToDeveloper ("In gameobject {0} path {1} did not lead to another gameobject.", menuEntry.gameObject, gameObjectPath);
			}
		}

	}

}