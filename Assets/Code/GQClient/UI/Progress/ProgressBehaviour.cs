using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GQ.Client.GQEvents;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;

namespace GQ.Client.UI.Progress
{

	public abstract class ProgressBehaviour : UIBehaviour
	{

		public ProgressController Progress { get; set; }

		/// <summary>
		/// Mutually connects this Behaviour with a Dialog Controller and initliazes the behaviour.
		/// </summary>
		public ProgressBehaviour(Task task = null) : base (task)
		{
			Progress = ProgressController.Instance;
			Progress.Behaviour = this;
		}

		/// <summary>
		/// Step counter that can be used
		/// </summary>
		protected int step;

		public override void Start ()
		{
			base.Start ();
		}

		/// <summary>
		/// Should be called before the dialog is made invisible or disposed.
		/// </summary>
		public override void Stop ()
		{
            Progress.Hide();
		}


		/// <summary>
		/// Closes the dialog.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		/// TODO move to Dialog class and change event args to some more generic type
		protected void CloseDialog (object callbackSender, EventArgs args)
		{
			Stop ();
		}

	}

}
