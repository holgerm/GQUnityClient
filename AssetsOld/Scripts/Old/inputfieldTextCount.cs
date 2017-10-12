using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class inputfieldTextCount : MonoBehaviour
{


	public int maxCount = 3;
	
	// Update is called once per frame
	void Update ()
	{

		if (GetComponent<InputField> ().text.Length > maxCount) {

			GetComponent<InputField> ().text = GetComponent<InputField> ().text.Substring (0, maxCount);

		}
	
	}

	public void removeString ()
	{


		GetComponent<InputField> ().text = GetComponent<InputField> ().text.Substring (0, GetComponent<InputField> ().text.Length - 1);
	}

	public void addString (string i)
	{


		GetComponent<InputField> ().text = GetComponent<InputField> ().text + "" + i;
	}
}
