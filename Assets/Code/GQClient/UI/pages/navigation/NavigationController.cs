using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;

namespace GQ.Client.UI
{

	public class NavigationController : PageController
	{



		#region Runtime API

		protected PageNavigation navPage;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void Initialize ()
		{
			navPage = (PageNavigation)page;

			// enable all defined options:
			enableOptions ();
		}

		void enableOptions ()
		{
			// TODO
		}

		#endregion

	}
}