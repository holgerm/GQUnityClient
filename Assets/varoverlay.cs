using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System;

public class varoverlay : MonoBehaviour {




	public Image bg;
	public Text text;
	public QuestAction action;


	void Start(){

		hide ();

	}

	public void show(){

		bg.enabled = true;
		text.enabled = true;


	}


	public void hide(){



		bg.enabled = false;
		text.enabled = false;
		


	}




	public void Update(){

		if (text.enabled) {
			string key = action.getAttribute ("var");
			string show = action.getAttribute ("description");
			if (action.value != null) {
			
				if (key == "score" && action.value.num_value != null && action.value.num_value.Count > 0) {
				
					show = show + "" + (int)action.value.num_value [0];
				
				} else if (action.value.bool_value != null && action.value.bool_value.Count > 0) {
					show = show + "" + action.value.bool_value [0];
				} else if (action.value.num_value != null && action.value.num_value.Count > 0) {
					show = show + "" + action.value.num_value [0];
				} else if (action.value.string_value != null && action.value.string_value.Count > 0) {
					show = show + "" + action.value.string_value [0];
				} else if (action.value.var_value != null && action.value.var_value.Count > 0) {
				
					if (!GameObject.Find ("QuestDatabase").GetComponent<actions> ().getVariable (action.value.var_value [0]).isNull()) {
					



						show = show+""+GameObject.Find("QuestDatabase").GetComponent<actions>().getVariable (action.value.var_value [0]).ToString();





						Debug.Log("found variable"+action.value.var_value [0]);
					} else {
						string d = action.value.var_value [0];
						bool date = false;
						if(d.StartsWith("date(")){
							date = true;
							d = d.Replace("date(","");
							d = d.Replace(")","");
						}


					
						if(date){

							double ergebnis = GameObject.Find ("QuestDatabase").GetComponent<actions> ().mathVariable (d);
							Debug.Log(ergebnis);
							TimeSpan time = TimeSpan.FromSeconds(ergebnis);
							
							double seconds = time.Seconds ;
							string seconds_str = seconds.ToString();

							if(seconds < 10){ seconds_str = "0"+seconds; }


							double minutes = time.Minutes;
							string minutes_str = minutes.ToString();

							if(minutes < 10){ minutes_str = "0"+(int)minutes; }
							if(minutes < 0){ minutes_str = "00"; }

							int hours = time.Hours;
							string hours_str = hours.ToString();
							
							if(hours < 10){ hours_str = "0"+hours; }
							if(hours < 0){ hours_str = "00"; }


							int days = (int)time.TotalDays;
							string days_str = days.ToString();

							if(days < 0){ days_str = "0"; }



							string finaldate = "";

							if(days > 0){

								finaldate = days_str+":";

							}

							if(hours > 0){

								finaldate = finaldate + "" + hours_str + ":";
							}

							finaldate = finaldate + "" + minutes_str + ":"+seconds_str;


							show = show + ""+ finaldate;




						} else {
					
						show = show + "" + GameObject.Find ("QuestDatabase").GetComponent<actions> ().mathVariable (d);
						}
					
					
					}
				}
			
			}

			text.text = show;
		}
	}



}
