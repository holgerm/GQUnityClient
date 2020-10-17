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
        mainCam.orthographicSize = height / 2f;
    }
}
