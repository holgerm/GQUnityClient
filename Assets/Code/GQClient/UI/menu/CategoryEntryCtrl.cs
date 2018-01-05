using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.UI;
using UnityEngine.UI;
using GQ.Client.Conf;
using QM.UI;
using QM.Util;

namespace GQ.Client.UI {

	public class CategoryEntryCtrl : CategoryCtrl {

		#region static stuff

		protected static readonly string PREFAB = "CategoryEntry";

		public static CategoryEntryCtrl Create (GameObject root, CategoryTreeCtrl.CategoryEntry catEntry, CategoryTreeCtrl catTree)
		{
			// Create the view object for this controller:
			GameObject go = PrefabController.Create (PREFAB, root);
			go.name = PREFAB + " (" + catEntry.category.name + ")";
			CategoryEntryCtrl entryCtrl = go.GetComponent<CategoryEntryCtrl> ();
			entryCtrl.categoryEntry = catEntry;
			entryCtrl.UpdateView ();

			// save tree controller:
			entryCtrl.treeCtrl = catTree;

			// save link from category entry model of its ui controller:
			catEntry.ctrl = entryCtrl;

			// add this category to the filter since it is on at start:
			entryCtrl.SetSelectedState(true);

			return entryCtrl;
		}

		#endregion


		#region instance stuff

		protected CategoryTreeCtrl.CategoryEntry categoryEntry;

		public Image categorySymbol;

		/// <summary>
		/// Updates the view of a category UI entry.
		/// </summary>
		public void UpdateView() {
			// eventually remove leading product id:
			string productIDStartOfCat = ConfigurationManager.Current.id + ".";
			string catId = categoryEntry.category.id;
			if (catId.StartsWith(productIDStartOfCat)) {
				catId = catId.Substring (productIDStartOfCat.Length);
			}
				
			// set the name of this category entry:
			categoryName.text = categoryEntry.category.name;

			// set the number of elements represented by this ctaegory:
			categoryCount.text = ""; // categoryEntry.NumberOfQuests().ToString(); TODO make Config?
			gameObject.SetActive (showMenuItem());	

			// set symbol for this category:
			categorySymbol.sprite = Resources.Load<Sprite>(categoryEntry.category.symbol.path);
			if (categorySymbol.sprite == null)
			{
				categorySymbol.GetComponent<Image> ().enabled = false;
			}
		}

		override protected bool showMenuItem() {
			if (ConfigurationManager.Current.showEmptyMenuEntries)
				return (Unfolded);
			else
				return (Unfolded && categoryEntry.NumberOfQuests () > 0);
		}

		bool selectedForFilter;

		public void SetSelectedState(bool newState) {
			selectedForFilter = newState;

			// Make the UI reflect selection status & change category filter in quest info manager:
			if (selectedForFilter) {
				categoryName.color = new Color(categoryName.color.r, categoryName.color.g, categoryName.color.b, 1f);
				categoryCount.color = new Color(categoryCount.color.r, categoryCount.color.g, categoryCount.color.b, 1f);
				categorySymbol.color = new Color(categorySymbol.color.r, categorySymbol.color.g, categorySymbol.color.b, 1f);
				treeCtrl.CategoryFilter.AddCategory (categoryEntry.category.id);
			}
			else {
				categoryName.color = new Color(categoryName.color.r, categoryName.color.g, categoryName.color.b, ConfigurationManager.Current.disabledAlpha);
				categoryCount.color = new Color(categoryCount.color.r, categoryCount.color.g, categoryCount.color.b, ConfigurationManager.Current.disabledAlpha);
				categorySymbol.color = new Color(categorySymbol.color.r, categorySymbol.color.g, categorySymbol.color.b, ConfigurationManager.Current.disabledAlpha);
				treeCtrl.CategoryFilter.RemoveCategory (categoryEntry.category.id);
			}
		}

		public void ToggleSelectedState () {
			SetSelectedState (!selectedForFilter);
		}

		#endregion

	}
}