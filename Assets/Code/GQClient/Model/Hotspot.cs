using GQ.Client.Model;
using System.Xml.Serialization;
using UnityEngine;
using GQ.Client.Err;
using System.Xml;
using System;
using System.Globalization;
using GQ.Client.Util;
using GQ.Client.Emulate;
using QM.Util;

namespace GQ.Client.Model
{
	
	[XmlRoot (GQML.HOTSPOT)]
	public class Hotspot : IXmlSerializable, ITriggerContainer
	{

		#region XML Parsing

		public System.Xml.Schema.XmlSchema GetSchema ()
		{
			return null;
		}

		public void WriteXml (System.Xml.XmlWriter writer)
		{
			Debug.LogWarning ("WriteXML not implemented for " + GetType ().Name);
		}

		public virtual Quest Quest {
			get {
				return Parent;
			}
		}

		/// <summary>
		/// Reader must be at the hotspot element (start). When it returns the reader is position behind the hotspot end element. 
		/// </summary>
		/// <param name="reader">Reader.</param>
		public void ReadXml (XmlReader reader)
		{
			GQML.AssertReaderAtStart (reader, GQML.HOTSPOT);

			ReadAttributes (reader);

			if (reader.IsEmptyElement) {
				// empty hotspot without events:
				reader.Read ();
				return;
			}

			// consume the Begin Action Element:
			reader.Read (); 

			XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
			xmlRootAttr.IsNullable = true;

			while (!GQML.IsReaderAtEnd (reader, GQML.HOTSPOT)) {

				if (reader.NodeType == XmlNodeType.Element)
					ReadContent (reader, xmlRootAttr);
			}

			// consume the closing action tag (if not empty page element)
			if (reader.NodeType == XmlNodeType.EndElement)
				reader.Read ();
		}

		#endregion


		#region Data

		public const double DEFAULT_RADIUS = 20.0d;
		public const string DEFAULT_NUMBER = "007";

		public int Id { get; protected set; }

		public string MarkerImageUrl { get; protected set; }

		public bool InitialActivity { get; protected set; }

		public bool Active { get; protected set; }

		public bool InitialVisibility { get; protected set; }

		public bool Visible { get; protected set; }

		public StatusValue Status { get; set; }

		public enum StatusValue {
			UNDEFINED,
			INSIDE,
			OUTSIDE
		}

		protected string LatLong { 
			set {
				string[] parts = value.Split (',');
				if (parts.Length != 2) {
					Log.SignalErrorToDeveloper (
						"Hotspot {0} in quest {1} contains bad location string '{2}'",
						Id,
						QuestManager.CurrentlyParsingQuest.Id,
						value
					);
				}
				latitude = Convert.ToDouble (parts [0], new CultureInfo ("en-US"));
				longitude = Convert.ToDouble (parts [1], new CultureInfo ("en-US"));
			} 
		}

		private double latitude;

		public double Latitude {
			get {
				return latitude;
			}
		}

		private double longitude;

		public double Longitude {
			get {
				return longitude;
			}
		}

		public double Radius { get; protected set; }

		public string Number { get; protected set; }

		public string QrCode { get; protected set; }

		public string Nfc { get; protected set; }

		public string IBeacon { get; protected set; }


