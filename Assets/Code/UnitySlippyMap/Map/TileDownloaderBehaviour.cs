//#define DEBUG_LOG
// 
//  TileDownloader.cs
//  
//  Author:
//       Jonathan Derrough <jonathan.derrough@gmail.com>
//  
// Copyright (c) 2017 Jonathan Derrough
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Code.UnitySlippyMap.Helpers;
using Code.UnitySlippyMap.Helpers.JobManager;
using UnityEngine;

namespace Code.UnitySlippyMap.Map
{
    /// <summary>
    /// A singleton class in charge of downloading, caching and serving tiles.
    /// </summary>
    public class TileDownloaderBehaviour : MonoBehaviour
    {
        #region Singleton implementation

        /// <summary>
        /// The instance.
        /// </summary>
        private static TileDownloaderBehaviour instance = null;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static TileDownloaderBehaviour Instance
        {
            get
            {
                if (null == (object)instance)
                {
                    instance = FindObjectOfType(typeof(TileDownloaderBehaviour)) as TileDownloaderBehaviour;
                    if (null == (object)instance)
                    {
                        var go = new GameObject("[TileDownloader]");
                        go.hideFlags = HideFlags.HideAndDontSave;
                        instance = go.AddComponent<TileDownloaderBehaviour>();
                        instance.EnsureDownloader();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Ensures the downloader.
        /// </summary>
        private void EnsureDownloader()
        {
            LoadTiles();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitySlippyMap.Map.TileDownloader"/> class.
        /// </summary>
        private TileDownloaderBehaviour()
        {
        }

        /// <summary>
        /// Raises the application quit event.
        /// </summary>
        private void OnApplicationQuit()
        {
            DestroyImmediate(this.gameObject);
        }

        #endregion

        #region Tile download subclasses

        /// <summary>
        /// A helper class for asynchronous IO operations.
        /// </summary>
        private class AsyncInfo
        {
            /// <summary>
            /// The tile entry.
            /// </summary>
            private TileEntry entry;

            /// <summary>
            /// Gets the tile entry.
            /// </summary>
            /// <value>The tile entry.</value>
            public TileEntry Entry { get { return entry; } }

            /// <summary>
            /// The filestream.
            /// </summary>
            private FileStream fs;

            /// <summary>
            /// Gets the FileStream instance.
            /// </summary>
            /// <value>The filestream.</value>
            public FileStream FS { get { return fs; } }

            /// <summary>
            /// Initializes a new instance of the <see cref="UnitySlippyMap.Map.TileDownloaderBehaviour+AsyncInfo"/> class.
            /// </summary>
            /// <param name="entry">Entry.</param>
            /// <param name="fs">Fs.</param>
            public AsyncInfo(TileEntry entry, FileStream fs)
            {
                this.entry = entry;
                this.fs = fs;
            }
        }

        /// <summary>
        /// The TileEntry class holds the information necessary to the TileDownloader to manage the tiles.
        /// It also handles the (down)loading/caching of the concerned tile, taking advantage of Prime31's JobManager
        /// </summary>
        public class TileEntry
        {
            /// <summary>
            /// The timestamp.
            /// </summary>
            [XmlAttribute("timestamp")]
            public double timestamp;

            /// <summary>
            /// The size.
            /// </summary>
            [XmlAttribute("size")]
            public int size;

            /// <summary>
            /// The GUID.
            /// </summary>
            [XmlAttribute("guid")]
            public string guid;

            /// <summary>
            /// The URL.
            /// </summary>
            [XmlAttribute("url")]
            public string url;

            /// <summary>
            /// The tile.
            /// </summary>
            [XmlIgnore]
            public TileBehaviour tile;

            /// <summary>
            /// The texture.
            /// </summary>
            [XmlIgnore]
            public Texture2D texture;

            /// <summary>
            /// The cached flag.
            /// </summary>
            [XmlIgnore]
            public bool cached = false;

            /// <summary>
            /// The error flag.
            /// </summary>
            [XmlIgnore]
            public bool error = false;

            /// <summary>
            /// The job.
            /// </summary>
            [XmlIgnore]
            public Job job;

            /// <summary>
            /// The job complete handler.
            /// </summary>
            [XmlIgnore]
            public Job.JobCompleteHandler jobCompleteHandler;

            /// <summary>
            /// Initializes a new instance of the <see cref="UnitySlippyMap.Map.TileDownloader+TileEntry"/> class.
            /// </summary>
            public TileEntry()
            {
                this.jobCompleteHandler = new Job.JobCompleteHandler(TileDownloaderBehaviour.Instance.JobTerminationEvent);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="UnitySlippyMap.Map.TileDownloader+TileEntry"/> class.
            /// </summary>
            /// <param name="url">URL.</param>
            /// <param name="tile">Tile.</param>
            public TileEntry(string url, TileBehaviour tile)
            {
                this.url = url;
                if (tile == null)
                    throw new ArgumentNullException("tile");
                this.tile = tile;
                this.jobCompleteHandler = new Job.JobCompleteHandler(TileDownloaderBehaviour.Instance.JobTerminationEvent);
            }

            private static int downloadCounter = 0;

            /// <summary>
            /// Starts the download.
            /// </summary>
            public void StartDownload()
            {
#if DEBUG_LOG
                Debug.Log(("Start Download Coroutine for tile: " + url).Green());
                Debug.Log((" Counter###: " + ++downloadCounter + "   TileEntry.StartDownload: " + url).Yellow());
#endif
                job = new Job(DownloadCoroutine(), this);
                job.JobComplete += jobCompleteHandler;
            }

            /// <summary>
            /// Stops the download.
            /// </summary>
            public void StopDownload()
            {
                job.JobComplete -= jobCompleteHandler;
                job.Kill();
            }

            private string url2filename(string input)
            {
                if (input.IndexOf('?') > 0)
                    input = input.Substring(0, input.IndexOf('?'));
                return input.Replace('/', '_').Replace('.', '-').Replace(':', '=');
            }

            private IEnumerator DownloadCoroutine()
            {
                tile.Showing = false;
                WWW www = null;
                string ext = ".png";
                bool shouldBeCached = false;
                string tileCachePath =
                    Path.Combine(
                        Application.persistentDataPath,
                        "tilecache", tile.GetTileSubPath())
                     + ext;
                if (File.Exists(tileCachePath))
                {
                    www = new WWW("file://" + tileCachePath);
                    shouldBeCached = false;
                }
                else
                {
                    shouldBeCached = true;
                    www = new WWW(url);
                }

                Debug.Log("L# 1: x: " + tile.xPos + "    y: " + tile.yPos + "  www: " + www.url + " progress: " + www.progress);
                yield return null; // www;

                while (!www.isDone)
                {
                    Debug.Log("L# 2: x: " + tile.xPos + "    y: " + tile.yPos + "  www: " + www.url + " progress: " + www.progress);
                    yield return null;
                }

                Debug.Log("L# 3: x: " + tile.xPos + "    y: " + tile.yPos + "  www: " + www.url + " progress: " + www.progress);
                if (String.IsNullOrEmpty(www.error) && www.text.Contains("404 Not Found") == false)
                {
                    Renderer renderer = tile.gameObject.GetComponent<Renderer>();

                    Destroy(renderer.material.mainTexture);
                    renderer.material.mainTexture = www.texture;
                    tile.Showing = true;

                    if (shouldBeCached)
                    {
                        byte[] bytes = www.bytes;

                        this.size = bytes.Length;
                        this.guid = url2filename(url);

                        string tileDir = Path.GetDirectoryName(tileCachePath);
                        if (!Directory.Exists(tileDir))
                        {
                            Directory.CreateDirectory(tileDir);
                        }

                        if (!www.url.Contains(tile.xPos.ToString() + @"/" + tile.yPos.ToString()))
                        {
                            Debug.Log("TT 1. loaded from url: " + www.url);
                            Debug.Log("TT 2. cached to file: " + tileCachePath + "   but x: " + tile.xPos + "  y: " + tile.yPos);
                        }

                        FileStream fs = new FileStream(tileCachePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
                        fs.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(EndWriteCallback), new AsyncInfo(this, fs));
                    }

                    this.timestamp = (DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
                }
                else
                {
                    this.error = true;
                }
                www.Dispose();
            }

            /// <summary>
            /// The callback called at the end of the writing operation.
            /// </summary>
            /// <param name="result">Result.</param>
            private static void EndWriteCallback(IAsyncResult result)
            {
                AsyncInfo info = result.AsyncState as AsyncInfo;
                info.Entry.cached = true;

                info.FS.EndWrite(result);
                info.FS.Flush();
                info.FS.Dispose();
            }
        }

        #endregion

        #region Private members & properties

        /// <summary>
        /// The tile URL looked for.
        /// </summary>
        private static string tileURLLookedFor;

        /// <summary>
        /// The match predicate to find tiles by URL
        /// </summary>
        /// <returns><c>true</c>, if URL match predicate was tiled, <c>false</c> otherwise.</returns>
        /// <param name="entry">Entry.</param>
        private static bool tileURLMatchPredicate(TileEntry entry)
        {
            if (entry.url == tileURLLookedFor)
                return true;
            return false;
        }

        /// <summary>
        /// The tiles to load.
        /// </summary>
        private List<TileEntry> tilesToLoad = new List<TileEntry>();

        /// <summary>
        /// The tiles loading.
        /// </summary>
        private List<TileEntry> tilesLoading = new List<TileEntry>();

        /// <summary>
        /// The tiles.
        /// </summary>
        private List<TileEntry> tiles = new List<TileEntry>();

        /// <summary>
        /// The tile path.
        /// </summary>
        private string tilePath;

        private int maxSimultaneousDownloads = 10;

        /// <summary>
        /// Gets or sets the max simultaneous downloads.
        /// </summary>
        /// <value>The max simultaneous downloads.</value>
        public int MaxSimultaneousDownloads { get { return maxSimultaneousDownloads; } set { maxSimultaneousDownloads = value; } }

        /// <summary>
        /// The size of the max cache.
        /// </summary>
        private int maxCacheSize = 20000000;
        // 20 Mo

        /// <summary>
        /// Gets or sets the size of the max cache.
        /// </summary>
        /// <value>The size of the max cache.</value>
        public int MaxCacheSize { get { return maxCacheSize; } set { maxCacheSize = value; } }

        /// <summary>
        /// The size of the cache.
        /// </summary>
        public static int cacheSize = 0;

        #endregion

        #region Public methods

        /// <summary>
        /// Gets a tile by its URL, the main texture of the material is assigned if successful.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="tile">Tile.</param>
        public void Get(string url, TileBehaviour tile)
        {
            if (this == null)
            {
#if DEBUG_LOG
                Debug.Log("DEBUG: TileDownloader.Get() THIS is NULL");
#endif
                return;
            }

            tileURLLookedFor = url;
            if (tilesToLoad.Exists(tileURLMatchPredicate))
            {
#if DEBUG_LOG
                Debug.Log(("NOT ADDING TILE to load ##1 for url: " + url).Red());
#endif
                return;
            }

            if (tilesLoading.Exists(tileURLMatchPredicate))
            {
#if DEBUG_LOG
                Debug.Log(("NOT ADDING TILE to load ##2 for url: " + url).Red());
#endif
                return;
            }

            TileEntry cachedEntry = tiles.Find(tileURLMatchPredicate);

            if (cachedEntry == null)
            {
#if DEBUG_LOG
                Debug.Log(("ADDING TILE to load NOT CACHED for url: " + url).Green());
#endif
                tilesToLoad.Add(
                    new TileEntry(url, tile)
                );
            }
            else
            {
                cachedEntry.cached = true;
                cachedEntry.tile = tile;
#if DEBUG_LOG
                Debug.Log(("ADDING TILE to load CACHED for url: " + url).Green());
#endif
                tilesToLoad.Add(cachedEntry);
            }
        }

        /// <summary>
        /// Cancels the request for a tile by its URL.
        /// </summary>
        /// <returns><c>true</c> if this instance cancel url; otherwise, <c>false</c>.</returns>
        /// <param name="url">URL.</param>
        public void Cancel(string url)
        {
            tileURLLookedFor = url;
            TileEntry entry = tilesToLoad.Find(tileURLMatchPredicate);
            if (entry != null)
            {
#if DEBUG_LOG
               Debug.Log(("REMOVING fro tilestoload: " + entry.url).Red());
#endif
                tilesToLoad.Remove(entry);
                return;
            }

            entry = tilesLoading.Find(tileURLMatchPredicate);
            if (entry != null)
            {
                tilesLoading.Remove(entry);
                entry.StopDownload();
                return;
            }
        }

        /// <summary>
        /// A method called when the job is done, successfully or not.
        /// </summary>
        /// <param name="job">Job.</param>
        /// <param name="e">E.</param>
        public void JobTerminationEvent(object job, JobEventArgs e)
        {
            TileEntry entry = e.Owner as TileEntry;
#if DEBUG_LOG
            Debug.Log(string.Format("Job Terminated. {0} {1} {2} {3}", e.WasKilled ? "killed".Red() : "", entry.cached ? "Cached".Yellow() : "", entry.error ? "Error".Red() : "", entry.url));
#endif
            tilesLoading.Remove(entry);

            if (e.WasKilled == false)
            {
                if (entry.error && entry.cached)
                {
                    // try downloading the tile again
                    entry.cached = false;
                    cacheSize -= entry.size;
                    tiles.Remove(entry);
                    Get(entry.url, entry.tile);
                    return;
                }

                tileURLLookedFor = entry.url;
                TileEntry existingEntry = tiles.Find(tileURLMatchPredicate);

                if (existingEntry != null)
                {
                    tiles.Remove(existingEntry);
                    cacheSize -= existingEntry.size;
                }

                entry.timestamp = (DateTime.Now.ToLocalTime() - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
                tiles.Add(entry);
                cacheSize += entry.size;
            }
            else
            {
#if DEBUG_LOG
                Debug.LogWarning(("JobTerminationEvent(): e.WasKilled e: " + e.ToString()).Yellow());
#endif
            }
        }

        /// <summary>
        /// Pauses all.
        /// </summary>
        public void PauseAll()
        {
            foreach (TileEntry entry in tilesLoading)
            {
                entry.job.Pause();
            }
        }

        /// <summary>
        /// Unpauses all.
        /// </summary>
        public void UnpauseAll()
        {
            foreach (TileEntry entry in tilesLoading)
            {
                entry.job.Unpause();
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Implementation of <see cref="http://docs.unity3d.com/ScriptReference/MonoBehaviour.html">MonoBehaviour</see>.Start().
        /// </summary>
        private void Start()
        {
            tilePath = Application.temporaryCachePath;
            TextureBogusExtension.Init(this);
        }

#if DEBUG_LOG
        bool tellAboutEmptyTilesLoadingList;
#endif

        /// <summary>
        /// Implementation of <see cref="http://docs.unity3d.com/ScriptReference/MonoBehaviour.html">MonoBehaviour</see>.Update().
        /// </summary>
        private void Update()
        {
            while (tilesToLoad.Count > 0 &&
                tilesLoading.Count <
                    MaxSimultaneousDownloads)
            {
                DownloadNextTile();
            }
        }

        /// <summary>
        /// Downloads the next tile.
        /// </summary>
        private void DownloadNextTile()
        {
            TileEntry entry = tilesToLoad[0];
            tilesToLoad.RemoveAt(0);
            tilesLoading.Add(entry);
            entry.StartDownload();
        }

        /// <summary>
        /// Implementation of <see cref="http://docs.unity3d.com/ScriptReference/MonoBehaviour.html">MonoBehaviour</see>.OnDestroy().
        /// </summary>
        private void OnDestroy()
        {
            KillAll();
            SaveTiles();
            instance = null;
        }

        /// <summary>
        /// Kills all.
        /// </summary>
        private void KillAll()
        {
            foreach (TileEntry entry in tilesLoading)
            {
                entry.job.Kill();
            }
        }

        /// <summary>
        /// Deletes the cached tile.
        /// </summary>
        /// <param name="t">T.</param>
        private void DeleteCachedTile(TileEntry t)
        {
            cacheSize -= t.size;
            File.Delete(tilePath + "/" + t.guid + ".png");
            tiles.Remove(t);
        }

        /// <summary>
        /// Saves the tile informations to an XML file stored in tilePath.
        /// </summary>
        private void SaveTiles()
        {
            string filepath = tilePath + "/" + "tile_downloader.xml";

#if DEBUG_LOG
            Debug.Log("DEBUG: TileDownloader.SaveTiles: file: " + filepath);
#endif

            XmlSerializer xs = new XmlSerializer(tiles.GetType());
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                xs.Serialize(sw, tiles);
            }
        }

        /// <summary>
        /// Loads the tile informations from an XML file stored in tilePath.
        /// </summary>
        private void LoadTiles()
        {
            string filepath = tilePath + "/" + "tile_downloader.xml";

            if (File.Exists(filepath) == false)
            {
                return;
            }

            XmlSerializer xs = new XmlSerializer(tiles.GetType());
            using (StreamReader sr = new StreamReader(filepath))
            {
                tiles = xs.Deserialize(sr) as List<TileEntry>;
            }

            foreach (TileEntry tile in tiles)
            {
                cacheSize += tile.size;
            }
        }

        #endregion
    }


}