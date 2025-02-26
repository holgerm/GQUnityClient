﻿using Code.GQClient.Conf;
using Code.GQClient.Util;
using Code.QM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.menu.viewToggle
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
			var go = PrefabController.Create ("prefabs", "ViewToggle", root);
			go.name = "ViewToggle";

			// set menu entry height:
//			MenuLayoutConfig.SetEntryHeight (go);

			// save tree controller & folder:
			var viewCtrl = go.GetComponent<ViewToggleController> ();

			var mtb = viewCtrl.GetComponent<MultiToggleButton> ();
			mtb.shownObjects = new GameObject[Config.Current.questInfoViews.Length];
			for (var i = 0; i < Config.Current.questInfoViews.Length; i++) {
                var viewName = Config.Current.questInfoViews[i];
				var mtbGoName = "ViewToggleTo" + viewName;
				mtb.shownObjects [i] = PrefabController.Create ("prefabs", mtbGoName, viewCtrl.gameObject);
				mtb.shownObjects [i].name = mtbGoName;
                mtb.shownObjects[i].transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("icons/" + viewName.ToLower());
			}
			// view no. 0 is set, view no. 1 is shown as next to reach by this menu entry:
			mtb.SetSelectedStartIndex (1);

			viewCtrl.gameObject.SetActive (true);
			return viewCtrl;
		}

		public void OnChangeQuestInfosViewer (GameObject viewer)
		{
			Base.Instance.ListCanvas.SetActive ("ViewToggleToList" == viewer.name);
			Base.Instance.TopicTreeCanvas.SetActive ("ViewToggleToTopicTree" == viewer.name);
			Base.Instance.Map.gameObject.SetActive ("ViewToggleToMap" == viewer.name);
			Base.Instance.MapCanvas.SetActive ("ViewToggleToMap" == viewer.name);

			Base.Instance.MenuCanvas.SetActive (false);
		}

	}

}