/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// This class of buffer tile image. \n
/// <strong>Please do not use it.</strong> \n
/// Perform all operations with the map through the class OnlineMaps.
/// </summary>
[Serializable]
public class OnlineMapsTile
{
    public static Color[] defaultColors;

    [NonSerialized]
    private static List<OnlineMapsTile> _tiles;

    [NonSerialized]
    public Action<OnlineMapsTile> OnSetColor;

    public OnlineMaps api;
    public Vector2 bottomRight;
    public object customData;
    public byte[] data;
    public Vector2 globalPosition;
    public bool hasColors;
    public bool isMapTile;
    public bool labels;
    public string language;

    [NonSerialized]
    public OnlineMapsTile parent;
    public short priority;
    public OnlineMapsProviderEnum provider;
    public OnlineMapsTileStatus status = OnlineMapsTileStatus.none;
    public Texture2D texture;
    public Vector2 topLeft;
    public Texture2D trafficTexture;
    public string trafficURL;
    public WWW trafficWWW;
    public int type;
    public bool used = true;
    public WWW www;
    public int x;
    public int y;
    public int zoom;

    private string _cacheFilename;
    private Color[] _colors;
    private string _url;

    [NonSerialized]
    private OnlineMapsTile[] childs = new OnlineMapsTile[4];
    private bool hasChilds;
    private byte[] labelData;
    private Color[] labelColors;

    public Color[] colors
    {
        get
        {
            return _colors ?? defaultColors;
        }
    }

    public string resourcesPath
    {
        get { return string.Format("OnlineMapsTiles/{0}/{1}/{2}", zoom, x, y); }
    }

    public static List<OnlineMapsTile> tiles
    {
        get { return _tiles ?? (_tiles = new List<OnlineMapsTile>()); }
        set { _tiles = value; }
    }

    public string url
    {
        get
        {
            if (string.IsNullOrEmpty(_url))
            {
                if (provider == OnlineMapsProviderEnum.arcGis) InitArcGis();
                else if (provider == OnlineMapsProviderEnum.google) InitGoogle();
                else if (provider == OnlineMapsProviderEnum.mapQuest) InitMapQuest();
                else if (provider == OnlineMapsProviderEnum.nokia) InitNokia();
                else if (provider == OnlineMapsProviderEnum.openStreetMap) InitOpenStreetMap();
                else if (provider == OnlineMapsProviderEnum.virtualEarth) InitVirtualEarth();
                else if (provider == OnlineMapsProviderEnum.custom) InitCustom();

                trafficURL = String.Format("http://mt0.google.com/mapstt?zoom={0}&x={1}&y={2}", zoom, x, y);
            }
            return _url;
        }
    }

    public OnlineMapsTile(int x, int y, int zoom, OnlineMaps api, bool isMapTile = true)
    {
        int maxX = 2 << (zoom - 1);
        if (x < 0) x += maxX;
        else if (x >= maxX) x -= maxX;

        this.x = x;
        this.y = y;
        this.zoom = zoom;

        this.api = api;
        this.isMapTile = isMapTile;

        texture = api.defaultTileTexture;

        provider = api.provider;
        type = api.type;
        labels = api.labels;
        language = api.language;

        topLeft = OnlineMapsUtils.TileToLatLong(x, y, zoom);
        bottomRight = OnlineMapsUtils.TileToLatLong(x + 1, y + 1, zoom);
        globalPosition = Vector2.Lerp(topLeft, bottomRight, 0.5f);

        if (isMapTile)
        {
            lock (tiles)
            {
                tiles.Add(this);
            }
        }
    }

    public OnlineMapsTile(int x, int y, int zoom, OnlineMaps api, OnlineMapsTile parent)
        : this(x, y, zoom, api)
    {
        this.parent = parent;
        if (parent != null && parent.status == OnlineMapsTileStatus.loaded) parent.SetChildColor(this);
    }

    public void ApplyColorsToChilds()
    {
        if (OnSetColor != null) OnSetColor(this);
        if (hasChilds)
        {
            foreach (OnlineMapsTile child in childs)
                if (child != null && (child.status != OnlineMapsTileStatus.loaded)) SetChildColor(child);
        }
    }

