using Newtonsoft.Json;
using UnityEngine;

namespace Code.GQClient.Conf
{
    public class ImagePath
    {
        [JsonProperty]
        protected readonly string path;

        public ImagePath(string path)
        {
            this.path = path;
        }

        public override string ToString()
        {
            return "ImagePath: " + path;
        }

        public bool IsInvalid()
        {
            return string.IsNullOrEmpty(path);
        }

        public override bool Equals(System.Object obj)
        {
            // Other null?
            if (obj == null)
                return path == null || path.Equals("");

            // Compare run-time types.
            if (GetType() != obj.GetType())
                return false;

            return path == ((ImagePath) obj).path;
        }

        public override int GetHashCode()
        {
            return path.GetHashCode();
        }

        public virtual Sprite GetSprite()
        {
            if (string.IsNullOrEmpty(path))
                return null;
            return Resources.Load<Sprite>(path);
        }

        public virtual Texture2D GetTexture2D()
        {
            if (string.IsNullOrEmpty(path))
                return null;
            return Resources.Load<Texture2D>(path);
        }

    }
}