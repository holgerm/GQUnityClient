using Code.GQClient.Util.tasks;

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

			Progress.Title.text = title;

			// show the dialog:
			Progress.Show();
		}

		public override void Stop ()
		{
			base.Stop ();

			Progress.Hide();
		}

		private string title;

		public void OnProgress(float percent)
		{
			if (Progress != null && Progress.ProgressSlider != null)
			{
				Progress.ProgressSlider.value = percent / 100f;
			}
		}
	}
}