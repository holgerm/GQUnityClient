/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

/// <summary>
/// The main class. With it you can control the map.
/// </summary>
[AddComponentMenu("Infinity Code/Online Maps/Online Maps")]
[Serializable]
public class OnlineMaps : MonoBehaviour
{
    /// <summary>
    /// The current version of Online Maps
    /// </summary>
    public const string version = "1.6.1.6";

    /// <summary>
    /// Allows you to customize the appearance of the tooltip.
    /// </summary>
    /// <param name="style">The reference to the style.</param>
    public delegate void OnPrepareTooltipStyleHandler(ref GUIStyle style);

    private static OnlineMaps _instance;

    /// <summary>
    /// Display control script.
    /// </summary>
    public OnlineMapsControlBase control;

    /// <summary>
    /// URL of custom provider.\n
    /// Support tokens:\n
    /// {x} - tile x\n
    /// {y} - tile y\n
    /// {zoom} - zoom level\n
    /// {quad} - uniquely identifies a single tile at a particular level of detail.
    /// </summary>
    public string customProviderURL = "http://localhost/{zoom}/{y}/{x}";

    /// <summary>
    /// Alignment marker default.
    /// </summary>
    public OnlineMapsAlign defaultMarkerAlign = OnlineMapsAlign.Bottom;

    /// <summary>
    /// Texture used by default for the marker.
    /// </summary>
    public Texture2D defaultMarkerTexture;

    /// <summary>
    /// Texture displayed until the tile is not loaded.
    /// </summary>
    public Texture2D defaultTileTexture;

    /// <summary>
    /// Specifies whether to dispatch the event.
    /// </summary>
    public bool dispatchEvents = true;

    /// <summary>
    /// The drawing elements.
    /// </summary>
    public List<OnlineMapsDrawingElement> drawingElements;

    /// <summary>
    /// Color, which is used until the tile is not loaded, unless specified field defaultTileTexture.
    /// </summary>
    public Color emptyColor = Color.gray;

    /// <summary>
    /// Map height in pixels.
    /// </summary>
    public int height;

    /// <summary>
    /// Specifies whether to display the labels on the map.
    /// </summary>
    public bool labels = true;

    /// <summary>
    /// Language of the labels on the map.
    /// </summary>
    public string language = "en";

    /// <summary>
    /// List of all 2D markers. <br/>
    /// Use AddMarker, RemoveMarker and RemoveAllMarkers.
    /// </summary>
    public OnlineMapsMarker[] markers;

    /// <summary>
    /// A flag that indicates that need to redraw the map.
    /// </summary>
    public bool needRedraw;

    /// <summary>
    /// Limits the range of map coordinates.
    /// </summary>
    public OnlineMapsPositionRange positionRange;

    /// <summary>
    /// Map provider.
    /// </summary>
    public OnlineMapsProviderEnum provider = OnlineMapsProviderEnum.nokia;

    /// <summary>
    /// A flag that indicates whether to redraw the map at startup.
    /// </summary>
    public bool redrawOnPlay;

    /// <summary>
    /// Skin for the tooltip.
    /// </summary>
    public GUISkin skin;

    /// <summary>
    /// Indicates when the marker will show tips.
    /// </summary>
    public OnlineMapsShowMarkerTooltip showMarkerTooltip = OnlineMapsShowMarkerTooltip.onHover;

    /// <summary>
    /// Reduced texture that is displayed when the user move map.
    /// </summary>
    public Texture2D smartTexture;

    /// <summary>
    /// Specifies from where the tiles should be loaded (Online, Resources, Online and Resources).
    /// </summary>
    public OnlineMapsSource source = OnlineMapsSource.Online;

    /// <summary>
    /// Specifies where the map should be drawn (Texture or Tileset).
    /// </summary>
    public OnlineMapsTarget target = OnlineMapsTarget.texture;

    /// <summary>
    /// Texture, which is used to draw the map. <br/>
    /// <strong>To change this value, use OnlineMaps.SetTexture.</strong>
    /// </summary>
    public Texture2D texture;

    /// <summary>
    /// Width of tileset in pixels.
    /// </summary>
    public int tilesetWidth = 1024;

    /// <summary>
    /// Height of tileset in pixels.
    /// </summary>
    public int tilesetHeight = 1024;

    /// <summary>
    /// Tileset size in scene;
    /// </summary>
    public Vector2 tilesetSize = new Vector2(1024, 1024);

    /// <summary>
    /// Tooltip, which will be shown.
    /// </summary>
    public string tooltip = string.Empty;

    /// <summary>
    /// Marker for which displays tooltip.
    /// </summary>
    public OnlineMapsMarkerBase tooltipMarker;

    /// <summary>
    /// Specifies whether to draw traffic.
    /// </summary>
    public bool traffic = false;

    /// <summary>
    /// Map type.
    /// </summary>
    public int type;

    /// <summary>
    /// Specifies whether when you move the map showing the reduction texture.
    /// </summary>
    public bool useSmartTexture = true;

    /// <summary>
    /// Specifies whether the user interacts with the map.
    /// </summary>
    public static bool isUserControl = false;

    /// <summary>
    /// Map width in pixels.
    /// </summary>
    public int width;

    /// <summary>
    /// Specifies the valid range of map zoom.
    /// </summary>
    public OnlineMapsRange zoomRange;

