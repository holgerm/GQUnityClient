using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.UI;
using UnityEngine.UI;
using GQ.Client.Conf;
using QM.UI;

namespace GQ.Client.UI {

	public class CategoryEntryCtrl : MonoBehaviour {

		public Image folderImage;
		public Text categoryName;
		public Text categoryCount;
		public Image categorySymbol;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		protected static readonly string PREFAB = "CategoryEntry";

		protected CategoryTreeCtrl.CategoryEntry categoryEntry;

		public static CategoryEntryCtrl Create (GameObject root, CategoryTreeCtrl.CategoryEntry catEntry, CategoryTreeCtrl catTree)
		{
			// Create the view object for this controller:
			GameObject go = PrefabController.Create (PREFAB, root);
			go.name = PREFAB + " (" + catEntry.category.name + ")";
			CategoryEntryCtrl entryCtrl = go.GetComponent<CategoryEntryCtrl> ();
			entryCtrl.categoryEntry = catEntry;
			entryCtrl.UpdateViewEntry ();
			return entryCtrl;
		}

		/// <summary>
		/// Updates the view of a category UI entry.
		/// </summary>
		protected void UpdateViewEntry() {

			// disable the folder image toggle button:
			ImageToggleButton tb = folderImage.GetComponent<ImageToggleButton> ();
			if (tb != null) {
				tb.enabled = false;
			}

			// eventually remove leading product id:
			string productIDStartOfCat = ConfigurationManager.Current.id + ".";
			string catId = categoryEntry.category.id;
			if (catId.StartsWith(productIDStartOfCat)) {
				catId = catId.Substring (productIDStartOfCat.Length);
			}
				
			// set the name of this category entry:
			categoryName.text = categoryEntry.category.name;

			// set the number of elements represented by this ctaegory:
			categoryCount.text = categoryEntry.NumberOfQuests().ToString();
			gameObject.SetActive (categoryEntry.NumberOfQuests () > 0);

			// set symbol for this category:
			categorySymbol.sprite = Resources.Load<Sprite>(categoryEntry.category.symbol.path);
		}

		public static CategoryEntryCtrl Create (GameObject root, CategoryTreeCtrl.CategoryFolder catFolder, CategoryTreeCtrl catTree)
		{
			// Create the view object for this controller:
			GameObject go = PrefabController.Create (PREFAB, root);
			go.name = PREFAB + " (" + catFolder.Name + ")";
			CategoryEntryCtrl entryCtrl = go.GetComponent<CategoryEntryCtrl> ();
			entryCtrl.categoryEntry = null;
			entryCtrl.UpdateViewFolder (catFolder);
			return entryCtrl;
		}

		/// <summary>
		/// Updates the view of a category UI folder.
		/// </summary>
		protected void UpdateViewFolder(CategoryTreeCtrl.CategoryFolder folder) {
			if (folder.Name == "" || folder.Entries.Count <= 1) {
				// hide the whole folder:
				gameObject.SetActive (false);
				return;
			}

			// show the folder, since we hav at least two entries:
			gameObject.SetActive (true);
			
			// enable the folder image toggle button:
			ImageToggleButton tb = folderImage.GetComponent<ImageToggleButton> ();
			if (tb != null) {
				tb.enabled = true;
			}

			// set the name of this category entry:
			categoryName.text = folder.Name;

			// calculate and set the number of quests represented by all categories within this folder:
			categoryCount.text = folder.NumberOfQuests().ToString();

			// hide symbol for folders:
			categorySymbol.gameObject.SetActive(false);
		}

	}
}