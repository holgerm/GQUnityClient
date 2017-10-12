using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class reloadquest : MonoBehaviour {

	public Sprite reloadimage;

	public void reloadQuestList () {

		GetComponent<Image>().sprite = reloadimage;

	}
}
