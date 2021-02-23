using System;
using System.IO;
using System.Text.RegularExpressions;
using Code.GQClient.FileIO;
using Code.GQClient.Util.http;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.GQClient.Conf
{
    public class RTImagePath : ImagePath
    {
        private readonly string FilePath;
        private readonly string ResourcePath;

        [JsonConstructor]
        public RTImagePath(string path) : base(path)
        {
            if (RTConfig.CurrentLoadingMode == RTConfig.LoadsFrom.Resource)
            {
                ResourcePath = path;
                FilePath = path;
                return;
            }

            if (CheckPathForUpdateInfo(path, out int length, out int updateTimestamp))
            {
                FilePath = path.Substring(startIndex: length);
                ResourcePath = Files.StripExtension(FilePath);

                if (NeedsUpdateTo(updateTimestamp))
                {
                    string serverFileUrl =
                        Path.Combine(
                            ConfigurationManager.GQ_SERVER_PORTALS_URL,
                            ConfigurationManager.Current.id,
                            ConfigurationManager.RT_CONFIG_DIR,
                            FilePath);
                    string localFilePath =
                        Path.Combine(
                            Application.persistentDataPath,
                            ConfigurationManager.RT_CONFIG_DIR,
                            FilePath);

                    Downloader d = new Downloader(
                        serverFileUrl,
                        timeout: 0,
                        localFilePath);
                    d.OnTaskCompleted += (sender, args) =>
                    {
                        // we save the update time mentioned in the RTProduct.json as file modified time:
                        DateTime dt =
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        dt = dt.AddSeconds(updateTimestamp);
                        File.SetLastWriteTime(localFilePath, dt);
                    };
                    d.Start();
                }
            }
            else
            {
                FilePath = path;
                ResourcePath = Files.StripExtension(FilePath);
            }
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

            if (!ConfigurationManager.RTProductUpdated)
            {
                return Resources.Load<Sprite>(ResourcePath);
            }
            else
            {
                Texture2D texture = GetTexture2D();
                if (null == texture)
                    throw new ArgumentException($"File at path {FilePath} not found.");

                Sprite sprite =
                    Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );
                return sprite;
            }
        }


        public override Texture2D GetTexture2D()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                return null;
            }

            if (!ConfigurationManager.RTProductUpdated)
            {
                Texture2D t = Resources.Load<Texture2D>(ResourcePath);
                return t;
            }

            if (CheckPathForUpdateInfo(path, out int length, out int updateTimestamp))
            {
                string filePath =
                    Path.Combine(Application.persistentDataPath, ConfigurationManager.RT_CONFIG_DIR, FilePath);

                if (File.Exists(filePath))
                {
                    byte[] bytes = File.ReadAllBytes(filePath);
                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(bytes);
                    return texture;
                }
                else
                {
                    Texture2D texture = Resources.Load<Texture2D>(ResourcePath);
                    return texture;
                }
            }
            else
            {
                Texture2D texture = Resources.Load<Texture2D>(ResourcePath);
                return texture;
            }
        }


        private static Regex updatedPath = new Regex(@"^update:(\d+):");

        private bool CheckPathForUpdateInfo(string path, out int length, out int serverTimestamp)
        {
            length = 0;
            serverTimestamp = -1;

            Match match = updatedPath.Match(path);

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