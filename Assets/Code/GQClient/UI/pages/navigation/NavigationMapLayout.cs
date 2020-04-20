#define DEBUG_LOG

using Code.GQClient.UI.layout;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.pages.navigation
{

    public class NavigationMapLayout : PageLayout
    {

        public GameObject MapButtonPanel;

        public override void layout()
        {
            base.layout();

            // TODO set background color for button panel:

            // set button background height:
            for (var i = 0; i < MapButtonPanel.transform.childCount; i++)
            {
                var perhapsAButton = MapButtonPanel.transform.GetChild(i).gameObject;
                var button = perhapsAButton.GetComponent<Button>();
                if (button != null)
                {
                    LayoutElement layElem = perhapsAButton.GetComponent<LayoutElement>();
                    if (layElem != null)
                    {
                        var height = Units2Pixels(FoyerMapScreenLayout.MapButtonHeightUnits);
                        SetLayoutElementHeight(layElem, height);
                        SetLayoutElementWidth(layElem, height);
                    }
                }
            }
        }

    }
}