    private void ApplyLabelTexture()
    {
        Texture2D t = new Texture2D(OnlineMapsUtils.tileSize, OnlineMapsUtils.tileSize);
        t.LoadImage(labelData);
        labelData = null;
        labelColors = t.GetPixels();
        
        if (api.target == OnlineMapsTarget.texture)
        {
            OnlineMapsThreadManager.AddThreadAction(MergeColors);
            Object.Destroy(t);
        }
        else
        {
            _colors = ((Texture2D)texture).GetPixels();
            MergeColors();
            t.SetPixels(_colors);
            texture = t;
            _colors = null;
        }
    }

    public void ApplyTexture(Texture2D texture)
    {
        _colors = texture.GetPixels();
        status = OnlineMapsTileStatus.loaded;
        hasColors = true;
    }

    public bool checkCRC(byte[] bytes)
    {
        if (provider == OnlineMapsProviderEnum.arcGis &&
            OnlineMapsUtils.GetMD5(bytes) == "f27d9de7f80c13501f470595e327aa6d") return false;
        if (provider == OnlineMapsProviderEnum.mapQuest &&
            OnlineMapsUtils.GetMD5(bytes) == "bb0f77951f30229b14a4d3ed0b836390") return false;
        return true;
    }

    private string CustimProviderReplaceToken(Match match)
    {
        string v = match.Value.ToLower().Trim('{', '}');
        if (v == "zoom") return zoom.ToString();
        if (v == "x") return x.ToString();
        if (v == "y") return y.ToString();
        if (v == "quad") return OnlineMapsUtils.TileToQuadKey(x, y, zoom);
        return v;
    }

