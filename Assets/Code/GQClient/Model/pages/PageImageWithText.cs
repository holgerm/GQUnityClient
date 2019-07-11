using System.Xml;

namespace GQ.Client.Model
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