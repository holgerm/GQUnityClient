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
        public string AllowLeaveOnUrlContains { get; internal set; }
        public string AllowLeaveOnUrlDoesNotContain { get; internal set; }
        
        public string AllowLeaveOnHtmlContains { get; internal set; }
        
        public string AllowLeaveOnHtmlDoesNotContain { get; internal set; }
        
        public bool LeaveOnAllow { get; internal set; }
		
        public string EndButtonText { get; set; }
        
        public string EndButtonTextWhenClosed { get; set; }
		public string ForwardButtonTextBeforeFinished { get; set; }
		#endregion

		#region XML Serialization
		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			File = GQML.GetStringAttribute (GQML.PAGE_WEBPAGE_FILE, reader);
			URL = GQML.GetStringAttribute (GQML.PAGE_WEBPAGE_URL, reader);
			EndButtonText = GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_ENDBUTTONTEXT, reader);
			EndButtonTextWhenClosed  = GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_ENDBUTTONTEXT_CLOSED, reader);

			AllowLeaveOnUrlContains = GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_ALLOW_LEAVE_ON_URL_CONTAINS, reader);
			AllowLeaveOnUrlDoesNotContain = GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_ALLOW_LEAVE_ON_URL_DOESNOTCONTAIN, reader);
			AllowLeaveOnHtmlContains = GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_ALLOW_LEAVE_ON_HTML_CONTAINS, reader);
			AllowLeaveOnHtmlDoesNotContain = GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_ALLOW_LEAVE_ON_HTML_DOESNOTCONTAIN, reader);
			LeaveOnAllow = GQML.GetOptionalBoolAttribute(GQML.PAGE_WEBPAGE_LEAVE_ON_ALLOW, reader);
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