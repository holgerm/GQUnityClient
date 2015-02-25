/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

/// <summary>
/// This class is responsible for drawing the map.\n
/// <strong>Please do not use it.</strong>\n
/// Perform all operations with the map through the class OnlineMaps.
/// </summary>
[Serializable]
public class OnlineMapsBuffer
{
    public delegate IEnumerable<OnlineMapsMarker> SortMarkersDelegate(IEnumerable<OnlineMapsMarker> markers);
    public static SortMarkersDelegate OnSortMarker;

    public OnlineMaps api;
    public Vector2 apiPosition;
    public int apiZoom;
    public OnlineMapsVector2i bufferPosition;
    public Color[] frontBuffer;
    public bool generateSmartBuffer = false;
    public int height;
    public List<OnlineMapsTile> newTiles;
    public OnlineMapsRedrawType redrawType;
    public OnlineMapsBufferStatus status = OnlineMapsBufferStatus.wait;
    public Color[] smartBuffer;
    public bool updateBackBuffer;
    public int width;

    private Color[] backBuffer;
    private int bufferZoom;
    private bool disposed;
    private OnlineMapsVector2i frontBufferPosition;
    private List<OnlineMapsBufferZoom> zooms;

    public Vector2 topLeftPosition
    {
        get
        {
            int countX = api.width / OnlineMapsUtils.tileSize;
            int countY = api.height / OnlineMapsUtils.tileSize;
            Vector2 p = OnlineMapsUtils.LatLongToTilef(apiPosition, apiZoom);

            p.x -= countX / 2f;
            p.y -= countY / 2f;

            return OnlineMapsUtils.TileToLatLong(p, apiZoom);
        }
    }

    public OnlineMapsBuffer(OnlineMaps api)
    {
        this.api = api;
    }

    private void ApplyNewTiles()
    {
        lock (newTiles)
        {
            foreach (OnlineMapsTile tile in newTiles)
            {
                if (tile.status == OnlineMapsTileStatus.disposed) continue;
                int counter = 30;
                while (tile.colors.Length < OnlineMapsUtils.sqrTileSize)
                {
                    Thread.Sleep(1);
                    counter--;
                    if (counter <= 0) break;
                }
                tile.ApplyColorsToChilds();
            }
            newTiles.Clear();
        }
    }

    public void ApplyTile(OnlineMapsTile tile)
    {
        if (newTiles == null) newTiles = new List<OnlineMapsTile>();
        lock (newTiles)
        {
            newTiles.Add(tile);
        }
    }

    private List<OnlineMapsTile> CreateParents(List<OnlineMapsTile> tiles, int zoom)
    {
        List<OnlineMapsTile> newParentTiles = new List<OnlineMapsTile>();

        OnlineMapsBufferZoom parentZoom = zooms.FirstOrDefault(z => z.id == zoom);
        if (parentZoom == null)
        {
            parentZoom = new OnlineMapsBufferZoom(zoom);
            zooms.Add(parentZoom);
        }

        foreach (OnlineMapsTile tile in tiles)
        {
            if (tile.parent == null) CreateTileParent(zoom, tile, parentZoom, newParentTiles);
            tile.parent.used = true;
        }

        foreach (OnlineMapsTile tile in newParentTiles.Where(t=> t.status != OnlineMapsTileStatus.loaded)) tile.GetColorsFromChilds();

        return newParentTiles;
    }

    private void CreateTileParent(int zoom, OnlineMapsTile tile, OnlineMapsBufferZoom parentZoom,
        List<OnlineMapsTile> newParentTiles)
    {
        int px = tile.x / 2;
        int py = tile.y / 2;

        OnlineMapsTile parent =
            parentZoom.tiles.FirstOrDefault(t => t.x == px && t.y == py);
        if (parent == null)
        {
            parent = new OnlineMapsTile(px, py, zoom, api) {OnSetColor = OnTileSetColor};
            parentZoom.tiles.Add(parent);
        }

        newParentTiles.Add(parent);
        parent.used = true;
        tile.SetParent(parent);
        if (parent.status == OnlineMapsTileStatus.loaded && tile.status != OnlineMapsTileStatus.loaded) parent.SetChildColor(tile);
    }

