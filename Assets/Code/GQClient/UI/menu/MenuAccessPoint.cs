using System.Collections;
using System.Collections.Generic;
using GQ.Client.Util;
using UnityEngine;

namespace GQ.Client.UI
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