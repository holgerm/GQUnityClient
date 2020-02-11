using System.Xml;
using Code.GQClient.Model.gqml;

namespace Code.GQClient.Model.pages
{
    public class PageImageWithText : PageNPCTalk
	{
        public PageImageWithText(XmlReader reader) : base(reader) { }

        public override string PageSceneName {
			get {
				return GQML.PAGE_TYPE_NPCTALK;
			}
		}
	}
}