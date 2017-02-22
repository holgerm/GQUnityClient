using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using GQ.Client.UI.Pages;

public class npctalk_nextbutton : MonoBehaviour, IPointerClickHandler {

	public page_npctalk controller;
	private float clickTime;
	// time of click
	public bool onClick = true;
	// is click allowed on button?
	public bool backbutton = false;

	public void OnPointerClick (PointerEventData data) {      
		if ( GetComponent<Button>().interactable ) {

		
			// single click
			if ( onClick ) {
				 
				if ( backbutton ) {
					controller.backButton();
				}
				else {
					controller.nextButton();
				}

			}
		
						
		}
	}
}
