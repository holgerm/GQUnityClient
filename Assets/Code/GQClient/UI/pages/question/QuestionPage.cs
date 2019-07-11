using System.Xml;

namespace GQ.Client.Model
{
    public abstract class QuestionPage : DecidablePage, Repeatable
    {
        public QuestionPage(XmlReader reader) : base(reader) { }

        public string RepeatButtonText { get; set; }

        public string RepeatText { get; set; }

        public string RepeatImage { get; set; }

        public bool RepeatUntilSuccess { get; set; }
    }
}
