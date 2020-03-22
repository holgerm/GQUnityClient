using System.Xml;
using Code.GQClient.Conf;
using Code.GQClient.Model.gqml;

namespace Code.GQClient.Model.pages
{
    public class PageWebPage : Page
	{
        #region State
        public PageWebPage(XmlReader reader) : base(reader) { }

        public string File { get; set; }
		public string URL { get; set; }
        public bool ShouldEndOnLoadUrlPart { get; internal set; }
        public string AllowForwardOnlyOnLoadUrlPart { get; internal set; }
		public string EndButtonText { get; set; }
		public string ForwardButtonTextBeforeFinished { get; set; }
		#endregion

		#region XML Serialization
		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			File = GQML.GetStringAttribute (GQML.PAGE_WEBPAGE_FILE, reader);
			URL = GQML.GetStringAttribute (GQML.PAGE_WEBPAGE_URL, reader);
			EndButtonText = GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_ENDBUTTONTEXT, reader);
			ForwardButtonTextBeforeFinished = ""; // TODO GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_FORWARDBUTTONTEXTBEFOREFINISHED, reader);

			// TODO: read ShouldEndOnLoadUrlPart and EndOnLoadUrlPart
			ShouldEndOnLoadUrlPart = ConfigurationManager.Current.webpageShouldEndOnLoadUrlPart; // TODO GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_FINISHURLCONTAINS, reader);
			AllowForwardOnlyOnLoadUrlPart = ConfigurationManager.Current.webpageEndOnLoadUrlPart; // TODO GQML.GetOptionalBoolAttribute(GQML.PAGE_WEBPAGE_ENDONURLPART, reader, false);

		}
		#endregion

		#region Runtime API
		public override void Start (bool canReturnToPrevious = false)
		{
			base.Start (canReturnToPrevious);
		}
		#endregion
	}
}