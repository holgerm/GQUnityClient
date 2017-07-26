using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;

namespace GQ.Client.UI {

	/// <summary>
	/// The base class for UI Behaviours, the glue between long running tasks and their optional UI representation.
	/// 
	/// For details cf. @ref TasksAndUI
	/// 
	/// Overview: @image html UpdateQuestInfoDialogOverview.png width=8cm
	/// </summary>
	public abstract class UIBehaviour {

		/// <summary>
		/// Connectes this Behaviour mutually with the given Task.
		/// </summary>
		/// <param name="task">Task.</param>
		public UIBehaviour (Task task) 
		{
			Task = task;
			task.AddBehaviour(this);
		}

		public Task Task { get; set; }

		/// <summary>
		/// This method should only be called by the associated task and not directly from other classes.
		/// </summary>
		public virtual void Start () {}


	}
}
