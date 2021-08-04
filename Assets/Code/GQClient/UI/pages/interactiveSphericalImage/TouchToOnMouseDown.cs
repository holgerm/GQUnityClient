using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchToOnMouseDown : MonoBehaviour
{
    public Camera cam;
    
    // Update is called once per frame
    void Update()
    {
        // Code for OnMouseDown
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase.Equals(TouchPhase.Began))
            {
                // Construct a ray from the current touch coordinates
                Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    hit.transform.gameObject.SendMessage("OnMouseDown");
                }
            }
        }
       
    }
}
