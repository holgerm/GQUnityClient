using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class categoryFilterItem : MonoBehaviour
{





	public categoryFilter filter;

	public string filterName;
	public Text name;
	public Toggle toggle;

	// Use this for initialization
	void Start ()
	{

		name.text = filterName;


		if (filter.category.isChosen (filterName)) {

			toggle.isOn = true;
		} else {
			toggle.isOn = false;
		}

	
	}



	public void valueChanged ()
	{



		if (toggle.isOn) {

			filter.category.chooseValue (filterName);


		} else {


			filter.category.unchooseValue (filterName);

		}

		filter.updateName ();


		GameObject.Find ("QuestDownload").GetComponentInChildren<createquestbuttons> ().resetList ();


	}
	


}
