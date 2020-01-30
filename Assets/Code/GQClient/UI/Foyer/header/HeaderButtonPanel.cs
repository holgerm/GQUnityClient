using GQ.Client.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace GQ.Client.UI
{

    public class HeaderButtonPanel : MonoBehaviour
    {
        public Button LeftButton;
        public Image LeftImage;
        public Button RightButton;
        public Image RightImage;

        public void Awake()
        {
            LeftImage.color = ConfigurationManager.Current.headerButtonFgColor;
            RightImage.color = ConfigurationManager.Current.headerButtonFgColor;
        }

        // Start is called before the first frame update
        public void SetInteractable(bool interactable)
        {
            LeftButton.interactable = interactable;
            Image i = LeftButton.transform.Find("Image").GetComponent<Image>();
            i = LeftImage;
            Color c = i.color;
            i.color = new Color(c.r, c.g, c.b, interactable ? 1f : ConfigurationManager.Current.disabledAlpha);

            RightButton.interactable = interactable;
            i = RightButton.transform.Find("Image").GetComponent<Image>();
            i = RightImage;
            c = i.color;
            i.color = new Color(c.r, c.g, c.b, interactable ? 1f : ConfigurationManager.Current.disabledAlpha);
        }
    }
}