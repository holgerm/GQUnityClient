using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;
using GQ.Client.Err;
using System;

namespace GQ.Client.Model
{
	[XmlRoot (GQML.RULE)]
	public class Rule : IXmlSerializable
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

		/// <summary>
		/// The contained actions.
		/// </summary>
		protected List<IAction> containedActions = new List<IAction> ();

		/// <summary>
		/// Reads the xml within a given rule element until it finds an action element. 
		/// It then delegates further parsing to the specific action subclass depending on the actions type attribute.
		/// </summary>
		public void ReadXml (System.Xml.XmlReader reader)
		{
			XmlSerializer serializer;

			string ruleName = reader.LocalName;

			reader.MoveToContent ();

			XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
			xmlRootAttr.IsNullable = true;
			xmlRootAttr.ElementName = GQML.ACTION;

			bool currentNodeStillToBeConsumed = false;

			while (currentNodeStillToBeConsumed || reader.Read ()) {
				currentNodeStillToBeConsumed = false;

				// if we reach the end of this condition element we are ready to leave this method.
				if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals (ruleName)) {
					break;
				}

				if (reader.NodeType == XmlNodeType.Element && reader.LocalName != GQML.ACTION) {
					Log.SignalErrorToDeveloper ("Unexcpected xml {0} named {1} inside rule found.", reader.NodeType.ToString (), reader.LocalName);
					reader.Skip ();
					currentNodeStillToBeConsumed = true;
					continue;
				}

				// now the reader is at an action element:
				string actionName = reader.GetAttribute (GQML.ACTION_TYPE);
				if (actionName == null) {
					Log.SignalErrorToDeveloper ("Action without type attribute found.");
					reader.Skip ();
					currentNodeStillToBeConsumed = true;
					continue;
				}

				// Determine the full name of the according action sub type (e.g. GQ.Client.Model.XML.SetVariableAction) 
				//		where SetVariable is taken form ath type attribute of the xml action element.
				string ruleTypeFullName = this.GetType ().FullName;
				int lastDotIndex = ruleTypeFullName.LastIndexOf (".");
				string modelNamespace = ruleTypeFullName.Substring (0, lastDotIndex);
				Type actionType = Type.GetType (string.Format ("{0}.{1}Action", modelNamespace, actionName));

				if (actionType == null) {
					Log.SignalErrorToDeveloper ("No Implementation for Action Type {0} found.", actionName);
					reader.Skip ();
					currentNodeStillToBeConsumed = true;
					continue;
				}

				serializer = new XmlSerializer (actionType, xmlRootAttr);
				containedActions.Add ((IAction)serializer.Deserialize (reader));
				currentNodeStillToBeConsumed = true;
			}
		}

		#endregion


		#region Functions

		public void Apply ()
		{
			foreach (var action in containedActions) {
				action.Execute ();
			}
		}

		#endregion
	
	}
}
