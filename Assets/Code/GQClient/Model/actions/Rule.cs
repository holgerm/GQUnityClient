using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;
using GQ.Client.Err;
using System;
using GQ.Client.Util;

namespace GQ.Client.Model
{
	public class Rule : ActionList, IXmlSerializable
	{

		#region Structure

		public System.Xml.Schema.XmlSchema GetSchema ()
		{
			return null;
		}

		public void WriteXml (System.Xml.XmlWriter writer)
		{
			Log.SignalErrorToDeveloper ("WriteXML not implemented for " + GetType ().Name);
		}

		/// <summary>
		/// Reads the xml within a given rule element until it finds an action element. 
		/// It then delegates further parsing to the specific action subclass depending on the actions type attribute.
		/// </summary>
		public void ReadXml (System.Xml.XmlReader reader)
		{
			GQML.AssertReaderAtStart (reader, GQML.RULE);

			XmlSerializer serializer;

			string ruleName = reader.LocalName;

			XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
			xmlRootAttr.IsNullable = true;
			xmlRootAttr.ElementName = GQML.ACTION;

			if (reader.IsEmptyElement) {
				reader.Read ();
				return;
			}

			// consume the starting rule element:
			reader.Read ();

			while (!GQML.IsReaderAtEnd (reader, GQML.RULE)) {
				
				if (GQML.IsReaderAtStart (reader, GQML.ACTION)) {
					string actionName = reader.GetAttribute (GQML.ACTION_TYPE);
					actionName = TextHelper.FirstLetterToUpper (actionName);
					if (actionName == null) {
						Log.SignalErrorToDeveloper ("Action without type attribute found.");
						reader.Skip ();
						continue;
					}

					// Determine the full name of the according action sub type (e.g. GQ.Client.Model.XML.SetVariableAction) 
					//		where SetVariable is taken form ath type attribute of the xml action element.
					string ruleTypeFullName = this.GetType ().FullName;
					int lastDotIndex = ruleTypeFullName.LastIndexOf (".");
					string modelNamespace = ruleTypeFullName.Substring (0, lastDotIndex);
					Type actionType = Type.GetType (string.Format ("{0}.Action{1}", modelNamespace, actionName));

					if (actionType == null) {
						Log.SignalErrorToDeveloper ("No Implementation for Action Type {0} found.", actionName);
						reader.Skip ();
						continue;
					}

					serializer = new XmlSerializer (actionType, xmlRootAttr);
					IAction action = (IAction)serializer.Deserialize (reader);
					action.Parent = this;
					containedActions.Add (action);
				} else {
					Log.SignalErrorToDeveloper ("Unexcpected xml {0} named {1} inside rule found.", reader.NodeType, reader.LocalName);
					reader.Read ();
				}
			} 

			GQML.AssertReaderAtEnd (reader, GQML.RULE);
			reader.Read ();
		}

		#endregion


	}
}