    public void Dispose()
    {
        try
        {
            lock (OnlineMapsTile.tiles)
            {
                foreach (OnlineMapsTile tile in OnlineMapsTile.tiles) tile.Dispose();
            }
            frontBuffer = null;
            backBuffer = null;
            smartBuffer = null;

            newTiles = null;
            zooms = null;
            OnlineMapsTile.tiles = null;
            disposed = true;
            GC.Collect();
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
        
    }

    public void GenerateFrontBuffer()
    {
        apiPosition = api.position;
        apiZoom = api.zoom;
        while (true)
        {
            while (status != OnlineMapsBufferStatus.start)
            {
                if (disposed) return;
                Thread.Sleep(1);
            }
            status = OnlineMapsBufferStatus.working;

            try
            {
                Vector2 pos = api.position;
                int zoom = api.zoom;
                bool fullRedraw = redrawType == OnlineMapsRedrawType.full;

                if (disposed)
                {
                    fullRedraw = true;
                    disposed = false;
                }
                else if (newTiles != null && api.target == OnlineMapsTarget.texture) ApplyNewTiles();

                bool backBufferUpdated = UpdateBackBuffer(pos, zoom, fullRedraw);
                GetFrontBufferPosition(pos, bufferPosition, zoom, api.width, api.height);

                if (backBufferUpdated)
                {
                    foreach (OnlineMapsDrawingElement element in api.drawingElements)
                        element.Draw(backBuffer, bufferPosition, width, height, zoom);
                    SetMarkersToBuffer(api.markers);
                }

                if (api.target == OnlineMapsTarget.texture)
                {
                    if (!api.useSmartTexture || !generateSmartBuffer) UpdateFrontBuffer(api.width, api.height);
                    else UpdateSmartBuffer(api.width, api.height);
                }
            }
            catch (Exception exception)
            {
                Debug.Log(exception.Message + "\n" + exception.StackTrace);
            }

            GC.Collect();

            status = OnlineMapsBufferStatus.complete;
            apiPosition = api.position;
            apiZoom = api.zoom;
        }
    }

    private OnlineMapsBufferZoom GetActiveZoom(int zoom)
    {
        OnlineMapsBufferZoom activeZoom;

        if (zooms == null) zooms = new List<OnlineMapsBufferZoom>();

        lock (zooms)
        {
            activeZoom = zooms.FirstOrDefault(z => z.id == zoom);
            if (activeZoom == null)
            {
                activeZoom = new OnlineMapsBufferZoom(zoom);
                zooms.Add(activeZoom);
            }
        }
        return activeZoom;
    }

    private OnlineMapsVector2i GetBackBufferPosition(Vector2 position, OnlineMapsVector2i _bufferPosition, int zoom, int apiWidth,
        int apiHeight)
    {
        Vector2 pos = OnlineMapsUtils.LatLongToTilef(position, zoom);

        int countX = apiWidth / OnlineMapsUtils.tileSize + 2;
        int countY = apiHeight / OnlineMapsUtils.tileSize + 2;

        pos.x -= countX / 2f + _bufferPosition.x - 1;
        pos.y -= countY / 2f + _bufferPosition.y - 1;

        int ix = (int) ((pos.x / countX) * width);
        int iy = (int) ((pos.y / countY) * height);

        return new OnlineMapsVector2i(ix, iy);
    }

    private void GetFrontBufferPosition(Vector2 position, OnlineMapsVector2i _bufferPosition, int zoom, int apiWidth,
        int apiHeight)
    {
        OnlineMapsVector2i pos = GetBackBufferPosition(position, _bufferPosition, zoom, apiWidth, apiHeight);
        int ix = pos.x;
        int iy = pos.y;

        if (iy < 0) iy = 0;
        else if (iy >= height - apiHeight) iy = height - apiHeight;

        frontBufferPosition = new OnlineMapsVector2i(ix, iy);
    }

    private Rect GetMarkerRect(OnlineMapsMarker marker)
    {
        const int s = OnlineMapsUtils.tileSize;
        Vector2 p = OnlineMapsUtils.LatLongToTilef(marker.position, bufferZoom);
        p.x -= bufferPosition.x;
        p.y -= bufferPosition.y;
        OnlineMapsVector2i ip = marker.GetAlignedPosition(new OnlineMapsVector2i((int)(p.x * s), (int)(p.y * s)));
        return new Rect(ip.x, ip.y, marker.width, marker.height);
    }

    private void InitTile(int zoom, OnlineMapsBufferZoom activeZoom, OnlineMapsVector2i pos, int maxY,
        List<OnlineMapsTile> newBaseTiles,
        int y, List<OnlineMapsTile> ts, int px)
    {
        int py = y + pos.y;
        if (py < 0 || py >= maxY) return;
        OnlineMapsTile tile = ts.FirstOrDefault(t => t.x == px && t.y == py);
        if (tile == null)
        {
            OnlineMapsTile parent = OnlineMapsTile.tiles.FirstOrDefault(
                t => t.zoom == zoom - 1 && t.x == px / 2 && t.y == py / 2);

            tile = new OnlineMapsTile(px, py, zoom, api, parent) {OnSetColor = OnTileSetColor};
            activeZoom.tiles.Add(tile);
        }
        newBaseTiles.Add(tile);
        tile.used = true;
        SetBufferTile(tile);
    }

    private void InitTiles(int zoom, OnlineMapsBufferZoom activeZoom, int countX, OnlineMapsVector2i pos, int countY,
        int maxY, List<OnlineMapsTile> newBaseTiles)
    {
        List<OnlineMapsTile> tiles =
            activeZoom.tiles.Where(t => t.provider == api.provider && t.type == api.type).ToList();

        int maxX = 2 << (bufferZoom - 1);
        for (int x = 0; x < countX; x++)
        {
            int px = x + pos.x;
            if (px < 0) px += maxX;
            else if (px >= maxX) px -= maxX;

            for (int y = 0; y < countY; y++) InitTile(zoom, activeZoom, pos, maxY, newBaseTiles, y, tiles, px);
        }
    }

    private void OnTileSetColor(OnlineMapsTile tile)
    {
        if (tile.zoom == bufferZoom) SetBufferTile(tile);
    }

    private Rect SetBufferTile(OnlineMapsTile tile)
    {
        if (api.target == OnlineMapsTarget.tileset) return default(Rect);
        const int s = OnlineMapsUtils.tileSize;
        int i = 0;
        int px = tile.x - bufferPosition.x;
        int py = tile.y - bufferPosition.y;

        int maxX = 2 << (tile.zoom - 1);

        if (px < 0) px += maxX;
        else if (px >= maxX) px -= maxX;

        px *= s;
        py *= s;

        if (px + s < 0 || py + s < 0 || px > width || py > height) return new Rect(0, 0, 0, 0);

        Color[] colors = tile.colors;

        lock (colors)
        {
            int maxSize = width * height;

            for (int y = py + s - 1; y >= py; y--)
            {
                int bp = y * width + px;
                if (bp + s < 0 || bp >= maxSize) continue;
                int l = s;
                if (bp < 0)
                {
                    l -= bp;
                    bp = 0;
                }
                else if (bp + s > maxSize)
                {
                    l -= maxSize - (bp + s);
                    bp = maxSize - s - 1;
                }

                try
                {
                    Array.Copy(colors, i, backBuffer, bp, l);
                }
                catch
                {
                }
                
                i += s;
            }

            return new Rect(px, py, OnlineMapsUtils.tileSize, OnlineMapsUtils.tileSize);
        }
    }

    private void SetColorToBuffer(Color clr, OnlineMapsVector2i ip, int y, int x)
    {
        if (clr.a == 0) return;
        if (clr.a < 1)
        {
            float alpha = clr.a;
            clr.a = 1;
            clr = Color.Lerp(backBuffer[(ip.y + y) * width + ip.x + x], clr, alpha);
        }
        backBuffer[(ip.y + y) * width + ip.x + x] = clr;
    }

    private void SetMarkerToBuffer(OnlineMapsMarker marker, Vector2 startPos, Vector2 endPos)
    {
        const int s = OnlineMapsUtils.tileSize;
        float mx = marker.position.x;
        if (((mx > startPos.x && mx < endPos.x) || (mx + 360 > startPos.x && mx + 360 < endPos.x) ||
             (mx - 360 > startPos.x && mx - 360 < endPos.x)) &&
            marker.position.y < startPos.y && marker.position.y > endPos.y)
        {
            while (marker.locked) Thread.Sleep(1);

            marker.locked = true;
            Vector2 p = OnlineMapsUtils.LatLongToTilef(marker.position, bufferZoom);
            p -= bufferPosition;
            OnlineMapsVector2i ip =
                marker.GetAlignedPosition(new OnlineMapsVector2i((int) (p.x * s), (int) (p.y * s)));

            Color[] markerColors = marker.colors;
            if (markerColors == null || markerColors.Length == 0) return;
            for (int x = 0; x < marker.width; x++)
            {
                if (ip.x + x < 0 || ip.x + x >= width) continue;
                for (int y = 0; y < marker.height; y++)
                {
                    if (ip.y + y < 0 || ip.y + y >= height) continue;
                    try
                    {
                        SetColorToBuffer(markerColors[(marker.height - y - 1) * marker.width + x], ip, y, x);
                    }
                    catch
                    {
                    }
                }
            }

            marker.locked = false;
        }
    }

    public void SetMarkersToBuffer(IEnumerable<OnlineMapsMarker> markers)
    {
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
        if (OnlineMapsControlBase.instance is OnlineMapsControlBase3D)
        {
            if (((OnlineMapsControlBase3D) OnlineMapsControlBase.instance).marker2DMode ==
                OnlineMapsMarker2DMode.billboard)
            {
                return;
            }
        }
#endif

        const int s = OnlineMapsUtils.tileSize;
        int countX = api.width / s + 2;
        int countY = api.height / s + 2;
        Vector2 startPos = OnlineMapsUtils.TileToLatLong(bufferPosition.x, bufferPosition.y, bufferZoom);
        Vector2 endPos = OnlineMapsUtils.TileToLatLong(bufferPosition.x + countX, bufferPosition.y + countY + 1,
            bufferZoom);

        if (endPos.x < startPos.x) endPos.x += 360;

        IEnumerable<OnlineMapsMarker> usedMarkers = markers.Where(m => m.enabled && m.range.InRange(bufferZoom));
        if (OnSortMarker != null) usedMarkers = OnSortMarker(usedMarkers);
        else usedMarkers = usedMarkers.OrderByDescending(m => m.position.y);

        foreach (OnlineMapsMarker marker in usedMarkers) SetMarkerToBuffer(marker, startPos, endPos);
    }

    private void UnloadOldTiles()
    {
        List<OnlineMapsTile> tiles = OnlineMapsTile.tiles;
        lock (tiles)
        {
            IEnumerable<OnlineMapsTile> oldTiles = tiles.Where(t => !t.used);

            foreach (OnlineMapsTile tile in oldTiles)
            {
                zooms.First(z => z.id == tile.zoom).tiles.Remove(tile);
                tile.Dispose();
            }

            //tiles.RemoveAll(t => t.status == OnlineMapsTileStatus.disposed);
        }
    }

    private bool UpdateBackBuffer(Vector2 position, int zoom, bool fullRedraw)
    {
        const int s = OnlineMapsUtils.tileSize;
        int countX = api.width / s + 2;
        int countY = api.height / s + 2;

        OnlineMapsVector2i pos = OnlineMapsUtils.LatLongToTile(position, zoom);
        pos.x -= countX / 2;
        pos.y -= countY / 2;

        int maxY = (2 << zoom) / 2;

        if (pos.y < 0) pos.y = 0;
        if (pos.y >= maxY - countY - 1) pos.y = maxY - countY - 1;

        if (api.target == OnlineMapsTarget.texture)
        {
            if (frontBuffer == null || frontBuffer.Length != api.width * api.height)
            {
                frontBuffer = new Color[api.width * api.height];
                fullRedraw = true;
            }

            if (backBuffer == null || width != countX * s || height != countY * s)
            {
                width = countX * s;
                height = countY * s;
                backBuffer = new Color[height * width];

                fullRedraw = true;
            }
        }

        if (!updateBackBuffer && !fullRedraw && bufferZoom == zoom && bufferPosition != null &&
            bufferPosition == pos) return false;

        updateBackBuffer = false;

        bufferPosition = pos;
        bufferZoom = zoom;

        OnlineMapsBufferZoom activeZoom = GetActiveZoom(zoom);

        List<OnlineMapsTile> newBaseTiles = new List<OnlineMapsTile>();

        foreach (OnlineMapsTile tile in OnlineMapsTile.tiles) tile.used = false;

        InitTiles(zoom, activeZoom, countX, pos, countY, maxY, newBaseTiles);

        if (api.target == OnlineMapsTarget.texture)
        {
            List<OnlineMapsTile> newParentTiles = CreateParents(newBaseTiles, zoom - 1);
            if (zoom - 2 >= 3)
            {
                newParentTiles = CreateParents(newParentTiles, zoom - 2);
                if (zoom - 3 >= 3) CreateParents(newParentTiles, zoom - 3);
            }
        }

        SetMarkersToBuffer(api.markers);
        UnloadOldTiles();

        lock (OnlineMapsTile.tiles)
        {
            UpdatePriorities(OnlineMapsTile.tiles);
        }
        
        return true;
    }

    private void UpdateFrontBuffer(int apiWidth, int apiHeight)
    {
        int i = 0;

        for (int y = frontBufferPosition.y + apiHeight - 1; y >= frontBufferPosition.y; y--)
        {
            Array.Copy(backBuffer, frontBufferPosition.x + y * width, frontBuffer, i, apiWidth);
            i += apiWidth;
        }
    }

    private void UpdatePriorities(List<OnlineMapsTile> tiles)
    {
        foreach (OnlineMapsTile tile in tiles)
        {
            int zOff = bufferZoom - tile.zoom;
            if (zOff == 3) tile.priority = 0;
            else if (zOff == 2) tile.priority = 1;
            else if (zOff == 1) tile.priority = 2;
            else if (zOff == 0) tile.priority = 3;
            else tile.priority = 4;
        }
    }

    private void UpdateSmartBuffer(int apiWidth, int apiHeight)
    {
        int w = apiWidth;
        int hw = w / 2;
        int hh = apiHeight / 2;

        if (smartBuffer == null || smartBuffer.Length != hw * hh) smartBuffer = new Color[hw * hh];

        for (int y = 0; y < hh; y++)
        {
            int sy = (hh - y - 1) * hw;
            int fy = (y * 2 + frontBufferPosition.y) * width + frontBufferPosition.x;
            int fny = (y * 2 + frontBufferPosition.y + 1) * width + frontBufferPosition.x + 1;
            for (int x = 0; x < hw; x++)
            {
                Color clr1 = backBuffer[fy + x * 2];
                Color clr2 = backBuffer[fny + x * 2];
                smartBuffer[sy + x] = Color.Lerp(clr1, clr2, 0.5f);
            }
        }
    }
}

[Serializable]
internal class OnlineMapsBufferZoom
{
    public readonly int id;
    public readonly List<OnlineMapsTile> tiles;

    public OnlineMapsBufferZoom(int zoom)
    {
        id = zoom;
        tiles = new List<OnlineMapsTile>();
    }
}