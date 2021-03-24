using Code.GQClient.Conf;
using Code.GQClient.UI.layout;

namespace Code.GQClient.UI.menu.radioGroup
{

	public class MenuElementTextImageLayout : MenuElementLayout
	{

		public override void layout ()
		{
			base.layout ();

			// set heights of text and image:
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Text", fgColor: Config.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Image", fgColor: Config.Current.menuFGColor);
		}

	}
}
