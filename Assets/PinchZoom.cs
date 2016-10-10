using UnityEngine;
using UnityEngine.UI;

public class PinchZoom : MonoBehaviour
{
	public float zoomSpeed = 0.05f;



	void Update()
	{
		// If there are two touches on the device...
		if (Input.touchCount == 2)
		{
			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;



			float scale = GetComponent<RectTransform>().localScale.x;

			// ... change the scale based on the change in distance between the touches.
			scale += deltaMagnitudeDiff * zoomSpeed;

			// Make sure the scale never drops below zero.
			scale = Mathf.Max(scale, 1f);

			// Make sure the scale never exceeds 10
			scale = Mathf.Min(scale, 10f);


			GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);

			
		}
	}
}