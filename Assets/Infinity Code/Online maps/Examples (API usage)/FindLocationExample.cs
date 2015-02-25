/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

public class FindLocationExample : MonoBehaviour
{
    private void Start()
    {
        // Start search Chicago.
        OnlineMapsGoogleAPIQuery query = OnlineMapsFindLocation.Find("Chicago");

        // Specifies that search results should be sent to OnFindLocationComplete.
        query.OnComplete += OnFindLocationComplete;
    }

    private void OnFindLocationComplete(string result)
    {
        // Get the coordinates of the first found location.
        Vector2 position = OnlineMapsFindLocation.GetCoordinatesFromResult(result);

        if (position != Vector2.zero)
        {
            // Create a new marker at the position of Chicago.
            OnlineMaps.instance.AddMarker(position, "Chicago");
        }
    }
}