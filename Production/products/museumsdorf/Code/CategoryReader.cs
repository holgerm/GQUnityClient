using System.Collections.Generic;
using Code.GQClient.Conf;
using Code.GQClient.Util;

namespace GQClient.Model
{

    public class CategoryReader
    {

        public static List<string> ReadCategoriesFromMetadata(MetaDataInfo[] metadata)
        {

            var categories = new List<string>();
            string netVal;
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
            if (categories.Count == 0 && ConfigurationManager.Current.defaultCategory != null)
            {
                categories.Add(ConfigurationManager.Current.defaultCategory);
            }
            return categories;
        }
    }

}