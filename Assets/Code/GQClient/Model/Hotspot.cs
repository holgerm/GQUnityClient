using GQ.Client.Model;
using System.Xml.Serialization;

[XmlRoot (GQML.HOTSPOT)]
public class Hotspot
{

	#region Structure

	[XmlAttribute ("id")]
	public int Id { get; protected set; }

	[XmlAttribute ("iBeacon")]
	public int IBeacon { get; protected set; }

	[XmlAttribute ("number")]
	public int Number { get; protected set; }

	[XmlAttribute ("qrcode")]
	public int QrCode { get; protected set; }

	[XmlAttribute ("nfc")]
	public int Nfc { get; protected set; }

	[XmlAttribute ("initialActivity")]
	public bool InitialActivity { get; protected set; }

	[XmlAttribute ("initialVisibility")]
	public bool InitialVisibility { get; protected set; }

	[XmlAttribute ("img")]
	public string ImageURI { get; protected set; }

	[XmlAttribute ("radius")]
	public double Radius { get; protected set; }

	[XmlAttribute ("latlong")]
	public string LatLon { get; protected set; }

	[XmlElement ("onEnter")]
	public Trigger EnterTrigger { get; protected set; }

	[XmlElement ("onLeave")]
	public Trigger LeaveTrigger { get; protected set; }

	[XmlElement ("onTap")]
	public Trigger TapTrigger { get; protected set; }

	public Hotspot ()
	{
		EnterTrigger = Trigger.Null;
		LeaveTrigger = Trigger.Null;
		TapTrigger = Trigger.Null;
	}

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
