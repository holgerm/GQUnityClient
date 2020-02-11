#define DEBUG_LOG

using System.Xml;
using Code.GQClient.Err;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Util;
using UnityEngine;

namespace Code.GQClient.Model.pages
{
    public class PageVideoPlay : Page
    {
        #region State
        public PageVideoPlay(XmlReader reader) : base(reader) { }

        public bool Controllable { get; set; }
        public bool Stream { get; set; } = true;
        public string VideoFile { get; set; }
        public string VideoType { get; set; }

        public override string ToString()
        {
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
            VideoType = GQML.GetStringAttribute(GQML.PAGE_VIDEOPLAY_VIDEOTYPE, reader, GQML.PAGE_VIDEOPLAY_VIDEOTYPE_NORMAL);
            if (VideoType != GQML.PAGE_VIDEOPLAY_VIDEOTYPE_YOUTUBE)
            {
                if (VideoFile.HasVideoEnding())
                {
#if DEBUG_LOG
                    Debug.Log("Vid-Player: VideoFile: " + VideoFile);
#endif
                    QuestManager.CurrentlyParsingQuest.AddMedia(VideoFile, "VideoPlay." + GQML.PAGE_VIDEOPLAY_FILE);
                }
                else
                {
                    Log.SignalErrorToAuthor("VideoPlay page (" + Id + ") has invalid video url: " + VideoFile + " videoType: " + VideoType);
                }
            }

            Controllable = GQML.GetRequiredBoolAttribute(GQML.PAGE_VIDEOPLAY_CONTROLLABLE, reader);
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