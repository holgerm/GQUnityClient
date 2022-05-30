using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Code.GQClient.Err;
using Code.GQClient.UI.author;
using Code.GQClient.Util.http;
using Code.QM.Util;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

#if UNITY_EDITOR

#endif

namespace Code.GQClient.Conf
{
    /// <summary>
    /// Config class specifies textual parameters of a product. It is used both at runtime to initilize the app's branding details from and 
    /// at editor time to back the product editor view and store the parameters while we use the editor.
    /// </summary>
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class RTConfig
    {
        #region Parse Helper

        private static readonly string rtProductFile = Path.Combine(
            Application.persistentDataPath,
            RT_CONFIG_DIR,
            RT_CONFIG_FILE);

        private static RTConfig _current;

        public static RTConfig Current
        {
            get => _current;
            set
            {
                _current = value;
                RTConfigChanged.Invoke();
            }
        }

        /// <summary>
        /// Loads RTConfig from local version of RTProduct.json. This is done synchronously. 
        /// </summary>
        public static void Load()
        {
            if (File.Exists(rtProductFile))
            {
                Debug.Log($"Reading RTConfig.json from local files");
                string json = File.ReadAllText(rtProductFile);
                Current = doDeserialize(json);
            }
            else
            {
                // no persistent file RTProduct.json found, hence we read from assets:
                TextAsset configAsset = Resources.Load("RTProduct") as TextAsset;

                if (configAsset == null)
                {
                    Log.SignalErrorToDeveloper("Asset RTProduct.json missing. App can not work properly.");
                    Current = new RTConfig(); // this is just a default null-object.
                }
                else
                {
                    Current = doDeserialize(configAsset.text);
                }
            }
        }

        /// <summary>
        /// Loads RTConfig from server version of RTProduct.json. This is done asynchronously
        /// and will call update events when successfully finished. 
        /// </summary>
        public static void LoadFromServer()
        {
            string rtProductUrl = Path.Combine(
                GQ_SERVER_PORTALS_URL,
                Config.Current.id,
                RT_CONFIG_DIR,
                RT_CONFIG_FILE);

            Downloader d = new Downloader(
                rtProductUrl,
                new DownloadHandlerBuffer(),
                timeout: 0,
                rtProductFile,
                verbose: false);
            d.OnSuccess += (dl, e) =>
            {
                Debug.Log($"Reading RTConfig.json from server");

                Current = doDeserialize(dl.WebRequest.downloadHandler.text);
            };
            d.Start();
        }

        private static RTConfig doDeserialize(string configText)
        {
            RTConfig rtConfig = JsonConvert.DeserializeObject<RTConfig>(configText);

            foreach (CategorySet cs in rtConfig.CategorySets)
            {
                foreach (Category c in cs.categories)
                {
                    rtConfig.categoryDict[c.id] = c;
                }
            }

            return rtConfig;
        }

        public static readonly Observable RTConfigChanged = new Observable();

        #endregion

        //////////////////////////////////////////
        // THE ACTUAL PRODUCT RUNTIME CONFIG DATA:

        public bool foldableCategoryFilters { get; set; }

        public bool categoryFiltersStartFolded { get; set; }

        public bool categoryFoldersStartFolded { get; set; }

        /// <summary>
        /// Used as characterization of the quest infos, e.g. to determine the shown symbols in the foyer list.
        /// </summary>
        /// <value>The main category set.</value>
        public string mainCategorySet { get; set; }

        /// <summary>
        /// Shows quests also in the case that at least one category class has no user selections.
        /// We interpret the complete deselection of a category class as "does not matter", so we show all.
        /// 
        /// For apps with only one class of categories this will probably be most often "false", while
        /// for apps with multiple category classes, "true" will probably be the best value.
        /// </summary>
        public bool showAllIfNoCatSelectedInFilter { get; set; }

        /// <summary>
        /// If a quest has no category stated or at least no correctly spelled category, this option decides
        /// whether we show it anyway or not.
        ///
        /// For apps with no special editors so that you have to specify the categories in metadata pages
        /// "true" is best bet, since specifying categories will often be overseen. For apps where the categories
        /// are highlighted during the editing process "false" will be the standard.
        /// In the future we will support error free editors that insist of a valid category definition
        /// before a quest can be published. Hence this option is only needed fot the time being. 
        /// </summary>
        public bool showIfNoCatDefined { get; set; }

        public List<CategorySet> CategorySets
        {
            get
            {
                if (_categorySets == null)
                {
                    _categorySets = new List<CategorySet>();
                    return _categorySets;
                }

                return _categorySets;
            }
            set { _categorySets = value; }
        }

        internal void RefreshCategoryDictionary()
        {
            // Debug.Log("RCD: #1".Yellow());
            // if (null == _categorySets)
            // {
            //     Debug.Log("RCD: #2 return since _catSets is null".Yellow());
            //     return;
            // }
            // _categoryDict = new Dictionary<string, Category>();

            Debug.Log($"RefreshCategoryDictionary(): CatSets#: {CategorySets.Count}");
            foreach (CategorySet cs in CategorySets)
            {
                foreach (Category c in cs.categories)
                {
                    categoryDict[c.id] = c;
                }
            }

            Debug.Log($"RefreshCategoryDictionary(): End: CatDict#: {categoryDict}");
            // CategoriesChanged?.Invoke();
        }

        public string defaultCategory { get; set; }

        [JsonIgnore] private List<CategorySet> _categorySets;

        public Category GetCategory(string catId)
        {
            if (null == categoryDict || !categoryDict.ContainsKey(catId))
            {
                return null;
            }

            return categoryDict[catId];
        }

        [JsonIgnore] internal Dictionary<string, Category> _categoryDict;


        [JsonIgnore]
        internal Dictionary<string, Category> categoryDict
        {
            get
            {
                if (_categoryDict == null)
                {
                    _categoryDict = new Dictionary<string, Category>();
                }

                return _categoryDict;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RTConfig"/> class and initializes it with generic default values.
        /// 
        /// This constructor is used by the ProductManager (explicit) as well as the JSON.Net deserialize method (via reflection).
        /// </summary>
        public RTConfig()
        {
            foldableCategoryFilters = true;
            categoryFiltersStartFolded = true;
            categoryFoldersStartFolded = true;
            showAllIfNoCatSelectedInFilter = false;
            showIfNoCatDefined = false;
        }

        public const string GQ_SERVER_PORTALS_URL = "https://quest-mill-web.intertech.de/portals";
        public static string RT_CONFIG_DIR => /* Author.LoggedIn ? "config-author" : */ "config";
        public const string RT_CONFIG_FILE = "RTProduct.json";
    }
}