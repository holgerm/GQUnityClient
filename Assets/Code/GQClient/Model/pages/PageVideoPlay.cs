using System.Xml;
using GQ.Client.Err;
using GQ.Client.Util;

namespace GQ.Client.Model
{
    public class PageVideoPlay : Page
    {
        #region State
        public PageVideoPlay(XmlReader reader) : base(reader) { }

        public bool Controllable { get; set; }
        public string VideoFile { get; set; }
        public string VideoType { get; set; }

        public override string ToString() {
            return base.ToString() 
                       + "\n\tvideo: " + VideoFile
                       + "\n\tvidType: " + VideoType;
        }
        #endregion

        #region XML Serialization
        protected override void ReadAttributes(XmlReader reader)
        {
            base.ReadAttributes(reader);

            VideoFile = GQML.GetStringAttribute(GQML.PAGE_VIDEOPLAY_FILE, reader);
            if (VideoType != GQML.PAGE_VIDEOPLAY_VIDEOTYPE_YOUTUBE && VideoFile.HasVideoEnding())
                QuestManager.CurrentlyParsingQuest.AddMedia(VideoFile, "VideoPlay." + GQML.PAGE_VIDEOPLAY_FILE);
            else {
                Log.SignalErrorToAuthor("VideoPlay page (" + Id + ") has invalid vide url: " + VideoFile);
            }

            Controllable = GQML.GetRequiredBoolAttribute(GQML.PAGE_VIDEOPLAY_CONTROLLABLE, reader);
            VideoType = GQML.GetStringAttribute(GQML.PAGE_VIDEOPLAY_VIDEOTYPE, reader, GQML.PAGE_VIDEOPLAY_VIDEOTYPE_NORMAL);
        }
        #endregion

        #region Runtime API
        public override void Start(bool canReturnToPrevious = false)
        {
            base.Start(canReturnToPrevious);
        }
        #endregion
    }
}