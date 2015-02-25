/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

public class DragMarkerByLongPressExample : MonoBehaviour 
{
	void Start ()
	{
        // Create a new marker.
	    OnlineMapsMarker marker = OnlineMaps.instance.AddMarker(Vector2.zero, "My Marker");

        // Subscribe to OnLongPress event.
        marker.OnLongPress += OnMarkerLongPress;
	}

    private void OnMarkerLongPress(OnlineMapsMarkerBase marker)
    {
        // Starts moving the marker.
        OnlineMapsControlBase.instance.dragMarker = (OnlineMapsMarker)marker;
        OnlineMapsControlBase.instance.isMapDrag = false;
    }
}
