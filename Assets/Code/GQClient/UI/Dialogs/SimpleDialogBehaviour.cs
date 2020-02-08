//#define DEBUG_LOG

using GQ.Client.Util;
using System;

namespace GQ.Client.UI.Dialogs
{

    /// <summary>
    /// Simple dialog behaviour usable just by giving title and details text to the constructor. 
    /// 
    /// Standard behaviour will start the dialog without buttons and show the title and details. 
    /// 
    /// When the task is completed the dialog will be closed. It will be shown for at least one frame.
    /// </summary>
    public class SimpleDialogBehaviour : DialogBehaviour, SimpleBehaviour
    {

        private string title;
        private string details;

        public SimpleDialogBehaviour(Task task, string title, string details) : base(task)
        {
            this.title = title;
            this.details = details;
        }

        public override void Start()
        {
            base.Start();

            Dialog.Title.text = title;
            Dialog.Details.text = details;

            // make completion close this dialog:
            Task.OnTaskEnded += CloseDialog;

            // show the dialog:
            Dialog.Show();
        }

        public void OnProgress(float percent)
        {
            if (Dialog != null && Dialog.Details != null)
            {
                Dialog.Details.text = details + String.Format(" ({0:#0.0}% erledigt)", percent);
#if DEBUG_LOG
                Debug.Log(("SimpleDialog Text Update in frame: " + Time.frameCount + "\n" + Dialog.Details.text).Yellow());
#endif
            }
        }

    }
}
