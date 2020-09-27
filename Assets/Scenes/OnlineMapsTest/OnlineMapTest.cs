using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineMapTest : MonoBehaviour
{

    public OnlineMaps map;

    public void ZoomIn()
    {
        if (map == null) return;

        map.zoom++;
    }
 
    public void ZoomOut()
    {
        if (map == null) return;

        map.zoom--;
    }
    
    public void North()
    {
        if (map == null) return;

        var curPos = map.position;
        map.position = new Vector2(curPos.x, curPos.y + 0.1f);
    }

    public void South()
    {
        if (map == null) return;

        var curPos = map.position;
        map.position = new Vector2(curPos.x + 0.1f, curPos.y);
    }

 }
