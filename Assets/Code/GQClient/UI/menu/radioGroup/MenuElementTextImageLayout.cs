using GQ.Client.Conf;

namespace GQ.Client.UI
{

	public class MenuElementTextImageLayout : MenuElementLayout
	{

		public override void layout ()
		{
			base.layout ();

			// set heights of text and image:
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Text", fgColor: ConfigurationManager.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Image", fgColor: ConfigurationManager.Current.menuFGColor);
		}

	}
}
