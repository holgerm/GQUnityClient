using System;
using System.Collections;
using System.Collections.Generic;
using Code.GQClient.Err;
using Code.GQClient.UI;
using UnityEngine;

namespace Code.GQClient.Util.tasks
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
            behaviours = new List<AbstractBehaviour>();
        }


        #region Link to Behaviours

        protected List<AbstractBehaviour> behaviours;

        public void AddBehaviour(AbstractBehaviour behaviour)
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
            OnTaskStarted?.Invoke(this, null);
            behaviours.ForEach(
                (AbstractBehaviour behaviour) => behaviour.Start()
            );

            ReadInput(input);

            IEnumerator results = DoTheWork();
            OnTaskEnded?.Invoke(this, null);
            return results;
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

        public event TaskCallback OnTaskStarted;
        public event TaskCallback OnTaskCompleted;
        public event TaskCallback OnTaskFailed;
        public event TaskCallback OnTaskEnded;

        private bool hasEnded = false;

        private Dictionary<string, string> responseHeaders;

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
                (AbstractBehaviour behaviour) => behaviour.Stop()
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
            }

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
            return OnTaskEnded?.GetInvocationList();
        }

        public Delegate[] GetOnCompletedInvocationList()
        {
            return OnTaskCompleted?.GetInvocationList();
        }

        public Delegate[] GetOnFailedInvocationList()
        {
            return OnTaskFailed?.GetInvocationList();
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
    };
}