using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GQ.Client.UI;
using GQ.Client.Err;

namespace GQ.Client.Util {

	/// <summary>
	/// 	
	/// The base class for background activities that can be accompanied by some foreground reflection, 
	/// e.g. a dialog etc..
	/// 
	/// For details cf. @ref TasksAndUI
	///
	/// </summary>
	public abstract class Task {

		public Task(bool runsAsCoroutine = false) {
			RunsAsCoroutine = runsAsCoroutine;
			behaviours = new List<UIBehaviour> ();
		}


		#region Link to Behavious

		protected List<UIBehaviour> behaviours;

		public void AddBehaviour(UIBehaviour behaviour) 
		{
			behaviours.Add (behaviour);
		}

		#endregion


		public int Step { get; protected set; }

		public bool RunsAsCoroutine { get; internal set; }

		/// <summary>
		/// Start the loading process and inform the QuestInfoManager about feedback via callbacks.
		/// 
		/// The parameters step and totalSteps signal which step of how many steps this loading 
		/// within a larger process currently is.
		/// </summary>
		public void Start (int step = 1) {
			Step = step;
			Debug.Log ("START Task step " + step + " type: " + GetType().Name);
			behaviours.ForEach (
				(UIBehaviour behaviour) => behaviour.Start ()
			);

			if (!RunsAsCoroutine) {
				if (Run ())
					RaiseTaskCompleted (Result);
				else
					RaiseTaskFailed ();
			}
			else {
				Base.Instance.StartCoroutine (RunAsCoroutine());
			}
		}

		/// <summary>
		/// Overrride this method to implement behaviour of your task subclass. 
		/// Return either true od false to signal that this task has succeeded or failed.
		/// 
		/// In order to give the tasks result to potential following tasks, 
		/// store it in the Result property before leaving this method.
		/// </summary>
		public virtual bool Run() {
			return true;
		}

		/// <summary>
		/// Override this method implementing the behaviour of your task, if it should run as coroutine. 
		/// In this case you must signal success or failure by calling RaiseTaskComplete() or RaiseTaskFailed() 
		/// and optional give the result as parameter (or store in in advance in the Result property).
		/// </summary>
		/// <returns>The as coroutine.</returns>
		public virtual IEnumerator RunAsCoroutine () {
			yield return null;
			RaiseTaskCompleted ();
			yield break;
		}

		public void StartCallback(object sender, TaskEventArgs e) {
			Step = e.Step + 1;

			InitAfterPreviousTask (sender, e);

			this.Start(Step);
		}

		/// <summary>
		/// Override this method to initialize this task based on the result of the previous tasks, 
		/// e.g. read specific input. This method is called when this task is chained after another task 
		/// within a TaskSequence and before the Start() method is called.
		/// </summary>
		public virtual void InitAfterPreviousTask (object sender, TaskEventArgs e) {
			return;
		}

		public virtual object Result { get; protected set; }

		public delegate void TaskCallback (object sender, TaskEventArgs e);

		public event TaskCallback OnTaskCompleted; 
		public event TaskCallback OnTaskFailed;
		public event TaskCallback OnTaskEnded;

		private bool hasEnded = false;

		public void RaiseTaskCompleted(object content = null) {
			if (hasEnded)
				return;
			else
				hasEnded = true;

			Debug.Log ("Task COMPLETED step: " + Step + " type: " + GetType().Name);

			if (OnTaskCompleted != null)
				OnTaskCompleted (this, new TaskEventArgs (step: Step, content: content));
			if (OnTaskEnded != null)
				OnTaskEnded (this, new TaskEventArgs (step: Step, content: content));
		}

		public virtual void RaiseTaskFailed(object content = null) {
			if (hasEnded)
				return;
			else
				hasEnded = true;
			
			Debug.Log ("Task FAILED step: " + Step + " type: " + GetType().Name);

			if (OnTaskFailed != null)
				OnTaskFailed (this, new TaskEventArgs (step: Step, content: content));
			if (OnTaskEnded != null)
				OnTaskEnded (this, new TaskEventArgs (step: Step, content: content));
		}

		#region Test Access

		public Delegate[] GetOnEndedInvocationList() {
			return OnTaskEnded != null ? OnTaskEnded.GetInvocationList () : null;
		}

		public Delegate[] GetOnCompletedInvocationList() {
			return OnTaskCompleted != null ? OnTaskCompleted.GetInvocationList () : null;
		}

		public Delegate[] GetOnFailedInvocationList() {
			return OnTaskFailed != null ? OnTaskFailed.GetInvocationList () : null;
		}

		#endregion

	}


	public class TaskEventArgs : EventArgs 
	{
		public int Step { get; protected set; }
		public object Content { get; protected set; }

		public TaskEventArgs(
			int step = 0,
			object content = null)
		{
			Step = step;
			Content = content;
		}
	}

	public enum TaskState { Started, Succeded, Failed, RunningAsCoroutine };

}
