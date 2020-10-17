using System.Collections.Generic;
using System.Text;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.UI.author;
using UnityEngine;

namespace GQClient.Model
{

    public abstract partial class QuestInfoFilter
    {

        public delegate void OnFilterChanged();

        public event OnFilterChanged filterChange;

        protected void RaiseFilterChangeEvent()
        {
            if (NotificationPaused)
                return;

            if (filterChange != null)
            {
                filterChange();
            }

            if (parentFilter != null)
                parentFilter.RaiseFilterChangeEvent();
        }

        bool _notificationPaused = false;

        public bool NotificationPaused
        {
            get
            {
                return _notificationPaused;
            }
            set
            {
                _notificationPaused = value;
                RaiseFilterChangeEvent();
            }
        }

        protected QuestInfoFilter parentFilter { get; set; }

        abstract public bool Accept(QuestInfo qi);

        abstract public List<string> AcceptedCategories(QuestInfo qi);

        public string CategoryToShow(QuestInfo qi)
        {
            return AcceptedCategories(qi).Count > 0 ? AcceptedCategories(qi)[0] : QuestInfo.WITHOUT_CATEGORY_ID;
        }


        public class All : QuestInfoFilter
        {

            public override bool Accept(QuestInfo qi)
            {
                return true;
            }

            public override string ToString()
            {
                return "All";
            }

            public override List<string> AcceptedCategories(QuestInfo qi)
            {
                return qi.Categories;
            }
        }

        public abstract class ActivatableFilter : QuestInfoFilter
        {
            protected bool _isActive;

            public bool IsActive
            {
                get => _isActive;
                set
                {
                    _isActive = value;
                    RaiseFilterChangeEvent();
                }
            }
        }

        public class HiddenQuestsFilter : ActivatableFilter
        {

