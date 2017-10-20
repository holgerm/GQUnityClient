using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonControler : MonoBehaviour {



	public UnityEvent onPressed;
	public UnityEvent onPressedEnd;


	private Button b;


	void Start(){


		b = GetComponent<Button> ();



	}

	void Update(){

//		Debug.Log (b.spriteState);
	}
	public void OnPointerDown(PointerEventData data)
	{



		onPressed.Invoke ();
	}

	
	public void OnPointerUp(PointerEventData data)
	{
		
		
		
		onPressedEnd.Invoke ();
	}


}
