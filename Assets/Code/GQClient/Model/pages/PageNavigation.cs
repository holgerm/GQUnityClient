using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
using GQ.Client.Util;

namespace GQ.Client.Model
{
    [XmlRoot(GQML.PAGE)]
    public class PageNavigation : Page
    {

        #region State
        public bool mapOption { get; set; }

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
        protected override void ReadAttributes(XmlReader reader)
        {
            base.ReadAttributes(reader);

            mapOption = GQML.GetOptionalBoolAttribute(GQML.PAGE_NAVIGATION_OPTION_MAP, reader);

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
        public override void Start() {
            base.Start();
            LocationSensor.Instance.OnLocationUpdate += Quest.UpdateHotspotMarkers;
        }

        public override void End()
        {
            base.End();
            LocationSensor.Instance.OnLocationUpdate -= Quest.UpdateHotspotMarkers;
        }
        #endregion
    }
}
