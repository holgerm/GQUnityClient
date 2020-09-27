using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OnlineMapsMarkerManager))]
public class MarkerSetup : MonoBehaviour
{
    private OnlineMapsMarkerManager markerManager;
    public Texture2D markerTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        if (markerManager == null)
        {
            markerManager = GetComponent<OnlineMapsMarkerManager>();
        }

        markerManager.Create(8, 50d, markerTexture, "test 1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
