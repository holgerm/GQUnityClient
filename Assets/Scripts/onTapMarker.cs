using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class onTapMarker : MonoBehaviour {



	public QuestRuntimeHotspot hotspot;



	public float positionmoved = 0f;





	void OnMouseDown(){


		if (!Application.isMobilePlatform && hotspot.active) {


			GetComponent<MeshRenderer>().material.color = Color.grey;


		}



	}

	void OnMouseUp(){


		if (!Application.isMobilePlatform && hotspot.active) {

			if (hotspot.startquest.id != 0) {
				
				
				GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().startQuest (hotspot.startquest);
			}

			GetComponent<MeshRenderer>().material.color = Color.white;


			Debug.Log("onTap invoked");
			hotspot.hotspot.onTap.Invoke ();

			
			
		}
		
		
	}
	void Update(){


		if (hotspot.active) {





				if (UnityEngine.Input.touchCount == 1) {

				
				if (UnityEngine.Input.GetTouch (0).phase == TouchPhase.Ended && positionmoved < 10f && !EventSystem.current.IsPointerOverGameObject()) {
					//Debug.Log("touch ended: "+positionmoved);

					Touch touch = UnityEngine.Input.GetTouch (0);

					Ray ray = GameObject.Find("MapCam").GetComponent<Camera>().ScreenPointToRay(touch.position);
					RaycastHit hit;
					
					if (Physics.Raycast(ray, out hit)) 
					       
						{

						if(hit.transform.gameObject == gameObject){


							if (hotspot.startquest.id != 0) {

								
								GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().startQuest (hotspot.startquest);
							}

							GetComponent<MeshRenderer>().material.color = Color.white;


						hotspot.hotspot.onTap.Invoke ();

						}

					}





				} else if (UnityEngine.Input.GetTouch (0).phase == TouchPhase.Moved) {

				if(	UnityEngine.Input.GetTouch (0).deltaPosition.x > 0){
						positionmoved += UnityEngine.Input.GetTouch (0).deltaPosition.x;
					} else {
						positionmoved -= UnityEngine.Input.GetTouch (0).deltaPosition.x;

					}

					if(UnityEngine.Input.GetTouch (0).deltaPosition.y > 0){
						positionmoved += UnityEngine.Input.GetTouch (0).deltaPosition.y;
					} else {
						positionmoved -= UnityEngine.Input.GetTouch (0).deltaPosition.y;

					}


					if(positionmoved > 10){

						GetComponent<MeshRenderer>().material.color = Color.white;


					}

					//Debug.Log("touch move: "+positionmoved);

				} else if(UnityEngine.Input.GetTouch (0).phase == TouchPhase.Began){





					Touch touch = UnityEngine.Input.GetTouch (0);
					
					Ray ray = GameObject.Find("MapCam").GetComponent<Camera>().ScreenPointToRay(touch.position);
					RaycastHit hit;
					
					if (Physics.Raycast(ray, out hit)) 
						
					{
						
						if(hit.transform.gameObject == gameObject){

					GetComponent<MeshRenderer>().material.color = Color.grey;

						}
					}



					positionmoved = 0f;

				}


				} 
			
		}

	}
}
