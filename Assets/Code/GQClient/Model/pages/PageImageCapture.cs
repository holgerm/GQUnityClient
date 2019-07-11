using System.Xml;

namespace GQ.Client.Model
{
    public class PageImageCapture : Page {

        #region State
        public PageImageCapture(XmlReader reader) : base(reader) { }

        public string ButtonText { get; set ; }

		public string File { get; set ; }

		public string Task { get; set; }

        public bool PreferFrontCam { get; set; }
        #endregion

        #region XML Serialization
        protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			ButtonText = GQML.GetStringAttribute (GQML.PAGE_IMAGECAPTURE_BUTTONTEXT, reader);

			File = GQML.GetStringAttribute (GQML.PAGE_IMAGECAPTURE_FILE, reader);

			Task = GQML.GetStringAttribute (GQML.PAGE_IMAGECAPTURE_TASK, reader);

            PreferFrontCam = GQML.GetOptionalBoolAttribute(GQML.PAGE_IMAGECAPTURE_PREFER_FRONT_CAM, reader);
        }
		#endregion
	}
}