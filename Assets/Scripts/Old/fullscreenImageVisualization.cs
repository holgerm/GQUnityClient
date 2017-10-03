using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class fullscreenImageVisualization : MonoBehaviour
{



	public Texture2D texture;

	public RawImage image;


	// Use this for initialization
	void Start()
	{
	
		image.texture = texture;


		float myX = (float)image.texture.width;
		float myY = (float)image.texture.height;
		float scaler = myX / 1080f;
		myY = myY / scaler;
		myX = 1080f;

		image.GetComponent<RectTransform>().sizeDelta = new Vector2(myX, myY);



	}


	void Update()
	{



		if (Input.touchCount == 1)
		{
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

			image.transform.position = new Vector3(image.transform.position.x + touchDeltaPosition.x,
				image.transform.position.y + touchDeltaPosition.y,
				image.transform.position.z);


		} 


	}

	public void close()
	{


		Destroy(gameObject);

	}
	

}
