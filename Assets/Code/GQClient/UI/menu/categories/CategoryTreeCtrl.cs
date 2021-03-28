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

        private string catSetName;

        // Use this for initialization
        void Start()
        {
            qim = QuestInfoManager.Instance;
//			CategoryFilter = qim.CategoryFilter; // TODO use CatSet instead
            qim.DataChange.AddListener(OnQuestInfoChanged);
            RTConfig.RTConfigChanged.AddListener(UpdateView);

            // fold the categories of this set (up to now they are unfolded):
            if (Config.Current.categoryFiltersStartFolded)
            {
                ToggleMenuView();
            }
        }

        private const string PREFAB = "CategoryTree";
        internal QuestInfoFilter.CategoryFilter CategoryFilter;

        public static CategoryTreeCtrl Create(GameObject root, QuestInfoFilter.CategoryFilter catFilter,
            List<Category> categories)
        {
            string catTreeGoName = PREFAB + " (" + catFilter.Name + ")";

            // delete game object if already existing:
            Transform oldExisting = root.transform.Find(catTreeGoName);
            if (oldExisting != null && oldExisting.gameObject.activeSelf)
            {
                oldExisting.gameObject.SetActive(false);
                DestroyImmediate(oldExisting.gameObject);
            }

            // Create the view object for this controller:
            GameObject go = PrefabController.Create("prefabs", PREFAB, root);
            go.name = catTreeGoName;

            // save tree controller & folder:
            CategoryTreeCtrl treeCtrl = go.GetComponent<CategoryTreeCtrl>();

            treeCtrl.CategoryFilter = catFilter;
            treeCtrl.Title.text = catFilter.Name;
            treeCtrl.gameObject.SetActive(true);
            treeCtrl.catSetName = catFilter.Name;

            treeCtrl.recreateUI();

            return treeCtrl;
        }

        #endregion


        #region Runtime API

        public void ToggleMenuView()
        {
            bool foldable = Config.Current.foldableCategoryFilters;
            if (!foldable || transform.childCount < 3)
                // if we can't fold or do not have entries (the header is always there) we skip this:
                return;

            // we use the first folder, i.e. the second child, or child nr. 1 (counting starts with 0) to determine the showstate:
            bool currentShowState = transform.GetChild(1).GetComponent<CategoryFolderCtrl>().ShowState;

            // toggle show for all folders:
            for (int i = 1; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                CategoryFolderCtrl folderCtrl = child.GetComponent<CategoryFolderCtrl>();
                if (folderCtrl != null)
                {
                    folderCtrl.Show(!currentShowState);
                }
            }
        }

        #endregion


        #region React on Events

        private void OnQuestInfoChanged(QuestInfoChangedEvent e)
        {
            switch (e.ChangeType)
            {
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


        public void UpdateView()
        {
            if (this == null || CategoryFilter == null)
            {
                return;
            }

            // pause filter change events:
            CategoryFilter.NotificationPaused = true;

            recreateUI();

            // reactivate filter change events after pause:
            CategoryFilter.NotificationPaused = false;
        }

        Dictionary<string, CategoryEntry> categoryEntries = new Dictionary<string, CategoryEntry>();
        internal Dictionary<string, CategoryFolder> categoryFolders = new Dictionary<string, CategoryFolder>();

        private void recreateUI()
        {
            // 1. Remove UI elements in Category Tree:
            foreach (Transform child in transform.Cast<Transform>().ToArray())
            {
                // Transform child = transform.GetChild (i);
                if (null != child.GetComponent<CategoryEntryCtrl>() || null != child.GetComponent<CategoryFolderCtrl>())
                {
                    // if child is an category entry we delete it:
                    DestroyImmediate(child.gameObject);
                }
            }

            // 2. Clear data:
            categoryEntries = new Dictionary<string, CategoryEntry>();
            categoryFolders = new Dictionary<string, CategoryFolder>();
            CategorySet catSet =
                Config.Current.CategorySets.Find(cs => cs.name == catSetName);

            // 3. Regenerate data: create skeleton of folders and entries:
            foreach (Category c in catSet.categories)
            {
                // create and add the new category entry:
                CategoryEntry catEntry = new CategoryEntry(c);
                categoryEntries.Add(c.id, catEntry);
                // Put the new entry in adequate folder and create the folder before if needed
                CategoryFolder catFolder;
                if (!categoryFolders.TryGetValue(c.folderName, out catFolder))
                {
                    catFolder = new CategoryFolder(c.folderName);
                    categoryFolders.Add(c.folderName, catFolder);
                }

                // add this category to its folder:
                catFolder.AddCategoryEntry(catEntry);
            }

            List<QuestInfo> questInfos = QuestInfoManager.Instance.GetListOfQuestInfos();

            // model: populate entries with quest infos:
            foreach (QuestInfo info in questInfos)
            {
                foreach (string catId in info.Categories)
                {
                    string cat = catId.StripQuotes();
                    CategoryEntry catEntry;
                    if (!categoryEntries.TryGetValue(cat, out catEntry))
                    {
                        // if a quest has an unknown category we skip that:
                        Log.SignalErrorToAuthor($"Quest {info.Name} {info.Id} has unknown category '{cat}'.");
                        continue;
                    }

                    // we take note of the category of the current quest in our tree model:
                    catEntry.AddQuestID(info.Id);
                }
            }

            // create all category tree UI entries:
            foreach (CategoryFolder folder in categoryFolders.Values)
            {
                CategoryFolderCtrl uiFolder =
                    CategoryFolderCtrl.Create(
                        root: this.gameObject,
                        catFolder: folder,
                        catTree: this
                    );
                uiFolder.transform.SetParent(this.transform);
                uiFolder.transform.SetAsLastSibling();

                foreach (CategoryEntry entry in folder.Entries)
                {
                    CategoryEntryCtrl uiEntry =
                        CategoryEntryCtrl.Create(
                            root: this.gameObject,
                            catEntry: entry,
                            catTree: this
                        );
                    uiEntry.transform.SetParent(this.transform);
                    uiEntry.transform.SetAsLastSibling();
                }
            }
        }

        bool _generalSelectionState = true;

        public void SetSelection4AllItems()
        {
            if (CategoryFilter == null)
            {
                return;
            }

            _generalSelectionState = !_generalSelectionState;
            CategoryFilter.NotificationPaused = true;
            foreach (var entry in categoryEntries.Values)
            {
                entry.ctrl.SetSelectedState(_generalSelectionState);
            }

            CategoryFilter.NotificationPaused = false;

            if (_generalSelectionState)
            {
                OnOff.color = new Color(OnOff.color.r, OnOff.color.g, OnOff.color.b, 1f);
            }
            else
            {
                OnOff.color = new Color(OnOff.color.r, OnOff.color.g, OnOff.color.b,
                    Config.Current.disabledAlpha);
            }
        }

        #endregion
    }
}