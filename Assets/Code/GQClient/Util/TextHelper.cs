//#define DEBUG_LOG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.expressions;
using Code.GQClient.Model.mgmt.quests;
using UnityEngine;

namespace Code.GQClient.Util
{
    public static class TextHelper
    {
        private const string REGEXP_VARNAMES = @"@[a-zA-Z.]+[a-zA-Z.0-9\-_]*@";

        private const string REGEXP_LINKS_HTML =
            @"<a +(?:(?:[a-zA-Z]+) *= *(?:""[^""]*?""|'[^']*?'))*.*?>(?'content'.*?)<\/a>";

        private const string REGEXP_HTML_LINK_ATTRIBUTES =
            @"(?'name'[a-zA-Z]+) *= *(?'val'""[^""]*?""|'[^']*?')";

        private const string REGEXP_LINKS_MARKDOWN =
            @"\[(?'content'([^\]]+))\]\((?'link'[^)]+)\)";

        private const string REGEXP_LINKS_DIRECT =
            @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,63}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)";


        public static string MakeReplacements(this string rawText)
        {
            if (string.IsNullOrEmpty(rawText))
            {
                return "";
            }

            var replaceVarNamesMethod = new MatchEvaluator(replaceVariableNames);
            var result = Regex.Replace(rawText, REGEXP_VARNAMES, replaceVarNamesMethod);

            result = result.Replace("<br>", "\n");
            return result;
        }

        static string replaceVariableNames(Match match)
        {
            var varName = match.Value.Substring(1, match.Value.Length - 2);
            var value = Variables.GetValue(varName).AsString();
            return value;
        }

        public static string HTMLDecode(this string rawText)
        {
            if (string.IsNullOrEmpty(rawText))
                return rawText;

            var result = rawText.Replace("&lt;", "<");
            result = result.Replace("&gt;", ">");
            result = result.Replace("&amp;", "&");

            return result;
        }

        /// <summary>
        /// Decodes HTML encodings (such as '&lt;'), replaces special tags (like &lt;br&gt;) and replaces Variable names with their current values.
        /// </summary>
        /// <returns>The hyper text.</returns>
        /// <param name="rawText">Raw text.</param>
        /// <param name="supportHtmlLinks">Support clickable links within the text, defaults to true.</param>
        public static string Decode4TMP(this string rawText, bool supportHtmlLinks = true)
        {
            var result = rawText.HTMLDecode();
            result = result.MakeReplacements();
            if (supportHtmlLinks)
                result = result.TransformLinks4TMP();

            return result;
        }

        /// <summary>
        /// Transforms raw html text into a TMP-formatted version
        /// </summary>
        /// <param name="htmlText"></param>
        /// <returns></returns>
        public static string TransformLinks4TMP(this string htmlText)
        {
            string inputText = htmlText;
            MatchCollection htmlLinkMatches = Regex.Matches(htmlText, REGEXP_LINKS_HTML);
            MatchCollection markdownLinkMatches = Regex.Matches(htmlText, REGEXP_LINKS_MARKDOWN);
            MatchCollection directLinkMatches = Regex.Matches(htmlText, REGEXP_LINKS_DIRECT);

            var replacements = new SortedDictionary<Match, string>(new MatchReplacementComparer());
            collectHtmlLinkReplacements(htmlLinkMatches, replacements);
            collectMarkdownLinkReplacements(markdownLinkMatches, replacements);
            collectDirectLinkReplacements(directLinkMatches, replacements);

            // execute all link replacements:
            var enumerator = replacements.GetEnumerator();
            int replacementLengthCorrection = 0;
            while (enumerator.MoveNext())
            {
                int index = enumerator.Current.Key.Index + replacementLengthCorrection;
                htmlText = htmlText.Remove(index, enumerator.Current.Key.Length);
                htmlText = htmlText.Insert(index, enumerator.Current.Value);
                replacementLengthCorrection += (enumerator.Current.Value.Length - enumerator.Current.Key.Length);
            }

            return htmlText;
        }

        private static void collectHtmlLinkReplacements(MatchCollection htmlLinkMatches,
            IDictionary<Match, string> replacements)
        {
            foreach (Match matchedAnchor in htmlLinkMatches)
            {
                if (!matchedAnchor.Success || replacements.ContainsKey(matchedAnchor))
                    continue;

                var matchedAttributes = Regex.Matches(matchedAnchor.Value, REGEXP_HTML_LINK_ATTRIBUTES);
                var enhancedAnchor = new StringBuilder("<link=");
                var hrefFound = false;
                foreach (Match matchedAttr in matchedAttributes)
                {
                    // we add all attributes but ignore when we already find a name attribute:
                    if (matchedAttr.Groups["name"].ToString().Equals("href"))
                    {
                        hrefFound = true;
                        string attribString = matchedAttr.Groups["val"].ToString();
                        // encode space by %20 within URL arguments, e.g. subject=space%20is%20encoded
                        attribString = attribString.Replace(" ", "%20");
                        enhancedAnchor.Append(attribString);
                        enhancedAnchor.Append(
                            $"><color=#{ColorUtility.ToHtmlStringRGBA(Config.Current.textLinkColor)}>");
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
                replacements.Add(matchedAnchor, enhancedAnchor.ToString());
            }
        }

        private static void collectMarkdownLinkReplacements(MatchCollection markdownLinkMatches,
            IDictionary<Match, string> replacements)
        {
            foreach (Match matchedAnchor in markdownLinkMatches)
            {
                if (!matchedAnchor.Success || replacements.ContainsKey(matchedAnchor))
                    continue;

                var enhancedAnchor = new StringBuilder(@"<link=""");
                string linkText = matchedAnchor.Groups["link"].Value;
                // encode space by %20 within URL arguments, e.g. subject=space%20is%20encoded
                linkText = linkText.Replace(" ", "%20");
                enhancedAnchor.Append(
                    linkText + @"""");
                enhancedAnchor.Append(
                    $"><color=#{ColorUtility.ToHtmlStringRGBA(Config.Current.textLinkColor)}>");

                enhancedAnchor.Append(matchedAnchor.Groups["content"]);
                enhancedAnchor.Append("</color></link>");

                // store the anchor replacement (old, new):
                replacements.Add(matchedAnchor, enhancedAnchor.ToString());
            }
        }

        private static void collectDirectLinkReplacements(MatchCollection markdownLinkMatches,
            IDictionary<Match, string> replacements)
        {
            foreach (Match matchedAnchor in markdownLinkMatches)
            {
                bool ShouldBeReplaced()
                {
                    foreach (Match replacement in replacements.Keys)
                    {
                        // check whether matchedAnchor overlaps with the current replacement:
                        if (matchedAnchor.Index < replacement.Index + replacement.Length &&
                            matchedAnchor.Length + matchedAnchor.Index > replacement.Index)
                        {
                            return false;
                        }
                    }

                    // no overlapping found, hence we replace this match:
                    return true;
                }

                if (!matchedAnchor.Success || !ShouldBeReplaced())
                    continue;

                var enhancedAnchor = new StringBuilder(@"<link=""");
                string linkText = matchedAnchor.Value;
                // encode space by %20 within URL arguments, e.g. subject=space%20is%20encoded
                linkText = linkText.Replace(" ", "%20");
                enhancedAnchor.Append(
                    linkText + @"""");
                enhancedAnchor.Append(
                    $"><color=#{ColorUtility.ToHtmlStringRGBA(Config.Current.textLinkColor)}>");

                enhancedAnchor.Append(linkText);
                enhancedAnchor.Append("</color></link>");

                // store the anchor replacement (old, new):
                replacements.Add(matchedAnchor, enhancedAnchor.ToString());
            }
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

        public static string FirstLetterToUpper(this string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
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

    class MatchReplacementComparer : IComparer<Match>
    {
        public int Compare(Match x, Match y)
        {
            return x.Index - y.Index;
        }
    }
}