using System.Xml;
using Code.GQClient.Model.actions;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;

namespace Code.GQClient.Model.pages
{
    public class PageReadNFC : Page
	{
        #region State
        public PageReadNFC(XmlReader reader) : base(reader) { }

        public string ImageUrl { get; set; }
		public string SaveToVar { get; set; }
		public string PromptText { get; set; }
		#endregion

		#region XML Serialization
		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			ImageUrl = GQML.GetStringAttribute (GQML.PAGE_READNFC_IMAGEURL, reader);
			QuestManager.CurrentlyParsingQuest.AddMedia (ImageUrl, "ReadNFC." + GQML.PAGE_READNFC_IMAGEURL);
			SaveToVar = GQML.GetStringAttribute (GQML.PAGE_READNFC_SAVE2VAR, reader);
			PromptText = GQML.GetStringAttribute (GQML.PAGE_READNFC_TEXT, reader);
		}

		protected Trigger NFCReadTrigger = Trigger.Null;

		protected override void ReadContent (XmlReader reader)
		{
			switch (reader.LocalName) {
			case GQML.ON_READ:
				NFCReadTrigger = new Trigger(reader);
				NFCReadTrigger.Parent = this;
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

        public void Read()
        {
            State = GQML.STATE_SUCCEEDED;

            if (NFCReadTrigger != Trigger.Null)
            {
                NFCReadTrigger.Initiate();
            }
        }
        #endregion
    }
}