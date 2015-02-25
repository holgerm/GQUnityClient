/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

/// <summary>
/// This class is used to search for a route by address or coordinates.\n
/// You can create a new instance using OnlineMapsFindDirection.Find.\n
/// </summary>
[System.Serializable]
public class OnlineMapsFindDirection : OnlineMapsGoogleAPIQuery
{
    /// <summary>
    /// Gets the type of query to Google API.
    /// </summary>
    /// <value>
    /// OnlineMapsQueryType.direction
    /// </value>
    public override OnlineMapsQueryType type
    {
        get { return OnlineMapsQueryType.direction; }
    }

    /// <summary>
    /// Constructor. \n
    /// <strong>Please do not use. </strong>\n
    /// To find route use OnlineMaps.FindDirection or OnlineMapsFindDirection.Find.
    /// </summary>
    /// <param name="origin">Title of the route begins.</param>
    /// <param name="destination">Title of the route ends.</param>
    public OnlineMapsFindDirection(string origin, string destination)
    {
        _status = OnlineMapsQueryStatus.downloading;
        string url = string.Format("http://maps.google.com/maps/api/directions/xml?origin={0}&destination={1}&sensor=false", origin.Replace(" ", "+"), destination.Replace(" ", "+"));
#if UNITY_WEBPLAYER
        www = new WWW("http://service.infinity-code.com/redirect.php?" + url);
#else
        www = new WWW(url);
#endif
    }

    /// <summary>
    /// Creates a new request for a route search.
    /// </summary>
    /// <param name="origin">
    /// Title of the route begins.
    /// </param>
    /// <param name="destination">
    /// Title of the route ends.
    /// </param>
    /// <returns>
    /// Query instance to the Google API.
    /// </returns>
    public static OnlineMapsGoogleAPIQuery Find(string origin, string destination)
    {
        return OnlineMaps.instance.FindDirection(origin, destination);
    }

    /// <summary>
    /// Creates a new request for a route search.
    /// </summary>
    /// <param name="origin">
    /// Title of the route begins.
    /// </param>
    /// <param name="destination">
    /// Coordinates of the route ends.
    /// </param>
    /// <returns>
    /// Query instance to the Google API.
    /// </returns>
    public static OnlineMapsGoogleAPIQuery Find(string origin, Vector2 destination)
    {
        return Find(origin, destination.y + "," + destination.x);
    }

    /// <summary>
    /// Creates a new request for a route search.
    /// </summary>
    /// <param name="origin">
    /// Coordinates of the route begins.
    /// </param>
    /// <param name="destination">
    /// Title of the route ends.
    /// </param>
    /// <returns>
    /// Query instance to the Google API.
    /// </returns>
    public static OnlineMapsGoogleAPIQuery Find(Vector2 origin, string destination)
    {
        return Find(origin.y + "," + origin.x, destination);
    }

    /// <summary>
    /// Creates a new request for a route search.
    /// </summary>
    /// <param name="origin">
    /// Coordinates of the route begins.
    /// </param>
    /// <param name="destination">
    /// Coordinates of the route ends.
    /// </param>
    /// <returns>
    /// Query instance to the Google API.
    /// </returns>
    public static OnlineMapsGoogleAPIQuery Find(Vector2 origin, Vector2 destination)
    {
        return Find(origin.y + "," + origin.x, destination.y + "," + destination.x);
    }
}