using System.Collections.Generic;

namespace Code.GQClient.UI.menu.categories
{
    public class CategoryFolder
    {
        public string Name;

        /// Will be set by the ui controller, when it is created from prefab, c.f. the Create() function in the CategoryEntryCtrl class.
        public CategoryFolderCtrl ctrl;

        public List<CategoryEntry> Entries;

        public CategoryFolder(string name)
        {
            Name = name;
            Entries = new List<CategoryEntry>();
            ctrl = null;
        }

        public void AddCategoryEntry(CategoryEntry entry)
        {
            Entries.Add(entry);
        }

        public void RemoveCategoryEntry(CategoryEntry entry)
        {
            Entries.Remove(entry);
        }

        /// <summary>
        /// Returns the number of quests currently represented by this category.
        /// </summary>
        /// <returns>The of quests.</returns>
        public int NumberOfQuests()
        {
            int nr = 0;
            foreach (CategoryEntry catEntry in Entries)
            {
                nr += catEntry.NumberOfQuests();
            }

            return nr;
        }
    }
}