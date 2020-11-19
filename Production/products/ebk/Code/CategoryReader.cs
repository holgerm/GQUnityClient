using System.Collections.Generic;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Util;
using UnityEngine;

namespace GQClient.Model
{
    public class CategoryReader
    {
        public static List<string> ReadCategoriesFromMetadata(MetaDataInfo[] metadata)
        {
            var categories = new List<string>();
            string netVal = null;
            foreach (var md in metadata)
            {
                switch (md.Key)
                {
                    case "category1":
                        netVal = md.Value.StripQuotes();
                        if (netVal != "")
                            categories.Insert(0, netVal);
                        break;
                    case "category":
                    case "category2":
                    case "category3":
                    case "category4":
                        netVal = md.Value.StripQuotes();
                        if (netVal != "")
                            categories.Add(netVal);
                        break;
                }
            }

            Debug.Log($"categories.Count: {categories.Count}, defCat null?: {null == ConfigurationManager.Current.defaultCategory}".Yellow());
            if (categories.Count == 0 && !string.IsNullOrEmpty(ConfigurationManager.Current.defaultCategory))
            {
                Debug.Log($"DEFAULT CAT USED".Green());
                categories.Add(ConfigurationManager.Current.defaultCategory);
            }
            else
            {
                Debug.Log($"NOT USED other cats# {categories.Count}".Red());

            }

            return categories;
        }
    }
}