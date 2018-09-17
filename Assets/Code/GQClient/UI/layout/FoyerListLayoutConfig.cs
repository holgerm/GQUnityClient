using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Err;
using GQ.Client.Conf;

namespace GQ.Client.UI
{

	/// <summary>
	/// Add this script to the foyer list screen.
	/// </summary>
	public class FoyerListLayoutConfig : ScreenLayout
	{

		public Transform questInfoList;
        public Image listBackgroundImage;

		public override void layout ()
		{
			base.layout ();
			Debug.Log ("FoyerListLayoutConfig.layout()");

            // set list divining lines color:
			if (questInfoList != null) {
				Image im = questInfoList.GetComponent<Image> ();
				if (im != null) {
                    im.color = ConfigurationManager.Current.listLineColor;
				}
			}

            // set background above and below the list:
            listBackgroundImage.color = ConfigurationManager.Current.listBgColor;

            // set spacing, i.e. separation lines width between list elements:
            VerticalLayoutGroup vlg = questInfoList.gameObject.GetComponent<VerticalLayoutGroup>();
            switch (ConfigurationManager.Current.listEntryDividingMode) {
                case ListEntryDividingMode.SeparationLines:
                    // set lines on top, between and bottom of the list:
                    vlg.padding.top = ConfigurationManager.Current.listStartLineWidth;
                    vlg.spacing = ConfigurationManager.Current.dividingLineWidth;
                    vlg.padding.bottom = ConfigurationManager.Current.listEndLineWidth;
                    break;
                case ListEntryDividingMode.AlternatingColors:
                    vlg.padding.top = 0;
                    vlg.spacing = 0;
                    vlg.padding.bottom = 0;
                    break;
            } 

		}


		#region Static Layout Helpers

		static public void SetListEntryLayout (GameObject listEntry, string gameObjectPath = null, float sizeScaleFactor = 1f, Color? fgColor = null)
		{
            ScreenLayout.SetEntryLayout (ListEntryHeightUnits, listEntry, gameObjectPath, sizeScaleFactor: sizeScaleFactor, fgColor: fgColor);
		}

		static public float ListEntryHeightUnits {
			get {
				return 
					calculateRestrictedHeight (
					ConfigurationManager.Current.listEntryHeightUnits,
					ConfigurationManager.Current.listEntryHeightMinMM,
					ConfigurationManager.Current.listEntryHeightMaxMM
				);
			}
		}

		#endregion
	}
}
