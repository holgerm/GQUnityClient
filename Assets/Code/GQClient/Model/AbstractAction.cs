using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;

namespace GQ.Client.Model
{
	public abstract class AbstractAction : IAction
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

		public abstract void ReadXml (System.Xml.XmlReader reader);

		#endregion


		#region Functions

		public abstract void Execute ();

		#endregion
	}
}
