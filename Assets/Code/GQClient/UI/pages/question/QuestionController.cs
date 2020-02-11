namespace Code.GQClient.UI.pages.question
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
