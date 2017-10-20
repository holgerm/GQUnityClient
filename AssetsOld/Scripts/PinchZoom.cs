using UnityEngine;
using UnityEngine.UI;

public class PinchZoom : MonoBehaviour
{
	public float zoomSpeed = 0.05f;


	public float doubleclickTimer = 0.5f;

	public float deltaMoveForDoubleClick = 0.1f;

	float doubleclickTimerSave = 0.5f;

	bool resetOnNextClick = false;


	float deltaMoved = 0f;

	void Start()
	{
		doubleclickTimerSave = doubleclickTimer;

	}


	void resetPositionAndScale()
	{

		GetComponent<RectTransform>().localScale = Vector3.one;
		GetComponent<RectTransform>().localPosition = Vector3.zero;


	}

	void Update()
	{

		if (doubleclickTimer >= 0)
		{
			doubleclickTimer -= Time.deltaTime;

		} else
		{

			resetOnNextClick = false;

		}


		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began && !resetOnNextClick)
		{
			deltaMoved = 0f;

		} else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
		{

			deltaMoved += (Mathf.Abs(Input.GetTouch(0).deltaPosition.x) + Mathf.Abs(Input.GetTouch(0).deltaPosition.y));

		} else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && deltaMoved < deltaMoveForDoubleClick)
		{

			if (resetOnNextClick && doubleclickTimer > 0f && deltaMoved < deltaMoveForDoubleClick)
			{
				resetPositionAndScale();

			} else
			{
				doubleclickTimer = doubleclickTimerSave;
				resetOnNextClick = true;

			}



		} else if (Input.touchCount == 2)
		{
			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);



			deltaMoved += Mathf.Abs(touchZero.deltaPosition.x) + Mathf.Abs(touchZero.deltaPosition.y);
			deltaMoved += Mathf.Abs(touchOne.deltaPosition.x) + Mathf.Abs(touchOne.deltaPosition.y);


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