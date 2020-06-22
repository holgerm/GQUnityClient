using Code.GQClient.UI.Dialogs;

public class CounterDialog : DialogBehaviour
{
    private string Title { get; set; }
    private string Message { get; set; }

    private int _counter;
    public int Counter
    {
        get => _counter;
        set
        {
            _counter = value;
            if (value == 0)
            {
                CloseDialog(this, null);
                return;
            }
            
            Dialog.Details.text = string.Format(Message, Counter);
        }
    }

    public CounterDialog (string title, string message, int initialCounter, float showAtLeastSeconds = 0f) : base (null, showAtLeastSeconds: showAtLeastSeconds) 
    // 'null' because we do NOT connect a Task, since message dialogs only rely on user interaction
    {
        this.Title = title;
        this.Message = message;
        this.Counter = initialCounter;
    }

    public override void Start ()
    {
        base.Start ();

        Dialog.Title.text = Title;
        Dialog.Title.gameObject.SetActive (true);
        Dialog.Img.gameObject.SetActive (false);
        Dialog.Details.text = string.Format(Message, Counter);
        Dialog.YesButton.gameObject.SetActive (false);
        Dialog.NoButton.gameObject.SetActive (false);

        // show the dialog:
        Dialog.Show ();
    }
    
}
