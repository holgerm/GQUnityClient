using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;
using GQ.Client.Err;

namespace GQ.Client.Model
{
	public class Trigger : IXmlSerializable
	{

		#region XML Serialization

		public System.Xml.Schema.XmlSchema GetSchema ()
		{
			return null;
		}

		public void WriteXml (System.Xml.XmlWriter writer)
		{
			Debug.LogWarning ("WriteXML not implemented for " + GetType ().Name);
		}

		/// <summary>
		/// The contained actions.
		/// </summary>
		protected List<Rule> containedRules = new List<Rule> ();

		/// <summary>
		/// Reads the xml within a given rule element until it finds an action element. 
		/// It then delegates further parsing to the specific action subclass depending on the actions type attribute.
		/// </summary>
		public void ReadXml (System.Xml.XmlReader reader)
		{
			if (reader.IsEmptyElement) {
				reader.Read ();
				return;
			}

			if (reader.NodeType != XmlNodeType.Element || !isTriggerType (reader.LocalName)) {
				// skip this unexpected inner node
				Log.SignalErrorToDeveloper ("Unexpected xml {0} {1} found in expression list.", reader.NodeType, reader.LocalName);
				reader.Read ();
				return;
			}

			string triggerName = reader.LocalName;

			XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
			xmlRootAttr.IsNullable = true;
			XmlSerializer serializer;

			// consume starting Trigger element:
			reader.Read ();

			while (!GQML.IsReaderAtEnd (reader, triggerName)) {

				if (reader.NodeType != XmlNodeType.Element)
					continue;

				if (GQML.IsReaderAtStart (reader, GQML.RULE)) {
					xmlRootAttr.ElementName = GQML.RULE;
					serializer = new XmlSerializer (typeof(Rule), xmlRootAttr);
				} else if (reader.NodeType == XmlNodeType.None) {
					Log.WarnDeveloper ("Trigger {0} has incorrect xml.", triggerName);
					return;
				}
			}

			GQML.AssertReaderAtEnd (reader, triggerName);
			reader.Read ();
		}

		private static List<string> triggerNodeNames = 
			new List<string> (
				new string[] { GQML.ON_START, GQML.ON_SUCCESS, GQML.ON_FAIL, GQML.ON_END, GQML.ON_ENTER, GQML.ON_LEAVE, GQML.ON_TAP });


		internal static bool isTriggerType (string xmlTriggerCandidate)
		{
			return triggerNodeNames.Contains (xmlTriggerCandidate);
		}

		#endregion


		#region Functions

		public virtual void Initiate ()
		{
			Debug.Log ("Trigger.Perform: #" + containedRules.Count);
			foreach (Rule rule in containedRules) {
				rule.Apply ();
			}
		}

		#endregion


		#region Null

		public static readonly Trigger Null = new NullTrigger ();

		private class NullTrigger : Trigger
		{

			internal NullTrigger ()
			{
			}

			public override void Initiate ()
			{
				
			}
		}

		#endregion

	}
}
