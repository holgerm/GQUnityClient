using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using GQ.Client.Conf;
using GQ.Client.Util;
using GQ.Client.Err;
using UnityEngine.UI;

namespace GQ.Client.UI {

	public class CategoryTreeCtrl : MonoBehaviour {

		public Text Title;
		public Text Number;
		public Image OnOff;

		private QuestInfoManager qim;

		public QuestInfoFilter.Category CategoryFilter;


		// Use this for initialization
		void Start () {
			qim = QuestInfoManager.Instance;
			CategoryFilter = new QuestInfoFilter.Category ();
			qim.FilterAnd(CategoryFilter);
			qim.OnDataChange += OnQuestInfoChanged;
		}
		
		#region React on Events

		public void OnQuestInfoChanged (object sender, QuestInfoChangedEvent e)
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
				UpdateView();
				break;							
			}
		}


		public void UpdateView ()
		{
			if (this == null) {
				return;
			}

			// pause filter change events:
			CategoryFilter.NotificationPaused = true;

			// initialize the category entry dictionary:
			categoryEntries = new Dictionary<string, CategoryEntry> ();
			categoryFolders = new Dictionary<string, CategoryFolder> ();
			foreach (Category c in ConfigurationManager.Current.categories) {
				// create and add the new category entry:
				CategoryEntry catEntry = new CategoryEntry (c);
				categoryEntries.Add (c.id, catEntry);
				// Put the new entry in adequate folder and create the folder before if needed
				CategoryFolder catFolder;
				if (!categoryFolders.TryGetValue(c.folderName, out catFolder)) {
					catFolder = new CategoryFolder (c.folderName);
					categoryFolders.Add (c.folderName, catFolder);
				}
				catFolder.AddCategoryEntry (catEntry);
			}

			// Build the internal category tree model:
			foreach (QuestInfo info in QuestInfoManager.Instance.GetListOfQuestInfos()) {
				foreach (string catId in info.Categories) {
					string cat = catId.StripQuotes ();
					CategoryEntry catEntry;
					if (!categoryEntries.TryGetValue (cat, out catEntry)) {
						// if a quest has an unknown category we skip that:
						Log.SignalErrorToAuthor("Quest {0} has unkown category {1}.", info.Name, cat);
						continue;
					}
					// we take note of the catgeory of the current quest in our tree model:
					catEntry.AddQuestID (info.Id);
				}
			}

			// delete all category entry UI elements:
			for (int i = 1; i < transform.childCount; i++) {
				Transform child = transform.GetChild (i);
				if (child.GetComponent<CategoryEntryCtrl> () != null) {
					// if child is an category entry we delete it:
					Destroy (child);
				}
			}

			// create all category tree UI entries:
			foreach (CategoryFolder folder in categoryFolders.Values) {
				CategoryFolderCtrl uiFolder = 
					CategoryFolderCtrl.Create (
						root: this.gameObject,
						catFolder: folder,
						catTree: this
					);
				uiFolder.transform.parent = this.transform;
				uiFolder.transform.SetAsLastSibling ();

				foreach (CategoryEntry entry in folder.Entries) {
					CategoryEntryCtrl uiEntry = 
						CategoryEntryCtrl.Create (
							root: this.gameObject,
							catEntry: entry,
							catTree: this
						);
					uiEntry.transform.parent = this.transform;
					uiEntry.transform.SetAsLastSibling ();
				}
			}

			// reactivate filter change events after pause:
			CategoryFilter.NotificationPaused = false;

			// set the number of all quests represented by the currently selected categories
			int nr = 0;
			foreach (CategoryFolder folder in categoryFolders.Values) {
				nr += folder.NumberOfQuests ();
			}
			Number.text = ""; // nr.ToString (); TODO make Config?
 		}

		bool generalSelectionState = true;

		public void SetSelection4AllItems() {
			generalSelectionState = !generalSelectionState;
			CategoryFilter.NotificationPaused = true;
			foreach (CategoryEntry entry in categoryEntries.Values) {
				entry.ctrl.SetSelectedState (generalSelectionState);
			}
			CategoryFilter.NotificationPaused = false;

			if (generalSelectionState) {
				OnOff.color = new Color(OnOff.color.r, OnOff.color.g, OnOff.color.b, 1f);
			}
			else {
				OnOff.color = new Color(OnOff.color.r, OnOff.color.g, OnOff.color.b, ConfigurationManager.Current.disabledAlpha);
			}
		}
			
		#endregion


		#region Internal Model of Tree

		public Dictionary<string, CategoryFolder> categoryFolders;

		public class CategoryFolder {
			public string Name;

			public List<CategoryEntry> Entries;

			public CategoryFolder(string name) {
				Name = name;
				Entries = new List<CategoryEntry>();
			}

			public void AddCategoryEntry(CategoryEntry entry) {
				Entries.Add (entry);
			}

			public void RemoveCategoryEntry(CategoryEntry entry) {
				Entries.Remove (entry);
			}

			/// <summary>
			/// Returns the number of quests currently represented by this category.
			/// </summary>
			/// <returns>The of quests.</returns>
			public int NumberOfQuests() {
				int nr = 0;
				foreach (CategoryEntry catEntry in Entries) {
					nr += catEntry.NumberOfQuests ();
				}
				return nr;
			}
		}

		protected Dictionary<string, CategoryEntry> categoryEntries;

		public class CategoryEntry {
			public Category category;
			List<int> questIds;

			/// Will be set by the ui controller, when it is created from prefab, c.f. the Create() function in the CategoryEntryCtrl class.
			public CategoryEntryCtrl ctrl;

			public CategoryEntry(Category category) {
				this.category = category;
				this.questIds = new List<int>();
				this.ctrl = null;
			}

			/// <summary>
			/// Adds a quest to be represented by this category entry (without duplicates).
			/// </summary>
			/// <param name="questID">Quest I.</param>
			public void AddQuestID(int questID) {
				if (!questIds.Contains(questID)) {
					questIds.Add (questID);
				}
			}

			/// <summary>
			/// Removes a quest as being respresented by this catgeory:
			/// </summary>
			/// <param name="questID">Quest I.</param>
			public void RemoveQuestID(int questID) {
				if (questIds.Contains(questID)) {
					questIds.Remove (questID);
				}
			}

			/// <summary>
			/// Returns the number of quests currently represented by this category.
			/// </summary>
			/// <returns>The of quests.</returns>
			public int NumberOfQuests() {
				return questIds.Count;
			}
		}

		#endregion
	}
}