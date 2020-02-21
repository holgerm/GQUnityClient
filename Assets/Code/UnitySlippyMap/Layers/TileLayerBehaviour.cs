// #define DEBUG_LOG

// 
//  TileLayer.cs
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
using System.Collections.Generic;
using Code.GQClient.Err;
using Code.GQClient.UI.map;
using Code.QM.Util;
using Code.UnitySlippyMap.Map;
using UnityEngine;
using UnitySlippyMap.WMS;

namespace Code.UnitySlippyMap.Layers
{
    /// <summary>
    /// An abstract class representing a tile layer.
    /// One can derive from it to leverage specific or custom tile services.
    /// </summary>
    public abstract class TileLayerBehaviour : LayerBehaviour
    {
        #region Protected members & properties

        /// <summary>
        /// The shared tile template
        /// </summary>
        protected static TileBehaviour tileTemplate;

        /// <summary>
        /// The tile template use count.
        /// </summary>
        protected static int tileTemplateUseCount = 0;

        /// <summary>
        /// The tiles.
        /// </summary>
        protected Dictionary<string, TileBehaviour> tiles = new Dictionary<string, TileBehaviour>();

        /// <summary>
        /// The is ready to be queried flag.
        /// </summary>
        protected bool isReadyToBeQueried = false;

        /// <summary>
        /// The needs to be updated when ready flag.
        /// </summary>
        protected bool needsToBeUpdatedWhenReady = false;


        /// <summary>
        /// A enumeration of the tile directions.
        /// </summary>
        protected enum NeighbourTileDirection
        {
            North,
            South,
            East,
            West
        }

        #endregion

        #region MonoBehaviour implementation

        /// <summary>
        /// Implementation of <see cref="http://docs.unity3d.com/ScriptReference/MonoBehaviour.html">MonoBehaviour</see>.Awake().
        /// </summary>
        protected void Awake()
        {
            // create the tile template if needed
            if (tileTemplate == null)
            {
                tileTemplate = TileBehaviour.CreateTileTemplate();
                tileTemplate.hideFlags = HideFlags.HideAndDontSave;
                tileTemplate.GetComponent<Renderer>().enabled = false;
            }

            ++tileTemplateUseCount;
        }

        /// <summary>
        /// Implementation of <see cref="http://docs.unity3d.com/ScriptReference/MonoBehaviour.html">MonoBehaviour</see>.Start().
        /// </summary>
        private void Start()
        {
            if (w == null)
            {
                w = new WATCH("tile");
            }

            w.Start();

            if (tileTemplate.transform.localScale.x != MapBehaviour.RoundedHalfMapScale)
                tileTemplate.transform.localScale = new Vector3(MapBehaviour.RoundedHalfMapScale, 1.0f,
                    MapBehaviour.RoundedHalfMapScale);
        }

        /// <summary>
        /// Implementation of <see cref="http://docs.unity3d.com/ScriptReference/MonoBehaviour.html">MonoBehaviour</see>.OnDestroy().
        /// </summary>
        private void OnDestroy()
        {
            --tileTemplateUseCount;

            // destroy the tile template if nobody is using anymore
            if (tileTemplate != null && tileTemplateUseCount == 0)
                Destroy(tileTemplate);
        }

        #endregion


        #region Layer implementation

