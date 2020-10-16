using TMPro;
using UnityEngine;

public class MapScaleAdaption : MonoBehaviour
{
    public OnlineMapsCameraOrbit cameraOrbit;
    public OnlineMapsMarkerManager markerManager;

    public OnlineMaps map;
    public TextMeshProUGUI scaleText;
    
    public float scaleDiff = 0.05f;
    public float scaleDisplay = 0.625f;

    private Camera mainCam;
    public void Start()
    {
        mainCam = Camera.main;
        float height = scaleDisplay * Screen.height;
        // float width =  scaleDisplay * Screen.width;
        mainCam.orthographicSize = height / 2f;
        Debug.Log($"DPI: {Screen.dpi}");
        // OnlineMapsTileSetControl tileset = map.GetComponent<OnlineMapsTileSetControl>();
        // tileset.sizeInScene = new Vector2(width, height);
    }
    
    public void ScaleUp()
    {
        float mscale = markerManager.defaultScale;
        cameraOrbit.distance /= (1f + scaleDiff);
        foreach (var marker in markerManager.items)
        {
            marker.scale  /= (1f + scaleDiff);
            mscale = marker.scale;
        }

        scaleText.text = $"Map scale: {cameraOrbit.distance} : {mscale}";
        map.Redraw();
    }
    
    public void ScaleDown()
    {
        float mscale = markerManager.defaultScale;
        cameraOrbit.distance *= (1f + scaleDiff);
        foreach (var marker in markerManager.items)
        {
            marker.scale  *= (1f + scaleDiff);
            mscale = marker.scale;
        }
        
        scaleText.text = $"Map scale: {cameraOrbit.distance} : {mscale}";
        map.Redraw();
    }

}
