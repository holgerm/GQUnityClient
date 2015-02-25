using UnityEngine;

public class MoveMarkersExample : MonoBehaviour
{
    // Move time
    private float time = 10;

    private OnlineMapsMarker marker;

    private Vector2 fromPosition;
    private Vector2 toPosition;

    // Relative position (0-1) between from and to
    private float angle = 0.5f;

    // Move direction
    private int direction = 1;

    void Start ()
	{
        OnlineMaps api = OnlineMaps.instance;
        marker = api.AddMarker(api.position);
	    fromPosition = api.topLeftPosition;
	    toPosition = api.bottomRightPosition;
	}

	void Update ()
	{
	    angle += Time.deltaTime / time * direction;
	    if (angle > 1)
	    {
	        angle = 2 - angle;
	        direction = -1;
	    }
        else if (angle < 0)
        {
            angle *= -1;
            direction = 1;
        }

        // Set new position
	    marker.position = Vector2.Lerp(fromPosition, toPosition, angle);

        // Marks the map should be redrawn.
        // Map is not redrawn immediately. It will take some time.
        OnlineMaps.instance.Redraw();
	}
}
