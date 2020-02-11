//#define DEBUG_LOG

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Code.GQClient.Model.expressions;

namespace Code.GQClient.Util
{

    public static class TextHelper
    {

        public const string regexPattern4Varnames = @"@[a-zA-Z.]+[a-zA-Z.0-9\-_]*@";
        public const string regexPattern4HTMLAnchors = @"<a +(?:(?:[a-zA-Z]+) *= *(?:""[^""]*?""|'[^']*?'))*.*?>(?'content'.*?)<\/a>";
        public const string regexPattern4HTMLAttributes = @"(?'name'[a-zA-Z]+) *= *(?'val'""[^""]*?""|'[^']*?')";


        public static string MakeReplacements(this string rawText)
        {
            if (string.IsNullOrEmpty(rawText))
            {
                return "";
            }

            MatchEvaluator replaceVarNamesMethod = new MatchEvaluator(replaceVariableNames);
            string result = Regex.Replace(rawText, regexPattern4Varnames, replaceVarNamesMethod);

            result = result.Replace("<br>", "\n");
            return result;
        }

        static string replaceVariableNames(Match match)
        {
            string varName = match.Value.Substring(1, match.Value.Length - 2);
            string value = Variables.GetValue(varName).AsString();
#if DEBUG_LOG
            Debug.LogFormat("Replaced var {0} against value {1}", varName, value);
#endif
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

        public static string HTMLDecode(string rawText)
        {
            if (rawText == null || rawText == "")
                return rawText;

            string result = rawText.Replace("&lt;", "<");
            result = result.Replace("&gt;", ">");
            result = result.Replace("&amp;", "&");

            return result;
        }

        /// <summary>
        /// Decodes HTML encodings (such as &lt;), replaces special tags (like <br>) and replaces Variable names with their current values.
        /// </summary>
        /// <returns>The hyper text.</returns>
        /// <param name="rawText">Raw text.</param>
        /// <param name="supportHtmlLinks">Support clickable links within the text, defaults to true.</param>
        public static string Decode4TMP(this string rawText, bool supportHtmlLinks = true)
        {
            string result = HTMLDecode(rawText);
            result = MakeReplacements(result);
            if (supportHtmlLinks)
                result = EnhanceHTMLAnchors4TMPText(result);

            return result;
        }

        private static string EnhanceHTMLAnchors4TMPText(string htmlText)
        {
            Dictionary<string, string> replacements = new Dictionary<string, string>();
            MatchCollection matchedAnchors = Regex.Matches(htmlText, regexPattern4HTMLAnchors);
            foreach (Match matchedAnchor in matchedAnchors)
            {
                if (!matchedAnchor.Success || replacements.ContainsKey(matchedAnchor.Value))
                    continue;

                StringBuilder newAnchorStartAndNameAttr = new StringBuilder("<a name=\"link\"");
                StringBuilder newAnchorFurtherAttributes = new StringBuilder("");

                MatchCollection matchedAttributes = Regex.Matches(matchedAnchor.Value, regexPattern4HTMLAttributes);

                foreach (Match matchedAttr in matchedAttributes)
                {
                    // we add all attributes but ignore when we already find a name attribute:
                    if (!matchedAttr.Groups["name"].ToString().Equals("name"))
                    {
                        newAnchorFurtherAttributes.Append(" " + matchedAttr.Groups["name"] + "=" + matchedAttr.Groups["val"]);
                    }
                }

                string enhancedAnchor =
                    newAnchorStartAndNameAttr.ToString() +
                    newAnchorFurtherAttributes.ToString() + ">" +
                    matchedAnchor.Groups["content"] +
                    "</a>";
                // store the anchor replacement (old, new):
                replacements.Add(matchedAnchor.Value, enhancedAnchor);
            }

            // execute all anchor replacements:
            Dictionary<string, string>.Enumerator enumerator = replacements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                htmlText = htmlText.Replace(enumerator.Current.Key, enumerator.Current.Value);
            }
            return htmlText;
        }

        public static string StripQuotes(this string original)
        {
            string result = original;
            if (result.StartsWith("\"", StringComparison.CurrentCulture))
            {
                result = result.Substring(1);
            }
            if (result.EndsWith("\"", StringComparison.CurrentCulture))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        public static string Capitalize(this string original)
        {
            if (original == null || original == "" || !Char.IsLetter(original[0]))
                return original;
            return (original.Substring(0, 1).ToUpper() + original.Substring(1));
        }

        public static bool HasVideoEnding(this string url)
        {
            return (url.ToLower().EndsWith(".mp4", StringComparison.CurrentCulture));
        }
    }
}
