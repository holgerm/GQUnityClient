using Code.GQClient.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.layout
{

    /// <summary>
    /// Add this script to the foyer list screen.
    /// </summary>
    public class FoyerTopicTreeLayoutConfig : ScreenLayout
    {

        public Transform questInfoList;
        public Image listBackgroundImage;

        // public override void layout()
        // {
        //     if (ConfigurationManager.Current == null) 
        //         return;
        //
        //     base.layout();
        //     var im = questInfoList.GetComponent<Image>();
        //
        //     // set background above and below the list:
        //     if (listBackgroundImage == null)
        //     {
        //         Debug.LogWarning("listBackgroundImage is null");
        //     }
        //     else
        //     {
        //         listBackgroundImage.color = ConfigurationManager.Current.listBgColor;
        //     }
        //
        //     // set spacing, i.e. separation lines width between list elements:
        //     VerticalLayoutGroup vlg = questInfoList.gameObject.GetComponent<VerticalLayoutGroup>();
        //     switch (ConfigurationManager.Current.listEntryDividingMode)
        //     {
        //         case ListEntryDividingMode.SeparationLines:
        //             // set lines on top, between and bottom of the list:
        //             vlg.padding.top = ConfigurationManager.Current.listStartLineWidth;
        //             vlg.spacing = ConfigurationManager.Current.dividingLineWidth;
        //             vlg.padding.bottom = ConfigurationManager.Current.listEndLineWidth;
        //             // set list divining lines color:
        //             if (im != null)
        //             {
        //                 im.enabled = true;
        //                 im.color = ConfigurationManager.Current.listLineColor;
        //             }
        //             questInfoList.GetComponent<Image>().enabled = true;
        //             break;
        //         case ListEntryDividingMode.AlternatingColors:
        //             vlg.padding.top = 0;
        //             vlg.spacing = 0;
        //             vlg.padding.bottom = 0;
        //             // no image behind the list elements since we do not need lines between the entries:
        //             if (im != null)
        //                 questInfoList.GetComponent<Image>().enabled = false;
        //             break;
        //     }
        //
        // }


        #region Static Layout Helpers

        static public void SetQuestInfoEntryLayout(GameObject listEntry, string gameObjectPath = null, float sizeScaleFactor = 1f, Color? fgColor = null)
        {
            ScreenLayout.SetQuestInfoEntryLayout(ListEntryHeightUnits, listEntry, gameObjectPath, sizeScaleFactor: sizeScaleFactor, fgColor: fgColor);
        }

        static public float ListEntryHeightUnits
        {
            get
            {
                return
                    calculateRestrictedHeight(
                    ConfigurationManager.Current.listEntryHeightUnits,
                    ConfigurationManager.Current.listEntryHeightMinMM,
                    ConfigurationManager.Current.listEntryHeightMaxMM
                );
            }
        }

        #endregion
    }
}
