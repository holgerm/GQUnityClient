using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;

public class CategorySetPart : MonoBehaviour
{

	public Text catSetName;

	public Transform catContent1;
	public Transform catContent2;
	public Transform catContent3;
	public Transform catContent4;

	public void Initialize (string categorySetName, params string[] catNames)
	{
		catSetName.text = categorySetName;

		if (catNames.Length > 0 && catNames [0] != null)
			showCat (catNames [0], catContent1);
		else
			catContent1.gameObject.SetActive (false);
		
		if (catNames.Length > 1 && catNames [1] != null)
			showCat (catNames [1], catContent2);
		else
			catContent2.gameObject.SetActive (false);
		
		if (catNames.Length > 2 && catNames [2] != null)
			showCat (catNames [2], catContent3);
		else
			catContent3.gameObject.SetActive (false);
		
		if (catNames.Length > 3 && catNames [3] != null)
			showCat (catNames [3], catContent4);
		else
			catContent4.gameObject.SetActive (false);
	}

	void showCat (string catName, Transform catElement)
	{
		Image catImage = catElement.Find ("Image").GetComponent<Image> ();
		Text catText = catElement.Find ("Text").GetComponent<Text> ();

		Category cat;

		if (ConfigurationManager.Current.categoryDict.TryGetValue (catName, out cat)) {
			if (cat.symbol != null)
				catImage.sprite = Resources.Load<Sprite> (cat.symbol.path);
			else {
				catImage.sprite = null;
				catImage.color = new Color (255f, 255f, 255f, 0f); // transparent
			}
			catText.text = cat.name;
		} else {
			catElement.gameObject.SetActive (false);
		}
	}
}
