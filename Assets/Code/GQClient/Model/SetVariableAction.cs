using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Xml.Serialization;
using GQ.Client.Err;
using System.Xml;

namespace GQ.Client.Model
{

	public class SetVariableAction : AbstractAction
	{
		#region Structure

		protected string varName = null;
		protected IExpression valueExpression = null;


		/// <summary>
		/// Reader must be at the action element (start).
		/// </summary>
		/// <param name="reader">Reader.</param>
		public override void ReadXml (System.Xml.XmlReader reader)
		{
			XmlSerializer serializer;

			string actionName = reader.LocalName;

			// read the var attribute:
			varName = reader.GetAttribute (GQML.ACTION_ATTRIBUTE_VARNAME);
			if (varName == null) {
				Log.SignalErrorToDeveloper ("SetVariable Action without var attribute found.");
			}

			while (reader.Read ()) {

				// if we reach the end of this condition element we are ready to leave this method.
				if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals (actionName)) {
					break;
				}

				if (reader.NodeType != XmlNodeType.Element)
					continue;

				switch (reader.LocalName) {
				// COMPOUND CONDITIONS:
				case GQML.ACTION_SETVARIABLE_VALUE:
					valueExpression = ExpressionHelper.ParseSingleExpressionFromXML (reader);
					break;
				// UNKOWN CASE:
				default:
					Log.WarnDeveloper ("SetVariable Action has additional unknown {0} element. (Ignored)", reader.LocalName);
					break;
				}
			}
		}

		#endregion


		#region Functions

		public override void Execute ()
		{
			if (varName == null) {
				Log.SignalErrorToDeveloper ("SetVariable Action without varname can not be executed. (Ignored)");
				return;
			}

			if (valueExpression == null) {
				Log.SignalErrorToDeveloper ("SetVariable Action without value can not be executed. (Ignored)");
				return;
			}

			Variables.SetVariableValue (varName, valueExpression.Evaluate ());
		}

		#endregion
	}
}
