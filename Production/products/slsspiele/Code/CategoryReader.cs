using System.Collections.Generic;
using Code.GQClient.Conf;
using Code.GQClient.Util;
using UnityEngine;

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
                    case "Spieltyp1":
                        netVal = md.Value.StripQuotes();
                        if (netVal != "")
                            categories.Insert(0, netVal);
                        break;
                    case "Spieltyp2":
                    case "Spieltyp3":
                    case "Spieleranzahl":
                    case "Dauer":
                    case "Wetter":
                    case "Orte":
                        netVal = md.Value.StripQuotes();
                        if (netVal != "")
                            categories.Add(netVal);
                        break;
                }
            }
            if (categories.Count == 0 && Config.Current.rt.defaultCategory != null)
            {
                categories.Add(Config.Current.rt.defaultCategory);
            }
            return categories;
        }
    }

}