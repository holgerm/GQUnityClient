using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Xml.Serialization;
using GQ.Client.Err;
using System.Xml;

namespace GQ.Client.Model
{

	public class IncrementVariableAction : AbstractAction
	{
		#region Structure

		protected string varName = null;


		/// <summary>
		/// Reader must be at the action element (start).
		/// </summary>
		/// <param name="reader">Reader.</param>
		public override void ReadXml (System.Xml.XmlReader reader)
		{
			// read the var attribute:
			varName = reader.GetAttribute (GQML.ACTION_ATTRIBUTE_VARNAME);
			if (varName == null) {
				Log.SignalErrorToDeveloper ("IncrementVariableAction Action without var attribute found.");
			}

			reader.Skip ();
		}

		#endregion


		#region Functions

		public override void Execute ()
		{
			if (varName == null) {
				Log.SignalErrorToDeveloper ("IncrementVariableAction Action without varname can not be executed. (Ignored)");
				return;
			}

			Value previousVal = Variables.GetValue (varName);

			switch (previousVal.GetType ()) {
			case Value.Type.NULL:
				Variables.SetVariableValue (varName, new Value (1));
				break;
			case Value.Type.Integer:
				Variables.SetVariableValue (varName, new Value (previousVal.AsInt () + 1));
				break;
			case Value.Type.Float:
				Variables.SetVariableValue (varName, new Value (previousVal.AsDouble () + 1d));
				break;
			case Value.Type.Bool:
				Variables.SetVariableValue (varName, new Value (true));
				break;
			case Value.Type.VariableName:
				Log.SignalErrorToAuthor ("IncrementVariable must not be used on Variables representing Variable Names.", previousVal.GetType ());
				break;
			case Value.Type.Text:
				string previousText = previousVal.AsString ();
				char lastChar = previousText [previousText.Length - 1];
				lastChar++;
				string newText = previousText.Substring (0, previousText.Length - 1) + lastChar.ToString ();
				Variables.SetVariableValue (varName, new Value (newText));
				break;
			default:
				Log.SignalErrorToDeveloper ("IncrementVariable not implemented for value type {0}.", previousVal.GetType ());
				break;
			}
		}

		#endregion
	}
}
