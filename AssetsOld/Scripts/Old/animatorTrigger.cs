using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class animatorTrigger : MonoBehaviour {




	public List<UnityEvent> events;
public void triggerEvent(int i){


		if (events [i] != null) {

			events[i].Invoke();
		}

	}
}
