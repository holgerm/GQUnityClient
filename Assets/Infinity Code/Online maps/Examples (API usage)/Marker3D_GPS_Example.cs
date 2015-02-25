/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

public class Marker3D_GPS_Example : MonoBehaviour
{
    public GameObject prefab;

    private OnlineMapsMarker3D locationMarker;

    private void Start()
    {
        //Create a marker to show the current GPS coordinates.
        //Instead of "null", you can specify the texture desired marker.
        locationMarker = GetComponent<OnlineMapsTextureControl>().AddMarker3D(Vector2.zero, prefab);

        //Hide handle until the coordinates are not received.
        locationMarker.enabled = false;

        //Subscribe to the GPS coordinates change
        OnlineMapsLocationService ls = GetComponent<OnlineMapsLocationService>();
        ls.OnLocationChanged += OnLocationChanged;
        ls.OnCompassChanged += OnCompassChanged;

        //Subscribe to zoom change
        OnlineMaps.instance.OnChangeZoom += OnChangeZoom;
    }

    private void OnChangeZoom()
    {
        //Example of scaling object
        int zoom = OnlineMaps.instance.zoom;

        if (zoom >= 5 && zoom < 10)
        {
            float s = 10f / (2 << (zoom - 5));
            locationMarker.transform.localScale = new Vector3(s, s, s);

            // show marker
            locationMarker.enabled = true;
        }
        else
        {
            // Hide marker
            locationMarker.enabled = false;
        }
    }

    private void OnCompassChanged(float f)
    {
        //Set marker rotation
        locationMarker.transform.rotation = Quaternion.Euler(0, f * 360, 0);
    }

    //This event occurs at each change of GPS coordinates
    private void OnLocationChanged(Vector2 position)
    {
        //Change the position of the marker to GPS coordinates
        locationMarker.position = position;

        //If the marker is hidden, show it
        if (!locationMarker.enabled) locationMarker.enabled = true;
    }
}