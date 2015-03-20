using UnityEngine;
using System.Collections;

public class SearchScript : MonoBehaviour {

	public enum SearchType {
		videos,
		channels,
		playlists
	}

	private string stringToEdit = "";

	public SearchType searchType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {

		stringToEdit = GUI.TextField(new Rect(0, 0, 200, 20), stringToEdit, 50);

		if(GUI.Button(new Rect(0,40,100,100),"Search" )){
			Debug.Log("Call Search "+stringToEdit);
			Search(stringToEdit);
		}
	}

	void Search(string searchText){
		StartCoroutine(MakeSearch(searchText));
	}

	IEnumerator MakeSearch(string text){
		WWW www = new WWW("https://gdata.youtube.com/feeds/api/videos?q="+text+"&max-results=10");
		yield return www;

	}


}
