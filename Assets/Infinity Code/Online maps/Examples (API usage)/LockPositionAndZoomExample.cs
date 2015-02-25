/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

public class LockPositionAndZoomExample : MonoBehaviour
{
	void Start () 
    {
        // Lock map zoom range
	    OnlineMaps.instance.zoomRange = new OnlineMapsRange(10, 15);

        // Lock map coordinates range
        OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(33, -119, 34, -118);
	}
}
