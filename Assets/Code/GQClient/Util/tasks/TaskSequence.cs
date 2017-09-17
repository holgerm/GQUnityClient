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

		/// <summary>
		/// Append the specified task to the end of the sequence and concatenates it, 
		/// so that it gets started after the former last has ended.. 
		/// </summary>
		/// <param name="task">Task.</param>
		public void Append(Task task) {
			tasks.Add (task);
			if (tasks.Count > 1) {
				tasks [tasks.Count - 2].OnTaskEnded += tasks [tasks.Count - 1].StartCallback;
			}
		}

		/// <summary>
		/// Append the specified task to the end of the sequence and concatenates it, 
		/// so that it gets started after the former last has been successfully completed .. 
		/// 
		/// If the previous task has not been completed the whole sequence is ended.
		/// </summary>
		/// <param name="task">Task.</param>
		public void AppendIfCompleted(Task task) {
			tasks.Add (task);
			if (tasks.Count > 1) {
				tasks [tasks.Count - 2].OnTaskCompleted += tasks [tasks.Count - 1].StartCallback;
			}
		}

		void concatenateTasks () {
			for (int i= 0; i < tasks.Count; i++) {
				if (tasks.Count - 1 > i) {
					tasks [i].OnTaskEnded += tasks [i + 1].StartCallback;
				}
			}
		}

		public override void Start (int step = 1)
		{
			base.Start(step);
			if (tasks != null && tasks.Count > 0) {
				if (Step == 0 && tasks.Count > 1)
					Step = 1;
				tasks [0].Start (Step);
			}
		}

		public override object Result {
			get {
				// TODO return the result of the last task if that is already completed. 
				// 		Hence we need an IsCompleted for Tasks.
				return "";
			}
			protected set { }
		}

	}
}

