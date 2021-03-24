using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code.GQClient.Conf;
using Code.GQClient.UI.author;

// ReSharper disable All

namespace GQClient.Model
{
    public abstract partial class QuestInfoFilter
    {
        public delegate void OnFilterChanged();

        public event OnFilterChanged FilterChange;

        protected void RaiseFilterChangeEvent()
        {
            if (NotificationPaused)
                return;

            if (FilterChange != null)
            {
                FilterChange();
            }

            if (ParentFilter != null)
                ParentFilter.RaiseFilterChangeEvent();
        }

        bool _notificationPaused = false;

        public bool NotificationPaused
        {
            get { return _notificationPaused; }
            set
            {
                _notificationPaused = value;
                RaiseFilterChangeEvent();
            }
        }

        protected QuestInfoFilter ParentFilter { get; set; }

        public abstract bool Accept(QuestInfo qi);

        public abstract List<string> AcceptedCategories(QuestInfo qi);

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
                IsActive = !Config.Current.ShowHiddenQuests;
                Author.SettingsChanged +=
                    (object sender, System.EventArgs e) =>
                    {
                        IsActive = !Config.Current.ShowHiddenQuests;
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
                get { return _isActive; }
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

            private Dictionary<string, List<string>> _catIds;

            /// <summary>
            /// We use a conjunctive normal form of category ids stored in this dictionary. All "normal" categories are
            /// kept in the category list named "standard", which is the standard disjunction. For each folder whose name begins with an asterisk (*)
            /// we store the contained category ids as a separate list i.e. another disjunction.
            /// </summary>
            private Dictionary<string, List<string>> catIds
            {
                get
                {
                    if (_catIds == null)
                    {
                        _catIds = new Dictionary<string, List<string>>();
                        _catIds["standard"] = new List<string>();
                    }

                    return _catIds;
                }
            }

            protected Dictionary<string, List<string>> staticFullCatIdList;

            public CategoryFilter(CategorySet catSet)
            {
                NotificationPaused = true;

                staticFullCatIdList = new Dictionary<string, List<string>>();
                catIds["standard"] = new List<string>();
                staticFullCatIdList["standard"] = new List<string>();
                foreach (Category c in catSet.categories)
                {
                    if (c.folderName.StartsWith("*"))
                    {
                        if (!catIds.ContainsKey(c.folderName))
                        {
                            catIds[c.folderName] = new List<string>();
                        }

                        if (!staticFullCatIdList.ContainsKey(c.folderName))
                        {
                            staticFullCatIdList[c.folderName] = new List<string>();
                        }

                        staticFullCatIdList[c.folderName].Add(c.id);
                    }
                    else
                    {
                        staticFullCatIdList["standard"].Add(c.id);
                    }

                    AddCategory(c);
                }

                NotificationPaused = false;
                Name = catSet.name;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="QuestInfoFilter"/> class.
            /// </summary>
            /// <param name="firstCategory">First category to be accepted by this filter.</param>
            /// <param name="categories">Further categories to be accepted. In fact you can simply specify any number of acceptable categories in one row.</param>
            public CategoryFilter(Category firstCategory, params Category[] categories)
            {
                NotificationPaused = true;
                AddCategory(firstCategory);
                foreach (Category c in categories)
                    AddCategory(c);
                NotificationPaused = false;
            }

            public void AddCategory(Category category)
            {
                string catDisjunctionName = "standard";
                if (category.folderName.StartsWith("*"))
                {
                    catDisjunctionName = category.folderName;
                }

                if (!catIds[catDisjunctionName].Contains(category.id))
                {
                    catIds[catDisjunctionName].Add(category.id);
                    RaiseFilterChangeEvent();
                }
            }

            public void RemoveCategory(Category category)
            {
                string catDisjunctionName = "standard";
                if (category.folderName.StartsWith("*"))
                {
                    catDisjunctionName = category.folderName;
                }

                if (catIds[catDisjunctionName].Contains(category.id))
                {
                    catIds[catDisjunctionName].Remove(category.id);
                    RaiseFilterChangeEvent();
                }
            }

            public void ClearCategories()
            {
                _catIds = null;
                RaiseFilterChangeEvent();
            }

            /// <summary>
            /// Accepts the given quest info when at least one of the quests categories is mentioned as
            /// accepted category for each
            /// disjunction of categories.
            ///
            /// We have conjunctive normal form of categories here, i.e. we have a conjunction of
            /// disjunctions. Here we proceed the conjunction, i.e. the outer loop.
            /// </summary>
            /// <param name="qi">Quest Info.</param>
            public override bool Accept(QuestInfo qi)
            {
                // This is the conjunction:
                foreach (var disjunctionName in catIds.Keys)
                {
                    var catList = catIds[disjunctionName];
                    // skip empty disjunctions inside the conjunction,
                    // i.e. if not any one category of this disjunction is active, any quest info is accepted.
                    if (Config.Current.showAllIfNoCatSelectedInFilter && catList.Count == 0)
                    {
                        continue;
                    }

                    if (!acceptedInCatDisjunction(qi, disjunctionName))
                    {
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// Checks acceptance of given QuestInfo for one disjunction of categories given by the catList. 
            /// </summary>
            /// <param name="qi"></param>
            /// <param name="catList"></param>
            /// <returns></returns>
            private bool acceptedInCatDisjunction(QuestInfo qi, string disjunctionName)
            {
                List<string> catList = catIds[disjunctionName];
                List<string> fullCatList = staticFullCatIdList[disjunctionName];

                // if (ConfigurationManager.Current.showAllIfNoCatSelectedInFilter)
                // {

                // skip if the quest info does not contain any of the categories of this disjunction,
                // i.e. it is unspecific regarding this group of categories:
                bool qiDoesNotContainAnyCatOfThisGroup = true;
                foreach (string cat in fullCatList)
                {
                    if (qi.Categories.Contains(cat))
                    {
                        qiDoesNotContainAnyCatOfThisGroup = false;
                        break;
                    }
                }

                if (qiDoesNotContainAnyCatOfThisGroup)
                {
                   return Config.Current.showIfNoCatDefined;
                    // in this case the quest info DOES NOT CONTAIN ANY of the categories of this group and
                    // though should be shown if specified so in option 'showIfNoCatDefined'.
                }
                // }


                foreach (var cat in catList)
                {
                    if (qi.Categories.Contains(cat) ||
                        (
                            qi.NewVersionOnServer != null && qi.NewVersionOnServer.Categories.Contains(cat)
                        )
                    )
                    {
                        return true;
                    }
                }

                return false;
            }

            public override string ToString()
            {
                var sb = new StringBuilder("Category is in { ");

                var catLists = catIds.Values.ToList();

                for (int i = 0; i < catLists.Count; i++)
                {
                    sb.Append(" { ");
                    for (var j = 0; j < catLists[i].Count; j++)
                    {
                        sb.Append(catLists[i][j]);
                        if (j + 1 < catLists[i].Count)
                        {
                            sb.Append(" or ");
                        }
                    }

                    sb.Append(" } ");
                    if (i + 1 < catIds.Values.Count)
                    {
                        sb.Append(" and ");
                    }
                }

                sb.Append(" }.");
                return sb.ToString();
            }

            public override List<string> AcceptedCategories(QuestInfo qi)
            {
                var accCats = new List<string>();
                foreach (var cat in catIds["standard"])
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
                        filter.ParentFilter = this;
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
                if (Config.Current.showAllIfNoCatSelectedInFilter &&
                    (subfilters == null || subfilters.Count == 0))
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