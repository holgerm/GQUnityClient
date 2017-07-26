using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.UI;

namespace GQ.Client.Util {

	public class TaskSequence : Task {

		protected List<Task> tasks;

		public TaskSequence(params Task[] tasks) : base() 
		{
			this.tasks = new List<Task>(tasks);
			concatenateTasks ();
		}

		public void Append(Task task) {
			tasks.Add (task);
			concatenateTasks ();
		}

		void concatenateTasks () {
			for (int i= 0; i < tasks.Count; i++) {
				if (tasks.Count - 1 > i) {
					tasks [i].OnTaskCompleted += tasks [i + 1].StartCallback;
				}
			}
		}

		public override void Start (int step = 1)
		{
			Debug.Log ("TaskSequenz.Start() step: " + step);
			base.Start(step);
			if (tasks != null && tasks.Count > 0) {
				if (Step == 0 && tasks.Count > 1)
					Step = 1;
				tasks [0].Start (Step);
			}
		}

	}
}

