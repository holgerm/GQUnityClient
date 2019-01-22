using GQ.Client.Model;
using GQ.Client.UI.Dialogs;

namespace GQ.Client.UI
{

    public abstract class QuestionController : PageController
    {
        public void Repeat()
        {
            RepeatDialog dialog = new RepeatDialog((QuestionPage)page);
            dialog.Start();
        }


    }
}