		protected virtual void ReadAttributes (XmlReader reader)
		{
			// Id:
			int id;
			if (Int32.TryParse (reader.GetAttribute (GQML.HOTSPOT_ID), out id)) {
				Id = id;
			} else {
				Log.SignalErrorToDeveloper ("Id for a hotspot could not be parsed. We found: " + reader.GetAttribute (GQML.QUEST_ID));
			}

			// Marker Image:
			MarkerImageUrl = GQML.GetStringAttribute (GQML.HOTSPOT_MARKERURL, reader);
			if (MarkerImageUrl != "")
				QuestManager.CurrentlyParsingQuest.AddMedia (MarkerImageUrl);

			// InitialActivity:
			InitialActivity = GQML.GetOptionalBoolAttribute (GQML.HOTSPOT_INITIAL_ACTIVITY, reader, true);
			Active = InitialActivity;

			// InitialVisibility:
			InitialVisibility = GQML.GetOptionalBoolAttribute (GQML.HOTSPOT_INITIAL_VISIBILITY, reader, true);
			Visible = InitialVisibility;

			// LatLong: TODO parse and transform in a location type etc.
			LatLong = GQML.GetStringAttribute (GQML.HOTSPOT_LATLONG, reader);

			// Radius:
			Radius = GQML.GetDoubleAttribute (GQML.HOTSPOT_RADIUS, reader, DEFAULT_RADIUS);

			// Number:
			Number = GQML.GetStringAttribute (GQML.HOTSPOT_NUMBER, reader, DEFAULT_NUMBER);

			// QR Code:
			QrCode = GQML.GetStringAttribute (GQML.HOTSPOT_QRCODE, reader);

			// NFC:
			Nfc = GQML.GetStringAttribute (GQML.HOTSPOT_NFC, reader);

			// iBeacon:
			IBeacon = GQML.GetStringAttribute (GQML.HOTSPOT_IBEACON, reader);
		}

		#endregion


		#region Triggers

		protected Trigger EnterTrigger = Trigger.Null;
		protected Trigger LeaveTrigger = Trigger.Null;
		protected Trigger TapTrigger = Trigger.Null;

		protected virtual void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			XmlSerializer serializer;

			switch (reader.LocalName) {
			case GQML.ON_ENTER:
				xmlRootAttr.ElementName = GQML.ON_ENTER;
				serializer = new XmlSerializer (typeof(Trigger), xmlRootAttr);
				EnterTrigger = (Trigger)serializer.Deserialize (reader);
				EnterTrigger.Parent = this;
				break;
			case GQML.ON_LEAVE:
				xmlRootAttr.ElementName = GQML.ON_LEAVE;
				serializer = new XmlSerializer (typeof(Trigger), xmlRootAttr);
				LeaveTrigger = (Trigger)serializer.Deserialize (reader);
				LeaveTrigger.Parent = this;
				break;
			case GQML.ON_TAP:
				xmlRootAttr.ElementName = GQML.ON_TAP;
				serializer = new XmlSerializer (typeof(Trigger), xmlRootAttr);
				TapTrigger = (Trigger)serializer.Deserialize (reader);
				TapTrigger.Parent = this;
				break;
			// UNKOWN CASE:
			default:
				Log.WarnDeveloper ("Hotspot has additional unknown {0} element. (Ignored)", reader.LocalName);
				reader.Skip ();
				break;
			}
		}

		#endregion


		#region State

		public Hotspot ()
		{
			Status = StatusValue.UNDEFINED;

			result = null;

			EnterTrigger = Trigger.Null;
			LeaveTrigger = Trigger.Null;
			TapTrigger = Trigger.Null;
		}

		public string result;

		public string Result {
			get {
				return result;
			}
		}

		public virtual Quest Parent { get; set; }

		#endregion


		#region Runtime API

		public bool InsideRadius(LocationInfoExt loc) {
			return LocationSensor.distance (loc.latitude, loc.longitude, Latitude, Longitude) <= Radius - 0.001d;
		}

		public bool OutsideRadius(LocationInfoExt loc) {
			return LocationSensor.distance (loc.latitude, loc.longitude, Latitude, Longitude) > Radius + 0.001d;
		}

		public virtual void Enter ()
		{
			Status = Hotspot.StatusValue.INSIDE;
			EnterTrigger.Initiate ();
		}

		public virtual void Leave ()
		{
			Status = Hotspot.StatusValue.OUTSIDE;
			LeaveTrigger.Initiate ();
		}

		public virtual void Tap ()
		{
			if (Base.Instance.EmulationMode) {
				EmuHotspotDialog.CreateAndShow (EnterTrigger, LeaveTrigger, TapTrigger);
			} else {
				TapTrigger.Initiate ();
			}
		}

		#endregion

	}

}