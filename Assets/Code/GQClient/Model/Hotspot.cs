using System;
using System.Globalization;
using System.Xml;
using Code.GQClient.Emulate;
using Code.GQClient.Err;
using Code.GQClient.Model.actions;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.UI.author;
using Code.GQClient.Util.input;
using Code.QM.Util;

namespace Code.GQClient.Model
{
    public class Hotspot : ITriggerContainer
    {

        #region XML Parsing
        public virtual Quest Quest
        {
            get
            {
                return Parent;
            }
        }

        /// <summary>
        /// Reader must be at the hotspot element (start). When it returns the reader is position behind the hotspot end element. 
        /// </summary>
        /// <param name="reader">Reader.</param>
        public Hotspot(XmlReader reader)
        {
            Status = StatusValue.UNDEFINED;

            EnterTrigger = Trigger.Null;
            LeaveTrigger = Trigger.Null;
            TapTrigger = Trigger.Null;

            GQML.AssertReaderAtStart(reader, GQML.HOTSPOT);

            ReadAttributes(reader);

            if (reader.IsEmptyElement)
            {
                // empty hotspot without events:
                reader.Read();
                return;
            }

            // consume the Begin Action Element:
            reader.Read();

            while (!GQML.IsReaderAtEnd(reader, GQML.HOTSPOT))
            {

                if (reader.NodeType == XmlNodeType.Element)
                    ReadContent(reader);
            }

            // consume the closing action tag (if not empty page element)
            if (reader.NodeType == XmlNodeType.EndElement)
                reader.Read();
        }
        #endregion

        #region Data
        public const double DEFAULT_RADIUS = 20.0d;
        public const string DEFAULT_NUMBER = "007";

        public int Id { get; protected set; }

        public string MarkerImageUrl { get; protected set; }

        public bool InitialActivity { get; protected set; }

        private bool _active;
        public bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                InvokeHotspotChanged(this);
            }
        }

        public bool InitialVisibility { get; protected set; }

        private bool _visible;
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
                InvokeHotspotChanged(this);
            }
        }

        public StatusValue Status { get; set; }

        public enum StatusValue
        {
            UNDEFINED,
            INSIDE,
            OUTSIDE
        }

        protected string LatLong
        {
            set
            {
                string[] parts = value.Split(',');
                if (parts.Length != 2)
                {
                    Log.SignalErrorToDeveloper(
                        "Hotspot {0} in quest {1} contains bad location string '{2}'",
                        Id,
                        QuestManager.CurrentlyParsingQuest.Id,
                        value
                    );
                }
                latitude = Convert.ToDouble(parts[0], new CultureInfo("en-US"));
                longitude = Convert.ToDouble(parts[1], new CultureInfo("en-US"));
            }
        }

        private double latitude;

        public double Latitude
        {
            get
            {
                return latitude;
            }
        }

        private double longitude;

        public double Longitude
        {
            get
            {
                return longitude;
            }
        }

        public double Radius { get; protected set; }

        public string Number { get; protected set; }

        public string QrCode { get; protected set; }

        public string Nfc { get; protected set; }

        public string IBeacon { get; protected set; }


        protected virtual void ReadAttributes(XmlReader reader)
        {
            // Id:
            int id;
            if (Int32.TryParse(reader.GetAttribute(GQML.HOTSPOT_ID), out id))
            {
                Id = id;
            }
            else
            {
                Log.SignalErrorToDeveloper(
                    "Id for a hotspot could not be parsed. We found: {0}, line {1} pos {2}",
                    reader.GetAttribute(GQML.ID),
                    ((IXmlLineInfo)reader).LineNumber,
                    ((IXmlLineInfo)reader).LinePosition);
            }

            // Marker Image:
            MarkerImageUrl = GQML.GetStringAttribute(GQML.HOTSPOT_MARKERURL, reader);
            QuestManager.CurrentlyParsingQuest.AddMedia(MarkerImageUrl, "Hotspot." + GQML.HOTSPOT_MARKERURL);

            // InitialActivity:
            InitialActivity = GQML.GetOptionalBoolAttribute(GQML.HOTSPOT_INITIAL_ACTIVITY, reader, true);
            Active = InitialActivity;

            // InitialVisibility:
            InitialVisibility = GQML.GetOptionalBoolAttribute(GQML.HOTSPOT_INITIAL_VISIBILITY, reader, true);
            Visible = InitialVisibility;

            // LatLong: TODO parse and transform in a location type etc.
            LatLong = GQML.GetStringAttribute(GQML.HOTSPOT_LATLONG, reader);

            // Radius:
            Radius = GQML.GetDoubleAttribute(GQML.HOTSPOT_RADIUS, reader, DEFAULT_RADIUS);

            // Number:
            Number = GQML.GetStringAttribute(GQML.HOTSPOT_NUMBER, reader, DEFAULT_NUMBER);

            // QR Code:
            QrCode = GQML.GetStringAttribute(GQML.HOTSPOT_QRCODE, reader);

            // NFC:
            Nfc = GQML.GetStringAttribute(GQML.HOTSPOT_NFC, reader);

            // iBeacon:
            IBeacon = GQML.GetStringAttribute(GQML.HOTSPOT_IBEACON, reader);
        }
        #endregion


        #region Triggers
        protected Trigger EnterTrigger = Trigger.Null;
        protected Trigger LeaveTrigger = Trigger.Null;
        protected Trigger TapTrigger = Trigger.Null;

        protected virtual void ReadContent(XmlReader reader)
        {
            switch (reader.LocalName)
            {
                case GQML.ON_ENTER:
                    EnterTrigger = new Trigger(reader);
                    EnterTrigger.Parent = this;
                    break;
                case GQML.ON_LEAVE:
                    LeaveTrigger = new Trigger(reader);
                    LeaveTrigger.Parent = this;
                    break;
                case GQML.ON_TAP:
                    TapTrigger = new Trigger(reader);
                    TapTrigger.Parent = this;
                    break;
                // UNKOWN CASE:
                default:
                    Log.WarnDeveloper("Hotspot has additional unknown {0} element. (Ignored) line {1} position {2}",
                        reader.LocalName,
                        ((IXmlLineInfo)reader).LineNumber,
                        ((IXmlLineInfo)reader).LinePosition);
                    reader.Skip();
                    break;
            }
        }
        #endregion

        #region State
        public virtual Quest Parent { get; set; }
        #endregion


        #region Runtime API
        public bool InsideRadius(LocationInfoExt loc)
        {
            return LocationSensor.distance(loc.latitude, loc.longitude, Latitude, Longitude) <= Radius - 0.001d;
        }

        public bool OutsideRadius(LocationInfoExt loc)
        {
            return LocationSensor.distance(loc.latitude, loc.longitude, Latitude, Longitude) > Radius + 0.001d;
        }

        public virtual void Enter()
        {
            Status = Hotspot.StatusValue.INSIDE;
            EnterTrigger.Initiate();
        }

        public virtual void Leave()
        {
            Status = Hotspot.StatusValue.OUTSIDE;
            LeaveTrigger.Initiate();
        }

        public virtual void Tap()
        {
            if (Author.LoggedIn)
            {
                EmuHotspotDialog.CreateAndShow(EnterTrigger, LeaveTrigger, TapTrigger);
            }
            else
            {
                TapTrigger.Initiate();
            }
        }
        #endregion


        #region Events
        public delegate void HotspotChangeCallBack(Hotspot h);

        public event HotspotChangeCallBack HotspotChanged;

        protected void InvokeHotspotChanged(Hotspot h)
        {
            if (HotspotChanged != null)
                HotspotChanged(h);
        }
        #endregion

    }

}