using GQ.Client.Util;
using UnityEngine;

namespace GQ.Client.UI
{

    public class InfoMenu : MonoBehaviour
    {
        // Start is called before the first frame update
        public void InfoMenuButtonPressed()
        {
            if (Base.Instance.partnersCanvas.gameObject.activeSelf)
            {
                Base.Instance.partnersCanvas.gameObject.SetActive(false);
                return;
            }

            if (Base.Instance.imprintCanvas.gameObject.activeSelf)
            {
                Base.Instance.imprintCanvas.gameObject.SetActive(false);
                return;
            }

            if (Base.Instance.feedbackCanvas.gameObject.activeSelf)
            {
                Base.Instance.feedbackCanvas.gameObject.SetActive(false);
                return;
            }

            if (Base.Instance.privacyCanvas.gameObject.activeSelf)
            {
                Base.Instance.privacyCanvas.gameObject.SetActive(false);
                return;
            }

            if (Base.Instance.authorCanvas.gameObject.activeSelf)
            {
                Base.Instance.authorCanvas.gameObject.SetActive(false);
                return;
            }

            Base.Instance.canvas4TopRightMenu.gameObject.SetActive(
                !Base.Instance.canvas4TopRightMenu.gameObject.activeSelf);

            if (Base.Instance.canvas4TopRightMenu.gameObject.activeSelf)
                Base.Instance.canvas4TopLeftMenu.gameObject.SetActive(false);
        }
    }
}
