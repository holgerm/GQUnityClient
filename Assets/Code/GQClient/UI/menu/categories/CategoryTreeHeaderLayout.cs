using Code.GQClient.Conf;
using Code.GQClient.UI.layout;

namespace Code.GQClient.UI.menu.categories
{
	public class CategoryTreeHeaderLayout : LayoutConfig
	{

		public override void layout ()
		{
			// set heights of text and image:
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, fgColor: Config.Current.menuBGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Name", fgColor: Config.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Button", fgColor: Config.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Button/Hint", sizeScaleFactor: 0.6f, fgColor: Config.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Button/OnOff", fgColor: Config.Current.menuFGColor);
		}

	}
}
