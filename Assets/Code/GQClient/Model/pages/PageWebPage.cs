using System.Xml;

namespace GQ.Client.Model
{
    public class PageWebPage : Page
	{
        #region State
        public PageWebPage(XmlReader reader) : base(reader) { }

        public string File { get; set; }
		public string URL { get; set; }
		#endregion

		#region XML Serialization
		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			File = GQML.GetStringAttribute (GQML.PAGE_WEBPAGE_FILE, reader);
			URL = GQML.GetStringAttribute (GQML.PAGE_WEBPAGE_URL, reader);
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