    /// <summary>
    /// Event caused when the user change map position.
    /// </summary>
    /// <remarks></remarks>
    /// <example>
    /// Example:
    /// <code>
    /// public class ChangePositionExample : MonoBehaviour
    /// {
    ///    private void OnChangePosition()
    ///    {
    ///        Debug.Log(OnlineMaps.instance.position);
    ///    }
    ///
    ///    private void OnEnable()
    ///    {
    ///        OnlineMaps.instance.OnChangePosition += OnChangePosition;
    ///    }
    /// }
    /// </code>
    /// </example>
    public Action OnChangePosition;

    /// <summary>
    /// Event caused when the user change map zoom.
    /// </summary>
    /// <remarks></remarks>
    /// <example>
    /// Example:
    /// <code>
    /// public class ChangeZoomExample : MonoBehaviour
    /// {
    ///     private void OnEnable()
    ///     {
    ///         OnlineMaps.instance.OnChangeZoom += OnChangeZoom;
    ///     }
    /// 
    ///     private void OnChangeZoom()
    ///     {
    ///         Debug.Log(OnlineMaps.instance.zoom);
    ///     }
    /// }
    /// </code>
    /// </example>
    public Action OnChangeZoom;

    /// <summary>
    /// Event caused after received and processed a request to search for a location.
    /// </summary>
    /// <remarks></remarks>
    /// <example>
    /// Example:
    /// <code>
    /// public class OnFindLocationAfterExample : MonoBehaviour
    /// {
    ///    private void OnEnable()
    ///    {
    ///        OnlineMaps.instance.OnFindLocationAfter += OnFindLocationAfter;
    ///    }
    ///
    ///    private void OnFindLocationAfter()
    ///    {
    ///        Debug.Log("Search for a location is completed.");
    ///    }
    /// }
    /// </code>
    /// </example>
    public Action OnFindLocationAfter;

    /// <summary>
    /// Event caused when preparing tooltip style.
    /// </summary>
    /// <remarks></remarks>
    /// <example>
    /// Example:
    /// <code>
    /// public class ModifyTooltipStyleExample : MonoBehaviour
    /// {
    ///     private void OnEnable()
    ///     {
    ///         OnlineMaps.instance.OnPrepareTooltipStyle += OnPrepareTooltipStyle;
    ///     }
    /// 
    ///     private void OnPrepareTooltipStyle(ref GUIStyle style)
    ///     {
    ///         style.fontSize = Screen.width / 50;
    ///     }
    /// }
    /// </code>
    /// </example>
    public OnPrepareTooltipStyleHandler OnPrepareTooltipStyle;

    /// <summary>
    /// An event that occurs when loading the tile. Allows you to intercept of loading tile, and load it yourself.
    /// </summary>
    public Action<OnlineMapsTile> OnStartDownloadTile;

    public Vector2 _position;
    public int _zoom;

    private Vector2 _bottomRightPosition;
    private OnlineMapsBuffer _buffer;
    private List<OnlineMapsGoogleAPIQuery> _googleQueries;
    private bool _labels;
    private string _language;
    private OnlineMapsProviderEnum _provider;
    private Vector2 _topLeftPosition;
    private bool _traffic;
    private int _type;
    
    private Texture2D activeTexture;
    private bool allowRedraw;
    private Action<bool> checkConnectioCallback;
    private WWW checkConnectionWWW;
    private Color[] defaultColors;
    private OnlineMapsTile downloads;
    private long lastGC;
    private OnlineMapsRedrawType redrawType = OnlineMapsRedrawType.none;
    private Thread renderThread;
    private OnlineMapsMarker rolledMarker;


    /// <summary>
    /// Singleton instance of map.
    /// </summary>
    public static OnlineMaps instance
    {
        get { return _instance; }
    }

    /// <summary>
    /// Gets whether the selected provider labels.
    /// </summary>
    /// <value>
    /// true if available labels, false if not.
    /// </value>
    public bool availableLabels
    {
        get
        {
            if (provider == OnlineMapsProviderEnum.google && type == 0) return true;
            if (provider == OnlineMapsProviderEnum.nokia && type == 0) return true;
            if (provider == OnlineMapsProviderEnum.virtualEarth && type == 0) return true;
            return false;
        }
    }

    /// <summary>
    /// Gets a list of available map types on selected provider.
    /// </summary>
    /// <value>
    /// A list of available map types.
    /// </value>
    public string[] availableTypes
    {
        get
        {
            string[] types = {"Satellite", "Relief", "Terrain", "Map"};
            if (provider == OnlineMapsProviderEnum.arcGis) return new[] {types[0], types[2]};
            if (provider == OnlineMapsProviderEnum.google) return new[] {types[0], types[1], types[2]};
            if (provider == OnlineMapsProviderEnum.mapQuest) return new[] {types[0], types[2]};
            if (provider == OnlineMapsProviderEnum.nokia) return new[] {types[0], types[2], types[3]};
            if (provider == OnlineMapsProviderEnum.openStreetMap) return new[] {types[3]};
            if (provider == OnlineMapsProviderEnum.virtualEarth) return new[] {types[0], types[2]};
            if (provider == OnlineMapsProviderEnum.custom) return null;
            return types;
        }
    }