            private static HiddenQuestsFilter _instance;
            public static HiddenQuestsFilter Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new HiddenQuestsFilter();
                    }
                    return _instance;
                }
            }

            private HiddenQuestsFilter()
            {
                IsActive = !ConfigurationManager.Current.ShowHiddenQuests;
                Author.SettingsChanged +=
                    (object sender, System.EventArgs e) =>
                    {
                        IsActive = !ConfigurationManager.Current.ShowHiddenQuests;
                    };
            }

            public override bool Accept(QuestInfo qi)
            {
                return !IsActive || !qi.IsHidden();
            }

            public override string ToString()
            {
                return "HiddenQuestsFilter " + (IsActive ? "(active)" : "(inactive)");
            }


            public override List<string> AcceptedCategories(QuestInfo qi)
            {
                return qi.Categories;
            }
        }

        /// <summary>
        /// If activated it only lets local quests pass through.
        /// </summary>
        public class LocalQuestInfosFilter : QuestInfoFilter
        {
            private static LocalQuestInfosFilter _instance;
            public static LocalQuestInfosFilter Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new LocalQuestInfosFilter();
                    }
                    return _instance;
                }
            }

            private LocalQuestInfosFilter()
            {
                IsActive = Author.ShowOnlyLocalQuests;
            }

            private bool _isActive;
            public bool IsActive
            {
                get
                {
                    return _isActive;
                }
                set
                {
                    _isActive = value;
                    RaiseFilterChangeEvent();
                }
            }

            public override bool Accept(QuestInfo qi)
            {
                return (!IsActive || qi.IsOnDevice);
            }

            public override List<string> AcceptedCategories(QuestInfo qi)
            {
                return qi.Categories;
            }
        }


        public class CategoryFilter : QuestInfoFilter
        {

            public string Name;

            private List<string> acceptedCategories = new List<string>();

            public CategoryFilter(CategorySet catSet)
            {
                NotificationPaused = true;
                foreach (Category c in catSet.categories)
                {
                    AddCategory(c.id);
                }
                NotificationPaused = false;
                Name = catSet.name;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="QuestInfoFilter"/> class.
            /// </summary>
            /// <param name="firstCategory">First category to be accepted by this filter.</param>
            /// <param name="categories">Further categories to be accepted. In fact you can simply sepcify any number of acceptable categories in one row.</param>
            public CategoryFilter(string firstCategory, params string[] categories)
            {
                NotificationPaused = true;
                AddCategory(firstCategory);
                foreach (string c in categories)
                    AddCategory(c);
                NotificationPaused = false;
            }

            public void AddCategories(params string[] categories)
            {
                foreach (string category in categories)
                {
                    if (!acceptedCategories.Contains(category))
                    {
                        acceptedCategories.Add(category);
                    }
                }
                RaiseFilterChangeEvent();
            }

            public void AddCategory(string category)
            {
                if (!acceptedCategories.Contains(category))
                {
                    acceptedCategories.Add(category);
                    RaiseFilterChangeEvent();
                }
            }

            public void RemoveCategory(string category)
            {
                if (acceptedCategories.Contains(category))
                {
                    acceptedCategories.Remove(category);
                    RaiseFilterChangeEvent();
                }
            }

            public void ClearCategories()
            {
                acceptedCategories = new List<string>();
                RaiseFilterChangeEvent();
            }

            /// <summary>
            /// Accepts the given quest info when at least one of the quests categories is mentioned as accepted category of this filter.
            /// </summary>
            /// <param name="qi">Quest Info.</param>
            public override bool Accept(QuestInfo qi)
            {
                foreach (var cat in acceptedCategories)
                {
                    if (qi.Categories.Contains(cat) ||
                        (
                            qi.NewVersionOnServer != null && qi.NewVersionOnServer.Categories.Contains(cat)
                        )
                    )
                        return true;
                }
                return false;
            }

            public override string ToString()
            {
                var sb = new StringBuilder("Category is in {");
                for (var i = 0; i < acceptedCategories.Count; i++)
                {
                    sb.Append(acceptedCategories[i]);
                    if (i + 1 < acceptedCategories.Count)
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append("}");
                return sb.ToString();
            }

            public override List<string> AcceptedCategories(QuestInfo qi)
            {
                var accCats = new List<string>();
                foreach (var cat in acceptedCategories)
                {
                    if (qi.Categories.Contains(cat))
                        accCats.Add(cat);
                }
                return accCats;
            }
        }


        public abstract class Multi : QuestInfoFilter
        {
            protected List<QuestInfoFilter> subfilters = new List<QuestInfoFilter>();
        }


        public class And : Multi
        {

            public And(params QuestInfoFilter[] filters)
            {
                foreach (QuestInfoFilter filter in filters)
                {
                    if (!subfilters.Contains(filter))
                    {
                        subfilters.Add(filter);
                        filter.parentFilter = this;
                    }
                }
            }

            public override bool Accept(QuestInfo qi)
            {
                var accepted = true;

                foreach (var filter in subfilters)
                {
                    accepted &= filter.Accept(qi);
                }

                return accepted;
            }

            public override string ToString()
            {
                var sb = new StringBuilder("And(");

                foreach (var sel in subfilters)
                {
                    sb.Append(sel.ToString() + ",");
                }
                sb.Remove(sb.Length - 1, 1); // remove the last comma
                sb.Append(")");

                return sb.ToString();
            }

            public override List<string> AcceptedCategories(QuestInfo qi)
            {
                // if we have no filters we return all categories:
                if (subfilters == null || subfilters.Count == 0)
                    return qi.Categories;

                var acceptedCategories = new List<string>();

                if (!Accept(qi))
                    return acceptedCategories;

                // the qi is accepted, so we also accept any categories to represent it:
                for (var j = 0; j < subfilters.Count; j++)
                {
                    for (var i = 0; i < subfilters[j].AcceptedCategories(qi).Count; i++)
                    {
                        if (!acceptedCategories.Contains(subfilters[j].AcceptedCategories(qi)[i]))
                        {
                            acceptedCategories.Add(subfilters[j].AcceptedCategories(qi)[i]);
                        }
                    }
                }
                return acceptedCategories;
            }
        }

        public class Or : Multi
        {

            public Or(params QuestInfoFilter[] filters)
            {
                subfilters.AddRange(filters);
            }

            public override bool Accept(QuestInfo qi)
            {
                bool accepted = false;

                foreach (QuestInfoFilter filter in subfilters)
                {
                    accepted |= filter.Accept(qi);
                }

                return accepted;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder("Or(");

                foreach (QuestInfoFilter sel in subfilters)
                {
                    sb.Append(sel.ToString());
                }

                sb.Append(")");

                return sb.ToString();
            }

            public override List<string> AcceptedCategories(QuestInfo qi)
            {
                // if we have no filters we return all categories:
                if (subfilters == null || subfilters.Count == 0)
                    return qi.Categories;

                // all categories which are accepted by any of the filters shoud be contained:
                List<string> acceptedCategories = new List<string>();
                for (int j = 0; j < subfilters.Count; j++)
                {
                    for (int i = 0; i < subfilters[j].AcceptedCategories(qi).Count; i++)
                    {
                        if (!acceptedCategories.Contains(subfilters[j].AcceptedCategories(qi)[i]))
                        {
                            acceptedCategories.Add(subfilters[j].AcceptedCategories(qi)[i]);
                        }
                    }
                }
                return acceptedCategories;
            }
        }
    }
}
