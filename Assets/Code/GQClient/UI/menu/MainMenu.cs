using Code.GQClient.Util;
using UnityEngine;

namespace Code.GQClient.UI.menu
{
    public class MainMenu : MonoBehaviour
    {
        // Start is called before the first frame update
        public void MainMenuButtonPressed()
        {
            if (Base.Instance.partnersCanvas != null)
                Base.Instance.partnersCanvas.gameObject.SetActive(false);
            Base.Instance.imprintCanvas.gameObject.SetActive(false);
            Base.Instance.privacyCanvas.gameObject.SetActive(false);
            Base.Instance.feedbackCanvas.gameObject.SetActive(false);
            Base.Instance.authorCanvas.gameObject.SetActive(false);

            Base.Instance.canvas4TopLeftMenu.gameObject.SetActive(
                !Base.Instance.canvas4TopLeftMenu.gameObject.activeSelf);

            if (Base.Instance.canvas4TopLeftMenu.gameObject.activeSelf)
                Base.Instance.canvas4TopRightMenu.gameObject.SetActive(false);
        }
    }
}