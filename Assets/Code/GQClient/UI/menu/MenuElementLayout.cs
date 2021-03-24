using Code.GQClient.Conf;
using Code.GQClient.UI.layout;

namespace Code.GQClient.UI.menu
{

	public class MenuElementLayout : LayoutConfig
	{
		/// <summary>
		/// Sets the height of this multi toggle button as menu entry.
		/// </summary>
		public override void layout ()
		{
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, fgColor: Config.Current.menuBGColor);
		}

	}
}