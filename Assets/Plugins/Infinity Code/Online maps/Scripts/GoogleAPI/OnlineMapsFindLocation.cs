/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System.Xml;
using UnityEngine;

/// <summary>
/// This class is used to search for a location by address.\n
/// You can create a new instance using OnlineMaps.FindLocation.\n
/// </summary>
[System.Serializable]
public class OnlineMapsFindLocation : OnlineMapsGoogleAPIQuery
{
    /// <summary>
    /// Gets the type of query to Google API.
    /// </summary>
    /// <value>
    /// OnlineMapsQueryType.location
    /// </value>
    public override OnlineMapsQueryType type
    {
        get { return OnlineMapsQueryType.location; }
    }

    /// <summary>
    /// Constructor. \n
    /// <strong>Please do not use. </strong>\n
    /// To find location use OnlineMaps.FindLocation or OnlineMapsFindLocation.Find.
    /// </summary>
    /// <param name="address">Location title</param>
    /// <param name="latlng">Location coordinates</param>
    public OnlineMapsFindLocation(string address = null, string latlng = null)
    {
        _status = OnlineMapsQueryStatus.downloading;
        string url = "http://maps.google.com/maps/api/geocode/xml?sensor=false";
        if (!string.IsNullOrEmpty(address)) url += "&address=" + address.Replace(" ", "+");
        if (!string.IsNullOrEmpty(latlng)) url += "&latlng=" + latlng.Replace(" ", "");
#if UNITY_WEBPLAYER
        www = new WWW("http://service.infinity-code.com/redirect.php?" + url);
#else
        www = new WWW(url);
#endif
    }

    /// <summary>
    /// Creates a new request for a location search.
    /// </summary>
    /// <param name="address">Location title</param>
    /// <param name="latlng">Location coordinates</param>
    /// <returns>Instance of the search query.</returns>
    public static OnlineMapsGoogleAPIQuery Find(string address = null, string latlng = null)
    {
        OnlineMapsFindLocation query = new OnlineMapsFindLocation(address, latlng);
        OnlineMaps.instance.AddGoogleAPIQuery(query);
        return query;
    }

    /// <summary>
    /// Gets the coordinates of the first results from OnlineMapsFindLocation result.
    /// </summary>
    /// <param name="result">Coordinates - if successful, Vector2.zero - if failed.</param>
    /// <returns>Vector2 coordinates</returns>
    public static Vector2 GetCoordinatesFromResult(string result)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);

            XmlNode location = doc.SelectSingleNode("//geometry/location");
            if (location == null) return Vector2.zero;

            return GetVector2FromNode(location);
        }
        catch { }
        return Vector2.zero;
    }

    /// <summary>
    /// Centers the map on the result of the search location.
    /// </summary>
    /// <param name="result">XML string. The result of the search location.</param>
    public static void MovePositionToResult(string result)
    {
        Vector2 position = GetCoordinatesFromResult(result);

        if (position != Vector2.zero)
        {
            OnlineMaps.instance.position = position;
            OnlineMaps.instance.Redraw();
        }
    }
}