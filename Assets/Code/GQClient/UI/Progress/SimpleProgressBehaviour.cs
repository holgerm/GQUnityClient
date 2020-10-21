using Code.GQClient.Util.tasks;
using UnityEngine;

namespace Code.GQClient.UI.Progress
{
    public class SimpleProgressBehaviour : ProgressBehaviour, SimpleBehaviour
	{
		public SimpleProgressBehaviour(Task task, string title, string details) : base(task)
		{
            this.title = title + " (" + details + ")";
		}

		public override void Start ()
		{
			base.Start ();

			base.Progress.Title.text = title;

			// show the dialog:
			base.Progress.Show();
		}

		public override void Stop ()
		{
			base.Stop ();
			base.Progress.Hide();
		}

		private string title;

		public void Progress(float percent)
		{
			if (base.Progress != null && base.Progress.ProgressSlider != null)
			{
				base.Progress.ProgressSlider.value = percent / 100f;
			}
		}
	}
}