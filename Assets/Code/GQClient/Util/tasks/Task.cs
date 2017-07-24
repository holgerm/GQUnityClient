using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GQ.Client.Util {

	/// <summary>
	/// Abstract Strategy class for Quest Info Loaders. There are at least three concrete loaders:
	/// 
	/// - ServerQuestInfoLoader
	/// - LocalQuestInfoLoader
	/// - TestQuestInfoLoader
	/// 
	/// </summary>
	public abstract class Task {

		/// <summary>
		/// Start the loading process and inform the QuestInfoManager about feedback via callbacks.
		/// 
		/// The parameters step and totalSteps signal which step of how many steps this loading 
		/// within a larger process currently is.
		/// </summary>
		public abstract void Start (int step = 0);

		public void StartCallback(object sender, TaskEventArgs e) {
			this.Start(e.Step);
		}

		public delegate void TaskCallback (object sender, TaskEventArgs e);

		public event TaskCallback OnTaskCompleted; 

		public virtual void RaiseTaskCompleted() {
			if (OnTaskCompleted != null)
				OnTaskCompleted (this, new TaskEventArgs ());
		}

		public class TaskEventArgs : EventArgs 
		{
			public int Step { get; protected set; }
			public Task NextTask { get; protected set; }

			public TaskEventArgs(
				int step = 0,
				Task nextTask = null)
			{
				Step = step;
				NextTask = nextTask;
			}
		}

	}

}
