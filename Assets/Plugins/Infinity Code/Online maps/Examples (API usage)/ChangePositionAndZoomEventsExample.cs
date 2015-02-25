/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

public class ChangePositionAndZoomEventsExample : MonoBehaviour
{
    private void OnChangePosition()
    {
        // When the position changes you will see in the console new map coordinates.
        Debug.Log(OnlineMaps.instance.position);
    }

    private void OnChangeZoom()
    {
        // When the zoom changes you will see in the console new zoom.
        Debug.Log(OnlineMaps.instance.zoom);
    }

    private void OnEnable()
    {
        // Subscribe to change position event.
        OnlineMaps.instance.OnChangePosition += OnChangePosition;

        // Subscribe to change zoom event.
        OnlineMaps.instance.OnChangeZoom += OnChangeZoom;
    }
}