/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

public class Marker_GPS_Example : MonoBehaviour
{
    // Marker, which should display the location.
    private OnlineMapsMarker playerMarker;

	void Start ()
	{
        // Create a new marker.
        playerMarker = OnlineMaps.instance.AddMarker(new Vector2(0, 0), null, "Player");

        // Get instance of LocationService.
	    OnlineMapsLocationService locationService = GetComponent<OnlineMapsLocationService>();

        // Subscribe to the change location event.
        locationService.OnLocationChanged += OnLocationChanged;
	}

    // When the location has changed
    private void OnLocationChanged(Vector2 position)
    {
        // Change the position of the marker.
        playerMarker.position = position;

        // Redraw map.
        OnlineMaps.instance.Redraw();
    }
}
