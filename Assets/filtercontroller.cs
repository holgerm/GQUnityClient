using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using GQ.Client.Model;

public class filtercontroller : MonoBehaviour
{
	
	
	
	
	public bool isActive = false;
	public questdatabase questdb;

	
	

	
	
	public void showMenu ()
	{
		
		if (QuestManager.Instance.CurrentQuest.currentpage.type == "WebPage") {
			
			GameObject.Find ("PageController").GetComponent<page_webpage> ().deactivateWebView ();
			
		}
		
	}

	
	public void hideMenu ()
	{
		
		if (QuestManager.Instance.CurrentQuest.currentpage.type == "WebPage") {
			
			GameObject.Find ("PageController").GetComponent<page_webpage> ().activateWebView ();
			
		}
		
		
	}

	public void showImpressum ()
	{
		
		GameObject.Find ("ImpressumCanvas").GetComponent<showimpressum> ().toggleImpressum ();
		
	}

	
	public void endQuestAnimation ()
	{
		
		GetComponent<Animator> ().SetTrigger ("endmenu");
		isActive = false;
		
	}

	
	public void enQuest ()
	{
		
		GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().endQuest ();
		
		
	}

	public void returntoMainMenu ()
	{
		
		isActive = false;
		
		SceneManager.LoadScene ("questlist");
		
	}

	
	
	public void reloadAllData ()
	{
		
		
		bool b =	GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().ReloadButtonPressed ();


		
	}

	public void showTopBar ()
	{
		isActive = true;
		
		
		if (this != null) {
			GetComponent<Animator> ().SetTrigger ("startMenu");
		}
	}

	
	
	public void hideTopBar ()
	{
		isActive = false;
		
		
	}
	
}
