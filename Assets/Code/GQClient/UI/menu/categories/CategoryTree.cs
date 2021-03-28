using System.Collections.Generic;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Util;
using GQClient.Model;
using UnityEngine;

namespace Code.GQClient.UI.menu.categories
{
    public class CategoryTree
    {
        List<Category> categories;
        internal QuestInfoFilter.CategoryFilter CategoryFilter;


        protected Dictionary<string, CategoryEntry> categoryEntries;
        public Dictionary<string, CategoryFolder> categoryFolders;

        public CategoryTree(List<Category> categories, QuestInfoFilter.CategoryFilter categoryFilter)
        {
            this.categories = categories;
            CategoryFilter = categoryFilter;
        }

        public Dictionary<string, CategoryFolder>.ValueCollection GetFolders()
        {
            if (null == categoryFolders)
                return null;

            return categoryFolders.Values;
        }

        public Dictionary<string, CategoryEntry>.ValueCollection GetEntries()
        {
            if (null == categoryEntries)
                return null;

            return categoryEntries.Values;
        }


        internal void RecreateModelTree()
        {
            // model: create skeleton of folders and entries:
            categoryEntries = new Dictionary<string, CategoryEntry>();
            categoryFolders = new Dictionary<string, CategoryFolder>();
            foreach (Category c in categories)
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

                catFolder.AddCategoryEntry(catEntry);
            }

            // model: populate entries with quest infos:
            foreach (QuestInfo info in QuestInfoManager.Instance.GetListOfQuestInfos())
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
        }
    }
}