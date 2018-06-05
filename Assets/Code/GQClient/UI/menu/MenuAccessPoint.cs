using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.UI
{

	public class MenuAccessPoint : MonoBehaviour
	{

		public GameObject MenuTopLeftContent;
		public Canvas canvas4TopLeftMenu;
		public GameObject MenuTopRightContent;
		public Canvas canvas4TopRightMenu;

		public Canvas imprintCanvas;
		public Canvas privacyCanvas;
		public Canvas authorCanvas;

		public void LeftMenuButtonPressed() {
			imprintCanvas.gameObject.SetActive (false);
			privacyCanvas.gameObject.SetActive (false);
			authorCanvas.gameObject.SetActive (false);

			canvas4TopLeftMenu.gameObject.SetActive (!canvas4TopLeftMenu.gameObject.activeSelf);

			if (canvas4TopLeftMenu.gameObject.activeSelf)
				canvas4TopRightMenu.gameObject.SetActive (false);
		}

		public void RightMenuButtonPressed() {
			if (imprintCanvas.gameObject.activeSelf) {
				imprintCanvas.gameObject.SetActive (false);
				return;
			}

			if (privacyCanvas.gameObject.activeSelf) {
				privacyCanvas.gameObject.SetActive (false);
				return;
			}

			if (authorCanvas.gameObject.activeSelf) {
				authorCanvas.gameObject.SetActive (false);
				return;
			}

			canvas4TopRightMenu.gameObject.SetActive (!canvas4TopRightMenu.gameObject.activeSelf);

			if (canvas4TopRightMenu.gameObject.activeSelf)
				canvas4TopLeftMenu.gameObject.SetActive (false);

		}

		public void CloseLeftMenu() {
			canvas4TopLeftMenu.gameObject.SetActive (false);
		}

		public void CloseRightMenu() {
			canvas4TopRightMenu.gameObject.SetActive (false);
		}
	}
}