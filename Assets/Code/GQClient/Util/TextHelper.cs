using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using GQ.Client.Model;

namespace GQ.Client.Util
{

	public class TextHelper
	{

		public const string regexPattern4Varnames = @"@[a-zA-Z.]+[a-zA-Z.0-9\-_]*@";

		public static string MakeReplacements (string rawText)
		{
			if (rawText == null) {
				return "";
			}

			MatchEvaluator replaceVarNamesMethod = new MatchEvaluator (replaceVariableNames);
			string result = Regex.Replace (rawText, regexPattern4Varnames, replaceVarNamesMethod);

			result = result.Replace ("<br>", "\n");
			return result;
		}

		static string replaceVariableNames (Match match)
		{
			string varName = match.Value.Substring (1, match.Value.Length - 2);
			string value = Variables.GetValue (varName).AsString ();
			Debug.Log ("VAR: " + varName + " has VALUE: " + value);
			return value;
		}

		public static string FirstLetterToUpper (string str)
		{
			if (str == null)
				return null;

			if (str.Length > 1)
				return char.ToUpper (str [0]) + str.Substring (1);

			return str.ToUpper ();
		}

		public static string HTMLDecode (string rawText)
		{
			string result = rawText.Replace ("&lt;", "<");
			result = result.Replace ("&gt;", ">");

			return result;
		}

		/// <summary>
		/// Decodes HTML encodings (such as &lt;), replaces special tags (like <br>) and replaces Variable names with their current values.
		/// </summary>
		/// <returns>The hyper text.</returns>
		/// <param name="rawText">Raw text.</param>
		public static string Decode4HyperText (string rawText)
		{
			string result = HTMLDecode (rawText);
			result = MakeReplacements (result);

			return result;
		}
	}

}
