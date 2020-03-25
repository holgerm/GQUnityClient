using Code.GQClient.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.Product
{

    public class PartnerCanvasLayout : MonoBehaviour
    {
        public Image scrollViewImage;
        private void Reset()
        {
            Layout();
        }

        // Start is called before the first frame update
        private void Start()
        {
            Layout();
        }

        private void Layout()
        {
            scrollViewImage.color = ConfigurationManager.Current.mainBgColor;
        }
    }
}
