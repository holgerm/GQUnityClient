using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using UnityEngine;

namespace Code.GQClient.Model.pages
{
    public class PageWebPage : Page
    {
        #region State

        public PageWebPage(XmlReader reader) : base(reader)
        {
        }

        public string File { get; set; }
        public string URL { get; set; }
        public string AllowLeaveOnUrlContains { get; internal set; }
        public string AllowLeaveOnUrlDoesNotContain { get; internal set; }

        public List<string> AllowLeaveOnHtmlContains { get; internal set; }

        public List<string> AllowLeaveOnHtmlDoesNotContain { get; internal set; }

        public bool LeaveOnAllow { get; internal set; }

        public string EndButtonText { get; set; }

        public string EndButtonTextWhenClosed { get; set; }
        public string ForwardButtonTextBeforeFinished { get; set; }
        
        public bool FullscreenLandscape { get; internal set; }

        #endregion

        #region XML Serialization

        public static readonly Regex PdfUrlRegex = new Regex(@"(?<url>.*.pdf)(#page=(?<page>\d+))?");

        protected override void ReadAttributes(XmlReader reader)
        {
            base.ReadAttributes(reader);

            File = GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_FILE, reader);
            URL = GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_URL, reader);
            if (PdfUrlRegex.IsMatch(URL))
            {
                var match = PageWebPage.PdfUrlRegex.Match(URL);
                string pdfUrl;
                if (match.Groups["url"].Success)
                {
                    pdfUrl = match.Groups["url"].Value;
                    QuestManager.CurrentlyParsingQuest.AddMedia(pdfUrl, "WebPage." + GQML.PAGE_WEBPAGE_URL + " (pdf)");
                }
            }

            EndButtonText = GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_ENDBUTTONTEXT, reader);
            EndButtonTextWhenClosed = GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_ENDBUTTONTEXT_CLOSED, reader);

            AllowLeaveOnUrlContains = GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_ALLOW_LEAVE_ON_URL_CONTAINS, reader);
            AllowLeaveOnUrlDoesNotContain =
                GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_ALLOW_LEAVE_ON_URL_DOESNOTCONTAIN, reader);
            AllowLeaveOnHtmlContains =
                GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_ALLOW_LEAVE_ON_HTML_CONTAINS, reader)
                    .SplitWithMaskedSeparator();
            Debug.Log($"AllowLeaveOnHtmlContains #: {AllowLeaveOnHtmlContains.Count}");
            foreach (var s in AllowLeaveOnHtmlContains)
            {
                Debug.Log($"AllowLeaveOnHtmlContains: {s}");
            }

            AllowLeaveOnHtmlDoesNotContain =
                GQML.GetStringAttribute(GQML.PAGE_WEBPAGE_ALLOW_LEAVE_ON_HTML_DOESNOTCONTAIN, reader)
                    .SplitWithMaskedSeparator();
            Debug.Log($"AllowLeaveOnHtmlDoesNotContain #: {AllowLeaveOnHtmlDoesNotContain.Count}");
            foreach (var s in AllowLeaveOnHtmlDoesNotContain)
            {
                Debug.Log($"AllowLeaveOnHtmlDoesNotContain: {s}");
            }
            LeaveOnAllow = GQML.GetOptionalBoolAttribute(GQML.PAGE_WEBPAGE_LEAVE_ON_ALLOW, reader);

            FullscreenLandscape = GQML.GetRequiredBoolAttribute(GQML.PAGE_WEBPAGE_FULLSCREEN_LANDSCAPE, reader);
        }

        #endregion

        #region Runtime API

        public override void Start(bool canReturnToPrevious = false)
        {
            base.Start(canReturnToPrevious);
        }

        #endregion
    }
}