using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnitySlippyMap;

public class onTapMarker : MonoBehaviour {


	public Map map;
	public QuestRuntimeHotspot hotspot;
	public float positionmoved = 0f;



	void Start () {

		map = transform.parent.parent.GetComponent<Map>();

	}

	void OnMouseDown () {


		if ( !EventSystem.current.IsPointerOverGameObject() && !Application.isMobilePlatform && hotspot.active && map.InputsEnabled ) {


			GetComponent<MeshRenderer>().material.color = Color.grey;


		}



	}

	void OnMouseUp () {
		if ( !EventSystem.current.IsPointerOverGameObject() && !Application.isMobilePlatform && hotspot.active && map.InputsEnabled ) {

			if ( hotspot.startquest != null && hotspot.startquest.id != 0 ) {
//				GameObject.Find("QuestDatabase").GetComponent<questdatabase>().closeMap();

				GameObject.Find("QuestDatabase").GetComponent<questdatabase>().startQuestAtEndOfFrame(hotspot.startquest);
			}
			else {

				Debug.Log("RETURN: marker,onMouseUp() -> hotspot.onTap.Invoke()");

				hotspot.hotspot.onTap.Invoke();
			}

			GetComponent<MeshRenderer>().material.color = Color.white;
		}
	}

	private bool alreadyTouched = false;

	void Update () {

		if ( map.InputsEnabled ) {
			GameObject qdbGO = GameObject.Find("QuestDatabase");
			if ( qdbGO == null ) {
				Debug.Log("Cannot find QuestDataBase GameObject");
				return;
			}
			questdatabase qdb = qdbGO.GetComponent<questdatabase>();
			if ( qdb == null ) {
				Debug.Log("Cannot get component questdatabase");
				return;
			}

			if ( hotspot == null || hotspot.hotspot == null ) {
				QuestRuntimeHotspot rtHotspot = qdb.getHotspot("" + hotspot.hotspot.id);
				if ( rtHotspot == null ) {
					Debug.Log("Cannot find hotspot with id: " + hotspot.hotspot.id);
					return;
				}
				hotspot = rtHotspot;
			}


			if ( hotspot.active ) {


				GetComponent<MeshRenderer>().sortingOrder = 10;




				if ( UnityEngine.Input.touchCount == 1 ) {

				
					if ( UnityEngine.Input.GetTouch(0).phase == TouchPhase.Ended && positionmoved < 10f && !EventSystem.current.IsPointerOverGameObject() ) {
						//Debug.Log("touch ended: "+positionmoved);

						Touch touch = UnityEngine.Input.GetTouch(0);

						Ray ray = GameObject.Find("MapCam").GetComponent<Camera>().ScreenPointToRay(touch.position);
						RaycastHit hit;
					
						if ( Physics.Raycast(ray, out hit) ) {

							if ( hit.transform.gameObject == gameObject && !alreadyTouched ) {
								alreadyTouched = true;


								if ( hotspot.startquest != null && hotspot.startquest.id != 0 ) {
									// In Foyer the quest indicated by the given hotspot shoul just be started:
									GameObject.Find("QuestDatabase").GetComponent<questdatabase>().startQuest(hotspot.startquest);
								}
								else {
									// In Quest the hotspot onTap Events should be started:
									hotspot.hotspot.onTap.Invoke();
								}

								GetComponent<MeshRenderer>().material.color = Color.white;
							}
						}

					}
					else
					if ( UnityEngine.Input.GetTouch(0).phase == TouchPhase.Moved ) {

						if ( UnityEngine.Input.GetTouch(0).deltaPosition.x > 0 ) {
							positionmoved += UnityEngine.Input.GetTouch(0).deltaPosition.x;
						}
						else {
							positionmoved -= UnityEngine.Input.GetTouch(0).deltaPosition.x;

						}

						if ( UnityEngine.Input.GetTouch(0).deltaPosition.y > 0 ) {
							positionmoved += UnityEngine.Input.GetTouch(0).deltaPosition.y;
						}
						else {
							positionmoved -= UnityEngine.Input.GetTouch(0).deltaPosition.y;

						}


						if ( positionmoved > 10 ) {

							GetComponent<MeshRenderer>().material.color = Color.white;


						}

						//Debug.Log("touch move: "+positionmoved);

					}
					else
					if ( UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began ) {





						Touch touch = UnityEngine.Input.GetTouch(0);
					
						Ray ray = GameObject.Find("MapCam").GetComponent<Camera>().ScreenPointToRay(touch.position);
						RaycastHit hit;
					
						if ( Physics.Raycast(ray, out hit) ) {
						
							if ( hit.transform.gameObject == gameObject ) {

								GetComponent<MeshRenderer>().material.color = Color.grey;

							}
						}



						positionmoved = 0f;

					}


				} 
			
			}

			if ( hotspot.visible ) {
				GetComponent<MeshRenderer>().enabled = true;
			}
			else {
				GetComponent<MeshRenderer>().enabled = false;


			}

		}
	}
}