    /// <summary>
    /// Gets whether the selected provider label language.
    /// </summary>
    /// <value>
    /// true if language available, false if not.
    /// </value>
    public bool availableLanguage
    {
        get
        {
            if (provider == OnlineMapsProviderEnum.arcGis) return false;
            if (provider == OnlineMapsProviderEnum.mapQuest) return false;
            if (provider == OnlineMapsProviderEnum.openStreetMap) return false;
            return true;
        }
    }

    /// <summary>
    /// Gets the bottom right position.
    /// </summary>
    /// <value>
    /// The bottom right position.
    /// </value>
    public Vector2 bottomRightPosition
    {
        get
        {
            if (_bottomRightPosition == default(Vector2))
            {
                int countX = width / OnlineMapsUtils.tileSize;
                int countY = height / OnlineMapsUtils.tileSize;
                Vector2 p = OnlineMapsUtils.LatLongToTilef(position, zoom);

                p.x += countX / 2f;
                p.y += countY / 2f;

                _bottomRightPosition = OnlineMapsUtils.TileToLatLong(p, zoom);
            }

            return _bottomRightPosition;
        }
    }

    /// <summary>
    /// Reference to the current draw buffer.
    /// </summary>
    public OnlineMapsBuffer buffer
    {
        get { return _buffer ?? (_buffer = new OnlineMapsBuffer(this)); }
    }

    /// <summary>
    /// The current state of the drawing buffer.
    /// </summary>
    public OnlineMapsBufferStatus bufferStatus
    {
        get { return buffer.status; }
    }

    /// <summary>
    /// Checks whether the current settings display label.
    /// </summary>
    public bool enabledLabels
    {
        get
        {
            if (provider == OnlineMapsProviderEnum.arcGis && type == 1) return true;
            if (provider == OnlineMapsProviderEnum.google) return true;
            if (provider == OnlineMapsProviderEnum.mapQuest && type == 1) return true;
            if (provider == OnlineMapsProviderEnum.nokia) return true;
            if (provider == OnlineMapsProviderEnum.virtualEarth) return true;
            return false;
        }
    }

    private List<OnlineMapsGoogleAPIQuery> googleQueries
    {
        get { return _googleQueries ?? (_googleQueries = new List<OnlineMapsGoogleAPIQuery>()); }
    }

    /// <summary>
    /// Coordinates of the center point of the map.
    /// </summary>
    public Vector2 position
    {
        get { return _position; }
        set
        {
            Vector2 p = value;
            if (positionRange != null) p = positionRange.CheckAndFix(p);

            Vector2 tp = OnlineMapsUtils.LatLongToTilef(p, zoom);
            int countY = height / OnlineMapsUtils.tileSize;
            float haftCountY = countY / 2f;
            int maxY = (2 << zoom) / 2;
            bool modified = false;
            if (tp.y - haftCountY < 0)
            {
                tp.y = haftCountY;
                modified = true;
            }
            else if (tp.y + haftCountY >= maxY - 1)
            {
                tp.y = maxY - haftCountY - 1;
                modified = true;
            }

            if (modified) p = OnlineMapsUtils.TileToLatLong(tp, zoom);

            if (_position == p) return;

            allowRedraw = true;
            needRedraw = true;
            if (redrawType == OnlineMapsRedrawType.none || redrawType == OnlineMapsRedrawType.move)
                redrawType = OnlineMapsRedrawType.move;
            else redrawType = OnlineMapsRedrawType.full;

            _position = p;
            _topLeftPosition = default(Vector2);
            _bottomRightPosition = default(Vector2);

            DispatchEvent(OnlineMapsEvents.changedPosition);
        }
    }

    /// <summary>
    /// Gets the top left position.
    /// </summary>
    /// <value>
    /// The top left position.
    /// </value>
    public Vector2 topLeftPosition
    {
        get
        {
            if (_topLeftPosition == default(Vector2))
            {
                int countX = width / OnlineMapsUtils.tileSize;
                int countY = height / OnlineMapsUtils.tileSize;
                Vector2 p = OnlineMapsUtils.LatLongToTilef(position, zoom);

                p.x -= countX / 2f;
                p.y -= countY / 2f;

                _topLeftPosition = OnlineMapsUtils.TileToLatLong(p, zoom);
            }

            return _topLeftPosition;
        }
    }

    /// <summary>
    /// Current zoom.
    /// </summary>
    public int zoom
    {
        get { return _zoom; }
        set
        {
            int z = Mathf.Clamp(value, 3, 20);
            if (zoomRange != null) z = zoomRange.CheckAndFix(z);
            z = CheckMapSize(z);
            if (_zoom == z) return;

            _zoom = z;
            _topLeftPosition = default(Vector2);
            _bottomRightPosition = default(Vector2);
            allowRedraw = true;
            needRedraw = true;
            redrawType = OnlineMapsRedrawType.full;
            DispatchEvent(OnlineMapsEvents.changedZoom);
        }
    }

    /// <summary>
    /// Adds a drawing element.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <remarks></remarks>
    /// <example>
    /// Example:
    /// <code>
    /// OnlineMaps api = GetComponent<OnlineMaps>();
    ///
    ///	List<Vector2> points = new List<Vector2>();
    /// points.Add(new Vector2(-118.18955f, 33.76331f));
    /// points.Add(new Vector2(-118.17965f, 33.77331f));
    /// api.AddDrawingElement(new OnlineMapsDrawingLine(points, Color.blue, 1));
    /// </code>
    /// </example>
    public void AddDrawingElement(OnlineMapsDrawingElement element)
    {
        drawingElements.Add(element);
        needRedraw = true;
    }

