using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class FlexibleWeightCorrection : MonoBehaviour
{

	public float myWeight = 4f;
	public float weightOfRest = 4f;

	public float standardScreenAspectRatio = 1.2f;
	public LayoutElement layoutElement;


	// Use this for initialization
	void Start ()
	{
//		holgersVersion ();

		kevinsVersion ();
	
	}

	void kevinsVersion ()
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
		
		float parentWidth = Camera.main.pixelWidth; 
		float parentHeighth = Camera.main.pixelHeight;

		float imageWidth = 1000f;
		float imageHeight = 100f; // menu bar hidden
		
		foreach (Image img in GetComponentsInChildren<Image>()) {
			if (img.sprite != null) {
				imageWidth = img.sprite.texture.width;
				imageHeight = img.sprite.texture.height;
			}
		}
		
		float thisHeightShare = 
			//			(parentRect.width / parentRect.height) * 
			(parentWidth / parentHeighth) * 
			(Mathf.Min ((1f / standardScreenAspectRatio), imageHeight / imageWidth));
		
		float myWeight = thisHeightShare * weightOfRest / (1 - thisHeightShare);
		
		GetComponent<LayoutElement> ().flexibleHeight = myWeight;
		
	}

}
