using GQ.Client.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace GQ.Client.UI
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
