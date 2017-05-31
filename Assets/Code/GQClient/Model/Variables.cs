using UnityEngine;
using System.Collections;
using GQ.Client.Model.XML;
using System.Text.RegularExpressions;
using System;

namespace GQ.Client.Model
{

	public class Variables
	{
	
		#region Persistence

		public static readonly string GQ_VAR_PREFIX = Definitions.GQ_PREFIX + ".var.";
		public const char VAR_TYPE_DELIMITER = ':';

		public static void SaveVariableToStore (QuestVariable questVar)
		{

			if (questVar == null)
				return;

			string varname = Variables.GQ_VAR_PREFIX + questVar.key;

			PlayerPrefs.SetString (varname, questVar.ToString ());
			PlayerPrefs.Save ();
		}

		public static QuestVariable LoadVariableFromStore (string varName)
		{
			string prefKey = Variables.GQ_VAR_PREFIX + varName;

			if (!PlayerPrefs.HasKey (prefKey)) {
				Debug.Log (string.Format ("WARNING: Tried to load variable {0} but didn't find it in store.", varName));
				return null;
			}

			string stringValue = PlayerPrefs.GetString (prefKey);

			int i = stringValue.IndexOf (VAR_TYPE_DELIMITER);
			if (i == -1) {
				Debug.LogWarning ("Loaded a variable from store with invalid content: " + stringValue);
				return null;
			}

			string type = stringValue.Substring (0, i);
			string value = stringValue.Substring (i + 1);

			Debug.Log (string.Format ("Loaded Variable {2} type: {0}, value: {1}", type, value, varName));

			QuestVariable resultVar = null;

			switch (type) {
			case GQML.NUMBER:
				resultVar = new QuestVariable (varName, double.Parse (value));
				break;
			case GQML.STRING:
				resultVar = new QuestVariable (varName, value);
				break;
			case GQML.BOOL:
				resultVar = new QuestVariable (varName, bool.Parse (value));
				break;
			default:
				Debug.LogWarning ("Loaded a variable from store with unknown type: " + type);
				resultVar = null;
				break;
			}

			return resultVar;
		}

		#endregion


		#region Runtime Registry

		public static Value getVariableValue (string varName)
		{
			return null; // TODO
		}

		#endregion

		#region Util Functions

		private const string VARNAME_REGEXP = @"(?!$)[a-zA-Z]+[a-zA-Z0-9_]*";
		private const string REGEXP_START = @"^";
		private const string REGEXP_END = @"$";

		public const string UNDEFINED_VAR = "_undefined";

		public static bool IsValidVariableName (string name)
		{
			Regex regex = new Regex (REGEXP_START + VARNAME_REGEXP + REGEXP_END);
			Match match = regex.Match (name);
			return match.Success;
		}

		/// <summary>
		/// Returns the longest valid name contained in the given nameCandidate starting from the beginning.
		/// </summary>
		/// <returns>The valid variable name from start.</returns>
		/// <param name="nameCandidate">Name candidate.</param>
		public static string LongestValidVariableNameFromStart (string nameCandidate)
		{
			Regex regex = new Regex (REGEXP_START + VARNAME_REGEXP);
			Match match = regex.Match (nameCandidate);
			if (!match.Success)
				throw new ArgumentException ("\"" + nameCandidate + "\" does not start with a valid variable name.");
			return match.Value;
		}

		#endregion
	}

}
