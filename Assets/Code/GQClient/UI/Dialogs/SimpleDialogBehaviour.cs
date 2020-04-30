//#define DEBUG_LOG

using System;
using Code.GQClient.Util.tasks;

namespace Code.GQClient.UI.Dialogs
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

        private readonly string _title;
        private readonly string _details;

        public SimpleDialogBehaviour(Task task, string title, string details) : base(task)
        {
            this._title = title;
            this._details = details;
        }

        public override void Start()
        {
            base.Start();

            Dialog.Title.text = _title;
            Dialog.Details.text = _details;

            // make completion close this dialog:
            Task.OnTaskEnded += CloseDialog;

            // show the dialog:
            Dialog.Show();
        }

        public void Progress(float percent)
        {
            if (Dialog != null && Dialog.Details != null)
            {
                Dialog.Details.text = _details + String.Format(" ({0:#0.0}% erledigt)", percent);
#if DEBUG_LOG
                Debug.Log(("SimpleDialog Text Update in frame: " + Time.frameCount + "\n" + Dialog.Details.text).Yellow());
#endif
            }
        }

    }
}
