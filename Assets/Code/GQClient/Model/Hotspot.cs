using GQ.Client.Model;
using System.Xml.Serialization;
using UnityEngine;
using GQ.Client.Err;
using System.Xml;
using System;

namespace GQ.Client.Model
{
	
	[XmlRoot (GQML.HOTSPOT)]
	public class Hotspot : IXmlSerializable, ITriggerContainer
	{

		#region Structure

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
				reader.Read ();
				Log.WarnAuthor ("Empty xml hotspot element found.");
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

		public bool InitialVisibility { get; protected set; }

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
				latitude = Convert.ToDouble (parts [0]);
				longitude = Convert.ToDouble (parts [1]);
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

			// InitialVisibility:
			InitialVisibility = GQML.GetOptionalBoolAttribute (GQML.HOTSPOT_INITIAL_VISIBILITY, reader, true);

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
			State = GQML.STATE_NEW;

			result = null;

			EnterTrigger = Trigger.Null;
			LeaveTrigger = Trigger.Null;
			TapTrigger = Trigger.Null;
		}

		private string state;

		public string State {
			get;
			protected set;
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

		public virtual void Enter ()
		{
			EnterTrigger.Initiate ();
		}

		public virtual void Leave ()
		{
			LeaveTrigger.Initiate ();
		}

		public virtual void Tap ()
		{
			TapTrigger.Initiate ();
		}

		#endregion

	}

}