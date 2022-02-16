using Newtonsoft.Json;

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

        public int catInfo;
        
        public Category()
        {
            this.id = "";
            name = "";
            folderName = "";
            symbol = null;
            catInfo = 0;
        }

        [JsonConstructor]
        public Category(string id, string name, string folderName, string symbolPath, int catInfo = 0)
        {
            this.id = id;
            this.name = name;
            this.folderName = folderName ?? "";
            if (!string.IsNullOrEmpty(symbolPath))
                this.symbol = new RTImagePath(symbolPath);
            this.catInfo = catInfo;
        }
    }
}