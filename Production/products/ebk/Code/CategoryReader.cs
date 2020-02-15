using System.Collections.Generic;
using Code.GQClient.Conf;
using Code.GQClient.Util;
using GQClient.Model;

namespace GQClient.Model
{

    public class CategoryReader
    {

        public static List<string> ReadCategoriesFromMetadata(MetaDataInfo[] metadata)
        {

            List<string> categories = new List<string>();
            string netVal;
            foreach (MetaDataInfo md in metadata)
            {
                switch (md.Key)
                {
                    case "category":
                    case "category1":
                        netVal = md.Value.StripQuotes();
                        if (netVal != "")
                            categories.Insert(0, netVal);
                        break;
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