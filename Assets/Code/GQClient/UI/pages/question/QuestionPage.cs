using System.Xml;
using Code.GQClient.Model.pages;

namespace Code.GQClient.UI.pages.question
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
