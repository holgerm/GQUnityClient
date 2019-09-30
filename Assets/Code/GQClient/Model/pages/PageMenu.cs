using System.Collections.Generic;
using System.Xml;
using GQ.Client.Util;

namespace GQ.Client.Model
{
    public class PageMenu : Page
	{
        #region State
        public PageMenu(XmlReader reader) : base(reader) { }

        public string Question { get; set ; }

		public bool Shuffle { get; set; }

		public List<MenuChoice> Choices = new List<MenuChoice> ();
		#endregion


		#region XML Serialization
		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			Question = GQML.GetStringAttribute (GQML.PAGE_QUESTION_QUESTION, reader);

			Shuffle = GQML.GetOptionalBoolAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_SHUFFLE, reader);
		}

		protected override void ReadContent (XmlReader reader)
		{
			switch (reader.LocalName) {
			case GQML.PAGE_QUESTION_ANSWER:
				MenuChoice a = new MenuChoice(reader);
				Choices.Add (a);
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

	public class MenuChoice
	{
		#region State
		public int Id {
			get;
			set;
		}

		public string Text {
			get;
			set;
		}
		#endregion


		#region XML Serialization
		public MenuChoice(XmlReader reader)
		{
			GQML.AssertReaderAtStart (reader, GQML.PAGE_QUESTION_ANSWER);

			// Content: Read and implicitly proceed the reader so that this node is completely consumed:
			Text = reader.ReadInnerXml ();
		}

        protected MenuChoice() { }
        #endregion

        #region Null
        public static NullChoice Null = new NullChoice ();

		public class NullChoice : MenuChoice
		{

			internal NullChoice ()
			{

			}
		}
		#endregion
	}
}
