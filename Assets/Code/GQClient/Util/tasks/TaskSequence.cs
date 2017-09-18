using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.UI;
using GQ.Client.Err;

namespace GQ.Client.Util {

	public class TaskSequence : Task {

		#region Building the Sequence

		protected List<Task> tasks;

		private Task lastTask = null;

		public TaskSequence(params Task[] tasks) : base() 
		{
			if (tasks != null && tasks.Length > 0) {
				this.tasks = new List<Task>(tasks);
				lastTask = this.tasks [this.tasks.Count - 1];
				concatenateTasks ();
			}
			else {
				this.tasks = new List<Task> ();
			}
		}

		/// <summary>
		/// Append the specified task to the end of the sequence and concatenates it, 
		/// so that it gets started after the former last has ended.. 
		/// </summary>
		/// <param name="task">Task.</param>
		public void Append(Task task) {
			if (started) {
				Log.SignalErrorToDeveloper("Cannot append a task to a seuqence that is already started.");
				return;
			}


			tasks.Add (task);

			if (tasks.Count > 1) {
				tasks [tasks.Count - 2].OnTaskEnded += tasks [tasks.Count - 1].StartCallback;
			}

			RunsAsCoroutine |= task.RunsAsCoroutine;

			lastTask = task;
		}

		/// <summary>
		/// Append the specified task to the end of the sequence and concatenates it, 
		/// so that it gets started after the former last has been successfully completed .. 
		/// 
		/// If the previous task has not been completed the whole sequence is ended.
		/// </summary>
		/// <param name="task">Task.</param>
		public void AppendIfCompleted(Task task) {
			if (started) {
				Log.SignalErrorToDeveloper("Cannot append a task to a seuqence that is already started.");
				return;
			}

			tasks.Add (task);
			if (tasks.Count > 1) {
				tasks [tasks.Count - 2].OnTaskCompleted += tasks [tasks.Count - 1].StartCallback;

				RunsAsCoroutine |= task.RunsAsCoroutine;
			}

			lastTask = task;
		}

		void concatenateTasks () {
			RunsAsCoroutine = false;

			for (int i= 0; i < tasks.Count; i++) {
				if (tasks.Count - 1 > i) {
					tasks [i].OnTaskEnded += tasks [i + 1].StartCallback;
				}
				RunsAsCoroutine |= tasks [i].RunsAsCoroutine;
			}
		}

		#endregion


		#region Run and End

		bool started = false;

		public override bool Run ()
		{
			started = true;

			lastTask.OnTaskCompleted += CompletedCallback;
			lastTask.OnTaskFailed += FailedCallback;

			if (tasks != null && tasks.Count > 0) {
				tasks [0].Start (Step);
			}

			return true;
		}

		public override IEnumerator RunAsCoroutine() {
			Run ();

			yield break;
		}

		public override object Result {
			get {
				// TODO return the result of the last task if that is already completed. 
				// 		Hence we need an IsCompleted for Tasks.
				return "";
			}
			protected set { }
		}

		private void CompletedCallback (object sender, TaskEventArgs e) {
			RaiseTaskCompleted (e.Content);
		}

		private void FailedCallback (object sender, TaskEventArgs e) {
			RaiseTaskFailed (e.Content);
		}


		#endregion

	}
}

