using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class npctalk_nextbutton : MonoBehaviour, IPointerClickHandler {

	public page_npctalk controller;
	private float clickTime;            // time of click
	public bool onClick = true;            // is click allowed on button?

	
	public void OnPointerClick(PointerEventData data)
	{      
		if (GetComponent<Button> ().interactable) {

		
						// single click
						if (onClick) {

								controller.nextButton ();

						}
		
						
				}
	}
}