        /// <summary>
        /// Updates the content. See <see cref="Layer.UpdateContent"/>.
        /// </summary>
        public override void UpdateContent()
        {
            if (tileTemplate.transform.localScale.x != MapBehaviour.RoundedHalfMapScale)
                tileTemplate.transform.localScale = new Vector3(MapBehaviour.RoundedHalfMapScale, 1.0f,
                    MapBehaviour.RoundedHalfMapScale);

            if (Map.CurrentCamera != null && isReadyToBeQueried)
            {
#if DEBUG_LOG
                Debug.Log("Updating Content while Map " + (Map.IsDirty ? " DIRTY ".Red() : " CLEAN".Green()));
#endif
                UpdateTiles();
            }
            else
                needsToBeUpdatedWhenReady = true;

            // move the tiles by the map's root translation
            Vector3 displacement = Map.gameObject.transform.position;
            if (displacement != Vector3.zero)
            {
                foreach (KeyValuePair<string, TileBehaviour> tile in tiles)
                {
                    tile.Value.transform.position += displacement;
                }

                //TODO why can not we use this: Map.CurrentCamera.transform.position -= displacement;
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// The tile address looked for.
        /// </summary>
        protected static string tileAddressLookedFor;

        /// <summary>
        /// Visited tiles match predicate.
        /// </summary>
        /// <returns><c>true</c>, if tile address matched, <c>false</c> otherwise.</returns>
        /// <param name="tileAddress">Tile address.</param>
        protected static bool visitedTilesMatchPredicate(string tileAddress)
        {
            if (tileAddress == tileAddressLookedFor)
                return true;
            return false;
        }

        #endregion

        #region Private methods

        private int curX;
        private int curY;

        private void UpdateTiles()
        {
            int tileX, tileY;
            int tileCountOnX, tileCountOnY;
            float offsetX, offsetZ;

            GetTileCountPerAxis(out tileCountOnX, out tileCountOnY);
            GetCenterTile(tileCountOnX, tileCountOnY, out tileX, out tileY, out offsetX, out offsetZ);
            curX = tileX;
            curY = tileY;

            WATCH.StartMeasure($"M {++_measureCounter}");
            PrepareAndRequestTiles(tileX, tileY, tileCountOnX, tileCountOnY, offsetX, offsetZ);
            WATCH.ShowMeasure($"M {_measureCounter}");
            Debug.Log(
                $"CREATE: {ADDED_CREATETILE} -- LOAD: {ADDED_LOADTEXTURE} -- UNCHANGED: {ADDED_UNCHANGEDTILES} -- CHANGED: {ADDED_CHANGEDTILES} --- NoNeighbor: {ADDED_NEIGHTBOURFALSETILES}");
            ADDED_CREATETILE = 0;
            ADDED_LOADTEXTURE = 0;
            ADDED_UNCHANGEDTILES = 0;
            ADDED_CHANGEDTILES = 0;
            ADDED_NEIGHTBOURFALSETILES = 0;
        }

        private static int _measureCounter = 0;
        public static long ADDED_LOADTEXTURE = 0;
        public static long ADDED_CREATETILE = 0;
        public static long ADDED_UNCHANGEDTILES = 0;
        public static long ADDED_CHANGEDTILES = 0;
        public static long ADDED_NEIGHTBOURFALSETILES = 0;


        // this is a new version of GrowTiles():
        void PrepareAndRequestTiles(int tileX, int tileY, int tileCountOnX, int tileCountOnY, float offsetX,
            float offsetZ)
        {
            // request the center itself:
            tilePreparationQueue.Enqueue(new TilePrepareSpec(tileX, tileY, tileCountOnX));
            for (int i = 1; i <= RingsAroundCenterToLoad; i++)
            {
                PrepareAndRequestTilesOnRing(tileX, tileY, tileCountOnX, tileCountOnY, offsetX, offsetZ, i);
            }

            // deactivate all but the current zoom level tile holders:
            activateCurrentZoomLevelTilesOnly();
        }

        // this is a new version of GrowTiles():
        void PrepareAndRequestTilesOnRing(int tileX, int tileY, int tileCountOnX, int tileCountOnY, float offsetX,
            float offsetZ, int ringNr)
        {
            // for (int i = 1; i <= ringNr; i++)
            // {
            //     if (!GetNeighbourTile(tileX, tileY, offsetX, offsetZ, tileCountOnX, tileCountOnY,
            //         NeighbourTileDirection.North, out tileX, out tileY, out offsetX, out offsetZ))
            //         return;
            // }

            // let n be the ringNr.
            // move into the start position, n tiles north of our center
            tileY -= ringNr;

            // prepare start tile north of center:
            tilePreparationQueue.Enqueue(new TilePrepareSpec(tileX, tileY, tileCountOnX));

            // move and prepare n tiles east:
            for (int i = 1; i <= ringNr; i++)
            {
                tileX++;
                // if (GetNeighbourTile(tileX, tileY, offsetX, offsetZ, tileCountOnX, tileCountOnY,
                //     NeighbourTileDirection.East, out tileX, out tileY, out offsetX, out offsetZ))
                tilePreparationQueue.Enqueue(new TilePrepareSpec(tileX, tileY, tileCountOnX));
            }

            // move and prepare 2 * n tiles south:
            for (int i = 1; i <= ringNr * 2; i++)
            {
                tileY++;
                // if (GetNeighbourTile(tileX, tileY, offsetX, offsetZ, tileCountOnX, tileCountOnY,
                //     NeighbourTileDirection.South, out tileX, out tileY, out offsetX, out offsetZ))
                tilePreparationQueue.Enqueue(new TilePrepareSpec(tileX, tileY, tileCountOnX));
            }

            // move and prepare 2 * n tiles west:
            for (int i = 1; i <= ringNr * 2; i++)
            {
                tileX--;
                // if (GetNeighbourTile(tileX, tileY, offsetX, offsetZ, tileCountOnX, tileCountOnY,
                //     NeighbourTileDirection.West, out tileX, out tileY, out offsetX, out offsetZ))
                tilePreparationQueue.Enqueue(new TilePrepareSpec(tileX, tileY, tileCountOnX));
            }

            // move and prepare 2 * n tiles north:
            for (int i = 1; i <= ringNr * 2; i++)
            {
                tileY--;
                // if (GetNeighbourTile(tileX, tileY, offsetX, offsetZ, tileCountOnX, tileCountOnY,
                //     NeighbourTileDirection.North, out tileX, out tileY, out offsetX, out offsetZ))
                tilePreparationQueue.Enqueue(new TilePrepareSpec(tileX, tileY, tileCountOnX));
            }

            // move and prepare n - 1 tiles east again, just before the starting tile:
            for (int i = 1; i < ringNr; i++)
            {
                tileX++;
                // if (GetNeighbourTile(tileX, tileY, offsetX, offsetZ, tileCountOnX, tileCountOnY,
                //     NeighbourTileDirection.East, out tileX, out tileY, out offsetX, out offsetZ))
                tilePreparationQueue.Enqueue(new TilePrepareSpec(tileX, tileY, tileCountOnX));
            }
        }

        private static WATCH w;


        private Transform getTileParent(int zoomLevel)
        {
            var parent = this.transform.Find(zoomLevel.ToString());

            if (parent == null)
            {
                var parentGO = new GameObject(zoomLevel.ToString());
                parent = parentGO.transform;
                parent.parent = this.transform;
            }

            return parent;
        }

        private void activateCurrentZoomLevelTilesOnly()
        {
            for (int i = (int) Math.Floor(Map.MinZoom); i <= (int) Math.Ceiling(Map.MaxZoom); i++)
            {
                Transform tileHolderForLevel = this.transform.Find(i.ToString());
                if (tileHolderForLevel != null)
                {
                    tileHolderForLevel.gameObject.SetActive(i == MapBehaviour.RoundedZoom);
                }
            }
        }

        private static int RingsAroundCenterToLoad => 5;

        // TODO: Calculate regarding display size
        private static int MaxTilesInMemory => 500;

        // TODO: Calculate regarding display size and memory size and memory settings of user
        /// <summary>
        /// Creates or resuses a tile behavior.
        /// </summary>
        /// <param name="x">The x position of the new tile.</param>
        /// <param name="y">The y position of the new tile.</param>
        /// <returns></returns>
        private TileBehaviour createTile(int x, int y)
        {
            TileBehaviour tile;

            if (TileBehaviours.Count < MaxTilesInMemory)
            {
                tile =
                    (Instantiate(tileTemplate.gameObject) as GameObject).GetComponent<TileBehaviour>();
            }
            else
            {
                tile = TileBehaviours.Dequeue();
                while (CurrentlyNeeded(tile))
                {
                    TileBehaviours.Enqueue(tile);
                    tile = TileBehaviours.Dequeue();
                }

                tile.Showing = false;
                tiles.Remove(tile.name);

                tile.reuses++;
                tile.oldName = tile.name;

                // Destroy(tile.GetComponent<Renderer>().material.mainTexture);
                if (tile.TextureIsDownloading)
                {
                    tile.DownloadingTextureIsCancelled = true;
                }
            }

            TileBehaviours.Enqueue(tile);

            return tile;
        }

        private bool CurrentlyNeeded(TileBehaviour tile)
        {
            bool needed =
                tile.zPos == MapBehaviour.RoundedZoom &&
                tile.xPos >= curX - RingsAroundCenterToLoad &&
                tile.xPos <= curX + RingsAroundCenterToLoad &&
                tile.yPos >= curY - RingsAroundCenterToLoad &&
                tile.yPos <= curY + RingsAroundCenterToLoad;
            return needed;
        }

        private Queue<TileBehaviour> _tileBehaviours;

        private Queue<TileBehaviour> TileBehaviours
        {
            get
            {
                if (_tileBehaviours == null)
                {
                    _tileBehaviours = new Queue<TileBehaviour>();
                }

                return _tileBehaviours;
            }
        }

        #endregion

        #region TileLayer interface

        /// <summary>
        /// Gets the numbers of tiles on each axis in respect to the map's zoom level.
        /// </summary>
        protected abstract void GetTileCountPerAxis(out int tileCountOnX, out int tileCountOnY);

        /// <summary>
        /// Gets the tile coordinates and offsets to the origin for the tile under the center of the map.
        /// </summary>
        protected abstract void GetCenterTile(int tileCountOnX, int tileCountOnY, out int tileX, out int tileY,
            out float offsetX, out float offsetZ);

        /// <summary>
        /// Gets the tile coordinates and offsets to the origin for the neighbour tile in the specified direction.
        /// </summary>
        protected abstract bool GetNeighbourTile(int tileX, int tileY, float offsetX, float offsetY, int tileCountOnX,
            int tileCountOnY, NeighbourTileDirection dir, out int nTileX, out int nTileY, out float nOffsetX,
            out float nOffsetZ);

        /// <summary>
        /// Requests the tile's texture and assign it.
        /// </summary>
        /// <param name='tileX'>
        /// Tile x.
        /// </param>
        /// <param name='tileY'>
        /// Tile y.
        /// </param>
        /// <param name='roundedZoom'>
        /// Rounded zoom.
        /// </param>
        /// <param name='tile'>
        /// Tile.
        /// </param>
        //protected abstract void RequestTile(int tileX, int tileY, int roundedZoom, TileBehaviour tile);

        /// <summary>
        /// Cancels the request for the tile's texture.
        /// </summary>
        /// <param name='tileX'>
        /// Tile x.
        /// </param>
        /// <param name='tileY'>
        /// Tile y.
        /// </param>
        /// <param name='roundedZoom'>
        /// Rounded zoom.
        /// </param>
        protected abstract void CancelTileRequest(int tileX, int tileY, int roundedZoom);

        #endregion

        #region Tile Downloader Coroutine

        private struct TilePrepareSpec
        {
            public int TileX;
            public int TileY;
            public int TileZ;
            public int TileCountOnX;
            public float HalfMapScale;

            public TilePrepareSpec(int tileX, int tileY, int tileCountOnX)
            {
                TileX = tileX;
                TileY = tileY;
                TileZ = MapBehaviour.RoundedZoom;
                TileCountOnX = tileCountOnX;
                HalfMapScale = MapBehaviour.RoundedHalfMapScale;
            }
        }

        private static Queue<TilePrepareSpec> tilePreparationQueue =
            new Queue<TilePrepareSpec>(MaxTilesInMemory);

        void PrepareAndRequestTile(TilePrepareSpec tilePrepareSpec)
        {
            // correct east and west exceedance:
            if (tilePrepareSpec.TileX < 0)
                tilePrepareSpec.TileX += tilePrepareSpec.TileCountOnX;
            else if (tilePrepareSpec.TileX >= tilePrepareSpec.TileCountOnX)
                tilePrepareSpec.TileX -= tilePrepareSpec.TileCountOnX;

            int centerTileX, centerTileY;
            float offsetX, offsetZ;
            GetCenterTile(0, 0, out centerTileX, out centerTileY,
                out offsetX, out offsetZ);
            offsetX += (tilePrepareSpec.TileX - centerTileX) * MapBehaviour.RoundedHalfMapScale;
            offsetZ -= (tilePrepareSpec.TileY - centerTileY) * MapBehaviour.RoundedHalfMapScale;

            var tileAddress = $"{tilePrepareSpec.TileZ}_{tilePrepareSpec.TileX}_{tilePrepareSpec.TileY}";
            if (tiles.ContainsKey(tileAddress) == false)
            {
                WATCH.StartMeasure("createTile");
                var tile = createTile(tilePrepareSpec.TileX, tilePrepareSpec.TileY);
                ADDED_CREATETILE += WATCH.TakeMeasure("createTile");
                tile.Showing = false;
                tile.SetPosition(tilePrepareSpec.TileX, tilePrepareSpec.TileY, tilePrepareSpec.TileZ);
                var transform1 = tile.transform;
                transform1.position = new Vector3(offsetX, tileTemplate.transform.position.y, offsetZ);
                transform1.localScale = new Vector3(tilePrepareSpec.HalfMapScale, 1.0f, tilePrepareSpec.HalfMapScale);
                tile.transform.parent = getTileParent(tilePrepareSpec.TileZ);

                tile.name = tileAddress;
                tiles.Add(tileAddress, tile);

                WATCH.StartMeasure("loadTexture");
                tile.LoadTexture();
                ADDED_LOADTEXTURE += WATCH.TakeMeasure("loadTexture");
                ADDED_CHANGEDTILES++;
            }
            else
            {
                ADDED_UNCHANGEDTILES++;
            }
        }

        public void Update()
        {
            for (int i = 0; i < 5; i++)
            {
                if (tilePreparationQueue.Count == 0)
                    return;

                PrepareAndRequestTile(tilePreparationQueue.Dequeue());
            }
        }

        #endregion
    }
}