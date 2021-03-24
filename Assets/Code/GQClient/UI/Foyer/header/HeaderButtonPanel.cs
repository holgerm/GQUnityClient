using Code.GQClient.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.Foyer.header
{

    public class HeaderButtonPanel : MonoBehaviour
    {
        public Button LeftButton;
        public Image LeftImage;
        public Button RightButton;
        public Image RightImage;

        public void Awake()
        {
            LeftImage.color = Config.Current.headerButtonFgColor;
            RightImage.color = Config.Current.headerButtonFgColor;
        }

        public void SetInteractable(bool interactable)
        {
            LeftButton.interactable = interactable;
            Image i = LeftButton.transform.Find("Image").GetComponent<Image>();
            i = LeftImage;
            Color c = i.color;
            i.color = new Color(c.r, c.g, c.b, interactable ? 1f : Config.Current.disabledAlpha);

            RightButton.interactable = interactable;
            i = RightButton.transform.Find("Image").GetComponent<Image>();
            i = RightImage;
            c = i.color;
            i.color = new Color(c.r, c.g, c.b, interactable ? 1f : Config.Current.disabledAlpha);
        }
    }
}