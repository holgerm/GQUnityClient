using Code.GQClient.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.layout
{

    [RequireComponent(typeof(Image))]
    public class MainContent : LayoutConfig
    {

        public override void layout()
        {
            Image image = GetComponent<Image>();
            if (image == null)
                return;

            image.color = ConfigurationManager.Current.contentBackgroundColor;
        }
    }
}
