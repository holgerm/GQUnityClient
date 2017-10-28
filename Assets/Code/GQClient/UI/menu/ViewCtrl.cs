using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;

namespace GQ.Client.UI
{

	public class ViewCtrl : MonoBehaviour
	{

		void Start ()
		{
			// TODO Assert that both map and list panels exist in the herarchy and have the correct name!
		}

		public GameObject ListCanvas;
		public GameObject MapCanvas;
		public GameObject MapHolder;

		public GameObject MenuCanvas;

		public void OnChangeQuestInfosViewer (GameObject viewer)
		{
			ListCanvas.SetActive (viewer.name == "ViewAsList");
			MapCanvas.SetActive (viewer.name == "ViewOnMap");
			MapHolder.SetActive (viewer.name == "ViewOnMap");

			MenuCanvas.SetActive (false);
		}

	}

}