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
	/// Makes the layout for all screens, i.e. all pages plus all foyer views and all additional full screen views (imprint, author login etc.).
	/// </summary>
	[RequireComponent (typeof(Image)), RequireComponent (typeof(LayoutElement))]
	public class ScreenLayout : LayoutConfig
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

			float height = Units2Pixels (HeaderHeightUnits);
			SetLayoutElementHeight (layElem, height);
		}

		/// <summary>
		/// Sets the height of the content. If the footer is not shown its height will be added to content.
		/// </summary>
		protected virtual void setContentHeight ()
		{
			if (ContentArea == null)
				return;

			LayoutElement layElem = ContentArea.GetComponent<LayoutElement> ();
			if (layElem == null)
				return;

			float height = Units2Pixels (ContentHeightUnits);
			SetLayoutElementHeight (layElem, height);
			if (Footer == null || !Footer.gameObject.activeSelf) {
				SetLayoutElementHeight (layElem, layElem.minHeight + Units2Pixels (FooterHeightUnits));
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

			layElem.minHeight = LayoutConfig.Units2Pixels (LayoutConfig.FooterHeightUnits);
			layElem.preferredHeight = layElem.minHeight;
		}

		#region Static Helpers

        protected static void SetEntryLayout (float heightUnits, GameObject menuEntry, string gameObjectPath = null, float sizeScaleFactor = 1f, Color? fgColor = null)
		{
            Color fgCol = (Color) ((fgColor == null) ? ConfigurationManager.Current.mainFgColor : fgColor);

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
                        transf.GetComponent<Image>().color = fgCol;
                    }

					// for texts we adapt the font size to be at most two thirds of the container element height:
					Text text = transf.GetComponent<Text> ();
					if (text != null) {
						text.fontSize = (int)Math.Floor (layElem.minHeight * 0.66f * sizeScaleFactor); 
                        text.color = fgCol;
                    }

                    // for Buttons we set the fgColor:
                    Button button = transf.GetComponent<Button>();
                    if (button != null)
                    {
                        ColorBlock newColors = button.colors;
                        newColors.normalColor = fgCol;
                        button.colors = newColors;
                    }
                }
            } else {
				Log.SignalErrorToDeveloper ("In gameobject {0} path {1} did not lead to another gameobject.", menuEntry.gameObject, gameObjectPath);
			}
		}

		#endregion
	}

}