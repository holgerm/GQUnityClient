#define DEBUG_LOG

using UnityEngine;
using UnityEngine.UI;

namespace GQ.Client.UI
{

    public class NavigationMapLayout : PageLayout
    {

        public GameObject MapButtonPanel;

        public override void layout()
        {
#if DEBUG_LOG
            Debug.Log(string.Format("NavigationMapLayout.layout() started. MapButtonPanel active?: {0}. Frame# {1}",
                MapButtonPanel.gameObject.activeInHierarchy,
                Time.frameCount));
#endif

            base.layout();

            // TODO set background color for button panel:

            // set button background height:
            for (int i = 0; i < MapButtonPanel.transform.childCount; i++)
            {
                GameObject perhapsAButton = MapButtonPanel.transform.GetChild(i).gameObject;
                Button button = perhapsAButton.GetComponent<Button>();
                if (button != null)
                {
                    LayoutElement layElem = perhapsAButton.GetComponent<LayoutElement>();
                    if (layElem != null)
                    {
                        float height = Units2Pixels(FoyerMapScreenLayout.MapButtonHeightUnits);
                        SetLayoutElementHeight(layElem, height);
                        SetLayoutElementWidth(layElem, height);
                    }
                }
            }
        }

    }
}
