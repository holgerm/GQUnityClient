using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.UI.layout;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.parts.header
{
    public class HeaderLayout : LayoutConfig
    {
        public GameObject Header;
        public GameObject MiddleButton;

        public override void layout()
        {
            setHeader();
        }

        protected virtual void setHeader()
        {
            if (Header == null)
            {
                Log.SignalErrorToDeveloper("Header is null.");
                return;
            }

            // set background color:
            var image = Header.GetComponent<Image>();
            if (image != null)
            {
                image.color = ConfigurationManager.Current.headerBgColor;
            }

            // setMiddleButton();

            var layElem = Header.GetComponent<LayoutElement>();
            if (layElem == null)
            {
                Log.SignalErrorToDeveloper("LayoutElement for Header is null.");
                return;
            }

            var height = Units2Pixels(HeaderHeightUnits);
            SetLayoutElementHeight(layElem, height);
        }

        protected virtual void setMiddleButton()
        {
            if (MiddleButton == null)
                return;

            // show top logo and load image:
            var middleTopLogo = MiddleButton.transform.Find("TopLogo");

            if (middleTopLogo != null)
            {
                middleTopLogo.gameObject.SetActive(true);
                Image mtlImage = middleTopLogo.GetComponent<Image>();
                if (mtlImage != null)
                {
                    Config cf = ConfigurationManager.Current;
                    mtlImage.sprite = cf.topLogo.GetSprite();
                }
            }
        }
    }
}