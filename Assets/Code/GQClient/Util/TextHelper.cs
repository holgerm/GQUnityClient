//#define DEBUG_LOG

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Code.GQClient.Conf;
using Code.GQClient.Model.expressions;
using UnityEngine;

namespace Code.GQClient.Util
{
    public static class TextHelper
    {
        public const string regexPattern4Varnames = @"@[a-zA-Z.]+[a-zA-Z.0-9\-_]*@";

        public const string regexPattern4HTMLAnchors =
            @"<a +(?:(?:[a-zA-Z]+) *= *(?:""[^""]*?""|'[^']*?'))*.*?>(?'content'.*?)<\/a>";

        public const string regexPattern4HTMLAttributes = @"(?'name'[a-zA-Z]+) *= *(?'val'""[^""]*?""|'[^']*?')";


        public static string MakeReplacements(this string rawText)
        {
            if (string.IsNullOrEmpty(rawText))
            {
                return "";
            }

            var replaceVarNamesMethod = new MatchEvaluator(replaceVariableNames);
            var result = Regex.Replace(rawText, regexPattern4Varnames, replaceVarNamesMethod);

            result = result.Replace("<br>", "\n");
            return result;
        }

        static string replaceVariableNames(Match match)
        {
            var varName = match.Value.Substring(1, match.Value.Length - 2);
            var value = Variables.GetValue(varName).AsString();
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

        private static string HTMLDecode(string rawText)
        {
            if (string.IsNullOrEmpty(rawText))
                return rawText;

            var result = rawText.Replace("&lt;", "<");
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
            var result = HTMLDecode(rawText);
            result = MakeReplacements(result);
            if (supportHtmlLinks)
                result = EnhanceHTMLAnchors4TMPText(result);

            return result;
        }

        private static string EnhanceHTMLAnchors4TMPText(string htmlText)
        {
            var replacements = new Dictionary<string, string>();
            var matchedAnchors = Regex.Matches(htmlText, regexPattern4HTMLAnchors);
            foreach (Match matchedAnchor in matchedAnchors)
            {
                if (!matchedAnchor.Success || replacements.ContainsKey(matchedAnchor.Value))
                    continue;

                var matchedAttributes = Regex.Matches(matchedAnchor.Value, regexPattern4HTMLAttributes);
                var enhancedAnchor = new StringBuilder("<link=");
                var hrefFound = false;
                foreach (Match matchedAttr in matchedAttributes)
                {
                    // we add all attributes but ignore when we already find a name attribute:
                    if (matchedAttr.Groups["name"].ToString().Equals("href"))
                    {
                        hrefFound = true;
                        enhancedAnchor.Append(matchedAttr.Groups["val"]);
                        enhancedAnchor.Append(
                            $"><color=#{ColorUtility.ToHtmlStringRGBA(ConfigurationManager.Current.textLinkColor)}>");
                    }
                }

                if (!hrefFound)
                {
                    // no href: we ignore this link match at all and do not add it to the replacements:
                    continue;
                }

                enhancedAnchor.Append(matchedAnchor.Groups["content"]);
                enhancedAnchor.Append("</color></link>");
                
                // store the anchor replacement (old, new):
                replacements.Add(matchedAnchor.Value, enhancedAnchor.ToString());
            }

            // execute all anchor replacements:
            var enumerator = replacements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                htmlText = htmlText.Replace(enumerator.Current.Key, enumerator.Current.Value);
            }

            return htmlText;
        }

        public static string StripQuotes(this string original)
        {
            var result = original;
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

        public static string MakeLinkText(string linkString)
        {
            if (!linkString.StartsWith(@"http://", StringComparison.Ordinal) &&
                !linkString.StartsWith(@"https://", StringComparison.Ordinal))
            {
                linkString = @"https://" + linkString;
            }

            if (linkString.EndsWith(@"/", StringComparison.Ordinal))
            {
                linkString = linkString.Substring(0, linkString.Length - 1);
            }

            return $"<link=\"{linkString}\"><color=blue>{linkString}</color></link>";
        }
    }
}