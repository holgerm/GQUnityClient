using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

namespace GQ.Client.Util {

	public class TextHelper {

		public const string regexPattern4Varnames = @"@[a-zA-Z]+[a-zA-Z0-9\-_]*@";

		public static string makeReplacements (string rawText) {
			if ( rawText == null ) {
				return "";
			}

			MatchEvaluator replaceVarNamesMethod = new MatchEvaluator(ReplaceVariableNames);
			string result = Regex.Replace(rawText, regexPattern4Varnames, replaceVarNamesMethod);

			result = result.Replace("<br>", "\n");
			return result;
		}

		public static string ReplaceVariableNames (Match match) {
			actions questactions = GameObject.Find("QuestDatabase").GetComponent<actions>();
			string varName = match.Value.Substring(1, match.Value.Length - 2);
			string value = questactions.getVariable(varName).getStringValue();
			return value;
		}

		public static string FirstLetterToUpper(string str)
		{
			if (str == null)
				return null;

			if (str.Length > 1)
				return char.ToUpper(str[0]) + str.Substring(1);

			return str.ToUpper();
		}


	}

}
