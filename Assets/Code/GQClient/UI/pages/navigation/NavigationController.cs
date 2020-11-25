// #define DEBUG_LOG

using System;
using Code.GQClient.Conf;
using Code.GQClient.Model.pages;
using Code.GQClient.UI.map;
using Code.GQClient.Util;
using Code.GQClient.Util.input;
using UnityEngine;

namespace Code.GQClient.UI.pages.navigation
{

    public class NavigationController : PageController
	{
		#region Runtime API
		protected PageNavigation navPage;
        public MapController mapCtrl;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void InitPage_TypeSpecific ()
		{
			LocationSensor.Instance.OnLocationUpdate += page.Quest.UpdateHotspotMarkers; // NEW: PROBLEM SOLVED?

            try
            {
                navPage = (PageNavigation)page;
            }
            catch(Exception e)
            {
                Debug.Log("Navigationctrl.InitPage() exception caught during cast: " +
                    e.Message + "\ncurrent page is: " + page.Quest.CurrentPage +
                    " given page is: " + page);
            }

            // footer:
            // hide footer if no return possible:
            FooterButtonPanel.transform.parent.gameObject.SetActive(navPage.Quest.History.CanGoBackToPreviousPage);
            forwardButton.gameObject.SetActive(false);

            // enable all defined options:
            enableOptions();

            // initial Zoom:
            mapCtrl.map.zoom = navPage.initialZoomLevel;
		}

		void enableOptions ()
		{
			if (navPage.mapOption) {
			}
			// TODO
		}
		#endregion

	}
}