    /// <summary>
    /// Adds a new request to the Google API in the processing queue.
    /// </summary>
    /// <param name="query">Queue</param>
    public void AddGoogleAPIQuery(OnlineMapsGoogleAPIQuery query)
    {
        googleQueries.Add(query);
    }

    /// <summary>
    /// Adds a 2D marker on the map.
    /// </summary>
    /// <param name="marker">
    /// The marker you want to add.
    /// </param>
    /// <returns>
    /// Marker instance.
    /// </returns>
    public OnlineMapsMarker AddMarker(OnlineMapsMarker marker)
    {
        List<OnlineMapsMarker> ms = markers.ToList();
        marker.Init();
        ms.Add(marker);
        markers = ms.ToArray();
        needRedraw = allowRedraw = true;
        return marker;
    }

    /// <summary>
    /// Adds a new 2D marker on the map.
    /// </summary>
    /// <param name="markerPosition">X - Longituge. Y - Latitude.</param>
    /// <param name="label">The text that will be displayed when you hover a marker.</param>
    /// <returns>Marker instance.</returns>
    public OnlineMapsMarker AddMarker(Vector2 markerPosition, string label)
    {
        return AddMarker(markerPosition, null, label);
    }

    /// <summary>
    /// Adds a new 2D marker on the map.
    /// </summary>
    /// <param name="markerPosition">X - Longituge. Y - Latitude.</param>
    /// <param name="markerTexture">
    /// <strong>Optional</strong><br/>
    /// Marker texture. <br/>
    /// In import settings must be enabled "Read / Write enabled". <br/>
    /// Texture format: ARGB32. <br/>
    /// If not specified, the will be used default marker texture.</param>
    /// <param name="label">
    /// <strong>Optional</strong><br/>
    /// The text that will be displayed when you hover a marker.</param>
    /// <returns>Marker instance.</returns>
    public OnlineMapsMarker AddMarker(Vector2 markerPosition, Texture2D markerTexture = null, string label = "")
    {
        if (markerTexture == null) markerTexture = defaultMarkerTexture;

        List<OnlineMapsMarker> ms = markers.ToList();
        OnlineMapsMarker marker = new OnlineMapsMarker
        {
            position = markerPosition,
            texture = markerTexture,
            label = label
        };
        marker.Init();
        ms.Add(marker);
        markers = ms.ToArray();
        needRedraw = allowRedraw = true;
        return marker;
    }

    /// <summary>
    /// Adds a 2D markers on the map.
    /// </summary>
    /// <param name="newMarkers">
    /// The markers.
    /// </param>
    public void AddMarkers(OnlineMapsMarker[] newMarkers)
    {
        List<OnlineMapsMarker> ms = markers.ToList();
        foreach (OnlineMapsMarker marker in newMarkers)
        {
            marker.Init();
            ms.Add(marker);
        }
        markers = ms.ToArray();
        needRedraw = allowRedraw = true;
    }

// ReSharper disable once UnusedMember.Global
    public void Awake()
    {
        _instance = this;

        if (target == OnlineMapsTarget.texture)
        {
            width = texture.width;
            height = texture.height;
        }
        else
        {
            width = tilesetWidth;
            height = tilesetHeight;
            texture = null;
        }

        control = GetComponent<OnlineMapsControlBase>();
        if (control == null) Debug.LogError("Can not find a Control.");
        else control.OnAwakeBefore();

        if (target == OnlineMapsTarget.texture)
        {
            if (texture != null) defaultColors = texture.GetPixels();

            if (defaultTileTexture == null)
            {
                OnlineMapsTile.defaultColors = new Color[OnlineMapsUtils.tileSize * OnlineMapsUtils.tileSize];
                for (int i = 0; i < OnlineMapsTile.defaultColors.Length; i++)
                    OnlineMapsTile.defaultColors[i] = emptyColor;
            }
            else OnlineMapsTile.defaultColors = defaultTileTexture.GetPixels();
        }

        foreach (OnlineMapsMarker marker in markers) marker.Init();

        if (target == OnlineMapsTarget.texture && useSmartTexture && smartTexture == null)
        {
            smartTexture = new Texture2D(texture.width / 2, texture.height / 2, TextureFormat.RGB24, false);
        }
    }

    private void CheckBaseProps()
    {
        if (provider != _provider || type != _type || _language != language || _labels != labels)
        {
            _labels = labels;
            _language = language;
            _provider = provider;
            _type = type;

            while (buffer.status == OnlineMapsBufferStatus.working)
            {
                Thread.Sleep(1);
            }

#if !UNITY_WEBPLAYER
            if (renderThread != null) renderThread.Abort();
#endif
            renderThread = null;

            buffer.Dispose();
            buffer.status = OnlineMapsBufferStatus.wait;
            
            Redraw();
        }
        if (traffic != _traffic)
        {
            _traffic = traffic;
            lock (OnlineMapsTile.tiles)
            {
                if (traffic)
                {
                    foreach (OnlineMapsTile tile in OnlineMapsTile.tiles)
                    {
#if !UNITY_WEBPLAYER
                        if (!string.IsNullOrEmpty(tile.trafficURL)) tile.trafficWWW = new WWW(tile.trafficURL);
#else
                        if (!string.IsNullOrEmpty(tile.trafficURL)) tile.trafficWWW = new WWW(OnlineMapsUtils.serviceURL + tile.trafficURL);
#endif
                    }
                }
                else
                {
                    foreach (OnlineMapsTile tile in OnlineMapsTile.tiles)
                    {
                        tile.trafficTexture = null;
                        tile.trafficWWW = null;
                    }
                }
            }
        }
    }

