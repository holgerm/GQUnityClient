using System.Xml;

namespace GQ.Client.Model
{
    public class PageTagScanner : DecidablePage
	{
        public PageTagScanner(XmlReader reader) : base(reader) { }

        #region State
        public bool ShowTagContent { get; set; }

		public string Prompt { get; set ; }
		#endregion

		#region XML Serialization
		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			ShowTagContent = GQML.GetRequiredBoolAttribute (GQML.PAGE_TYPE_QRTAGSCANNER_SHOWTAGCONTENT, reader);

			Prompt = GQML.GetStringAttribute (GQML.PAGE_TYPE_QRTAGSCANNER_PROMPT, reader);
		}

		protected override void ReadContent (XmlReader reader)
		{
			switch (reader.LocalName) {
			case GQML.PAGE_TYPE_QRTAGSCANNER_EXPECTEDCODE:
				ExpectedCode a = new ExpectedCode(reader);
				ExpectedCodes.Add (a);
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

	public class ExpectedCode : IText
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
		public ExpectedCode(XmlReader reader)
		{
			GQML.AssertReaderAtStart (reader, GQML.PAGE_TYPE_QRTAGSCANNER_EXPECTEDCODE);

			// Read Attributes:
			Id = GQML.GetIntAttribute (GQML.ID, reader);
			// Content: Read and implicitly proceed the reader so that this node is completely consumed:
			Text = reader.ReadInnerXml ();
		}

        protected ExpectedCode() { }
        #endregion

        #region Null
        public static NullExpectedCode Null = new NullExpectedCode ();

		public class NullExpectedCode : ExpectedCode
		{

			internal NullExpectedCode ()
			{

			}
		}
		#endregion
	}
}