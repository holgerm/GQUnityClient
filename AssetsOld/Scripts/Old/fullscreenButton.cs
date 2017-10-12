using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class fullscreenButton : MonoBehaviour
{


	public fullscreenImageVisualization prefab;

	public RawImage image;



	public void goIntoFullscreen ()
	{




		fullscreenImageVisualization p =	Instantiate (prefab);
		p.texture = (image.texture as Texture2D);

	}
}
