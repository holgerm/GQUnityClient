using UnityEngine;
using System.Collections;

public class fitscreen : MonoBehaviour {

	void Start () {
		

		float height = 2.0f * Mathf.Tan(0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad);
		if (height > 1f) {
						height = 1f;
				}
		float width = height * Screen.width / Screen.height;
		transform.localScale = new Vector3(height, 0.1f,width);
	}
}
