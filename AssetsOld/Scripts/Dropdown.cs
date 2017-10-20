using UnityEngine;
using System.Collections;

public class Dropdown : MonoBehaviour {


	public string valuetoread;
	public questdatabase questdb;


	// Use this for initialization
	void Start () {
	
		questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
