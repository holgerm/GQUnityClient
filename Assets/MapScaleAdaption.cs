using TMPro;
using UnityEngine;

public class MapScaleAdaption : MonoBehaviour
{
    public OnlineMapsCameraOrbit cameraOrbit;
    public OnlineMapsMarkerManager markerManager;

    public OnlineMaps map;
    public TextMeshProUGUI scaleText;
    
    public float scaleDiff = 0.05f;
    
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

    public void ZoomIn()
    {
        map.floatZoom *= 1.03f;
        map.Redraw();
    }

    public void ZoomOut()
    {
        map.floatZoom /= 1.03f;
        map.Redraw();
    }
}
