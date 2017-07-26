using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GQ.Client.UI;

namespace GQ.Client.Util {

	public abstract class Task {

		public Task() {
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

		/// <summary>
		/// Start the loading process and inform the QuestInfoManager about feedback via callbacks.
		/// 
		/// The parameters step and totalSteps signal which step of how many steps this loading 
		/// within a larger process currently is.
		/// </summary>
		public virtual void Start (int step = 0) {
			Step = step;
			behaviours.ForEach (
				(UIBehaviour behaviour) => behaviour.Start ()
			);
		}

		public virtual void StartCallback(object sender, TaskEventArgs e) {
			this.Start(e.Step + 1);
		}

		public abstract object Result { get; }

		public delegate void TaskCallback (object sender, TaskEventArgs e);

		public event TaskCallback OnTaskCompleted; 
		public event TaskCallback OnTaskFailed; 

		protected virtual void RaiseTaskCompleted(object content = null) {
			if (OnTaskCompleted != null)
				OnTaskCompleted (this, new TaskEventArgs (step: Step, content: content));
		}

		protected virtual void RaiseTaskFailed() {
			if (OnTaskFailed != null)
				OnTaskFailed (this, new TaskEventArgs (step: Step));
		}
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

}
