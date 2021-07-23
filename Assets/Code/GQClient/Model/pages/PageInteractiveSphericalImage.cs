// #define DEBUG_LOG

using System.Collections.Generic;
using System.Xml;
using Code.GQClient.Err;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Util;
using UnityEngine;

namespace Code.GQClient.Model.pages
{
    public class PageInteractiveSphericalImage : Page
    {
        public PageInteractiveSphericalImage(XmlReader reader) : base(reader) { }
        
        public string SphericalImage { get; set; }

        public List<Interaction> Interactions = new List<Interaction> ();

        
        #region XML Serialization
        protected override void ReadAttributes (XmlReader reader)
        {
            base.ReadAttributes (reader);

            SphericalImage = GQML.GetStringAttribute (GQML.PAGE_INTERACTIVESPHERICALIMAGE_IMAGE, reader);
            Debug.Log($"Loaded 360 image path: {SphericalImage}");
            QuestManager.CurrentlyParsingQuest.AddMedia (SphericalImage, "InteractiveSphericalImage." + GQML.PAGE_INTERACTIVESPHERICALIMAGE_IMAGE);
        }
        
        protected override void ReadContent (XmlReader reader)
        {
            switch (reader.LocalName) {
                case GQML.PAGE_INTERACTIVESPHERICALIMAGE_INTERACTION:
                    Interaction anInteraction = new Interaction(reader);
                    Interactions.Add (anInteraction);
                    break;
                default:
                    base.ReadContent (reader);
                    break;
            }
        }
        #endregion

    }
    
    public class Interaction
    {
        #region State
        public int Id
        {
            get;
        }

        public int Altitude
        {
            get;
        }

        public int Azimuth
        {
            get;
        }

        public string Icon
        {
            get;
        }

        public string Content
        {
            get;
        }
        #endregion

        #region XML Serialization
        public Interaction(XmlReader reader)
        {
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

            // Content: Read and implicitly proceed the reader so that this node is completely consumed:
            Content = reader.ReadInnerXml();
        }

        // for direct manual creation:
        public Interaction() { }
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
    }

}