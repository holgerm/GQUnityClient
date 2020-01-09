//#define DEBUG_LOG

// 
//  Tile.cs
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
using GQ.Client.Conf;
using GQ.Client.Err;
using GQ.Client.Util;
using UnityEngine;
using UnitySlippyMap.Layers;

namespace UnitySlippyMap.Map
{
    /// <summary>
    /// The tile implementation inherits from MonoBehaviour.
    /// </summary>
    public class TileBehaviour : MonoBehaviour
    {
        #region Private members & properties

        ///// <summary>
        ///// The showing flag.
        ///// </summary>
        //private bool showing = false;

        /// <summary>
        /// Gets a value indicating whether this <see cref="UnitySlippyMap.Map.Tile"/> is showing.
        /// </summary>
        /// <value><c>true</c> if showing; otherwise, <c>false</c>.</value>
        public bool Showing
        {
            get { return GetComponent<Renderer>().enabled; }
            set
            {
                GetComponent<Renderer>().enabled = value;
            }
        }

        internal bool TextureIsDownloading;
        internal bool DownloadingTextureIsCancelled;

        /// <summary>
        /// The material.
        /// </summary>
        private Material material;

        /// <summary>
        /// The duration of the apparition.
        /// </summary>
        private float apparitionDuration = 0.5f;

        /// <summary>
        /// The apparition start time.
        /// </summary>
        private float apparitionStartTime = 0.0f;

        public int xPos;
        public int yPos;
        public int zPos;

        public string GetTileSubPath()
        {
            return string.Format("{0}/{1}/{2}", zPos, xPos, yPos);
        }
        #endregion

        #region Public enums
        /// <summary>
        /// The anchor points enumeration.
        /// </summary>
        public enum AnchorPoint
        {
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }
        #endregion

        #region Public methods
        public override string ToString()
        {
            return (string.Format("TileBehaviour: {0}, {1}, {2}", zPos, xPos, yPos));
        }

        /// <summary>
        /// Creates a tile template GameObject.
        /// </summary>
        public static TileBehaviour CreateTileTemplate()
        {
            return CreateTileTemplate("[Tile Template]", AnchorPoint.MiddleCenter);
        }

        /// <summary>
        /// Creates a tile template GameObject.
        /// </summary>
        /// <returns>The tile template.</returns>
        /// <param name="name">Name.</param>
        public static TileBehaviour CreateTileTemplate(string name)
        {
            return CreateTileTemplate(name, AnchorPoint.MiddleCenter);
        }

        /// <summary>
        /// Creates a tile template GameObject.
        /// </summary>
        /// <returns>The tile template.</returns>
        /// <param name="anchorPoint">Anchor point.</param>
        public static TileBehaviour CreateTileTemplate(AnchorPoint anchorPoint)
        {
            return CreateTileTemplate("[Tile Template]", anchorPoint);
        }

        /// <summary>
        /// Creates a tile template GameObject.
        /// </summary>
        /// <returns>The tile template.</returns>
        /// <param name="tileName">Tile name.</param>
        /// <param name="anchorPoint">Anchor point.</param>
        public static TileBehaviour CreateTileTemplate(string tileName, AnchorPoint anchorPoint)
        {
            GameObject tileTemplate = new GameObject(tileName);
            TileBehaviour tile = tileTemplate.AddComponent<TileBehaviour>();
            MeshFilter meshFilter = tileTemplate.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = tileTemplate.AddComponent<MeshRenderer>();
            meshRenderer.enabled = true;
            BoxCollider boxCollider = tileTemplate.AddComponent<BoxCollider>();

            // add the geometry
            Mesh mesh = meshFilter.mesh;
            switch (anchorPoint)
            {
                case AnchorPoint.TopLeft:
                    mesh.vertices = new Vector3[] {
                    new Vector3 (1.0f, 0.0f, 0.0f),
                    new Vector3 (1.0f, 0.0f, -1.0f),
                    new Vector3 (0.0f, 0.0f, -1.0f),
                    new Vector3 (0.0f, 0.0f, 0.0f)
                };
                    break;
                case AnchorPoint.TopCenter:
                    mesh.vertices = new Vector3[] {
                    new Vector3 (0.5f, 0.0f, 0.0f),
                    new Vector3 (0.5f, 0.0f, -1.0f),
                    new Vector3 (-0.5f, 0.0f, -1.0f),
                    new Vector3 (-0.5f, 0.0f, 0.0f)
                };
                    break;
                case AnchorPoint.TopRight:
                    mesh.vertices = new Vector3[] {
                    new Vector3 (0.0f, 0.0f, 0.0f),
                    new Vector3 (0.0f, 0.0f, -1.0f),
                    new Vector3 (-1.0f, 0.0f, -1.0f),
                    new Vector3 (-1.0f, 0.0f, 0.0f)
                };
                    break;
                case AnchorPoint.MiddleLeft:
                    mesh.vertices = new Vector3[] {
                    new Vector3 (1.0f, 0.0f, 0.5f),
                    new Vector3 (1.0f, 0.0f, -0.5f),
                    new Vector3 (0.0f, 0.0f, -0.5f),
                    new Vector3 (0.0f, 0.0f, 0.5f)
                };
                    break;
                case AnchorPoint.MiddleRight:
                    mesh.vertices = new Vector3[] {
                    new Vector3 (0.0f, 0.0f, 0.5f),
                    new Vector3 (0.0f, 0.0f, -0.5f),
                    new Vector3 (-1.0f, 0.0f, -0.5f),
                    new Vector3 (-1.0f, 0.0f, 0.5f)
                };
                    break;
                case AnchorPoint.BottomLeft:
                    mesh.vertices = new Vector3[] {
                    new Vector3 (1.0f, 0.0f, 1.0f),
                    new Vector3 (1.0f, 0.0f, 0.0f),
                    new Vector3 (0.0f, 0.0f, 0.0f),
                    new Vector3 (0.0f, 0.0f, 1.0f)
                };
                    break;
                case AnchorPoint.BottomCenter:
                    mesh.vertices = new Vector3[] {
                    new Vector3 (0.5f, 0.0f, 1.0f),
                    new Vector3 (0.5f, 0.0f, 0.0f),
                    new Vector3 (-0.5f, 0.0f, 0.0f),
                    new Vector3 (-0.5f, 0.0f, 1.0f)
                };
                    break;
                case AnchorPoint.BottomRight:
                    mesh.vertices = new Vector3[] {
                    new Vector3 (0.0f, 0.0f, 1.0f),
                    new Vector3 (0.0f, 0.0f, 0.0f),
                    new Vector3 (-1.0f, 0.0f, 0.0f),
                    new Vector3 (-1.0f, 0.0f, 1.0f)
                };
                    break;
                default: // MiddleCenter
                    mesh.vertices = new Vector3[] {
                    new Vector3 (0.5f, 0.0f, 0.5f),
                    new Vector3 (0.5f, 0.0f, -0.5f),
                    new Vector3 (-0.5f, 0.0f, -0.5f),
                    new Vector3 (-0.5f, 0.0f, 0.5f)
                };
                    break;
            }
            mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

            // add normals
            mesh.normals = new Vector3[] {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up
            };
            // add uv coordinates
            mesh.uv = new Vector2[] {
                new Vector2 (1.0f, 1.0f),
                new Vector2 (1.0f, 0.0f),
                new Vector2 (0.0f, 0.0f),
                new Vector2 (0.0f, 1.0f)
            };

            // add a material
            string shaderName = "Larku/UnlitTransparent";
            Shader shader = Resources.Load<Shader>("LarkuUnlitTransparent");
            // was (did not work since 2019.2.6): Shader shader = Shader.Find (shaderName);

#if DEBUG_LOG
		Debug.Log("DEBUG: shader for tile template: " + shaderName + ", exists: " + (shader != null));
#endif

            tile.material = meshRenderer.material = new Material(shader);

            // setup the collider
            boxCollider.size = new Vector3(1.0f, 0.0f, 1.0f);

#if DEBUG_LOG
            Bounds b = boxCollider.bounds;
            Debug.Log("CreateTileTemplate bounds: " + b);
#endif

            return tile;
        }

