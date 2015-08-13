using UnityEngine;
using System.Collections;

public class onTapMarker : MonoBehaviour {



	public QuestRuntimeHotspot hotspot;


void OnMouseDown(){



		if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ()) {

			if (hotspot.active) {


				if (hotspot.startquest != null) {


					GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().startQuest (hotspot.startquest);
				}
				hotspot.hotspot.onTap.Invoke ();

			}
		}
	}
}
