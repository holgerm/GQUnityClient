using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using GQ.Client.Model;
using System.Collections.Generic;
using System.Text;

namespace GQ.Client.Util
{

	public static class TextHelper
	{

		public const string regexPattern4Varnames = @"@[a-zA-Z.]+[a-zA-Z.0-9\-_]*@";
		public const string regexPattern4HTMLAnchors = @"<a +(?:(?:[a-zA-Z]+) *= *(?:""[^""]*?""|'[^']*?'))*.*?>(?'content'.*?)<\/a>";
		public const string regexPattern4HTMLAttributes = @"(?'name'[a-zA-Z]+) *= *(?'val'""[^""]*?""|'[^']*?')";


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
			result = EnhanceHTMLAnchors4HyperText (result);

			return result;
		}

		private static string EnhanceHTMLAnchors4HyperText (string htmlText)
		{
			Dictionary<string, string> replacements = new Dictionary<string, string> ();
			MatchCollection matchedAnchors = Regex.Matches (htmlText, regexPattern4HTMLAnchors);
			foreach (Match matchedAnchor in matchedAnchors) {
				if (!matchedAnchor.Success || replacements.ContainsKey (matchedAnchor.Value))
					continue;

				StringBuilder newAnchorStartAndNameAttr = new StringBuilder ("<a name=\"link\"");
				StringBuilder newAnchorFurtherAttributes = new StringBuilder ("");

				MatchCollection matchedAttributes = Regex.Matches (matchedAnchor.Value, regexPattern4HTMLAttributes);

				foreach (Match matchedAttr in matchedAttributes) {
					// we add all attributes but ignore when we already find a name attribute:
					if (!matchedAttr.Groups ["name"].ToString ().Equals ("name")) {
						newAnchorFurtherAttributes.Append (" " + matchedAttr.Groups ["name"] + "=" + matchedAttr.Groups ["val"]);
					}
				}

				string enhancedAnchor = 
					newAnchorStartAndNameAttr.ToString () +
					newAnchorFurtherAttributes.ToString () + ">" +
					matchedAnchor.Groups ["content"] +
					"</a>";
				// store the anchor replacement (old, new):
				replacements.Add (matchedAnchor.Value, enhancedAnchor);
			}

			// execute all anchor replacements:
			Dictionary<string, string>.Enumerator enumerator = replacements.GetEnumerator ();
			while (enumerator.MoveNext ()) {
				htmlText = htmlText.Replace (enumerator.Current.Key, enumerator.Current.Value);
			}
			return htmlText;
		}

		public static string StripQuotes(this string original) {
			string result = original;
			if (result.StartsWith("\"")) {
				result = result.Substring (1);
			}
			if (result.EndsWith("\"")) {
				result = result.Substring (0, result.Length - 1);
			}
			return result;
		}
	}
}
