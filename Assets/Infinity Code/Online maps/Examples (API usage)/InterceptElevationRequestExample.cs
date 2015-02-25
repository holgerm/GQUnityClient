/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

public class InterceptElevationRequestExample : MonoBehaviour 
{
    private OnlineMapsTileSetControl control;

    void Start ()
    {
        // Get Tileset control.
        control = (OnlineMapsTileSetControl) OnlineMapsControlBase.instance;

        // Intercept elevation request
        control.OnGetElevation += OnGetElevation;
    }

    private void OnGetElevation(Vector2 topLeftCoords, Vector2 bottomRightCoords)
    {
        // Elevation map must be 32x32
        short[,] elevation = new short[32, 32];

        // Here you get the elevation from own sources.

        // Set elevation map
        control.SetElevationData(elevation);
    }
}
