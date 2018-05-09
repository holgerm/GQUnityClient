using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;
using GQ.Client.Util;
using QM.UI;
using GQ.Client.Conf;
using System.Diagnostics;
using QM.Util;

namespace GQ.Client.UI
{

	public class ViewToggleController : MonoBehaviour
	{

		/// <summary>
		/// When this method is called it is guaranteed that at least two views are referred by shownObjects[] of the MultiToggleButton.
		/// </summary>
		/// <param name="root">Root.</param>
		public static ViewToggleController Create (GameObject root)
		{
			// Create the view object for this controller:
			GameObject go = PrefabController.Create ("ViewToggle", root);
			go.name = "ViewToggle";

			// set menu entry height:
//			MenuLayoutConfig.SetEntryHeight (go);

			// save tree controller & folder:
			ViewToggleController viewCtrl = go.GetComponent<ViewToggleController> ();

			MultiToggleButton mtb = viewCtrl.GetComponent<MultiToggleButton> ();
			mtb.shownObjects = new GameObject[ConfigurationManager.Current.questInfoViews.Length];
			for (int i = 0; i < ConfigurationManager.Current.questInfoViews.Length; i++) {
				string mtbGoName = "ViewToggleTo" + ConfigurationManager.Current.questInfoViews [i];
				mtb.shownObjects [i] = PrefabController.Create (mtbGoName, viewCtrl.gameObject);
				mtb.shownObjects [i].name = mtbGoName;
			}
			// view no. 0 is set, view no. 1 is shown as next to reach by this menu entry:
			mtb.SetSelectedStartIndex (1);

			viewCtrl.gameObject.SetActive (true);
			return viewCtrl;
		}

		public void OnChangeQuestInfosViewer (GameObject viewer)
		{
			WATCH w = new WATCH ("Change Quests View");
			w.Start ();
			Base.Instance.ListCanvas.SetActive (viewer.name == "ViewToggleToList");
			Base.Instance.MapCanvas.SetActive (viewer.name == "ViewToggleToMap");
			Base.Instance.MapHolder.SetActive (viewer.name == "ViewToggleToMap");
			w.StopAndShow ();

			Base.Instance.MenuCanvas.SetActive (false);
		}

	}

}