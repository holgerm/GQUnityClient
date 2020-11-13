using Code.GQClient.Err;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.GQClient.Conf
{
    public class Category
    {
        public string id;

        /// <summary>
        /// The display name.
        /// </summary>
        public string name;

        public string folderName;

        public RTImagePath symbol;

        public Category()
        {
            this.id = "";
            name = "";
            folderName = "";
            symbol = null;
        }

        [JsonConstructor]
        public Category(string id, string name, string folderName, string symbolPath)
        {
            this.id = id;
            this.name = name;
            this.folderName = folderName ?? "";
            if (!string.IsNullOrEmpty(symbolPath))
                this.symbol = new RTImagePath(symbolPath);
        }
    }
}