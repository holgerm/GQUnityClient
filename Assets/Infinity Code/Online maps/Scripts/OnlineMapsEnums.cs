/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

/// <summary>
/// Alignment of marker.
/// </summary>
public enum OnlineMapsAlign
{
    TopLeft,
    Top,
    TopRight,
    Left,
    Center,
    Right,
    BottomLeft,
    Bottom,
    BottomRight
}

/// <summary>
/// Buffer state
/// </summary>
public enum OnlineMapsBufferStatus
{
    wait,
    working,
    complete,
    start,
    disposed
}

/// <summary>
/// OnlineMaps events.
/// </summary>
public enum OnlineMapsEvents
{
    changedPosition,
    changedZoom
}

public enum OnlineMapsMarker2DMode
{
    flat,
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
    billboard
#endif
}

/// <summary>
/// OnlineMaps provider.
/// </summary>
public enum OnlineMapsProviderEnum
{
    arcGis,
    google,
    nokia,
    mapQuest,
    virtualEarth,
    openStreetMap,
    custom = 999
}

/// <summary>
/// Status of the request to the Google Maps API.
/// </summary>
public enum OnlineMapsQueryStatus
{
    downloading,
    success,
    error,
    disposed
}

/// <summary>
/// Type of the request to the Google Maps API.
/// </summary>
public enum OnlineMapsQueryType
{
    none,
    location,
    direction,
    osm
}

/// <summary>
/// Map redraw type.
/// </summary>
public enum OnlineMapsRedrawType
{
    full,
    area,
    move,
    none
}

/// <summary>
/// Where will draw the map.
/// </summary>
public enum OnlineMapsTarget
{
    texture,
    tileset
}

/// <summary>
/// When need to show marker tooltip.
/// </summary>
public enum OnlineMapsShowMarkerTooltip
{
    onHover,
    always,
    none
}

/// <summary>
/// Source of map tiles.
/// </summary>
public enum OnlineMapsSource
{
    Online,
    Resources,
    ResourcesAndOnline
}

/// <summary>
/// Tile state
/// </summary>
public enum OnlineMapsTileStatus
{
    none,
    loading,
    loaded,
    loadedFromCache,
    error,
    disposed
}