using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using GQ.Client.Model;
using GQ.Client.Err;

namespace GQ.Client.Model
{

	public class Variables
	{
	
		#region Persistence

		public static readonly string GQ_VAR_PREFIX = Definitions.GQ_PREFIX + ".var.";
		public const char VAR_TYPE_DELIMITER = ':';

		//		public static void SaveVariableToStore (QuestVariable questVar)
		//		{
		//
		//			if (questVar == null)
		//				return;
		//
		//			string varname = Variables.GQ_VAR_PREFIX + questVar.key;
		//
		//			PlayerPrefs.SetString (varname, questVar.ToString ());
		//			PlayerPrefs.Save ();
		//		}
		//
		//		public static QuestVariable LoadVariableFromStore (string varName)
		//		{
		//			string prefKey = Variables.GQ_VAR_PREFIX + varName;
		//
		//			if (!PlayerPrefs.HasKey (prefKey)) {
		//				Debug.Log (string.Format ("WARNING: Tried to load variable {0} but didn't find it in store.", varName));
		//				return null;
		//			}
		//
		//			string stringValue = PlayerPrefs.GetString (prefKey);
		//
		//			int i = stringValue.IndexOf (VAR_TYPE_DELIMITER);
		//			if (i == -1) {
		//				Debug.LogWarning ("Loaded a variable from store with invalid content: " + stringValue);
		//				return null;
		//			}
		//
		//			string type = stringValue.Substring (0, i);
		//			string value = stringValue.Substring (i + 1);
		//
		//			Debug.Log (string.Format ("Loaded Variable {2} type: {0}, value: {1}", type, value, varName));
		//
		//			QuestVariable resultVar = null;
		//
		//			switch (type) {
		//			case GQML.NUMBER:
		//				resultVar = new QuestVariable (varName, double.Parse (value));
		//				break;
		//			case GQML.STRING:
		//				resultVar = new QuestVariable (varName, value);
		//				break;
		//			case GQML.BOOL:
		//				resultVar = new QuestVariable (varName, bool.Parse (value));
		//				break;
		//			default:
		//				Debug.LogWarning ("Loaded a variable from store with unknown type: " + type);
		//				resultVar = null;
		//				break;
		//			}
		//
		//			return resultVar;
		//		}

		#endregion


		#region Registry

		private static Dictionary<string, Value> variables = new Dictionary<string, Value> ();

		#endregion


		#region API

		/// <summary>
		/// Get the Value of the Variables with the specified varName or Value.Null if no such variable is found. This method will never return null.
		/// </summary>
		/// <param name="varName">Variable name.</param>
		public static Value GetValue (string varName)
		{
			if (!IsValidVariableName (varName)) {
				Log.WarnAuthor ("Assess to Variable named {0} is not possible. This is not a valid variable name.", varName);
				return Value.Null;
			}

			if (varName.StartsWith ("$")) {
				return GetReadOnlyVariableValue (varName);
			}

			Value foundValue;
			if (variables.TryGetValue (varName, out foundValue)) {
				return foundValue;
			} else {
				Log.WarnAuthor ("Variable {0} was not found.", varName);
				return Value.Null;
			}
		}

		public static void ClearAll ()
		{
			variables.Clear ();
		}

		/// <summary>
		/// Stores the newValue in Variable named varName. If this variable contained a value previously that gets replaced by the newValue.
		/// </summary>
		/// <returns><c>true</c>, if variable value replaced a previously given value, <c>false</c> otherwise.</returns>
		/// <param name="varName">Variable name.</param>
		/// <param name="newValue">New value.</param>
		public static bool SetVariableValue (string varName, Value newValue)
		{
			if (!IsValidUserDefinedVariableName (varName)) {
				Log.SignalErrorToAuthor ("Variable Name may not start with '$' Symbol, so you may not use {0} as you did in a SetVariable action.", varName);
				return false;
			}

			bool existedAlready = variables.ContainsKey (varName);
			if (existedAlready) {
				variables.Remove (varName);
			}
			variables.Add (varName, newValue);
			return existedAlready;
		}

		#endregion


		#region Access Read Only Variables

		private static Value GetReadOnlyVariableValue (string varName)
		{
			if (varName.StartsWith (GQML.VAR_PAGE_PREFIX)) {
				string pageIdString;
				if (varName.EndsWith (GQML.VAR_PAGE_RESULT)) {
					pageIdString = varName.Substring (
						GQML.VAR_PAGE_PREFIX.Length, 
						varName.Length - (GQML.VAR_PAGE_PREFIX.Length + GQML.VAR_PAGE_RESULT.Length)
					);
				} else if (varName.EndsWith (GQML.VAR_PAGE_STATE)) {
					pageIdString = varName.Substring (
						GQML.VAR_PAGE_PREFIX.Length, 
						varName.Length - (GQML.VAR_PAGE_PREFIX.Length + GQML.VAR_PAGE_STATE.Length)
					);
				} else {
					Log.WarnAuthor ("Page feature used in system variable named {0} is unknown.", varName);
					return Value.Null;
				}

				int pageId;
				if (Int32.TryParse (pageIdString, out pageId) == false) {
					Log.WarnDeveloper ("Page ID {0} cannot be interpreted.", pageIdString);
					return Value.Null;
				}
				IPage page = QuestManager.Instance.CurrentQuest.GetPageWithID (pageId);
				if (page == null) {
					Log.WarnDeveloper ("Page with id {0} not found in quest.", pageId);
					return Value.Null;
				}

				if (varName.EndsWith (GQML.VAR_PAGE_RESULT)) {
					return new Value (page.Result, Value.Type.Text);
				} else if (varName.EndsWith (GQML.VAR_PAGE_STATE)) {
					return new Value (page.State, Value.Type.Text);
				} 
			}

			return Value.Null;

		}

		#endregion


		#region Util Functions

		private const string VARNAME_USERDEFINED_REGEXP = @"(?!$)[a-zA-Z]+[a-zA-Z0-9_.]*";
		private const string VARNAME_REGEXP = @"(\$?|\$\_)?[a-zA-Z]+[a-zA-Z0-9_.]*";
		private const string REGEXP_START = @"^";
		private const string REGEXP_END = @"$";

		public const string UNDEFINED_VAR = "_undefined";

		public static bool IsValidUserDefinedVariableName (string name)
		{
			Regex regex = new Regex (REGEXP_START + VARNAME_USERDEFINED_REGEXP + REGEXP_END);
			Match match = regex.Match (name);
			return match.Success;
		}

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