    private void CheckDownloadComplete()
    {
        if (checkConnectionWWW != null)
        {
            if (checkConnectionWWW.isDone)
            {
                checkConnectioCallback(string.IsNullOrEmpty(checkConnectionWWW.error));
                checkConnectionWWW = null;
                checkConnectioCallback = null;
            }
        }

        lock (OnlineMapsTile.tiles)
        {
            if (OnlineMapsTile.tiles.Count == 0) return;
            if (OnlineMapsTile.tiles.RemoveAll(t => t.status == OnlineMapsTileStatus.disposed) > 0) GCCollect();

            List<OnlineMapsTile> tiles = OnlineMapsTile.tiles.OrderBy(t=>t.priority).ToList();

            if (tiles.Count == 0) return;

            long startTicks = DateTime.Now.Ticks;

            foreach (OnlineMapsTile tile in tiles)
            {
                if (DateTime.Now.Ticks - startTicks > 50000) break;

                if (tile.status == OnlineMapsTileStatus.loading && tile.www != null && tile.www.isDone)
                {
                    if (string.IsNullOrEmpty(tile.www.error) && tile.checkCRC(tile.www.bytes))
                    {
                        if (target == OnlineMapsTarget.texture)
                        {
                            tile.OnDownloadComplete();
                            buffer.ApplyTile(tile);
                        }
                        else if (tile.zoom == buffer.apiZoom)
                        {
                            Texture2D tileTexture = new Texture2D(256, 256, TextureFormat.RGB24, false)
                            {
                                wrapMode = TextureWrapMode.Clamp
                            };
                            tile.www.LoadImageIntoTexture(tileTexture);
                            tile.texture = tileTexture;
                            tile.status = OnlineMapsTileStatus.loaded;
                        }

                        CheckRedrawType();
                    }
                    else tile.OnDownloadError();

                    if (tile.www != null)
                    {
                        tile.www.Dispose();
                        tile.www = null;
                    }
                }
                
                if (tile.status == OnlineMapsTileStatus.loaded && tile.trafficWWW != null && tile.trafficWWW.isDone)
                {
                    if (string.IsNullOrEmpty(tile.trafficWWW.error))
                    {
                        if (target == OnlineMapsTarget.texture)
                        {
                            if (tile.OnLabelDownloadComplete()) buffer.ApplyTile(tile);
                        }
                        else
                        {
                            Texture2D trafficTexture = new Texture2D(256, 256, TextureFormat.RGB24, false)
                            {
                                wrapMode = TextureWrapMode.Clamp
                            };
                            trafficTexture.LoadImage(tile.trafficWWW.bytes);
                            tile.trafficTexture = trafficTexture;
                        }
                        CheckRedrawType();
                    }

                    if (tile.trafficWWW != null)
                    {
                        tile.trafficWWW.Dispose();
                        tile.trafficWWW = null;
                    }
                }
            }

            StartDownloading();
        }
    }

    private void CheckGoogleAPIQuery()
    {
        if (googleQueries != null)
        {
            bool reqDelete = false;
            foreach (OnlineMapsGoogleAPIQuery item in googleQueries)
            {
                item.CheckComplete();
                if (item.status != OnlineMapsQueryStatus.downloading)
                {
                    if (item.type == OnlineMapsQueryType.location && OnFindLocationAfter != null) OnFindLocationAfter();
                    item.Destroy();
                    reqDelete = true;
                }
            }
            if (reqDelete)
            {
                googleQueries.RemoveAll(f => f.status == OnlineMapsQueryStatus.disposed);
            }
        }
    }

    private int CheckMapSize(int z)
    {
        try
        {
            int maxX = (2 << z) / 2 * OnlineMapsUtils.tileSize;
            int maxY = (2 << z) / 2 * OnlineMapsUtils.tileSize;
            int w = (target == OnlineMapsTarget.texture) ? texture.width : tilesetWidth;
            int h = (target == OnlineMapsTarget.texture) ? texture.height : tilesetHeight;
            if (maxX <= w || maxY <= h) return CheckMapSize(z + 1);
        }
        catch{}
        
        return z;
    }

    /// <summary>
    /// Sets the desired type of redrawing the map.
    /// </summary>
    public void CheckRedrawType()
    {
        if (allowRedraw)
        {
            redrawType = OnlineMapsRedrawType.full;
            needRedraw = true;
        }
    }

