using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
using System;
using GQ.Client.Err;
using GQ.Client.UI;

namespace GQ.Client.Model
{

    [XmlRoot(GQML.PAGE)]
    public class PageVideoPlay : Page
    {

        #region State

        public bool Controllable { get; set; }
        public string VideoFile { get; set; }
        public string VideoType { get; set; }

        #endregion


        #region XML Serialization

        protected override void ReadAttributes(XmlReader reader)
        {
            base.ReadAttributes(reader);

            VideoFile = GQML.GetStringAttribute(GQML.PAGE_VIDEOPLAY_FILE, reader);
            if (VideoFile != "")
                QuestManager.CurrentlyParsingQuest.AddMedia(VideoFile);

            Controllable = GQML.GetRequiredBoolAttribute(GQML.PAGE_VIDEOPLAY_CONTROLLABLE, reader);
            VideoType = GQML.GetStringAttribute(GQML.PAGE_VIDEOPLAY_VIDEOTYPE, reader, GQML.PAGE_VIDEOPLAY_VIDEOTYPE_NORMAL);
        }

        #endregion


        #region Runtime API

        public override void Start()
        {
            base.Start();
        }

        #endregion

    }

}
