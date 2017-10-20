using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using System.Collections;
using UnityEngine.EventSystems;

public class customButton : MonoBehaviour, IPointerDownHandler
{





	public UnityEvent onClickDown;
	public UnityEvent onClickUp;
	public UnityEvent onClick;


	public void OnPointerDown (PointerEventData data)
	{
		onClickDown.Invoke ();
	}


	public void OnPointer (PointerEventData data)
	{
		onClick.Invoke ();
	}

	public void OnPointerUp (PointerEventData data)
	{
		onClickUp.Invoke ();
	}

}
