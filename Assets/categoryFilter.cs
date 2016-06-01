using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class categoryFilter : MonoBehaviour
{

	public QuestMetaCategory category;


	public Text title;
	public Text chosen;


	public Transform listParent;
	public categoryFilterItem prefab;


	public categoryFilter old;

	public categoryFilter prefab2;


	// Use this for initialization
	void Start ()
	{
		updateName ();


		if (prefab != null) {

			instantiateItems ();

		}
	
	}





	public void updateName ()
	{

		if (old != null) {

			old.updateName ();
		}

		title.text = category.name;

		if (chosen != null) {
			chosen.text = "";
			List<string> display = category.chosenValues;
			if (display != null && display.Count > 0) {


				if (display.Count > 0) {

					foreach (string s in display) {
						if (chosen.text.Length < 40) {
							chosen.text += s + ", ";
						}

					}

					chosen.text = chosen.text.Substring (0, chosen.text.Length - 2);

				}
				if (chosen.text.Length >= 40) {
			
					chosen.text += "...";

				}

			} else {
				chosen.text = "Alle";

			}

		}
	}


	public void instantiateItems ()
	{

		foreach (string s in category.possibleValues) {

			categoryFilterItem cFI = Instantiate (prefab);
			cFI.transform.SetParent (listParent, false);
			cFI.transform.localScale = Vector3.one;
			cFI.filterName = s;
			cFI.filter = this;

		}



	}


	public void instantianteFilter ()
	{



		if (prefab2 != null) {

			categoryFilter cF = Instantiate (prefab2);

			cF.transform.SetParent (GameObject.Find ("ImpressumCanvas").transform, false);
			cF.transform.localScale = Vector3.one;
			cF.old = this;
			cF.category = category;
		}


	}


	public void destroyMe ()
	{

		Destroy (gameObject);
	}


}
