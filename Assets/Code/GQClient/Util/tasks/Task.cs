using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GQ.Client.UI;
using GQ.Client.Err;
using System.Text;
using QM.Util;

namespace GQ.Client.Util
{

    /// <summary>
    /// 	
    /// The base class for background activities that can be accompanied by some foreground reflection, 
    /// e.g. a dialog etc..
    /// 
    /// For details cf. @ref TasksAndUI
    ///
    /// </summary>
    public abstract class Task
    {

        public Task(bool runsAsCoroutine = false)
        {
            RunsAsCoroutine = runsAsCoroutine;
            behaviours = new List<UIBehaviour>();
        }


        #region Link to Behavious

        protected List<UIBehaviour> behaviours;

        public void AddBehaviour(UIBehaviour behaviour)
        {
            behaviours.Add(behaviour);
        }

        #endregion


        public int Step { get; protected set; }

        public bool RunsAsCoroutine { get; internal set; }

        /// <summary>
        /// Starts the loading process and informs the QuestInfoManager about feedback via callbacks.
        /// 
        /// The parameters step and totalSteps signal which step of how many steps this loading 
        /// within a larger process currently is.
        /// </summary>
        public void Start(object input = null, int step = 1)
        {
            Step = step;

            CoroutineStarter.Run(RunAsCoroutine(input));
        }

        /// <summary>
        /// Override this method implementing the behaviour of your task. 
        /// 
        /// You must signal success or failure by calling RaiseTaskComplete() or RaiseTaskFailed() 
        /// and optional give the result as parameter (or store in in advance in the Result property).
        /// </summary>
        public virtual IEnumerator RunAsCoroutine(object input = null)
        {
            behaviours.ForEach(
                (UIBehaviour behaviour) => behaviour.Start()
            );

            ReadInput(input);

            return DoTheWork();
        }

        /// <summary>
        /// Overwrite this method to read input immediately before this task starts to work. This input is given by the call to the Run method.
        /// </summary>
        /// <param name="input">Input.</param>
        protected virtual void ReadInput(object input)
        {
            return;
        }

        /// <summary>
        /// You must implement this method to do the work of your task subclass. 
        /// This method is called after behaviours have been started and input is read.
        /// </summary>
        /// <returns>The the work.</returns>
        protected abstract IEnumerator DoTheWork();

        /// <summary>
        /// Use this property to store the result of this task. 
        /// Within TaskSequences this result is given as input to the next task.
        /// </summary>
        /// <value>The result.</value>
        public virtual object Result { get; set; }


        #region Events

        public delegate void TaskCallback(object sender, TaskEventArgs e);

        public event TaskCallback OnTaskCompleted;
        public event TaskCallback OnTaskFailed;
        public event TaskCallback OnTaskEnded;

        private bool hasEnded = false;

        public void RaiseTaskCompleted(object content = null)
        {
            if (hasEnded)
                return;
            else
            {
                hasEnded = true;
            }

            BeforeCompleted();

            StopBehaviours();

            if (OnTaskCompleted != null)
                OnTaskCompleted(this, new TaskEventArgs(step: Step, content: content));
            if (OnTaskEnded != null)
                OnTaskEnded(this, new TaskEventArgs(step: Step, content: content));
        }

        public void StopBehaviours()
        {
            behaviours.ForEach(
                (UIBehaviour behaviour) => behaviour.Stop()
            );
        }

        protected virtual void BeforeCompleted()
        {
            return;
        }

        public virtual void RaiseTaskFailed(object content = null)
        {
            if (hasEnded)
                return;
            else
            {
                hasEnded = true;
                Debug.Log("Task " + GetType().Name + " has ended.");
            }

            Debug.Log("Task FAILED step: " + Step + " type: " + GetType().Name);

            BeforeFailed();

            StopBehaviours();

            if (OnTaskFailed != null)
                OnTaskFailed(this, new TaskEventArgs(step: Step, content: content));
            if (OnTaskEnded != null)
                OnTaskEnded(this, new TaskEventArgs(step: Step, content: content));
        }

        protected virtual void BeforeFailed()
        {
            return;
        }

        #endregion


        #region Test Access

        public Delegate[] GetOnEndedInvocationList()
        {
            return OnTaskEnded != null ? OnTaskEnded.GetInvocationList() : null;
        }

        public Delegate[] GetOnCompletedInvocationList()
        {
            return OnTaskCompleted != null ? OnTaskCompleted.GetInvocationList() : null;
        }

        public Delegate[] GetOnFailedInvocationList()
        {
            return OnTaskFailed != null ? OnTaskFailed.GetInvocationList() : null;
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

    public enum TaskState
    {
        Started,
        Succeded,
        Failed,
        RunningAsCoroutine
    }

    ;

}
