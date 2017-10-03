using UnityEngine;
using System.Collections;

public class scrollAnimation : MonoBehaviour
{

	// Use this for initialization

	public RectTransform scrollRect;

	public void setPosYTo1000()
	{
		if (scrollRect != null)
		{
			scrollRect.anchoredPosition = new Vector2(scrollRect.anchoredPosition.x, -1000f);
		}

	}
}
