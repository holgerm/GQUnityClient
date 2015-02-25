using UnityEngine;
using System.Collections;

public class onTapMarker : MonoBehaviour {



	public QuestRuntimeHotspot hotspot;


void OnMouseDown(){



		if (hotspot.active) {

						hotspot.hotspot.onEnter.Invoke ();

						hotspot.hotspot.onTap.Invoke ();

				}
	}
}
