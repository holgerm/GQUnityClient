using System.Collections.Generic;
using Newtonsoft.Json;

namespace Code.GQClient.Conf
{
    public class CategorySet
    {
        public string name;

        public List<Category> categories;

        [JsonConstructor]
        public CategorySet(string name, List<Category> categories)
        {
            this.name = name;
            if (categories == null)
                categories = new List<Category>();
            this.categories = categories;
        }

        public CategorySet()
        {
            name = "";
            categories = new List<Category>();
        }
    }
}