using Code.GQClient.Conf;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.menu.categories
{

	public class CategoryEntryCtrl : CategoryCtrl
	{

		#region static stuff

		protected const string PREFAB = "CategoryEntry";

		public static CategoryEntryCtrl Create (GameObject root, CategoryEntry catEntry, CategoryTreeCtrl catTree)
		{
			// Create the view object for this controller:
			var go = PrefabController.Create ("prefabs", PREFAB, root);
			go.name = PREFAB + " (" + catEntry.category.name + ")";

			var entryCtrl = go.GetComponent<CategoryEntryCtrl> ();
			entryCtrl.categoryEntry = catEntry;
			entryCtrl.UpdateView ();

			// save tree controller:
			entryCtrl.treeCtrl = catTree;

			// save link from category entry model of its ui controller:
			catEntry.ctrl = entryCtrl;

			// add this category to the filter since it is on at start:
			entryCtrl.SetSelectedState (true);
			
			return entryCtrl;
		}

		#endregion


		#region instance stuff

		protected CategoryEntry categoryEntry;

		public Image categorySymbol;

		/// <summary>
		/// Updates the view of a category UI entry.
		/// </summary>
		public void UpdateView ()
		{
			// eventually remove leading product id:
			var productIdStartOfCat = Config.Current.id + ".";
			var catId = categoryEntry.category.id;
			if (catId.StartsWith (productIdStartOfCat)) {
				catId = catId.Substring (productIdStartOfCat.Length);
			}
				
			// set the name of this category entry:
			categoryName.text = categoryEntry.category.name;

			// set the number of elements represented by this category:
			categoryCount.text = ""; // categoryEntry.NumberOfQuests().ToString(); TODO make Config?
			gameObject.SetActive (showMenuItem ());	

			// set symbol for this category:
			categorySymbol.sprite = categoryEntry.category.symbol != null ?
				categoryEntry.category.symbol.GetSprite() :
				null;
			if (categorySymbol.sprite == null) {
				categorySymbol.GetComponent<Image> ().enabled = false;
			}
		}

		protected override bool showMenuItem ()
		{
			var entryVisible = Unfolded || categoryEntry.category.folderName.Equals ("");

			if (Config.Current.ShowEmptyMenuEntries)
			{
				return (entryVisible);
			}

			return (categoryEntry.NumberOfQuests() > 0 && entryVisible);
		}

		public bool _selectedForFilter;

		public void SetSelectedState (bool newState)
		{
			_selectedForFilter = newState;

			// Make the UI reflect selection status & change category filter in quest info manager:
			UpdateView4State();

			if (_selectedForFilter)
			{
				treeCtrl.CategoryFilter.AddCategory(categoryEntry.category);
			}
			else
			{
				treeCtrl.CategoryFilter.RemoveCategory(categoryEntry.category);
			}
		}

		public void UpdateView4State()
		{
			if (_selectedForFilter)
			{
				categoryName.color = new Color(categoryName.color.r, categoryName.color.g, categoryName.color.b, 1f);
				categoryCount.color = new Color(categoryCount.color.r, categoryCount.color.g, categoryCount.color.b, 1f);
				categorySymbol.color = new Color(categorySymbol.color.r, categorySymbol.color.g, categorySymbol.color.b, 1f);
			}
			else
			{
				categoryName.color = new Color(categoryName.color.r, categoryName.color.g, categoryName.color.b,
					Config.Current.disabledAlpha);
				categoryCount.color = new Color(categoryCount.color.r, categoryCount.color.g, categoryCount.color.b,
					Config.Current.disabledAlpha);
				categorySymbol.color = new Color(categorySymbol.color.r, categorySymbol.color.g, categorySymbol.color.b,
					Config.Current.disabledAlpha);
			}
		}

		public void ToggleSelectedState ()
		{
			SetSelectedState (!_selectedForFilter);
		}

		public void Show (bool show)
		{
			gameObject.SetActive (show);
		}

		private bool shownInUpdate = false;

		#endregion

	}
}