using Code.GQClient.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.parts.header
{
    public class PageHeaderController : MonoBehaviour
    {

        public GameObject RightButton;

        // Use this for initialization
        void Start()
        {
            // show right button image and activate function if configured:
            RefreshActivation(Config.Current.menu2ShownInQuests);
        }

        void RefreshActivation(bool newState) {
            RightButton.GetComponent<Button>().enabled =  newState;
            RightButton.transform.Find("Image").GetComponent<Image>().enabled = newState;
        }

    }
}
