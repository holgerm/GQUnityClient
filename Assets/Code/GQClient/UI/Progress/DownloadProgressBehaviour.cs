using System;
using GQ.Client.Util;
using GQ.Client.Err;

namespace GQ.Client.UI.Progress
{
    public class DownloadProgressBehaviour : ProgressBehaviour, DownloadBehaviour
	{

		AbstractDownloader DownloadTask { get; set; }

		public DownloadProgressBehaviour(Task task, string title = "Laden ...") : base(task)
		{
			if (Task is AbstractDownloader) {
				DownloadTask = Task as AbstractDownloader;
			} else
            {
                Log.SignalErrorToDeveloper("DownloadProgressBehaviour may only be used with Tasks that are AbstractDownloaders.");
            }

			this.title = title;
		}

		public override void Start ()
		{
			base.Start ();

			// to prevent registering the same listeners multiple times, in case we initialize multiple times ...
			detachUpdateListeners ();

			// attach listeners before the task gets started:
			attachUpdateListeners ();
		}

		public override void Stop ()
		{
			base.Stop ();

			detachUpdateListeners ();
		}

		void attachUpdateListeners ()
		{
            if (DownloadTask == null)
                return;

			DownloadTask.OnStart += OnDownloadStarted;
			DownloadTask.OnProgress += UpdateLoadingScreenProgress;
			DownloadTask.OnSuccess += OnDownloadSucceeded;
			DownloadTask.OnError += UpdateLoadingScreenError;
			DownloadTask.OnTimeout += UpdateLoadingScreenTimeout;
		}

        void detachUpdateListeners ()
		{
            if (DownloadTask == null)
                return;

            DownloadTask.OnStart -= OnDownloadStarted;
			DownloadTask.OnProgress -= UpdateLoadingScreenProgress;
			DownloadTask.OnSuccess -= CloseDialog;
			DownloadTask.OnError -= UpdateLoadingScreenError;
			DownloadTask.OnTimeout -= UpdateLoadingScreenTimeout;
		}

		private string title;

		/// <summary>
		/// Callback for the OnUpdateStart event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void OnDownloadStarted (object callbackSender, DownloadEvent args)
		{
            Progress.Title.text = title;
			Progress.ProgressSlider.value = 0f;

			// now we show the progress panel:
			Progress.Show ();
		}

		private void OnDownloadSucceeded(AbstractDownloader d, DownloadEvent e)
		{
			Progress.Hide();
		}

		/// <summary>
		/// Callback for the OnProgress event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenProgress (object callbackSender, DownloadEvent args)
		{
			Progress.ProgressSlider.value = args.Progress;
		}

		/// <summary>
		/// Callback for the OnError event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenError (object callbackSender, DownloadEvent args)
		{
			Progress.Title.text = String.Format ("Fehler: {0}", args.Message);
		}


		/// <summary>
		/// Callback for the OnTimeout event.
		/// </summary>
		/// <param name="callbackSender">Callback sender.</param>
		/// <param name="args">Arguments.</param>
		public void UpdateLoadingScreenTimeout (object callbackSender, DownloadEvent args)
		{
			Progress.Title.text = String.Format ("Problem: {0}", args.Message);
		}
	}

}