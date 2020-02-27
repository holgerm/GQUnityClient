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
using System.ComponentModel;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.UI.map;
using Code.GQClient.Util;
using Code.QM.Util;
using Code.UnitySlippyMap.Helpers;
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
            if (tileTemplate.transform.localScale.x != MapBehaviour.RoundedHalfMapScale)
                tileTemplate.transform.localScale = new Vector3(MapBehaviour.RoundedHalfMapScale, 1.0f,
                    MapBehaviour.RoundedHalfMapScale);
            
            _tileParents = new Dictionary<int, Transform>();
            for (int i = (int) Math.Floor(Map.MinZoom); i <= (int) Math.Ceiling(Map.MaxZoom); i++)
            {
                var levelT = new GameObject(i.ToString()).transform;
                levelT.parent = this.transform;
                _tileParents.Add(i, levelT);
                levelT.gameObject.SetActive(false);
            }
        }

        private Dictionary<int, Transform> _tileParents;
        
        private Transform GetTileParent(int zoomLevel)
        {
            return _tileParents[zoomLevel];
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

            if (TileObjectCache.Count == 0) 
                return;
            
            var tile = TileObjectCache.Dequeue();
            while (tile != null)
            {
                DestroyImmediate(tile.MyMaterial.mainTexture);
                DestroyImmediate(tile);
                if (TileObjectCache.Count > 0)
                tile = TileObjectCache.Dequeue();
            }
            TileObjectCache = null;
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

        #region Private methods

        private void UpdateTiles()
        {
            GetTileCountPerAxis(out var tileCountOnX, out _);
            GetCenterTile(0, 0, out var tileX, out var tileY, out _, out _);

            PrepareAndRequestTiles(tileX, tileY, tileCountOnX);
        }

        // this is a new version of GrowTiles():
        void PrepareAndRequestTiles(int tileX, int tileY, int tileCountOnX)
        {
            // request the center itself:
            EnqueueTileForPreparation(tileX, tileY, tileCountOnX);
            for (int i = 1; i <= RingsAroundCenterToLoad; i++)
            {
                PrepareAndRequestTilesOnRing(tileX, tileY, tileCountOnX, i);
            }
       }

        // this is a new version of GrowTiles():
        void PrepareAndRequestTilesOnRing(int tileX, int tileY, int tileCountOnX, int ringNr)
        {
            // let n be the ringNr.
            // move into the start position, n tiles north of our center
            tileY -= ringNr;

            // prepare start tile north of center:
            EnqueueTileForPreparation(tileX, tileY, tileCountOnX);

            // move and prepare n tiles east:
            for (int i = 1; i <= ringNr; i++)
            {
                tileX++;
                EnqueueTileForPreparation(tileX, tileY, tileCountOnX);
            }

            // move and prepare 2 * n tiles south:
            for (int i = 1; i <= ringNr * 2; i++)
            {
                tileY++;
                EnqueueTileForPreparation(tileX, tileY, tileCountOnX);
            }

            // move and prepare 2 * n tiles west:
            for (int i = 1; i <= ringNr * 2; i++)
            {
                tileX--;
                EnqueueTileForPreparation(tileX, tileY, tileCountOnX);
            }

            // move and prepare 2 * n tiles north:
            for (int i = 1; i <= ringNr * 2; i++)
            {
                tileY--;
                EnqueueTileForPreparation(tileX, tileY, tileCountOnX);
            }

            // move and prepare n - 1 tiles east again, just before the starting tile:
            for (int i = 1; i < ringNr; i++)
            {
                tileX++;
                EnqueueTileForPreparation(tileX, tileY, tileCountOnX);
            }
        }
        
        private void activateCurrentZoomLevelTilesOnly()
        {
            for (int i = (int) Math.Floor(Map.MinZoom); i <= (int) Math.Ceiling(Map.MaxZoom); i++)
            {
                _tileParents[i].gameObject.SetActive(i == MapBehaviour.RoundedZoom);
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
            
            Debug.Log($"TileObjectCache.Count : {TileObjectCache.Count}");

            if (TileObjectCache.Count < MaxTilesInMemory)
            {
                tile =
                    (Instantiate(tileTemplate.gameObject) as GameObject).GetComponent<TileBehaviour>();
            }
            else
            {
                tile = TileObjectCache.Dequeue();
                while (CurrentlyNeeded(tile))
                {
                    TileObjectCache.Enqueue(tile);
                    tile = TileObjectCache.Dequeue();
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

            TileObjectCache.Enqueue(tile);

            return tile;
        }

        private bool CurrentlyNeeded(TileBehaviour tile)
        {
            int myX, myY;
            GetCenterTile(0, 0, out myX, out myY, out _, out _);

            bool needed =
                tile.zPos == MapBehaviour.RoundedZoom &&
                tile.xPos >= myX - RingsAroundCenterToLoad &&
                tile.xPos <= myX + RingsAroundCenterToLoad &&
                tile.yPos >= myY - RingsAroundCenterToLoad &&
                tile.yPos <= myY + RingsAroundCenterToLoad;
            return needed;
        }

        private static Queue<TileBehaviour> _tileBehaviours;

        private static Queue<TileBehaviour> TileObjectCache
        {
            set
            {
                if (value == null)
                    _tileBehaviours = null;
            }
            get
            {
                if (_tileBehaviours == null)
                {
                    Debug.Log("NEW TILEBEHAVIOUR QUEUE".Red());
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
            public string Adress;
            public int CenterX, CenterY;

            public TilePrepareSpec(int tileX, int tileY, int tileCountOnX, int centerX, int centerY)
            {
                TileX = tileX;
                TileY = tileY;
                TileZ = MapBehaviour.RoundedZoom;
                TileCountOnX = tileCountOnX;
                HalfMapScale = MapBehaviour.RoundedHalfMapScale;
                Adress = $"{TileZ}_{TileX}_{TileY}";
                CenterX = centerX;
                CenterY = centerY;
            }
        }

        private static int MaxTiles2LoadAtOnce
        {
            get { return (RingsAroundCenterToLoad * 2 + 1) * (RingsAroundCenterToLoad * 2 + 1); }
        }

        private static Queue<TilePrepareSpec> tilePreparationQueue =
            new Queue<TilePrepareSpec>(MaxTiles2LoadAtOnce);

        private void EnqueueTileForPreparation(int tileX, int tileY, int tileCountOnX)
        {
            if (tiles.ContainsKey($"{MapBehaviour.RoundedZoom}_{tileX}_{tileY}"))
                return;

            if (tilePreparationQueue.Count == MaxTiles2LoadAtOnce)
            {
                tilePreparationQueue.Dequeue();
            }

            CalculateCenterTile(out var cx, out var cy);

            var tileSpec =
                new TilePrepareSpec(tileX, tileY, tileCountOnX, cx, cy);

            tilePreparationQueue.Enqueue((tileSpec));
        }

        private void CalculateCenterTile(out int tileX, out int tileY)
        {
            int[] tileCoordinates =
                GeoHelpers.WGS84ToTile(Map.CenterWGS84[0], Map.CenterWGS84[1], MapBehaviour.RoundedZoom);

            tileX = tileCoordinates[0];
            tileY = tileCoordinates[1];
        }

        private void CalculateOffsets(int centerX, int centerY, out float offsetX, out float offsetZ)
        {
            double[] centerTile =
                GeoHelpers.TileToWGS84(centerX, centerY, MapBehaviour.RoundedZoom);
            double[] centerTileMeters =
                Map.WGS84ToEPSG900913Transform
                    .Transform(centerTile); //GeoHelpers.WGS84ToMeters(centerTile[0], centerTile[1]);

            offsetX = MapBehaviour.RoundedHalfMapScale / 2.0f -
                      (float) (Map.CenterEPSG900913[0] - centerTileMeters[0]) * Map.RoundedScaleMultiplier;
            offsetZ = -MapBehaviour.RoundedHalfMapScale / 2.0f -
                      (float) (Map.CenterEPSG900913[1] - centerTileMeters[1]) * Map.RoundedScaleMultiplier;
        }

        private TilePrepareSpec DequeueTileForPreparation()
        {
            // if (tilePreparationQueue.Count == 1)
            // {
            //     activateCurrentZoomLevelTilesOnly();
            // }

            return tilePreparationQueue.Dequeue();
        }

        void PrepareAndRequestTile(TilePrepareSpec tilePrepareSpec)
        {
            // correct east and west exceedance:
            if (tilePrepareSpec.TileX < 0)
                tilePrepareSpec.TileX += tilePrepareSpec.TileCountOnX;
            else if (tilePrepareSpec.TileX >= tilePrepareSpec.TileCountOnX)
                tilePrepareSpec.TileX -= tilePrepareSpec.TileCountOnX;

            CalculateOffsets(tilePrepareSpec.CenterX, tilePrepareSpec.CenterY, out var offsetX, out var offsetZ);
            offsetX += (tilePrepareSpec.TileX - tilePrepareSpec.CenterX) * tilePrepareSpec.HalfMapScale;
            offsetZ -= (tilePrepareSpec.TileY - tilePrepareSpec.CenterY) * tilePrepareSpec.HalfMapScale;

            if (tiles.ContainsKey(tilePrepareSpec.Adress) == false)
            {
                var tile = createTile(tilePrepareSpec.TileX, tilePrepareSpec.TileY);
                tile.SetPosition(tilePrepareSpec.TileX, tilePrepareSpec.TileY, tilePrepareSpec.TileZ);
                var transform1 = tile.transform;
                transform1.position = new Vector3(offsetX, tileTemplate.transform.position.y,
                    offsetZ);
                transform1.localScale = new Vector3(tilePrepareSpec.HalfMapScale, 1.0f, tilePrepareSpec.HalfMapScale);
                tile.transform.parent = GetTileParent(tilePrepareSpec.TileZ);

                tile.name = tilePrepareSpec.Adress;
                tiles.Add(tilePrepareSpec.Adress, tile);

                Base.Instance.StartCoroutine(tile.LoadTexture(this));
                tilesLoading.Add(tile);
            }
        }

        private HashSet<TileBehaviour> tilesLoading = new HashSet<TileBehaviour>();

        public void TileLoadingFinished(TileBehaviour tile)
        {
            tilesLoading.Remove(tile);
            if (tilesLoading.Count == 0)
            {
                activateCurrentZoomLevelTilesOnly();
            }
        }

        public void Update()
        {
            for (int i = 0; i < 5; i++)
            {
                if (tilePreparationQueue.Count == 0)
                {
                    return;
                }

                PrepareAndRequestTile(DequeueTileForPreparation());
            }
        }

        #endregion
    }
}