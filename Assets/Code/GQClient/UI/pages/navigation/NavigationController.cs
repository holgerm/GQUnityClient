using GQ.Client.Model;
using GQ.Client.Util;
using QM.Util;
using UnityEngine;

namespace GQ.Client.UI
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

            navPage = (PageNavigation)page;

            // footer:
            // hide footer if no return possible:
            FooterButtonPanel.transform.parent.gameObject.SetActive(navPage.Quest.History.CanGoBackToPreviousPage);
            Debug.Log("Nav Ctrl: Set Footer to " + navPage.Quest.History.CanGoBackToPreviousPage);
            forwardButton.gameObject.SetActive(false);

            // enable all defined options:
            enableOptions();
		}

		void enableOptions ()
		{
			if (navPage.mapOption) {
				Device.location.InitLocationMock ();
			}
			// TODO
		}

		/// <summary>
		/// Removes the map location update listener before the navigation page controlled by this controller is left.
		/// </summary>
		public override void CleanUp() {
			LocationSensor.Instance.OnLocationUpdate -= mapCtrl.map.UpdatePosition;
		}

		#endregion

	}
}