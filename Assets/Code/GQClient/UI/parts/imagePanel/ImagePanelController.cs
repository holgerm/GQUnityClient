using Code.GQClient.Conf;
using Code.GQClient.UI.pages;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.parts.imagePanel
{
    public class ImagePanelController : MonoBehaviour {

        public RawImage image;
        public GameObject imagePanel;

        float fitInAndShowImage(Texture2D texture)
        {
            AspectRatioFitter fitter = image.GetComponent<AspectRatioFitter>();
            float imageRatio = (float)texture.width / (float)texture.height;
            float imageAreaHeight = PageController.ContentWidthUnits / imageRatio;  // if image fits, so we use its height (adjusted to the area):

            if (imageRatio < PageController.ImageRatioMinimum)
            {
                // image too high to fit:
                imageAreaHeight = Config.Current.imageAreaHeightMaxUnits;
            }
            if (PageController.ImageRatioMaximum < imageRatio)
            {
                // image too wide to fit:
                imageAreaHeight = Config.Current.imageAreaHeightMinUnits;
            }

            //imagePanel.GetComponent<LayoutElement> ().flexibleHeight = LayoutConfig.Units2Pixels (imageAreaHeight);
            //contentPanel.GetComponent<LayoutElement> ().flexibleHeight = CalculateMainAreaHeight (imageAreaHeight);

            fitter.aspectRatio = imageRatio; // i.e. the adjusted image area aspect ratio
            fitter.aspectMode =
                Config.Current.fitExceedingImagesIntoArea
                    ? AspectRatioFitter.AspectMode.FitInParent
                    : AspectRatioFitter.AspectMode.EnvelopeParent;

            image.texture = texture;
            imagePanel.SetActive(true);

            return imageAreaHeight;

        }
    }
}
