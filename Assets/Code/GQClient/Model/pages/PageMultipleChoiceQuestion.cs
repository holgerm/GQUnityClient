using System.Collections.Generic;
using System.Xml;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.UI.pages.question;

namespace Code.GQClient.Model.pages
{
    public class PageMultipleChoiceQuestion : QuestionPage
	{
        public PageMultipleChoiceQuestion(XmlReader reader) : base(reader) { }

        #region State
        public string Question { get; set ; }

		public bool ShowOnlyImages { get; set; }

		public bool Shuffle { get; set; }

		public string BackGroundImage { get; set; }

		public List<MCQAnswer> Answers = new List<MCQAnswer> ();
		#endregion


		#region XML Serialization
		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			RepeatButtonText = GQML.GetStringAttribute (GQML.PAGE_QUESTION_LOOP_BUTTON_TEXT, reader, GQML.PAGE_QUESTION_LOOP_BUTTON_TEXT_DEFAULT);

			RepeatText = GQML.GetStringAttribute (GQML.PAGE_QUESTION_LOOP_TEXT, reader);

			RepeatImage = GQML.GetStringAttribute (GQML.PAGE_QUESTION_LOOP_IMAGE, reader);
			QuestManager.CurrentlyParsingQuest.AddMedia (RepeatImage, "MultipleChoiceQuestion." + GQML.PAGE_QUESTION_LOOP_IMAGE);

			RepeatUntilSuccess = GQML.GetOptionalBoolAttribute (GQML.PAGE_QUESTION_LOOP_UNTIL_SUCCESS, reader);

			Question = GQML.GetStringAttribute (GQML.PAGE_QUESTION_QUESTION, reader);

			ShowOnlyImages = GQML.GetOptionalBoolAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_SHOW_ONLY_IMAGES, reader);

			Shuffle = GQML.GetOptionalBoolAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_SHUFFLE, reader);

			BackGroundImage = GQML.GetStringAttribute (GQML.PAGE_QUESTION_BACKGROUND_IMAGE, reader);
			QuestManager.CurrentlyParsingQuest.AddMedia (BackGroundImage, "MultipleChoiceQuestion." + GQML.PAGE_QUESTION_BACKGROUND_IMAGE);
		}

		protected override void ReadContent (XmlReader reader)
		{
			switch (reader.LocalName) {
			case GQML.PAGE_QUESTION_ANSWER:
				MCQAnswer a = new MCQAnswer(reader);
				Answers.Add (a);
				break;
			default:
				base.ReadContent (reader);
				break;
			}
		}
		#endregion


		#region Runtime API
		public override void Start (bool canReturnToPrevious = false)
		{
			base.Start (canReturnToPrevious);
		}
		#endregion
	}

	public class MCQAnswer
	{
		#region State
		public int Id {
			get;
			set;
		}

		public bool Correct {
			get;
			set;
		}

		public string Text {
			get;
			set;
		}


		public string Image {
			get;
			set;
		}
		#endregion

		#region XML Serialization
		public MCQAnswer(XmlReader reader)
		{
			GQML.AssertReaderAtStart (reader, GQML.PAGE_QUESTION_ANSWER);

			// Read Attributes:
			Id = GQML.GetIntAttribute (GQML.ID, reader);
			Correct = GQML.GetRequiredBoolAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_ANSWER_CORRECT, reader);
			Image = GQML.GetStringAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_ANSWER_IMAGE, reader);
			QuestManager.CurrentlyParsingQuest.AddMedia (Image, "MultipleChoiceQuestion#Answer." + GQML.PAGE_MULTIPLECHOICEQUESTION_ANSWER_IMAGE);

			// Content: Read and implicitly proceed the reader so that this node is completely consumed:
			Text = reader.ReadInnerXml ();
		}

        protected MCQAnswer() { }
        #endregion

        #region Null
        public static NullAnswer Null = new NullAnswer ();

		public class NullAnswer : MCQAnswer
		{

			internal NullAnswer () : base()
			{

			}
		}
		#endregion
	}
}