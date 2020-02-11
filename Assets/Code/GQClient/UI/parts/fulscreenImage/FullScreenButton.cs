using GQ.Client.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace GQ.Client.UI
{

    [RequireComponent(typeof(Button)), RequireComponent(typeof(RectTransform))]
    public class FullScreenButton : MonoBehaviour
    {
        public RawImage image;
        public FullScreenImageScrollRect scrollRect;

        // Use this for initialization
        void Start()
        {
            RectTransform buttonRT = GetComponent<RectTransform>();
            float size = LayoutConfig.Units2Pixels(
                LayoutConfig.calculateRestrictedHeight(
                    ConfigurationManager.Current.overlayButtonSizeUnits,
                    ConfigurationManager.Current.overlayButtonSizeMinMM,
                    ConfigurationManager.Current.overlayButtonSizeMaxMM
                )
            );
            buttonRT.sizeDelta = new Vector2(size, size);

            // register opening function of scrollrect to show the fullscreen image:
            Button bt = gameObject.GetComponent<Button>();
            bt.onClick.AddListener(delegate { scrollRect.Open(image); });
        }

    }
}