using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class FlexibleWeightCorrection : MonoBehaviour
{

	public float myWeight = 4f;
	public float weightOfRest = 4f;

	public float standardScreenAspectRatio = 4f / 3f;
	public LayoutElement layoutElement;


	// Use this for initialization
	void Start ()
	{
		float parentWidth = Camera.main.pixelWidth; 
		float parentHeighth = Camera.main.pixelHeight;

		float weightSum = myWeight + weightOfRest;

		float newHeight = parentWidth * standardScreenAspectRatio * (myWeight / weightSum);
		float newWeight = newHeight * (weightOfRest / (parentHeighth - newHeight));

		GetComponent<LayoutElement> ().flexibleHeight = newWeight;
	
	}

	void holgersVersion ()
	{
		Rect rect = GetComponent<RectTransform> ().rect;
		
		Rect parentRect = gameObject.transform.parent.GetComponent<RectTransform> ().rect;
		
		Debug.Log ("parentRect.width : parentRect.height  " + parentRect.width + " : " + parentRect.height);
		
		float imageWidth = 1000f;
		float imageHeight = 100f; // menu bar hidden
		
		foreach (Image img in GetComponentsInChildren<Image>()) {
			if (img.sprite != null) {
				imageWidth = img.sprite.texture.width;
				imageHeight = img.sprite.texture.height;
			}
		}
		
		
		Debug.Log ("imageWidth : imageHeight  " + imageWidth + " : " + imageHeight);
		
		float thisHeightShare = 
			//			(parentRect.width / parentRect.height) * 
			(1080f / 1920f) * 
			(Mathf.Min ((2f / 3f), imageHeight / imageWidth));
		
		float myWeight = thisHeightShare * weightOfRest / (1 - thisHeightShare);
		
		GetComponent<LayoutElement> ().flexibleHeight = myWeight;
		
	}

}
