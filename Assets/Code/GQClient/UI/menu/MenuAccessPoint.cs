using System.Collections;
using System.Collections.Generic;
using GQ.Client.Util;
using UnityEngine;

namespace GQ.Client.UI
{

	public class MenuAccessPoint : MonoBehaviour
	{
		public void LeftMenuButtonPressed() {
            Base.Instance.imprintCanvas.gameObject.SetActive (false);
            Base.Instance.privacyCanvas.gameObject.SetActive(false);
            Base.Instance.feedbackCanvas.gameObject.SetActive (false);
            Base.Instance.authorCanvas.gameObject.SetActive (false);

            Base.Instance.canvas4TopLeftMenu.gameObject.SetActive (
                !Base.Instance.canvas4TopLeftMenu.gameObject.activeSelf);

			if (Base.Instance.canvas4TopLeftMenu.gameObject.activeSelf)
                Base.Instance.canvas4TopRightMenu.gameObject.SetActive (false);
		}

		public void RightMenuButtonPressed() {
			if (Base.Instance.imprintCanvas.gameObject.activeSelf) {
                Base.Instance.imprintCanvas.gameObject.SetActive (false);
				return;
			}

            if (Base.Instance.feedbackCanvas.gameObject.activeSelf)
            {
                Base.Instance.feedbackCanvas.gameObject.SetActive(false);
                return;
            }

            if (Base.Instance.privacyCanvas.gameObject.activeSelf) {
                Base.Instance.privacyCanvas.gameObject.SetActive (false);
				return;
			}

			if (Base.Instance.authorCanvas.gameObject.activeSelf) {
                Base.Instance.authorCanvas.gameObject.SetActive (false);
				return;
			}

            Base.Instance.canvas4TopRightMenu.gameObject.SetActive (
                !Base.Instance.canvas4TopRightMenu.gameObject.activeSelf);

			if (Base.Instance.canvas4TopRightMenu.gameObject.activeSelf)
                Base.Instance.canvas4TopLeftMenu.gameObject.SetActive (false);

		}

		public void CloseLeftMenu() {
            Base.Instance.canvas4TopLeftMenu.gameObject.SetActive (false);
		}

		public void CloseRightMenu() {
            Base.Instance.canvas4TopRightMenu.gameObject.SetActive (false);
		}
	}
}