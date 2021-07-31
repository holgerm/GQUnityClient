// #define DEBUG_LOG

using System.Collections.Generic;
using System.Xml;
using Code.GQClient.Model.actions;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using UnityEngine;

namespace Code.GQClient.Model.pages
{
    public class PageInteractiveSphericalImage : Page
    {
        public PageInteractiveSphericalImage(XmlReader reader) : base(reader)
        {
        }

        public string SphericalImage { get; set; }

        public List<Interaction> Interactions = new List<Interaction>();


        #region XML Serialization

        protected override void ReadAttributes(XmlReader reader)
        {
            base.ReadAttributes(reader);

            SphericalImage = GQML.GetStringAttribute(GQML.PAGE_INTERACTIVESPHERICALIMAGE_IMAGE, reader);
            QuestManager.CurrentlyParsingQuest.AddMedia(SphericalImage,
                "InteractiveSphericalImage." + GQML.PAGE_INTERACTIVESPHERICALIMAGE_IMAGE);
        }

        protected override void ReadContent(XmlReader reader)
        {
            switch (reader.LocalName)
            {
                case GQML.PAGE_INTERACTIVESPHERICALIMAGE_INTERACTION:
                    Interaction anInteraction = new Interaction(reader, this);
                    Interactions.Add(anInteraction);
                    break;
                default:
                    base.ReadContent(reader);
                    break;
            }
        }

        #endregion
    }

    public class Interaction : ITriggerContainer
    {
        #region State

        public int Id { get; }

        public int Altitude { get; }

        public int Azimuth { get; }

        public string Icon { get; }

        public string Content { get; }

        #endregion

        #region XML Serialization

        protected Trigger OnFocusTrigger = Trigger.Null;
        protected Trigger OnTapTrigger = Trigger.Null;

        public Interaction(XmlReader reader, Page parentPage)
        {
            _page = parentPage;

            GQML.AssertReaderAtStart(reader, GQML.PAGE_INTERACTIVESPHERICALIMAGE_INTERACTION);

            // Read Attributes:
            Id = GQML.GetIntAttribute(GQML.ID, reader);
            Altitude = GQML.GetIntAttribute(GQML.PAGE_INTERACTIVESPHERICALIMAGE_INTERACTION_ALTITUDE, reader);
            Azimuth = GQML.GetIntAttribute(GQML.PAGE_INTERACTIVESPHERICALIMAGE_INTERACTION_AZIMUTH, reader);
            Icon = GQML.GetStringAttribute(GQML.PAGE_INTERACTIVESPHERICALIMAGE_INTERACTION_ICON, reader);
            QuestManager.CurrentlyParsingQuest.AddMedia(
                Icon,
                "InteractiveSphericalImage#Interaction." +
                GQML.PAGE_INTERACTIVESPHERICALIMAGE_INTERACTION_ICON);


            // Read Content:
            reader.Read(); // reads the starting interaction tag.

            while (!GQML.IsReaderAtEnd(reader, GQML.PAGE_INTERACTIVESPHERICALIMAGE_INTERACTION))
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    // TODO switch (reader.LocalName) { ... Content, OnFocus, OnTap ... 
                    switch (reader.LocalName)
                    {
                        case GQML.ON_FOCUS:
                            OnFocusTrigger = new Trigger(reader);
                            OnFocusTrigger.Parent = this;
                            break;
                        case GQML.ON_TAP:
                            OnTapTrigger = new Trigger(reader);
                            OnTapTrigger.Parent = this;
                            break;
                        case GQML.PAGE_INTERACTIVESPHERICALIMAGE_INTERACTION_CONTENT:
                            Content = reader.ReadInnerXml();
                            break;
                        default:
                            break;
                    }
                }
            }

            reader.Read(); // read end element of interaction
        }

        // for direct manual creation:
        public Interaction()
        {
        }

        #endregion


        #region Runtime

        public void Tapped()
        {
            if (OnTapTrigger != Trigger.Null)
            {
                OnTapTrigger.Initiate();
            }
        }

        #endregion

        #region Null

        public static NullInteraction Null = new NullInteraction();

        public class NullInteraction : Interaction
        {
            internal NullInteraction() : base()
            {
            }
        }

        #endregion

        public Quest Quest
        {
            get => _page.Quest;
        }

        private readonly Page _page;
    }
}