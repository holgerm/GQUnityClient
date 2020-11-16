using Code.GQClient.Conf;
using Code.QM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.menu.categories
{

	public class CategoryFolderCtrl : CategoryCtrl
	{

		#region static stuff

		protected static readonly string PREFAB = "CategoryFolder";

		public static CategoryFolderCtrl Create (GameObject root, CategoryFolder catFolder, CategoryTreeCtrl catTree)
		{
			// Create the view object for this controller:
			var go = PrefabController.Create ("prefabs", PREFAB, root);
			go.name = PREFAB + " (" + catFolder.Name + ")";

			// save tree controller & folder:
			var folderCtrl = go.GetComponent<CategoryFolderCtrl> ();
			folderCtrl.treeCtrl = catTree;
			folderCtrl.folder = catFolder;

			// set the back link from the model to this controller:
			catFolder.ctrl = folderCtrl;

			// initialize the UI Entry for this folder:
			folderCtrl.ShowState = ConfigurationManager.CurrentRT.categoryFiltersStartFolded;
			folderCtrl.UpdateView (catFolder);

			// hook the show/hide children method onto the image toggle button of this folder:
			var itb = folderCtrl.folderImage.GetComponent<ImageToggleButton> ();
			//itb.ToggleButton.onClick.AddListener (itb.Toggle);
			itb.ToggleButton.onClick.AddListener (folderCtrl.ToggleShowFolderContents);
			// ... and onto the button of the whole entry:
			var folderBtn = folderCtrl.GetComponent<Button> ();
			folderBtn.onClick.AddListener (itb.Toggle);
			folderBtn.onClick.AddListener (folderCtrl.ToggleShowFolderContents);

			return folderCtrl;
		}

		#endregion


		#region Instance stuff

		public Image folderImage;

		CategoryFolder folder;

		/// <summary>
		/// Updates the view of a category UI folder.
		/// </summary>
		protected void UpdateView (CategoryFolder folder)
		{
			// show the folder, according to the current rules:
			gameObject.SetActive (showMenuItem ());
			
			// enable the folder image toggle button:
			var tb = folderImage.GetComponent<ImageToggleButton> ();
			if (tb != null) {
				tb.enabled = true;
			}

			// set the name of this category entry:
			categoryName.text = folder.Name;

			// calculate and set the number of quests represented by all categories within this folder:
			categoryCount.text = ""; // folder.NumberOfQuests().ToString(); TODO make Config?
		}

		/// <summary>
		/// This method will only be called from UnityEvent of folder's image toggle button onClick.
		/// </summary>
		private void ToggleShowFolderContents ()
		{
			// determine this folders show state:
			var stateIsShow = folderImage.GetComponent<ImageToggleButton> ().stateIsOn;

			// set activity of all contained category entries according to folder show state:
			if (treeCtrl.model.categoryFolders.TryGetValue (folder.Name, out var folderCtrl)) {
				foreach (var cat in folderCtrl.Entries) {
					cat.ctrl.Unfolded = stateIsShow;
					cat.ctrl.UpdateView ();
				}
			}
		}

		override protected bool showMenuItem ()
		{
			if (ConfigurationManager.Current.ShowEmptyMenuEntries)
            {
                return (folder.Name != "");
            }
            else
            {
                return (folder.Name != "" && folder.Entries.Count > 0);
            }
        }

		#endregion


		#region Runtime API

		public bool ShowState {
			get;
			private set;
		}

		public void Show (bool show)
		{
			ShowState = show;
			gameObject.SetActive (show && showMenuItem ());
			foreach (var ec in folder.Entries) {
				ec.ctrl.Show (show && (ec.ctrl.Unfolded || folder.Name == ""));
			}
		}

		#endregion
	}
}