    /// <summary>
    /// Allows you to test the connection to the Internet.
    /// </summary>
    /// <param name="callback">Function, which will return the availability of the Internet.</param>
    public void CheckServerConnection(Action<bool> callback)
    {
        OnlineMapsTile tempTile = new OnlineMapsTile(350, 819, 11, this, false);
        string url = tempTile.url;
        tempTile.Dispose();

        checkConnectioCallback = callback;

#if UNITY_WEBPLAYER
        checkConnectionWWW = new WWW(OnlineMapsUtils.serviceURL + url);
#else
        checkConnectionWWW = new WWW(url);
#endif
    }

    /// <summary>
    /// Dispatch map events.
    /// </summary>
    /// <param name="evs">Events you want to dispatch.</param>
    /// <remarks>Example:</remarks>
    /// <example>
    /// <code>
    /// public class DispatchEventsExample : MonoBehaviour
    /// {
    ///     private void OnGUI()
    ///     {
    ///         if (GUI.Button(new Rect(5, 5, 100, 20), "Show London"))
    ///         {
    ///             OnlineMaps map = OnlineMaps.instance;
    ///             map.dispathEvents = false;
    ///             map.position = new Vector2(2.131348f, 41.41389f);
    ///             map.zoom = 11;
    ///             map.dispathEvents = true;
    ///             map.DispatchEvent(OnlineMapsEvents.changedPosition, OnlineMapsEvents.changedZoom);
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public void DispatchEvent(params OnlineMapsEvents[] evs)
    {
        if (!dispatchEvents) return;

        foreach (OnlineMapsEvents ev in evs)
        {
            if (ev == OnlineMapsEvents.changedPosition && OnChangePosition != null) OnChangePosition();
            else if (ev == OnlineMapsEvents.changedZoom && OnChangeZoom != null) OnChangeZoom();
        }
    }

    /// <summary>
    /// Find route by coordinates or title.
    /// </summary>
    /// <param name="origin">
    /// Coordinates or the name of the route begins.
    /// </param>
    /// <param name="destination">
    /// Coordinates or the name of the route ends.
    /// </param>
    /// <returns>
    /// Query instance to the Google API.
    /// </returns>
    /// <remarks></remarks>
    /// <example>
    /// Example:
    /// <code>
    /// OnlineMapsFindDirection.Find("Los Angeles", new Vector2(-118.178960f, 35.063995f)).OnComplete += OnFindDirectionComplete;
    ///
    /// private void OnFindDirectionComplete(string response)
    /// {
    ///    List<OnlineMapsDirectionStep> steps = OnlineMapsDirectionStep.TryParse(response);
    ///    if (steps != null)
    ///    {
    ///        foreach (OnlineMapsDirectionStep step in steps)
    ///        {
    ///            Debug.Log(step.instructions);
    ///        }
    ///
    ///        List<Vector2> points = OnlineMapsDirectionStep.GetPoints(steps);
    ///        Debug.Log(points.Count);
    ///    }
    /// }
    /// </code>         
    /// </example>
    public OnlineMapsGoogleAPIQuery FindDirection(string origin, string destination)
    {
        OnlineMapsFindDirection fl = new OnlineMapsFindDirection(origin, destination);
        googleQueries.Add(fl);
        return fl;
    }

    /// <summary>
    /// Search for location coordinates by the specified string and change the current position in the first search results.
    /// </summary>
    /// <param name="search">Address you want to find.</param>
    /// <returns>Instance of the search query.</returns>
    /// <remarks></remarks>
    /// <example>
    /// Example:
    /// <code>
    /// OnlineMaps.instance.FindLocation("Memphis");
    /// </code>
    /// </example>
    public OnlineMapsGoogleAPIQuery FindLocation(string search)
    {
        OnlineMapsFindLocation fl = new OnlineMapsFindLocation(search);
        fl.OnComplete += OnlineMapsFindLocation.MovePositionToResult;
        googleQueries.Add(fl);
        return fl;
    }

    /// <summary>
    /// Search for location coordinates by the specified string and return search result to callback function.
    /// </summary>
    /// <param name="search">Address you want to find.</param>
    /// <param name="callback">Function, which will be given search results as XML string.</param>
    /// <returns>Instance of the search query.</returns>
    /// <remarks>Example:</remarks>
    /// <example>
    /// <code>
    /// OnlineMaps.instance.FindLocation("Chicago", OnLocationFinded);
    /// 
    /// private void OnLocationFinded(string response)
    /// {
    ///     Debug.Log(response);
    /// }
    /// </code>
    /// </example>
    public OnlineMapsFindLocation FindLocation(string search, Action<string> callback)
    {
        OnlineMapsFindLocation fl = new OnlineMapsFindLocation(search);
        fl.OnComplete += callback;
        googleQueries.Add(fl);
        return fl;
    }

