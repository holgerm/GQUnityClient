using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class fullscreenImageVisualization : MonoBehaviour
{



	public Texture2D texture;

	public RawImage image;


	// Use this for initialization
	void Start ()
	{
	
		image.texture = texture;


		float myX = (float)image.texture.width;
		float myY = (float)image.texture.height;
		float scaler = myX / 1080f;
		myY = myY / scaler;
		myX = 1080f;

		image.GetComponent<RectTransform> ().sizeDelta = new Vector2 (myX, myY);



	}


	void Update ()
	{



		if (Input.touchCount == 1) {
			Vector2 touchDeltaPosition = Input.GetTouch (0).deltaPosition;

			image.transform.position = new Vector3 (image.transform.position.x + touchDeltaPosition.x,
				image.transform.position.y + touchDeltaPosition.y,
				image.transform.position.z);


		} else if (Input.touchCount == 2) {
			// Store both touches.
			Touch touchZero = Input.GetTouch (0);
			Touch touchOne = Input.GetTouch (1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

		
			// ... change the orthographic size based on the change in distance between the touches.
			image.transform.localScale = new Vector3 (image.transform.localScale.x + deltaMagnitudeDiff,
				image.transform.localScale.y + deltaMagnitudeDiff,
				image.transform.localScale.z + deltaMagnitudeDiff);

			// Make sure the localScale never drops below one.
			if (image.transform.localScale.x < 1f) {
				image.transform.localScale = Vector3.one;
			}

		}


	}

	public void close ()
	{


		Destroy (gameObject);

	}
	

}