    public void Dispose()
    {
        if (status == OnlineMapsTileStatus.disposed) return;
        www = null;
        _colors = null;
        _url = null;
        trafficTexture = null;
        OnSetColor = null;
        status = OnlineMapsTileStatus.disposed;
        if (hasChilds) foreach (OnlineMapsTile child in childs) if (child != null) child.parent = null;
        if (parent != null)
        {
            if (parent.childs != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (parent.childs[i] == this)
                    {
                        parent.childs[i] = null;
                        break;
                    }
                }
            }
            parent = null;
        }
        childs = null;
        hasChilds = false;
        hasColors = false;
    }

    public void GetColorsFromChilds()
    {
        if (!childs.All(c => c != null && c.status == OnlineMapsTileStatus.loaded)) return;

        const int s = OnlineMapsUtils.tileSize;
        const int hs = s / 2;
        _colors = new Color[OnlineMapsUtils.sqrTileSize];

        for (int i = 0; i < 4; i++)
        {
            int cx = i / 2;
            int cy = 1 - i % 2;
            OnlineMapsTile tile = childs[i];
            if (tile == null) OnlineMapsUtils.ApplyColorArray(ref _colors, cx * hs, cy * hs, hs, hs, ref defaultColors, cx * hs, cy * hs);
            else OnlineMapsUtils.ApplyColorArray(ref _colors, cx * hs, cy * hs, hs, hs, ref tile._colors, 0, 0, 2, 2);
        }
        hasColors = true;
    }

    public Rect GetRect()
    {
        return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
    }

    public bool InScreen(Vector2 tl, Vector2 br)
    {
        if (bottomRight.x < tl.x) return false;
        if (topLeft.x > br.x) return false;
        if (topLeft.y < br.y) return false;
        if (bottomRight.y > tl.y) return false;
        return true;
    }

    private void InitArcGis()
    {
        string maptype = "World_Imagery";
        if (type == 1) maptype = "World_Topo_Map";
        const string server =
            "http://server.arcgisonline.com/ArcGIS/rest/services/{3}/MapServer/tile/{0}/{1}/{2}";
        _url = String.Format(server, zoom, y, x, maptype);
    }

    private void InitCustom()
    {
        _url = Regex.Replace(api.customProviderURL, @"{\w+}", CustimProviderReplaceToken);
    }

    private void InitGoogle()
    {
        string server = "https://khm0.google.ru/kh/v=152&src=app&hl={3}&x={0}&y={1}&z={2}&s=";
        if (type == 0 && labels) server = string.Format("http://mt0.googleapis.com/vt/lyrs=y&hl={3}&x={0}&y={1}&z={2}", x, y, zoom, language);

        if (type == 1) server = "https://mts0.google.com/vt/lyrs=t@131,r@216000000&src=app&hl={3}&x={0}&y={1}&z={2}&s=";
        else if (type == 2) server = "https://mts0.google.com/vt/src=app&hl={3}&x={0}&y={1}&z={2}&s=";
        _url = String.Format(server, x, y, zoom, language);
    }

    private void InitMapQuest()
    {
        string maptype = "sat";
        if (type == 1) maptype = "map";
        const string server = "http://ttiles01.mqcdn.com/tiles/1.0.0/vy/{3}/{0}/{1}/{2}.png";
        _url = String.Format(server, zoom, x, y, maptype);
    }

    private void InitNokia()
    {
        string maptype = "satellite.day";
        if (type == 0 && labels) maptype = "hybrid.day";
        else if (type == 1) maptype = "terrain.day";
        else if (type == 2) maptype = "normal.day";

        const string server =
            "http://{0}.maps.nlp.nokia.com/maptile/2.1/maptile/newest/{4}/{1}/{2}/{3}/256/png8?lg={5}&token=10KBSE6DwHtRnKzeu6Ohkw&app_id=tXMk8FVbu9QmIHL3eMtl";
        _url = string.Format(server, 1, zoom, x, y, maptype, language);
    }

    private void InitOpenStreetMap()
    {
        const string server = "http://a.tile.openstreetmap.org/{0}/{1}/{2}.png";
        _url = String.Format(server, zoom, x, y);
    }

    private void InitVirtualEarth()
    {
        string quad = OnlineMapsUtils.TileToQuadKey(x, y, zoom);
        string server = "";
        if (type == 0 && !labels)
        {
            server = "http://ak.t0.tiles.virtualearth.net/tiles/a{0}.jpeg?mkt={1}&g=1457&n=z";
        }
        else if (type == 0 && labels)
        {
            server = "http://ak.dynamic.t0.tiles.virtualearth.net/comp/ch/{0}?mkt={1}&it=A,G,L,LA&og=30&n=z";
            
        }
        else if (type == 1)
        {
            server = "http://ak.dynamic.t0.tiles.virtualearth.net/comp/ch/{0}?mkt={1}&it=G,VE,BX,L,LA&og=30&n=z";
        }
        _url = String.Format(server, quad, language);
    }

    public void LoadTexture()
    {
        if (status == OnlineMapsTileStatus.error) return;

        Texture2D texture = new Texture2D(OnlineMapsUtils.tileSize, OnlineMapsUtils.tileSize);
        texture.LoadImage(data);
        ApplyTexture(texture);
        Object.Destroy(texture);

        if (labelData != null) ApplyLabelTexture();
    }

    private void MergeColors()
    {
        for (int i = 0; i < _colors.Length; i++)
        {
            float a = labelColors[i].a;
            if (a != 0)
            {
                labelColors[i].a = 1;
                _colors[i] = Color.Lerp(_colors[i], labelColors[i], a);
            }
        }
    }

    public void OnDownloadComplete()
    {
        data = www.bytes;
        LoadTexture();
        data = null;
    }

    public void OnDownloadError()
    {
        status = OnlineMapsTileStatus.error;
    }

    public bool OnLabelDownloadComplete()
    {
        labelData = trafficWWW.bytes;
        if (status == OnlineMapsTileStatus.loaded)
        {
            ApplyLabelTexture();
            return true;
        }
        return false;
    }

    private void SetChild(OnlineMapsTile tile)
    {
        int cx = tile.x % 2;
        int cy = tile.y % 2;
        childs[cx * 2 + cy] = tile;
        hasChilds = true;
    }

    public void SetChildColor(OnlineMapsTile child)
    {
        if (child == null) return;
        if (colors == null) _colors = new Color[OnlineMapsUtils.sqrTileSize];
        if (child._colors == null) child._colors = new Color[OnlineMapsUtils.sqrTileSize];

        const int s = OnlineMapsUtils.tileSize;
        const int hs = s / 2;
        int sx = (child.x % 2) * hs;
        int sy = hs - (child.y % 2) * hs;

        for (int py = 0; py < s; py++)
        {
            int spy = py * s;
            int hpy = (py / 2 + sy) * s;
            for (int px = 0; px < s; px++)
            {
                try
                {
                    child._colors[spy + px] = _colors[hpy + px / 2 + sx];
                }
                catch
                {
                }
            }
        }

        child.hasColors = true;
        if (child.OnSetColor != null) child.OnSetColor(child);

        if (child.hasChilds)
        {
            foreach (OnlineMapsTile tile in child.childs)
                if (tile != null && (tile.status != OnlineMapsTileStatus.loaded)) child.SetChildColor(tile);
        }
    }

    public void SetParent(OnlineMapsTile tile)
    {
        parent = tile;
        parent.SetChild(this);
    }

    public override string ToString()
    {
        return string.Format("{0}x{1}.jpg", x, y);
    }
}