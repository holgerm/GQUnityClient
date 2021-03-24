using System;
using System.IO;
using System.Text.RegularExpressions;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using Code.GQClient.Util.http;
using Code.QM.Util;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.GQClient.Conf
{
    public class RTImagePath : ImagePath
    {
        private readonly string FilePath;
        private readonly string ResourcePath;

        public const string DEFAULT_CAT_IMAGE_PATH = "textures/readable/default";

        public readonly Observable RTImageChanged = new Observable();

        [JsonConstructor]
        public RTImagePath(string path) : base(path)
        {
            if (CheckPathForUpdateInfo(path, out int length, out int updateTimestamp))
            {
                FilePath = path.Substring(startIndex: length);

                if (NeedsUpdateTo(updateTimestamp))
                {
                    string serverFileUrl =
                        Path.Combine(
                            RTConfig.GQ_SERVER_PORTALS_URL,
                            Config.Current.id,
                            RTConfig.RT_CONFIG_DIR,
                            FilePath);
                    string localFilePath =
                        Path.Combine(
                            Application.persistentDataPath,
                            RTConfig.RT_CONFIG_DIR,
                            FilePath);

                    Downloader d = new Downloader(
                        url: serverFileUrl,
                        timeout: 0,
                        targetPath: localFilePath);
                    d.OnTaskCompleted += (sender, args) =>
                    {
                        // we save the update time mentioned in the RTProduct.json as file modified time:
                        DateTime dt =
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        dt = dt.AddSeconds(updateTimestamp);
                        File.SetLastWriteTime(localFilePath, dt);
                        RTImageChanged.Invoke();
                    };
                    d.Start();
                }
            }
            else
            {
                FilePath = path;
            }

            ResourcePath = Files.StripExtension(FilePath);
        }

        public override bool Equals(System.Object obj)
        {
            // Other null?
            if (obj == null)
                return path == null || path.Equals("");

            // Compare run-time types.
            if (GetType() != obj.GetType())
                return false;

            return path == ((RTImagePath) obj).path;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "RTImagePath: " + path;
        }

        public override Sprite GetSprite()
        {
            if (string.IsNullOrEmpty(path))
                return null;

            Sprite sprite = null;

            if (!ConfigurationManager.RTProductUpdated)
            {
                sprite = Resources.Load<Sprite>(ResourcePath);
                if (null == sprite)
                {
                    Debug.Log("## 4: Used Defaultsymbol RTImagePath.GetSprite sprite is null".Red());
                    sprite = Resources.Load<Sprite>(DEFAULT_CAT_IMAGE_PATH);
                }

                return sprite;
            }
            else
            {
                Texture2D texture = GetTexture2D();
                if (null == texture)
                {
                    Debug.Log("## 5: Used Defaultsymbol RTImagePath.GetSprite texture is null".Red());
                    sprite = Resources.Load<Sprite>(DEFAULT_CAT_IMAGE_PATH);
                }
                else
                {
                    sprite =
                        Sprite.Create(
                            texture,
                            new Rect(0, 0, texture.width, texture.height),
                            new Vector2(0.5f, 0.5f)
                        );
                }

                return sprite;
            }
        }


        public override Texture2D GetTexture2D()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                return null;
            }

            if (CheckPathForUpdateInfo(path, out int length, out int updateTimestamp))
            {
                string filePath =
                    Path.Combine(Application.persistentDataPath, RTConfig.RT_CONFIG_DIR, FilePath);

                if (File.Exists(filePath))
                {
                    byte[] bytes = File.ReadAllBytes(filePath);
                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(bytes);
                    return texture;
                }

                return Resources.Load<Texture2D>(ResourcePath);
            }


            return Resources.Load<Texture2D>(ResourcePath);
        }


        private static readonly Regex UpdatedPath = new Regex(@"^update:(\d+):");

        private bool CheckPathForUpdateInfo(string path, out int length, out int serverTimestamp)
        {
            length = 0;
            serverTimestamp = -1;

            Match match = UpdatedPath.Match(path);

            if (!match.Success)
                return false;

            length = match.Length;

            if (match.Groups.Count < 2) return false;

            serverTimestamp = int.Parse(match.Groups[1].Value);
            return true;
        }

        private bool NeedsUpdateTo(int updateTimestamp)
        {
            string filePath =
                Path.Combine(
                    Application.persistentDataPath,
                    FilePath);

            if (!File.Exists(filePath)) return true;

            DateTime dt = File.GetLastWriteTimeUtc(filePath);
            return dt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds < updateTimestamp;
        }
    }
}