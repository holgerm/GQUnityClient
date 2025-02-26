﻿using System.Xml;
using Code.GQClient.Model.gqml;
using Code.GQClient.Util.input;

namespace Code.GQClient.Model.pages
{
    public class PageNavigation : Page
    {
        #region State
        public bool mapOption { get; set; }
        public int initialZoomLevel { get; set; }

        public bool listOption { get; set; }

        public bool qrOption { get; set; }
        public string qrText { get; set; }
        public string qrNotFoundText { get; set; }

        public bool nfcOption { get; set; }
        public string nfcText { get; set; }
        public string nfcNotFoundText { get; set; }

        public bool numberOption { get; set; }
        public string numberText { get; set; }
        public string numberNotFoundText { get; set; }

        public bool iBeaconOption { get; set; }
        public string iBeaconText { get; set; }
        public string iBeaconNotFoundText { get; set; }
        #endregion

        #region XML Serialization
        public PageNavigation(XmlReader reader) : base(reader) { }

        protected override void ReadAttributes(XmlReader reader)
        {
            base.ReadAttributes(reader);

            mapOption = GQML.GetOptionalBoolAttribute(GQML.PAGE_NAVIGATION_OPTION_MAP, reader);
            initialZoomLevel = GQML.GetIntAttribute(GQML.PAGE_NAVIGATION_MAP_ZOOMLEVEL, reader);

            listOption = false; // GQML.GetRequiredBoolAttribute (GQML.PAGE_NAVIGATION_OPTION_LIST, reader);

            qrOption = GQML.GetOptionalBoolAttribute(GQML.PAGE_NAVIGATION_OPTION_QR, reader);
            qrText = GQML.GetStringAttribute(GQML.PAGE_NAVIGATION_TEXT_QR, reader);
            qrNotFoundText = GQML.GetStringAttribute(GQML.PAGE_NAVIGATION_TEXT_QR_NOTFOUND, reader);

            nfcOption = GQML.GetOptionalBoolAttribute(GQML.PAGE_NAVIGATION_OPTION_NFC, reader);
            nfcText = GQML.GetStringAttribute(GQML.PAGE_NAVIGATION_TEXT_NFC, reader);
            nfcNotFoundText = GQML.GetStringAttribute(GQML.PAGE_NAVIGATION_TEXT_NFC_NOTFOUND, reader);

            numberOption = GQML.GetOptionalBoolAttribute(GQML.PAGE_NAVIGATION_OPTION_NUMBER, reader);
            numberText = GQML.GetStringAttribute(GQML.PAGE_NAVIGATION_TEXT_NUMBER, reader);
            numberNotFoundText = GQML.GetStringAttribute(GQML.PAGE_NAVIGATION_TEXT_NUMBER_NOTFOUND, reader);

            iBeaconOption = GQML.GetOptionalBoolAttribute(GQML.PAGE_NAVIGATION_OPTION_IBEACON, reader);
            iBeaconText = GQML.GetStringAttribute(GQML.PAGE_NAVIGATION_TEXT_IBEACON, reader);
            iBeaconNotFoundText = GQML.GetStringAttribute(GQML.PAGE_NAVIGATION_TEXT_IBEACON_NOTFOUND, reader);
        }
        #endregion


        #region Runtime API
        private int testCount = 0;
        
        public override void Start(bool canReturnToPrevious = false) {
            base.Start(canReturnToPrevious);
        }

        public override void CleanUp()
        {
            base.CleanUp();
            LocationSensor.Instance.OnLocationUpdate -= Quest.UpdateHotspotMarkers;
        }
        #endregion
    }
}