        public Texture GetTexture()
        {
            Material material = this.gameObject.GetComponent<Renderer>().material;
            if (material != null)
            {
                return material.mainTexture;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the tile key.
        /// </summary>
        /// <returns>The tile key.</returns>
        /// <param name="roundedZoom">Rounded zoom.</param>
        /// <param name="tileX">Tile x.</param>
        /// <param name="tileY">Tile y.</param>
        public static string GetTileKey(int roundedZoom, int tileX, int tileY)
        {
            return roundedZoom + "_" + tileX + "_" + tileY;
        }

        internal void SetPosition(int x, int y, int z)
        {
            this.xPos = x;
            this.yPos = y;
            this.zPos = z;
        }

        public string URL
        {
            get
            {
                return
                    @"https://api.tiles.mapbox.com/v4/" + ConfigurationManager.Current.mapID + "/" +
                    GetTileSubPath() +
                    "@2x.png?access_token=" + ConfigurationManager.Current.mapKey;
            }
        }



        public string oldName;
        public int reuses;


        internal void LoadTexture()
        {
            Base.Instance.StartCoroutine(LoadTextureCoroutine());
        }

        private IEnumerator LoadTextureCoroutine()
        {
            WWW www;
            TextureIsDownloading = true;
            string ext = ".png";
            bool shouldBeCached = false;
            string tileCachePath =
                Path.Combine(
                    Application.persistentDataPath,
                    "tilecache", GetTileSubPath())
                 + ext;
            if (File.Exists(tileCachePath))
            {
                www = new WWW("file://" + tileCachePath);
                shouldBeCached = false;
            }
            else
            {
                shouldBeCached = true;
                www = new WWW(URL);
            }

            yield return null; // www;

            while (!www.isDone)
            {
                // We cancel download if zoom has changed:
                DownloadingTextureIsCancelled =
                    MapBehaviour.RoundedZoom != zPos;

                if (DownloadingTextureIsCancelled)
                {
                    DownloadingTextureIsCancelled = false;
                    TextureIsDownloading = false;
                    www.Dispose();

                    yield break;
                    // TERMINATE LOADING
                }
                yield return null;
            }

            DownloadingTextureIsCancelled =
                MapBehaviour.RoundedZoom != zPos;

            if (DownloadingTextureIsCancelled)
            {
                DownloadingTextureIsCancelled = false;
                TextureIsDownloading = false;
                www.Dispose();

                yield break;
                // TERMINATE LOADING
            }

            if (String.IsNullOrEmpty(www.error) && www.text.Contains("404 Not Found") == false)
            {
                Renderer myRenderer = GetComponent<Renderer>();

                myRenderer.material.mainTexture = www.texture;
                Showing = true;

                if (shouldBeCached)
                {
                    string tileDir = Path.GetDirectoryName(tileCachePath);
                    if (!Directory.Exists(tileDir))
                    {
                        Directory.CreateDirectory(tileDir);
                    }

                    File.WriteAllBytes(tileCachePath, www.bytes);
                }
            }

            www.Dispose();
            TextureIsDownloading = false;
        }

        private void EndWriteCallback(IAsyncResult ar)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}