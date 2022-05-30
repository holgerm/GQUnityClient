using Code.GQClient.Conf;
using Code.GQClient.UI.layout;

namespace Code.GQClient.UI.menu.categories
{

	public class CategoryEntryLayout : LayoutConfig
	{

		public override void layout ()
		{
			// set heights of text and image:
            MenuLayoutConfig.SetMenuEntryLayout (gameObject, fgColor: Config.Current.categoryEntryBGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "FolderImage", sizeScaleFactor: CategoryFolderLayout.FolderImageScaleFactor, fgColor: Config.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Name", fgColor: Config.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Number", fgColor: Config.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "Symbol", fgColor: Config.Current.menuFGColor);
			MenuLayoutConfig.SetMenuEntryLayout (gameObject, "InfoQuest", fgColor: Config.Current.menuFGColor);
			var catEntryCtrl = gameObject.GetComponent<CategoryEntryCtrl>();
			if (catEntryCtrl != null) catEntryCtrl.UpdateView4State();
		}
	}
}
