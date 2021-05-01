using Code.GQClient.Util;
using NUnit.Framework;
using UnityEngine;

namespace GQTests.Util
{
    public class TextHelperTransformLinksTest
    {
        [Test]
        public void NoLinkUnchanged()
        {
            Assert.AreEqual("", "".TransformLinks4TMP());
            Assert.AreEqual("Word", "Word".TransformLinks4TMP());
            const string loremIpsum =
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa.";
            Assert.AreEqual(loremIpsum, loremIpsum.StripQuotes());
        }

        #region HTML Links

        [Test]
        public void HtmlLink()
        {
            const string original = @"<a href=""https://link.url.de"">Linktext</a>";
            const string expected = @"<link=""https://link.url.de""><color=#0000FFFF>Linktext</color></link>";
            const string prefix = @"Hallo hier noch etwas Text vorher ..... ";
            const string postfix = @"   und hier noch etwas danach ...";

            Assert.AreEqual(expected, original.TransformLinks4TMP());
            Assert.AreEqual(prefix + expected + postfix, (prefix + original + postfix).TransformLinks4TMP());
        }

        [Test]
        public void HtmlLinks()
        {
            const string htmlLink = @"<a href=""https://link.url.de"">Linktext</a>";
            const string tmpLink = @"<link=""https://link.url.de""><color=#0000FFFF>Linktext</color></link>";
            const string something = @"Hallo hier noch etwas Text vorher .....";
            const string original = something + htmlLink + something + htmlLink + htmlLink + something + htmlLink;
            const string expected = something + tmpLink + something + tmpLink + tmpLink + something + tmpLink;

            Assert.AreEqual(expected, original.TransformLinks4TMP());
        }

        [Test]
        public void HtmlLinkToMail()
        {
            const string original = @"<a href=""mailto:mail@provider.de"">Linktext</a>";
            const string expected = @"<link=""mailto:mail@provider.de""><color=#0000FFFF>Linktext</color></link>";
            const string prefix = @"Hallo hier noch etwas Text vorher ..... ";
            const string postfix = @"   und hier noch etwas danach ...";

            Assert.AreEqual(expected, original.TransformLinks4TMP());
            Assert.AreEqual(prefix + expected + postfix, (prefix + original + postfix).TransformLinks4TMP());
        }

        [Test]
        public void HtmlLinkToMailWithParamSpaceUrlEncoded()
        {
            const string original = @"<a href=""mailto:mail@provider.de?subject=Subject%20of%20Mail"">Linktext</a>";
            const string expected =
                @"<link=""mailto:mail@provider.de?subject=Subject%20of%20Mail""><color=#0000FFFF>Linktext</color></link>";

            Assert.AreEqual(expected, original.TransformLinks4TMP());
        }

        [Test]
        public void HtmlLinkToMailWithParamSpace()
        {
            const string original = @"<a href=""mailto:mail@provider.de?subject=Subject of Mail"">Linktext</a>";
            const string expected =
                @"<link=""mailto:mail@provider.de?subject=Subject%20of%20Mail""><color=#0000FFFF>Linktext</color></link>";

            Assert.AreEqual(expected, original.TransformLinks4TMP());
        }

        [Test]
        public void HtmlLinkToMailWithParamSpaces()
        {
            const string original =
                @"<a href=""mailto:mail@provider.de?body=Body of Mail&subject=Subject of Mail"">Linktext</a>";
            const string expected =
                @"<link=""mailto:mail@provider.de?body=Body%20of%20Mail&subject=Subject%20of%20Mail""><color=#0000FFFF>Linktext</color></link>";

            Assert.AreEqual(expected, original.TransformLinks4TMP());
        }

        #endregion


        #region Direct Links

        [Test]
        public void DirectLink()
        {
            const string original = @"https://link.url.de";
            const string expected =
                @"<link=""https://link.url.de""><color=#0000FFFF>https://link.url.de</color></link>";
            const string prefix = @"Hallo hier noch etwas Text vorher ..... ";
            const string postfix = @"   und hier noch etwas danach ...";

            Assert.AreEqual(expected, original.TransformLinks4TMP());
            Assert.AreEqual(prefix + expected + postfix, (prefix + original + postfix).TransformLinks4TMP());
        }

        [Test]
        public void DirectLinks()
        {
            const string original_1 = @"https://link_1.url.de";
            const string expected_1 =
                @"<link=""https://link_1.url.de""><color=#0000FFFF>https://link_1.url.de</color></link>";
            const string original_2 = @"https://link_2.url.de";
            const string expected_2 =
                @"<link=""https://link_2.url.de""><color=#0000FFFF>https://link_2.url.de</color></link>";
            const string original_3 = @"https://link_3.url.de";
            const string expected_3 =
                @"<link=""https://link_3.url.de""><color=#0000FFFF>https://link_3.url.de</color></link>";
            const string something = @" something else ... ";
            const string original =
                something + original_1 + something + original_2 + something + original_3 + something;
            const string expected =
                something + expected_1 + something + expected_2 + something + expected_3 + something;

            Assert.AreEqual(expected, original.TransformLinks4TMP());
        }

