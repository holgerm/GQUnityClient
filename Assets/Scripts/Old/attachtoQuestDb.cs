using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class attachtoQuestDb : MonoBehaviour {

	public bool asScrollView;
	public bool asDownloadText;








	IEnumerator Start () {


		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		if ( asScrollView ) {

			GameObject.Find("QuestDatabase").GetComponent<questdatabase>().publicquestslist = GetComponent<Image>();

		}
		else
		if ( asDownloadText ) {
			
			GameObject.Find("QuestDatabase").GetComponent<questdatabase>().webloadingmessage = GetComponent<Text>();
			
		} 

	}
}
