using GQ.Client.Conf;

namespace GQ.Client.UI
{
	public class CategoryTreeHeaderLayout : LayoutConfig
	{

		public override void layout ()
		{
			// set heights of text and image:
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, fgColor: ConfigurationManager.Current.menuBGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Name", fgColor: ConfigurationManager.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Button", fgColor: ConfigurationManager.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Button/Hint", sizeScaleFactor: 0.6f, fgColor: ConfigurationManager.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Button/OnOff", fgColor: ConfigurationManager.Current.menuFGColor);
		}

	}
}