        #endregion


        #region Markdown Links

        [Test]
        public void MarkdownLink()
        {
            const string original = @"[Linktext](https://link.url.de)";
            const string expected = @"<link=""https://link.url.de""><color=#0000FFFF>Linktext</color></link>";
            const string prefix = @"Hallo hier noch etwas Text vorher ..... ";
            const string postfix = @"   und hier noch etwas danach ...";

            Assert.AreEqual(expected, original.TransformLinks4TMP());
            Assert.AreEqual(prefix + expected + postfix, (prefix + original + postfix).TransformLinks4TMP());
        }

        [Test]
        public void MarkdownLinks()
        {
            const string original_1 = @"[Linktext](https://link.url.de)";
            const string expected_1 = @"<link=""https://link.url.de""><color=#0000FFFF>Linktext</color></link>";
            const string original_2 = @"[Linktext 2](https://link.url.de)";
            const string expected_2 = @"<link=""https://link.url.de""><color=#0000FFFF>Linktext 2</color></link>";
            const string original_3 = @"[Linktext 2](https://link.url.de)";
            const string expected_3 = @"<link=""https://link.url.de""><color=#0000FFFF>Linktext 2</color></link>";
            const string something = @"... something else ... ";
            const string original = something + original_1 + something + original_2 + original_3 + something;
            const string expected = something + expected_1 + something + expected_2 + expected_3 + something;

            Assert.AreEqual(expected, original.TransformLinks4TMP());
        }

        [Test]
        public void MarkdownLinkToMail()
        {
            const string original = @"[Linktext](mailto:mail@provider.de)";
            const string expected = @"<link=""mailto:mail@provider.de""><color=#0000FFFF>Linktext</color></link>";
            const string prefix = @"Hallo hier noch etwas Text vorher ..... ";
            const string postfix = @"   und hier noch etwas danach ...";

            Assert.AreEqual(expected, original.TransformLinks4TMP());
            Assert.AreEqual(prefix + expected + postfix, (prefix + original + postfix).TransformLinks4TMP());
        }

        [Test]
        public void MarkdownLinkToMailWithParamSpaceUrlEncoded()
        {
            const string original = @"[Linktext](mailto:mail@provider.de?subject=Subject%20of%20Mail)";
            const string expected =
                @"<link=""mailto:mail@provider.de?subject=Subject%20of%20Mail""><color=#0000FFFF>Linktext</color></link>";
            const string prefix = @"Hallo hier noch etwas Text vorher ..... ";
            const string postfix = @"   und hier noch etwas danach ...";

            Assert.AreEqual(expected, original.TransformLinks4TMP());
            Assert.AreEqual(prefix + expected + postfix, (prefix + original + postfix).TransformLinks4TMP());
        }

        [Test]
        public void MarkdownLinkToMailWithParamSpaces()
        {
            const string original = @"[Linktext](mailto:mail@provider.de?body=Body of Mail&subject=Subject of Mail)";
            const string expected =
                @"<link=""mailto:mail@provider.de?body=Body%20of%20Mail&subject=Subject%20of%20Mail""><color=#0000FFFF>Linktext</color></link>";
            const string prefix = @"Hallo hier noch etwas Text vorher ..... ";
            const string postfix = @"   und hier noch etwas danach ...";

            Assert.AreEqual(expected, original.TransformLinks4TMP());
            Assert.AreEqual(prefix + expected + postfix, (prefix + original + postfix).TransformLinks4TMP());
        }

        #endregion


        #region Mixed Links

        [Test]
        public void HtmlAndMarkdownLinks()
        {
            const string originalHtml = @"<a href=""https://link.url.de"">Html Linktext</a>";
            const string originalMarkdown = @"[Markdown Linktext](https://link.url.de)";
            const string something = @"... something else ... ";

            const string expectedHtml = @"<link=""https://link.url.de""><color=#0000FFFF>Html Linktext</color></link>";
            const string expectedMarkdown =
                @"<link=""https://link.url.de""><color=#0000FFFF>Markdown Linktext</color></link>";

            // with something in between:
            string original = something + originalHtml + something + originalMarkdown + something;
            string expected = something + expectedHtml + something + expectedMarkdown + something;
            Assert.AreEqual(expected, original.TransformLinks4TMP());

            // directly adjacent:
            original = something + originalHtml + originalMarkdown + something;
            expected = something + expectedHtml + expectedMarkdown + something;
            Assert.AreEqual(expected, original.TransformLinks4TMP());

            // with something in between and multiple occurrences:
            original = something + originalHtml + something + originalMarkdown + something + originalHtml + something +
                       originalMarkdown;
            expected = something + expectedHtml + something + expectedMarkdown + something + expectedHtml + something +
                       expectedMarkdown;
            Assert.AreEqual(expected, original.TransformLinks4TMP());

            // directly adjacent and multiple occurrences:
            original = something + originalHtml + originalMarkdown + originalHtml + originalMarkdown + something;
            expected = something + expectedHtml + expectedMarkdown + expectedHtml + expectedMarkdown + something;
            Assert.AreEqual(expected, original.TransformLinks4TMP());
        }


