using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;

namespace GQ.Client.UI {

	public abstract class UIBehaviour {

		/// <summary>
		/// Connectes thsi Behaviour mutually with the given Task.
		/// </summary>
		/// <param name="task">Task.</param>
		public UIBehaviour (Task task) 
		{
			Task = task;
			task.Behaviour = this;
		}

		public Task Task { get; set; }

	}
}
