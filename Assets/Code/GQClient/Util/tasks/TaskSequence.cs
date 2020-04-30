using System.Collections;
using System.Collections.Generic;
using Code.GQClient.Err;

namespace Code.GQClient.Util.tasks
{

    public class TaskSequence : Task
    {

        #region Building the Sequence

        private List<Task> tasks;

        private Task _lastTask = null;

        public TaskSequence(params Task[] tasks) : base()
        {
            if (tasks != null && tasks.Length > 0)
            {
                this.tasks = new List<Task>(tasks);
                _lastTask = this.tasks[this.tasks.Count - 1];
            }
            else
            {
                this.tasks = new List<Task>();
            }
        }

        /// <summary>
        /// Append the specified task to the end of the sequence and concatenates it, 
        /// so that it gets started after the former last has ended.. 
        /// </summary>
        /// <param name="task">Task.</param>
        public void Append(Task task)
        {
            if (started)
            {
                Log.SignalErrorToDeveloper("Cannot append a task to a seuqence that is already started.");
                return;
            }


            tasks.Add(task);
            _lastTask = task;
        }

        #endregion


        #region Run and End

        bool started = false;

        object Input;

        protected override void ReadInput(object input = null)
        {
            this.Input = input;
        }

        protected override IEnumerator DoTheWork()
        {
            started = true;

            tasks[tasks.Count -1].OnTaskCompleted += CompletedCallback;

            foreach (var t in tasks)
            {
                t.OnTaskFailed += FailedCallback;

                var taskEnumerator = t.RunAsCoroutine(Input);
                while (taskEnumerator.MoveNext())
                {
                    yield return taskEnumerator.Current;
                }

                t.StopBehaviours();
                t.OnTaskFailed -= FailedCallback;

                Input = t.Result;
            }

            StopBehaviours();
        }

        public override object Result
        {
            get
            {
                // TODO return the result of the last task if that is already completed. 
                // 		Hence we need an IsCompleted for Tasks.
                return "";
            }
            set { }
        }

        private void CompletedCallback(object sender, TaskEventArgs e)
        {
            RaiseTaskCompleted(e.Content);
        }

        private void FailedCallback(object sender, TaskEventArgs e)
        {
            RaiseTaskFailed(e.Content);
        }
        #endregion
    }
}

