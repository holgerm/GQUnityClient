using System;
using System.Collections.Generic;
using System.Xml;
using Code.GQClient.Err;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.UI.pages.question;
using Code.GQClient.Util;

namespace Code.GQClient.Model.pages
{
    public class PageTextQuestion : QuestionPage
    {
        public PageTextQuestion(XmlReader reader) : base(reader) { }

        #region State
        public string Question { get; set; }

        public string Prompt { get; set; }

        public string BackGroundImage { get; set; }

        private List<TQAnswer> Answers = new List<TQAnswer>();
        #endregion

        #region XML Serialization
        protected override void ReadAttributes(XmlReader reader)
        {
            base.ReadAttributes(reader);

            RepeatButtonText = GQML.GetStringAttribute(GQML.PAGE_QUESTION_LOOP_BUTTON_TEXT, reader, GQML.PAGE_QUESTION_LOOP_BUTTON_TEXT_DEFAULT);

            RepeatText = GQML.GetStringAttribute(GQML.PAGE_QUESTION_LOOP_TEXT, reader);

            RepeatImage = GQML.GetStringAttribute(GQML.PAGE_QUESTION_LOOP_IMAGE, reader);
            if (RepeatImage != "")
                QuestManager.CurrentlyParsingQuest.AddMedia(RepeatImage, "TextQuestion." + GQML.PAGE_QUESTION_LOOP_IMAGE);

            RepeatUntilSuccess = GQML.GetOptionalBoolAttribute(GQML.PAGE_QUESTION_LOOP_UNTIL_SUCCESS, reader);

            Question = GQML.GetStringAttribute(GQML.PAGE_QUESTION_QUESTION, reader);

            Prompt = GQML.GetStringAttribute(GQML.PAGE_TEXTQUESTION_PROMPT, reader);

            BackGroundImage = GQML.GetStringAttribute(GQML.PAGE_QUESTION_BACKGROUND_IMAGE, reader);
            if (BackGroundImage != "")
                QuestManager.CurrentlyParsingQuest.AddMedia(BackGroundImage, "TextQuestion." + GQML.PAGE_QUESTION_BACKGROUND_IMAGE);
        }

        protected override void ReadContent(XmlReader reader)
        {
            switch (reader.LocalName)
            {
                case GQML.PAGE_QUESTION_ANSWER:
                    TQAnswer a = new TQAnswer(reader);
                    Answers.Add(a);
                    break;
                default:
                    base.ReadContent(reader);
                    break;
            }
        }
        #endregion


        #region Runtime API
        public override void Start(bool canReturnToPrevious = false)
        {
            base.Start(canReturnToPrevious);
        }

        public override bool AnswerCorrect(string input)
        {
            List<string> expectedTexts = Answers.ConvertAll(answer => answer.Text.Trim().MakeReplacements());

            return AcceptInputText(input, expectedTexts);
        }
        #endregion

    }

    public class TQAnswer
    {
        #region State
        public int Id
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public TQAnswer(XmlReader reader)
        {
            GQML.AssertReaderAtStart(reader, GQML.PAGE_QUESTION_ANSWER);

            // Read Attributes:
            Id = GQML.GetIntAttribute(GQML.ID, reader);

            // Content: Read and implicitly proceed the reader so that this node is completely consumed:
            Text = reader.ReadInnerXml();
        }

        protected TQAnswer() { }
        #endregion

        #region Null
        public static NullAnswer Null = new NullAnswer();

        public class NullAnswer : TQAnswer
        {
            internal NullAnswer() { }
        }
        #endregion
    }
}