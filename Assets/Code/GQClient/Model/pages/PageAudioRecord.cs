using System.Xml;
using Code.GQClient.Model.gqml;

namespace Code.GQClient.Model.pages
{
    public class PageAudioRecord : Page
	{
		#region State
        public PageAudioRecord(XmlReader reader) : base(reader) { }

		public string FileName { get; set; }
		
		public int MaxRecordTime { get; set; }
		
		public string PromptText { get; set; }
		#endregion

		#region XML Serialization
		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			FileName = GQML.GetStringAttribute (GQML.PAGE_AUDIORECORD_FILE, reader);
			MaxRecordTime = GQML.GetIntAttribute(GQML.PAGE_AUDIORECORD_MAXRECTIME, reader, 30);
			PromptText = GQML.GetStringAttribute (GQML.PAGE_AUDIORECORD_TASK, reader);
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