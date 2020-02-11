using Code.GQClient.Util;
using UnityEngine;

namespace Code.GQClient.UI.menu
{

    public class MenuAccessPoint : MonoBehaviour
	{
        public void CloseLeftMenu()
        {
            Base.Instance.canvas4TopLeftMenu.gameObject.SetActive (false);
		}

		public void CloseRightMenu() {
            Base.Instance.canvas4TopRightMenu.gameObject.SetActive (false);
		}
	}
}