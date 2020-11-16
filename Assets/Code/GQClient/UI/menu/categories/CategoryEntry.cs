using System.Collections.Generic;
using Code.GQClient.Conf;

namespace Code.GQClient.UI.menu.categories
{
    public class CategoryEntry
    {
        public Category category;
        List<int> questIds;

        /// Will be set by the ui controller, when it is created from prefab, c.f. the Create() function in the CategoryEntryCtrl class.
        public CategoryEntryCtrl ctrl;

        public CategoryEntry(Category category)
        {
            this.category = category;
            this.questIds = new List<int>();
            this.ctrl = null;
        }

        /// <summary>
        /// Adds a quest to be represented by this category entry (without duplicates).
        /// </summary>
        /// <param name="questID">Quest I.</param>
        public void AddQuestID(int questID)
        {
            if (!questIds.Contains(questID))
            {
                questIds.Add(questID);
            }
        }

        /// <summary>
        /// Removes a quest as being respresented by this catgeory:
        /// </summary>
        /// <param name="questID">Quest I.</param>
        public void RemoveQuestID(int questID)
        {
            if (questIds.Contains(questID))
            {
                questIds.Remove(questID);
            }
        }

        /// <summary>
        /// Returns the number of quests currently represented by this category.
        /// </summary>
        /// <returns>The of quests.</returns>
        public int NumberOfQuests()
        {
            return questIds.Count;
        }
    }
}