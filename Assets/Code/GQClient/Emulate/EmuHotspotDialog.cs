﻿using Code.GQClient.Model.actions;
using Code.GQClient.UI;
using Code.GQClient.Util;
using UnityEngine;

namespace Code.GQClient.Emulate
{

	/// <summary>
	/// Emulates the Interaction with a hotspot (tap, enter, leave on the client in emulation mode. 
	/// 
	/// This dialog is shown in the app when in emulation mode and tapping a hotspot. 
	/// It offers the user to emulate enter, tap or leave events on the tapped hotspot.
	/// 
	/// This class contains structure and behaviour together.
	/// </summary>
	public class EmuHotspotDialog : PrefabController
	{

        #region Initialization

        static readonly string PREFAB_ASSETBUNDLE = "authormode";
        static readonly string PREFAB_NAME = "EmuHotspotDialog";

		Trigger enterTrigger;
		Trigger leaveTrigger;
		Trigger tapTrigger;

		public static void CreateAndShow (Trigger enterTrigger, Trigger leaveTrigger, Trigger tapTrigger)
		{
			// create:
			var emuDialogGO = Create (PREFAB_ASSETBUNDLE, PREFAB_NAME, Base.Instance.DialogCanvas);
			var emuDialog = emuDialogGO.GetComponent<EmuHotspotDialog> ();

			// init:
			emuDialog.enterTrigger = enterTrigger;
			emuDialog.leaveTrigger = leaveTrigger;
			emuDialog.tapTrigger = tapTrigger;

			// show for interaction:
			emuDialogGO.SetActive (true);
			Debug.Log("EMU Dialog set active");
		}

		#endregion


		#region Emulation Interaction

		public void emulateOnTap ()
		{
			tapTrigger.Initiate ();
			Destroy ();
		}

		public void emulateOnEnter ()
		{
			enterTrigger.Initiate ();
			Destroy ();
		}

		public void emulateOnLeave ()
		{
			leaveTrigger.Initiate ();
			Destroy ();
		}

		#endregion

	}
}
