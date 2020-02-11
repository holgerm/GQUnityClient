using System.Xml;

namespace GQ.Client.Model
{
    public class PageStartAndExitScreen : Page
	{
        #region State
        public PageStartAndExitScreen(XmlReader reader) : base(reader) { }

        // TODO: Add endbuttontext to this page type in Editor
        public string EndButtonText { get; set ; }

		public string ImageUrl { get; set; }

		public bool Loop { get; set; }

		public int Duration { get; set; }

		public int Fps { get; set; }
		#endregion

		#region Runtime API
		public override void Start (bool canReturnToPrevious = false)
		{
			base.Start (canReturnToPrevious);
		}
		#endregion

		#region XML Serialization
		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			EndButtonText = ">";

			ImageUrl = GQML.GetStringAttribute (GQML.PAGE_STARTANDEXITSCREEN_IMAGEURL, reader);
			if (ImageUrl != "")
				QuestManager.CurrentlyParsingQuest.AddMedia (ImageUrl, "StartAndExitScreen." + GQML.PAGE_STARTANDEXITSCREEN_IMAGEURL);

			Loop = GQML.GetOptionalBoolAttribute (GQML.PAGE_STARTANDEXITSCREEN_LOOP, reader);

			Duration = GQML.GetIntAttribute (GQML.PAGE_STARTANDEXITSCREEN_DURATION, reader) / 1000;

			Fps = GQML.GetIntAttribute (GQML.PAGE_STARTANDEXITSCREEN_FPS, reader);
		}

        protected Trigger TapTrigger = Trigger.Null;
        protected override void ReadContent(XmlReader reader)
        {
            switch (reader.LocalName)
            {
                case GQML.ON_TAP:
                    TapTrigger = new Trigger(reader);
                    TapTrigger.Parent = this;
                    break;
                default:
                    base.ReadContent(reader);
                    break;
            }
        }

        internal void Tap()
        {
            TapTrigger.Initiate();
        }
        #endregion
    }
}