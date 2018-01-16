using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Candlelight.UI;

namespace GQ.Client.UI
{

	public class DialogItemCtrl : MonoBehaviour {

		public HyperText DialogItemHyperText;

		public void Initialize(string itemText) {
			this.DialogItemHyperText.text = itemText;
		}

		public static DialogItemCtrl Create(Transform rootTransform, string text) {
			GameObject go = (GameObject)Instantiate (
				Resources.Load ("DialogItem"),
				rootTransform,
				false
			);
			go.SetActive (true);

			DialogItemCtrl diCtrl = go.GetComponent<DialogItemCtrl> ();
			diCtrl.Initialize (text);

			return diCtrl;
		}
	}
}
