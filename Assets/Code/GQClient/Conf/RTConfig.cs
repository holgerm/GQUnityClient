using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Code.GQClient.Err;
using Code.GQClient.UI.author;
using Code.GQClient.UI.map;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

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

        public static bool __JSON_Currently_Parsing = false;

        /// <summary>
        /// Used during deserialization of json file. Shows whether the json text used in-app resources (true) or
        /// application persistent files (false). Used to put that information e.g. into ImagePath objects.
        /// </summary>
        public static LoadsFrom CurrentLoadingMode;

        public enum LoadsFrom
        {
            Resource,
            LocalFile,
            RemoteFile
        }

        public static RTConfig _doDeserialize(string configText, LoadsFrom loadMode)
        {
            __JSON_Currently_Parsing = true;
            CurrentLoadingMode = loadMode;
            RTConfig config = JsonConvert.DeserializeObject<RTConfig>(configText);
            config.RefreshCategoryDictionary();
            __JSON_Currently_Parsing = false;
            return config;
        }

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

        public List<CategorySet> CategorySets
        {
            get
            {
                if (_categorySets == null)
                {
                    _categorySets = new List<CategorySet>();
                    return _categorySets;
                }

                categoryDict = new Dictionary<string, Category>();
                foreach (CategorySet cs in _categorySets)
                {
                    foreach (Category c in cs.categories)
                    {
                        categoryDict[c.id] = c;
                    }
                }

                return _categorySets;
            }
            set { _categorySets = value; }
        }

        internal void RefreshCategoryDictionary()
        {
            categoryDict = new Dictionary<string, Category>();
            foreach (CategorySet cs in _categorySets)
            {
                foreach (Category c in cs.categories)
                {
                    categoryDict[c.id] = c;
                }
            }
        }

        public string defaultCategory { get; set; }

        [JsonIgnore] private List<CategorySet> _categorySets;

        public Category GetCategory(string catId)
        {
            return categoryDict[catId];
        }

        [JsonIgnore] internal Dictionary<string, Category> categoryDict;


        /// <summary>
        /// Initializes a new instance of the <see cref="RTConfig"/> class and initializes it with generic default values.
        /// 
        /// This constructor is used by the ProductManager (explicit) as well as the JSON.Net deserialize method (via reflection).
        /// </summary>
        public RTConfig()
        {
            // set default values:
            categoryDict = new Dictionary<string, Category>();
            foldableCategoryFilters = true;
            categoryFiltersStartFolded = true;
            categoryFoldersStartFolded = true;
        }
    }
}