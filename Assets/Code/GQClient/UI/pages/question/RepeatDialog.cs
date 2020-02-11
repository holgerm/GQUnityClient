using Code.GQClient.UI.Dialogs;
using Code.GQClient.Util;

namespace Code.GQClient.UI.pages.question
{
    public class RepeatDialog : DialogBehaviour
    {
        private Repeatable question { get; set; }

        public RepeatDialog(Repeatable question) : base(null)
        // 'null' because we do NOT connect a Task, sice retry dialogs only rely on user interaction
        {
            this.question = question;
        }

        public override void Start()
        {
            base.Start();

            Dialog.Title.gameObject.SetActive(true);
            Dialog.Title.text = "Wiederholen";

            Dialog.Img.gameObject.SetActive(false);
            //if (question.RepeatImage != "")
            //{
            //    Dialog.Img.gameObject.SetActive(true);
            //    //Dialog.Img = 
            //}

            Dialog.Details.text = question.RepeatText.Decode4TMP(false);
            Dialog.SetYesButton(question.RepeatButtonText, CloseDialog);
            Dialog.NoButton.gameObject.SetActive(false);

            // show the dialog:
            Dialog.Show();
        }
    }
}