        [Test]
        public void HtmlAndDirectLinks()
        {
            const string originalHtml = @"<a href=""https://link.url.de"">Html Linktext</a>";
            const string originalDirect = @"https://link_1.url.de";
            const string something = @" something else ... ";

            const string expectedHtml = @"<link=""https://link.url.de""><color=#0000FFFF>Html Linktext</color></link>";
            const string expectedDirect =
                @"<link=""https://link_1.url.de""><color=#0000FFFF>https://link_1.url.de</color></link>";

            // with something in between:
            string original = something + originalHtml + something + originalDirect + something;
            string expected = something + expectedHtml + something + expectedDirect + something;
            Assert.AreEqual(expected, original.TransformLinks4TMP());

            // directly adjacent:
            original = something + originalHtml + originalDirect + something;
            expected = something + expectedHtml + expectedDirect + something;
            Assert.AreEqual(expected, original.TransformLinks4TMP());

            // with something in between and multiple occurrences:
            original = something + originalHtml + something + originalDirect + something + originalHtml + something +
                       originalDirect;
            expected = something + expectedHtml + something + expectedDirect + something + expectedHtml + something +
                       expectedDirect;
            Assert.AreEqual(expected, original.TransformLinks4TMP());

            // directly adjacent and multiple occurrences:
            original = something + originalHtml + originalDirect + originalHtml + originalDirect + something;
            expected = something + expectedHtml + expectedDirect + expectedHtml + expectedDirect + something;
            Assert.AreEqual(expected, original.TransformLinks4TMP());
        }

        [Test]
        public void MarkdownAndDirectLinks()
        {
            const string originalMarkdown = @"[Markdown Linktext](https://link.url.de)";
            const string originalDirect = @"https://link_1.url.de";
            const string something = @" something else ...";

            const string expectedMarkdown =
                @"<link=""https://link.url.de""><color=#0000FFFF>Markdown Linktext</color></link>";
            const string expectedDirect =
                @"<link=""https://link_1.url.de""><color=#0000FFFF>https://link_1.url.de</color></link>";

            // with something in between:
            string original = something + originalDirect + something + originalMarkdown + something;
            string expected = something + expectedDirect + something + expectedMarkdown + something;
            Assert.AreEqual(expected, original.TransformLinks4TMP());

            // directly adjacent:
            original = something + originalDirect + originalMarkdown + something;
            expected = something + expectedDirect + expectedMarkdown + something;
            Assert.AreEqual(expected, original.TransformLinks4TMP());

            // with something in between and multiple occurrences:
            original = something + originalDirect + something + originalMarkdown + something + originalDirect +
                       something +
                       originalMarkdown;
            expected = something + expectedDirect + something + expectedMarkdown + something + expectedDirect +
                       something +
                       expectedMarkdown;
            Assert.AreEqual(expected, original.TransformLinks4TMP());

            // directly adjacent and multiple occurrences:
            original = something + originalDirect + originalMarkdown + originalDirect + originalMarkdown + something;
            expected = something + expectedDirect + expectedMarkdown + expectedDirect + expectedMarkdown + something;
            Assert.AreEqual(expected, original.TransformLinks4TMP());
        }

        #endregion


        #region Practical Example

        [Test]
        public void Tel_Mail_Href_all_html_style()
        {
            const string original = 
                @"Ansprechpartner:<br>
                  Stefan Fuchs<br>
                  Vinzentiusstr. 58<br>
                  83395 Freilassing<br>
                  <br>
                  Telefon: <a href=""tel:+491724893793"">0172 / 489 37 93</a><br>
                  Email: <a href=""mailto:bereitschaft@brk-freilassing.bayern@gmx.de"">bereitschaft@brk-freilassing.bayern</a><br>
                  Webseite: <a href=""https://www.brk-freilassing.bayern"">www.brk-freilassing.bayern</a>";
            const string expected = 
                @"Ansprechpartner:<br>
                  Stefan Fuchs<br>
                  Vinzentiusstr. 58<br>
                  83395 Freilassing<br>
                  <br>
                  Telefon: <link=""tel:+491724893793""><color=#2C008BFF>0172 / 489 37 93</color></link><br>
                  Email: <link=""mailto:bereitschaft@brk-freilassing.bayern@gmx.de""><color=#2C008BFF>bereitschaft@brk-freilassing.bayern</color></link><br>
                  Webseite: <link=""https://www.brk-freilassing.bayern""><color=#2C008BFF>www.brk-freilassing.bayern</color></link>";
            string result = original.TransformLinks4TMP();
            Debug.Log($"{original} --> {result}");
            Assert.AreEqual(expected, result);
        }

        #endregion
    }
}