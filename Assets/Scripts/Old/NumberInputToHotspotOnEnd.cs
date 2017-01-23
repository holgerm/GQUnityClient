using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class NumberInputToHotspotOnEnd : MonoBehaviour
{



	public InputField field;

	public Text noresult;




	public void triggerOnEnter ()
	{

		noresult.enabled = false;

		int x = 0;
		int.TryParse (field.text, out x);

		x -= 200;

		if (x > 0) {

		
			List<QuestRuntimeHotspot> list =	GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().getActiveHotspots ();

			if (list.Count > x) {

				list [x].hotspot.onEnter.Invoke ();

			} else {
				noresult.enabled = true;


			}

		} else {



			noresult.enabled = true;



		}

		field.text = "";

	}



}
