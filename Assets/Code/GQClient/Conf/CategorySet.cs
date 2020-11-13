using System.Collections.Generic;
using Code.GQClient.Err;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.GQClient.Conf
{
    public class CategorySet
    {
        public string name;

        public List<Category> categories;

        [JsonConstructor]
        public CategorySet(string name, List<Category> categories)
        {
            Debug.Log($"CategorySet {name} #cats: {categories.Count}");
            this.name = name;
            if (categories == null)
                categories = new List<Category>();
            this.categories = categories;
        }

        public CategorySet()
        {
            Debug.Log($"CategorySet()".Red());
            name = "";
            categories = new List<Category>();
        }
    }
}