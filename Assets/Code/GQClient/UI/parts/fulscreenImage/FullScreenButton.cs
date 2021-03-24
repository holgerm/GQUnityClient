using Code.GQClient.Conf;
using Code.GQClient.UI.layout;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.parts.fulscreenImage
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
                    Config.Current.overlayButtonSizeUnits,
                    Config.Current.overlayButtonSizeMinMM,
                    Config.Current.overlayButtonSizeMaxMM
                )
            );
            buttonRT.sizeDelta = new Vector2(size, size);

            // register opening function of scrollrect to show the fullscreen image:
            Button bt = gameObject.GetComponent<Button>();
            bt.onClick.AddListener(delegate { scrollRect.Open(image); });
        }

    }
}