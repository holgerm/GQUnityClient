using System.Collections.Generic;
using System.Linq;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using GQClient.Model;
using Code.GQClient.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.menu.categories
{

    public class CategoryTreeCtrl : MonoBehaviour
	{
		#region Inspector & Initialization

		public TextMeshProUGUI Title;
		public TextMeshProUGUI Hint;
		public Image OnOff;

		private QuestInfoManager qim;

		// Use this for initialization
		void Start ()
		{
			qim = QuestInfoManager.Instance;
//			CategoryFilter = qim.CategoryFilter; // TODO use CatSet instead
			qim.OnDataChange += OnQuestInfoChanged;
			ConfigurationManager.OnRTConfigChanged += UpdateView;

			// fold the categories of this set (up to now they are unfolded):
			if (ConfigurationManager.CurrentRT.categoryFiltersStartFolded) {
				ToggleMenuView ();
			}
		}

		protected static readonly string PREFAB = "CategoryTree";
		internal CategoryTree model;

		public static CategoryTreeCtrl Create (GameObject root, QuestInfoFilter.CategoryFilter catFilter, List<Category> categories)
		{
			// Create the view object for this controller:
			GameObject go = PrefabController.Create ("prefabs", PREFAB, root);
			go.name = PREFAB + " (" + catFilter.Name + ")";

			// save tree controller & folder:
			CategoryTreeCtrl treeCtrl = go.GetComponent<CategoryTreeCtrl> ();
			
			treeCtrl.model = new CategoryTree(categories, catFilter);
   			treeCtrl.Title.text = catFilter.Name;
			treeCtrl.gameObject.SetActive (true);

			return treeCtrl;
		}

		#endregion


		#region Runtime API

		public void ToggleMenuView ()
		{
			bool foldable = ConfigurationManager.CurrentRT.foldableCategoryFilters;
			if (!foldable || transform.childCount < 3)
				// if we can't fold or do not have entries (the header is always there) we skip this:
				return;

			// we use the first folder, i.e. the second child, or child nr. 1 (counting starts with 0) to determine the showstate:
			bool currentShowState = transform.GetChild (1).GetComponent<CategoryFolderCtrl> ().ShowState;

			// toggle show for all folders:
			for (int i = 1; i < transform.childCount; i++) {
				Transform child = transform.GetChild (i);
				CategoryFolderCtrl folderCtrl = child.GetComponent<CategoryFolderCtrl> ();
				if (folderCtrl != null) {
					folderCtrl.Show (!currentShowState);
				}
			}
		}

		#endregion


		#region React on Events

		private void OnQuestInfoChanged (object sender, QuestInfoChangedEvent e)
		{
			switch (e.ChangeType) {
			case ChangeType.AddedInfo:
				// Create new Category Entry and add it to the tree:
				// TODO
				break;
			case ChangeType.ChangedInfo:
				// Change details of existing category entry based n details of the change:
				// TODO
				break;
			case ChangeType.RemovedInfo:
				// Count down one element for according category entry and eventually remove the entry from the visible tree:
				// TODO
				break;							
			case ChangeType.ListChanged:
				// Remove the complete existing category tree and build up a new one based on all quest infos of the quest info manager:
				UpdateView ();
				break;							
			}
		}


		public void UpdateView ()
		{
			Debug.Log("Updating CatTreeView".Yellow());
			if (this == null || model.CategoryFilter == null) {
				return;
			}

			// pause filter change events:
			model.CategoryFilter.NotificationPaused = true;

			model.RecreateModelTree ();
			recreateUI ();

			// reactivate filter change events after pause:
			model.CategoryFilter.NotificationPaused = false;
			Debug.Log("Updating CatTreeView DONE".Yellow());
		}

		private void recreateUI ()
		{
			// delete all category entry UI elements:
			foreach (Transform child in transform.Cast<Transform>().ToArray()) {
				// Transform child = transform.GetChild (i);
				if (null != child.GetComponent<CategoryEntryCtrl> () || null != child.GetComponent<CategoryFolderCtrl> ()) {
					// if child is an category entry we delete it:
					DestroyImmediate(child.gameObject);
				}
			}

			// create all category tree UI entries:
			foreach (CategoryFolder folder in model.GetFolders()) {
//				if (folder.Name != "") {
				CategoryFolderCtrl uiFolder = 
					CategoryFolderCtrl.Create (
						root: this.gameObject,
						catFolder: folder,
						catTree: this
					);
				uiFolder.transform.SetParent (this.transform);
				uiFolder.transform.SetAsLastSibling ();
//				}

				foreach (CategoryEntry entry in folder.Entries) {
					CategoryEntryCtrl uiEntry = 
						CategoryEntryCtrl.Create (
							root: this.gameObject,
							catEntry: entry,
							catTree: this
						);
					uiEntry.transform.SetParent (this.transform);
					uiEntry.transform.SetAsLastSibling ();
				}
			}
		}

		bool generalSelectionState = true;

		public void SetSelection4AllItems ()
		{
			if (model.CategoryFilter == null)
			{
				return;
			}

			generalSelectionState = !generalSelectionState;
			model.CategoryFilter.NotificationPaused = true;
			foreach (var entry in model.GetEntries()) {
				entry.ctrl.SetSelectedState (generalSelectionState);
			}
			model.CategoryFilter.NotificationPaused = false;

			if (generalSelectionState) {
				OnOff.color = new Color (OnOff.color.r, OnOff.color.g, OnOff.color.b, 1f);
			} else {
				OnOff.color = new Color (OnOff.color.r, OnOff.color.g, OnOff.color.b, ConfigurationManager.Current.disabledAlpha);
			}
		}

		#endregion
	}
}