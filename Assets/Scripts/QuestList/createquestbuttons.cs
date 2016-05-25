using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class createquestbuttons : MonoBehaviour
{


	public SampleListDivider sampleListDivider;
	public GameObject sampleButton;
	public questdatabase questdb;
	public InputField filterinput;
	public Text downloadmsg;
	public Quest currentquest;
	public List<Quest> filteredOnlineList;
	public List<Quest> filteredOfflineList;
	public Text header;
	public bool sortbyname = false;
	public int portal_id = 1;
	public string namefilter;
	private WWW www;


	public string sortedby = "Erstellungsdatum";

	void Start ()
	{
		questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();

		if (!Application.isWebPlayer) {

			loadLocalQuests ();

			if ((questdb.allquests.Count < 1 && filteredOfflineList.Count < 1) || Configuration.instance.showcloudquestsimmediately) {
//				LoadQuestsFromServer();
			} else {
				DisplayList ();
			
			}
		}
	}

	public void loadLocalQuests ()
	{
		foreach (Quest q in questdb.GetLocalQuests ()) {
			filteredOfflineList.Add (q);
		}
	}

	public void setSortByName (bool b)
	{

		sortbyname = b;

		DisplayList ();


	}

	public void filterForName (string s)
	{




		namefilter = s;
		if (questdb.localquests.Count > 0) {
			filteredOfflineList.Clear ();
			

			
			
			
			if (s == "") {
				
				foreach (Quest q in questdb.localquests) {
					filteredOfflineList.Add (q);
				}
			} else {
				
				foreach (Quest q in questdb.localquests) {
					
					//Debug.Log (q.name + " contains " + s + "? " + q.name.Contains (s));
					if (q.meta_Search_Combined.ToUpper ().Contains (s.ToUpper ())) {
						filteredOfflineList.Add (q);
					}
				}
			}
			
		}

		if (questdb.allquests.Count > 0) {
			filteredOnlineList.Clear ();

			foreach (RectTransform go in GetComponentsInChildren<RectTransform>()) {
				if (go != transform) {
					Destroy (go.gameObject);
				}
			}



			if (s == "") {

				foreach (Quest q in questdb.allquests) {
					filteredOnlineList.Add (q);
				}
			} else {

				if (questdb.allquests.Count > 0) {
					foreach (Quest q in questdb.allquests) {

						if (q.name.ToUpper ().Contains (s.ToUpper ())) {
							filteredOnlineList.Add (q);
						}
					}
				}
			}

		}
		DisplayList ();
	}

	public void LoadQuestsFromServer ()
	{
		filteredOnlineList.Clear ();
		questdb.ReloadQuestListAndRefresh ();
	}

	public void resetList ()
	{
		filteredOfflineList.Clear ();

		foreach (Quest q in questdb.GetLocalQuests()) {
			filteredOfflineList.Add (q);
		}
		filterForName (namefilter);
	}





	public List<Quest> sortByMetaDataAsc (List<Quest> quests, string meta)
	{
		List<Quest> sortedlist = new List<Quest> ();

		Quest[] queststoshow = quests.ToArray ();
		
		Array.Sort<Quest> (queststoshow, (x, y) => String.Compare (x.getMetaComparer ("Wert"), y.getMetaComparer ("Wert")));

		sortedlist.AddRange (queststoshow);

		return sortedlist;
	}


	public void  DisplayList ()
	{

		foreach (RectTransform go in GetComponentsInChildren<RectTransform>()) {
			if (go != transform) {
				Destroy (go.gameObject);
			}
		}

		List<Quest> showonline = new List<Quest> ();
		List<Quest> showoffline = new List<Quest> ();

		foreach (Quest q in filteredOfflineList) {
			showoffline.Add (q);
		}
		if (Configuration.instance.cloudQuestsVisible) {
			foreach (Quest q in filteredOnlineList) {
				showonline.Add (q);
			}
		}


		if (sortbyname) {


			
			showonline.Sort ();
			showoffline.Sort ();
//TODO: Finish
//			if(sortedby == "Name"){
//
//			showonline.Sort ();
//			showoffline.Sort ();
//
//			} else {
//				showonline = sortByMetaDataAsc(showonline,sortedby);
//			}


		} else {
			showoffline.Reverse ();
		}



		if (showonline.Count > 0 && showoffline.Count > 0) {


			header.text = "Alle Quests";
			SampleListDivider local = Instantiate (sampleListDivider) as SampleListDivider;
			local.title = "Local";
			local.transform.SetParent (transform);
			local.transform.localScale = new Vector3 (1f, 1f, 1f);

		} else {

			if (showonline.Count > 0) { 
				header.text = "Cloud Quests";
			} else if (showoffline.Count > 0) {
				header.text = "Lokale Quests";
			}

		}

		foreach (var item in showoffline) {
			GameObject newButton = Instantiate (sampleButton) as GameObject;
			SampleButton button = newButton.GetComponent <SampleButton> ();
			button.nameLabel.text = item.name;
			button.q = item;
			newButton.transform.SetParent (transform);
			newButton.transform.localScale = new Vector3 (1f, 1f, 1f);
			
		}

		if (showonline.Count > 0 && showoffline.Count > 0) {

			SampleListDivider cloud = Instantiate (sampleListDivider) as SampleListDivider;
			cloud.title = "Cloud";
			cloud.transform.SetParent (transform);
			cloud.transform.localScale = new Vector3 (1f, 1f, 1f);
		}
		foreach (var item in showonline) {
			GameObject newButton = Instantiate (sampleButton) as GameObject;
			SampleButton button = newButton.GetComponent <SampleButton> ();
			button.nameLabel.text = item.name;
			button.q = item;
			newButton.transform.SetParent (transform);
			newButton.transform.localScale = new Vector3 (1f, 1f, 1f);

		}

		downloadmsg.enabled = false;
		filterinput.interactable = true;

	}







}





