using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mapdisplaytoggle : MonoBehaviour {







	private List<string> disabledrenderers;

	void Start(){

		disabledrenderers = new List<string> ();

	}
	public void hideMap(){


		foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {

			if(sr.enabled){
			sr.enabled = false;
				disabledrenderers.Add(sr.gameObject.name);
			}

		}

		foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {

			if(mr.enabled){
				mr.enabled = false;
				disabledrenderers.Add(mr.gameObject.name);
			}

		}

	}


	
	public void showMap(){
		
		
		foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
			if(disabledrenderers.Contains(sr.gameObject.name)){
			sr.enabled = true;
				disabledrenderers.Remove(sr.gameObject.name);
			}
			
		}
		
		foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
			
				if(disabledrenderers.Contains(mr.gameObject.name)){
					mr.enabled = true;
					disabledrenderers.Remove(mr.gameObject.name);
				}
			
		}
		
	}





}