    /// <summary>
    /// Unloads unused assets and initializes the garbage collection.
    /// </summary>
    public void GCCollect()
    {
        lastGC = DateTime.Now.Ticks;
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    /// <summary>
    /// Gets 2D marker from screen.
    /// </summary>
    /// <param name="screenPosition">
    /// Screen position.
    /// </param>
    /// <returns>
    /// The 2D marker.
    /// </returns>
    public OnlineMapsMarker GetMarkerFromScreen(Vector2 screenPosition)
    {
        return markers.FirstOrDefault(marker => marker.HitTest(screenPosition, zoom));
    }

// ReSharper disable once UnusedMember.Local
    

    private void LateUpdate()
    {
        if (control == null) return;
        if (buffer.status == OnlineMapsBufferStatus.complete)
        {
            if (allowRedraw)
            {
                if (target == OnlineMapsTarget.texture)
                {
                    if (!useSmartTexture || !buffer.generateSmartBuffer)
                    {
                        texture.SetPixels(buffer.frontBuffer);
                        texture.Apply(false);
                        if (control.activeTexture != texture) control.SetTexture(texture);
                    }
                    else
                    {
                        smartTexture.SetPixels(buffer.smartBuffer);
                        smartTexture.Apply(false);
                        if (control.activeTexture != smartTexture) control.SetTexture(smartTexture);

                        if (!isUserControl) needRedraw = true;
                    }
                }
                
                if (control is OnlineMapsControlBase3D) ((OnlineMapsControlBase3D)control).UpdateControl();
            }
            buffer.status = OnlineMapsBufferStatus.wait;
        }
        if (allowRedraw && needRedraw)
        {
            if (buffer.status == OnlineMapsBufferStatus.wait)
            {
                position = position.FixAngle();
                buffer.redrawType = redrawType;
                buffer.generateSmartBuffer = isUserControl; 
                buffer.status = OnlineMapsBufferStatus.start;
                if (renderThread == null)
                {
                    renderThread = new Thread(buffer.GenerateFrontBuffer);
                    renderThread.Start();
                }
                redrawType = OnlineMapsRedrawType.none;
                needRedraw = false;
            }
        }
    }

// ReSharper disable once UnusedMember.Local
    private void OnDestroy()
    {
        buffer.Dispose();
        _buffer = null;
        if (defaultColors != null && texture != null)
        {
            texture.SetPixels(defaultColors);
            texture.Apply();
        }
        if (renderThread != null) renderThread.Abort();
        renderThread = null;
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

// ReSharper disable once UnusedMember.Local
    private void OnDisable ()
    {
        if (_instance == this) _instance = null;
        if (renderThread != null) renderThread.Abort();
        renderThread = null;
    }

    private void OnEnable()
    {
        _instance = this;

        if (language == "") language = provider != OnlineMapsProviderEnum.nokia ? "en" : "eng";
        if (drawingElements == null) drawingElements = new List<OnlineMapsDrawingElement>();
        _provider = provider;
        _type = type;
        _language = language;
        _position = position;
        _zoom = zoom;
    }

// ReSharper disable once UnusedMember.Local
    private void OnGUI()
    {
        if (skin != null) GUI.skin = skin;

        if (!string.IsNullOrEmpty(tooltip) || showMarkerTooltip == OnlineMapsShowMarkerTooltip.always)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
			
            if (OnPrepareTooltipStyle != null) OnPrepareTooltipStyle(ref style);

            if (!string.IsNullOrEmpty(tooltip))
            {
                if (tooltipMarker.OnDrawTooltip != null) tooltipMarker.OnDrawTooltip(tooltipMarker);
                else if (OnlineMapsMarkerBase.OnMarkerDrawTooltip != null) OnlineMapsMarkerBase.OnMarkerDrawTooltip(tooltipMarker);
                else OnGUITooltip(style, tooltip, Input.mousePosition);
            }

            if (showMarkerTooltip == OnlineMapsShowMarkerTooltip.always)
            {
                foreach (OnlineMapsMarker marker in markers)
                {
                    Rect rect = marker.screenRect;
                    if (rect.xMax > 0 && rect.xMin < Screen.width && rect.yMax > 0 && rect.yMin < Screen.height)
                    {
                        if (marker.OnDrawTooltip != null) marker.OnDrawTooltip(marker);
                        else if (OnlineMapsMarkerBase.OnMarkerDrawTooltip != null) OnlineMapsMarkerBase.OnMarkerDrawTooltip(marker);
                        else OnGUITooltip(style, marker.label, new Vector2(rect.x + rect.width / 2, rect.y + rect.height));
                    }
                }
            }
        }

    }

    private void OnGUITooltip(GUIStyle style, string text, Vector2 position)
    {
        GUIContent tip = new GUIContent(text);
        Vector2 size = style.CalcSize(tip);
        GUI.Label(new Rect(position.x - size.x / 2, Screen.height - position.y - size.y - 20, size.x + 10, size.y + 5), text, style);
    }

    /// <summary>
    /// Full redraw map.
    /// </summary>
    public void Redraw()
    {
        needRedraw = true;
        allowRedraw = true;
        redrawType = OnlineMapsRedrawType.full;
        buffer.updateBackBuffer = true;
    }

    /// <summary>
    /// Remove all 2D markers from map.
    /// </summary>
    public void RemoveAllMarkers()
    {
        markers = new OnlineMapsMarker[0];
        Redraw();
    }

    /// <summary>
    /// Remove the specified 2D marker from the map.
    /// </summary>
    /// <param name="marker">The marker you want to remove.</param>
    public void RemoveMarker(OnlineMapsMarker marker)
    {
        List<OnlineMapsMarker> ms = markers.ToList();
        ms.Remove(marker);
        markers = ms.ToArray();
        Redraw();
    }

    /// <summary>
    /// Saves the current state of map.
    /// </summary>
    public void Save()
    {
        if (target == OnlineMapsTarget.texture) defaultColors = texture.GetPixels();
    }

    /// <summary>
    /// Sets the position and zoom.
    /// </summary>
    /// <param name="X">Longitude</param>
    /// <param name="Y">Latitude</param>
    /// <param name="ZOOM">Zoom</param>
    public void SetPositionAndZoom(float X, float Y, int ZOOM = 0)
    {
        position = new Vector2(X, Y);
        if (ZOOM != 0) zoom = ZOOM;
    }

    /// <summary>
    /// Sets the texture, which will draw the map.
    /// Texture displaying on the source you need to change yourself.
    /// </summary>
    /// <param name="newTexture">Texture, where you want to draw the map.</param>
    /// <remarks>Example:</remarks>
    /// <example>
    /// <code>
    /// //Create a new texture
    /// Texture2D newTexture = new Texture2D(512, 256, TextureFormat.RGB24, false);
    /// 
    /// //Set a new texture on GUITexture and centered
    /// guiTexture.texture = newTexture;
    /// guiTexture.pixelInset = new Rect(newTexture.width / -2, newTexture.height / -2, newTexture.width, newTexture.height);
    /// 
    /// //Indicate that need draw a map on a new texture.
    /// OnlineMaps.instance.SetTexture(newTexture);
    /// </code>
    /// </example>
    public void SetTexture(Texture2D newTexture)
    {
        texture = newTexture;
        width = newTexture.width;
        height = newTexture.height;
        allowRedraw = true;
        needRedraw = true;
        redrawType = OnlineMapsRedrawType.full;
    }

    /// <summary>
    /// Checks if the marker in the specified screen coordinates, and shows him a tooltip.
    /// </summary>
    /// <param name="screenPosition">Screen coordinates</param>
    public void ShowMarkersTooltip(Vector2 screenPosition)
    {
        tooltip = string.Empty;
        tooltipMarker = null;

        OnlineMapsMarker marker = GetMarkerFromScreen(screenPosition);

        if (showMarkerTooltip == OnlineMapsShowMarkerTooltip.onHover && marker != null)
        {
            tooltip = marker.label;
            tooltipMarker = marker;
        }

        if (rolledMarker != marker)
        {
            if (rolledMarker != null && rolledMarker.OnRollOut != null) rolledMarker.OnRollOut(rolledMarker);
            rolledMarker = marker;
            if (rolledMarker != null && rolledMarker.OnRollOver != null) rolledMarker.OnRollOver(rolledMarker);
        }
    }

// ReSharper disable once UnusedMember.Local
    private void Start()
    {
        if (redrawOnPlay)
        {
            allowRedraw = true;
            redrawType = OnlineMapsRedrawType.full;
        }
        needRedraw = true;
        _zoom = CheckMapSize(_zoom);
    }

    private void StartDownloading()
    {
        lock (OnlineMapsTile.tiles)
        {
            IOrderedEnumerable<OnlineMapsTile> tiles = OnlineMapsTile.tiles.Where(t => t.status == OnlineMapsTileStatus.none).OrderBy(t => t.priority);

            long startTick = DateTime.Now.Ticks;

            int countDownload = 0;
#if UNITY_IOS && !UNITY_EDITOR
            int maxDownloads = 3;
#else
            int maxDownloads = 3;
#endif
            foreach (OnlineMapsTile tile in tiles)
            {
                if (DateTime.Now.Ticks - startTick > 100000) return;

                countDownload++;
                if (countDownload > maxDownloads) return;

                if (OnStartDownloadTile != null) OnStartDownloadTile(tile);
                else StartDownloadTile(tile);
            }
        }
    }

    /// <summary>
    /// Starts dowloading of specified tile.
    /// </summary>
    /// <param name="tile">Tile to be downloaded.</param>
    public void StartDownloadTile(OnlineMapsTile tile)
    {
        if (source != OnlineMapsSource.Online)
        {
            UnityEngine.Object tileTexture = Resources.Load(tile.resourcesPath);
            if (tileTexture != null)
            {
                if (target == OnlineMapsTarget.texture)
                {
                    tile.ApplyTexture(tileTexture as Texture2D);
                    buffer.ApplyTile(tile);
                }
                else
                {
                    tile.texture = tileTexture as Texture2D;
                    tile.status = OnlineMapsTileStatus.loaded;
                }
                CheckRedrawType();
                return;
            }

            if (source == OnlineMapsSource.Resources)
            {
                tile.status = OnlineMapsTileStatus.error;
                return;
            }
        }

#if UNITY_WEBPLAYER
        tile.www = new WWW(OnlineMapsUtils.serviceURL + tile.url);
        if (traffic && !string.IsNullOrEmpty(tile.trafficURL)) tile.trafficWWW = new WWW(OnlineMapsUtils.serviceURL + tile.trafficURL);
#else
        tile.www = new WWW(tile.url);
        if (traffic && !string.IsNullOrEmpty(tile.trafficURL)) tile.trafficWWW = new WWW(tile.trafficURL);
#endif
        tile.status = OnlineMapsTileStatus.loading;
    }

// ReSharper disable once UnusedMember.Local
    private void Update()
    {
        CheckBaseProps();
        CheckGoogleAPIQuery();
        CheckDownloadComplete();

        if (DateTime.Now.Ticks - lastGC > OnlineMapsUtils.second * 5) GCCollect();
    }

    
}