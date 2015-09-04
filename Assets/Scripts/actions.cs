using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class actions : MonoBehaviour
{



	public questaudio audio_prefab;
	public QuestMessage message_prefab;
	public Canvas msgcanvas;
	public Quest quest;
	private WWW www;
	public questdatabase questdb;
	public int score = 0;
	public int loopcount = 0;
	public List<QuestVariable> variables;
	public List<questaudio> questaudiosources; // TODO check for null objects e.g. by changing scenes and destroying objects
	public questaudio npcaudio;
	public List<AudioSource> audiosources;
	public List<QuestRuntimeAsset> photos;
	public List<QuestRuntimeAsset> audioclips;



	private QuestAction gpsRoute;
	private bool updateGPSRoute = false;
	public float gPSRouteUpdateInterval = 10f;


	private float gPSRouteUpdateInterval_save = 10f;


	void Update ()
	{


		if (updateGPSRoute && questdb.currentquest.currentpage.type == "MapOSM") {

			gPSRouteUpdateInterval -= Time.deltaTime;


			if (gPSRouteUpdateInterval < 0f) {

				gPSRouteUpdateInterval = gPSRouteUpdateInterval_save;


				addRoute (gpsRoute);

			}


		}

	}



	void Start ()
	{


		gPSRouteUpdateInterval_save = gPSRouteUpdateInterval;

		questdb = GetComponent<questdatabase> ();
		variables = new List<QuestVariable> ();
		questaudiosources = new List<questaudio> ();
		photos = new List<QuestRuntimeAsset> ();
		audioclips = new List<QuestRuntimeAsset> ();

	}

	public void reset ()
	{

		variables = new List<QuestVariable> ();
		questaudiosources = new List<questaudio> ();
		photos = new List<QuestRuntimeAsset> ();

		loopcount = 0;
		score = 0;

	}

	public void startWebXml (string x)
	{


	}

	public void sendVartoWeb ()
	{

		if (Application.isWebPlayer) {

			string myvars = "";
			if (variables != null && variables.Count > 0) {
				foreach (QuestVariable qv in variables) {

				
					if (qv.string_value != null && qv.string_value.Count > 0) {
						myvars += "<b>" + qv.key + ": " + qv.string_value [0] + "<br/>";
					} else if (qv.num_value != null && qv.num_value.Count > 0) {

						myvars += "<b>" + qv.key + "</b>: " + qv.num_value [0] + "<br/>";

					} else if (qv.bool_value != null && qv.bool_value.Count > 0) {
						
						myvars += "<b>" + qv.key + ": " + qv.bool_value [0] + "<br/>";
						
					}

				}




			}
			Application.ExternalCall ("updateVariables", myvars);

		}
		
	}

	public void doAction (QuestAction action)
	{

		quest = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ().currentquest;

		if (action.type == "StartMission") {
			changePage (action);
		} else if (action.type == "EndGame") {
			photos = new List<QuestRuntimeAsset> ();
			questdb.endQuest ();
		} else	if (action.type == "PlayAudio") {
			PlayAudio (action);
		} else if (action.type == "SetVariable") {
//			Debug.Log("setting var");
			setVariable (action);
			sendVartoWeb ();
		} else if (action.type == "LoadVariable") {
			loadVariable (action);
			sendVartoWeb ();
		} else if (action.type == "If") {
			ifCondition (action);
		} else if (action.type == "Loop") {
			Loop (action);
		} else if (action.type == "Break") {
			breakLoop ();
		} else if (action.type == "Vibrate") {
			vibrate (action);
		} else if (action.type == "IncrementVariable") {
			incvar (action);
			sendVartoWeb ();
		} else if (action.type == "DecrementVariable") {
			decvar (action);
			sendVartoWeb ();
		} else if (action.type == "AddToScore") {
			addtoscore (action);
			sendVartoWeb ();
		} else if (action.type == "ShowMessage") {
			showmessage (action);
		} else if (action.type == "AddRoute") {
			addRoute (action);
		} else if (action.type == "SetHotspotState") {
			sethotspotstate (action);
		} else if (action.type == "LoadVar") {
			loadVariable (action);
		} else if (action.type == "SaveVar") {
			saveVariable (action);
		} else if (action.type == "ShowVar") {
			showVariableOverlay (action);
		} else if (action.type == "HideVar") {
			hideVariableOverlay (action);
		}
		
		
	}




	public void addRoute (QuestAction action)
	{



		if (GameObject.Find ("PageController_Map") != null) {

			page_map mapcontroller = GameObject.Find ("PageController_Map").GetComponent<page_map> ();
				


			if (action.hasAttribute ("from") || action.hasAttribute ("to")) {
			

				float lon1 = 0f;
				float lat1 = 0f;
				float lon2 = 0f;
				float lat2 = 0f;
				

				if (action.getAttribute ("from") != "" && action.getAttribute ("from") != "0") {


					QuestRuntimeHotspot from = questdb.getHotspot (action.getAttribute ("from"));

					if (from != null) {
						lon1 = from.lon;
						lat1 = from.lat;

					}

				} else {

					lon1 = (float)GameObject.Find ("QuestDatabase").GetComponent<GPSPosition> ().CoordinatesWGS84 [1];
					lat1 = (float)GameObject.Find ("QuestDatabase").GetComponent<GPSPosition> ().CoordinatesWGS84 [0];
					gpsRoute = action;
					
					
				}



				if (action.getAttribute ("to") != "" && action.getAttribute ("to") != "0") {

					QuestRuntimeHotspot to = questdb.getHotspot (action.getAttribute ("to"));


					if (to != null) {
						lon2 = to.lon;
						lat2 = to.lat;
					}

				} else {


					lon1 = (float)GameObject.Find ("QuestDatabase").GetComponent<GPSPosition> ().CoordinatesWGS84 [1];
					lat1 = (float)GameObject.Find ("QuestDatabase").GetComponent<GPSPosition> ().CoordinatesWGS84 [0];
					gpsRoute = action;

					
				}






				if ((lon1 != lon2) && (lat1 != lat2)) {



					if (gpsRoute != null) {

						updateGPSRoute = true;

					}

				





					string url = "http://www.yournavigation.org/api/1.0/gosmore.php?" +
						"format=kml" +
						"&flat=" + lon1 +
						"&flon=" + lat1 +
						"&tlat=" + lon2 +
						"&tlon=" + lat2 +
						"&v=foot&" +
						"fast=1" +
						"&layer=mapnik" +
						"&instructions=1" +
						"&lang=de";

					Debug.Log (url);
					WWW routewww = new WWW (url);

					StartCoroutine (waitForRouteFile (routewww));

					if (Application.isWebPlayer) {

						questdb.debug ("Es wurde eine neue Route gesetzt. Diese sind aktuell nicht in der Editor-Vorschau sichtbar. Benutze zum Testen eine Android- oder iOS-App.");
					}

				} else {


					questdb.debug ("Eine Route muss aus zwei nicht-identischen Orten bestehen.");

					updateGPSRoute = false;
				}

			} else {

				questdb.debug ("Eine Route muss mindestens einen festen Hotspot beinhalten.");
				updateGPSRoute = false;

			}


		} else {


			questdb.debug ("Die Map muss mindestens einmal vorher geöffnet worden sein.");
			updateGPSRoute = false;

		}



	}




	public IEnumerator waitForRouteFile (WWW mywww)
	{

		yield return mywww;



		if (mywww.error == null || mywww.error == "") {

			Debug.Log (mywww.text);


			string routefile = mywww.text;

			routefile = routefile.Substring (routefile.IndexOf ("<coordinates>"));
			routefile = routefile.Substring (14, routefile.IndexOf ("</coordinates>") - 14);




			if (GameObject.Find ("PageController_Map") != null) {
				
				page_map mapcontroller = GameObject.Find ("PageController_Map").GetComponent<page_map> ();


				mapcontroller.currentroute = new Route ();

				Debug.Log ("doing new oute");


				string[] coordinates = routefile.Split (new string[] { Environment.NewLine }, StringSplitOptions.None);

				foreach (string s in coordinates) {


					if (s.Contains (",")) {


						string[] co = s.Split (',');


						mapcontroller.currentroute.addPoint (co [0], co [1]);




					}




				}

				mapcontroller.drawCurrentRoute ();


			}



		} else {

			Debug.Log ("Route WWW Error:" + mywww.error);

		}




	}

	void changePage (QuestAction action)
	{
		quest.previouspages.Add (quest.currentpage);



		if (questdb.currentquest.hasAttribute ("individualReturnDefinitions")
			&& questdb.currentquest.getAttribute (("individualReturnDefinitions")) == "true") {

			if (action.getAttribute ("allowReturn") != "") {

				if (action.getAttribute ("allowReturn") == "0") {
					questdb.allowReturn = false;


				} else {
					questdb.allowReturn = true;

				}

			} else {

				questdb.allowReturn = true;

			}

		} else {

			questdb.allowReturn = true;

		}

		questdb.changePage (int.Parse (action.getAttribute ("id")));
	
	}


	void showVariableOverlay (QuestAction action)
	{

		GameObject.Find ("VarOverlayCanvas").GetComponent<varoverlay> ().action = action;
		GameObject.Find ("VarOverlayCanvas").GetComponent<varoverlay> ().show ();



	}

	void hideVariableOverlay (QuestAction action)
	{
		GameObject.Find ("VarOverlayCanvas").GetComponent<varoverlay> ().hide ();
	}



	public void sethotspotstate (QuestAction action)
	{

		bool needshotspot = false;

		if (action.hasAttribute ("applyToAll")) {

			if (action.getAttribute ("applyToAll") == "1") {


				// all Hotspots


			} else {

				// needs Hotspot
				needshotspot = true;


			}


		} else {


			// needs Hotspot
			needshotspot = true;



		}



		if (needshotspot) {



			// get Hotspot
			if (action.hasAttribute ("hotspot")) {



				QuestRuntimeHotspot qh = questdb.getHotspot (action.getAttribute ("hotspot"));

				if (qh != null) {


					if (action.hasAttribute ("activity")) {


						if (action.getAttribute ("activity") == "inaktiv") {


							qh.active = false;

						} else if (action.getAttribute ("activity") == "aktiv") {
							
							qh.active = true;

							
						} 


					}


					if (action.hasAttribute ("visibility")) {
						
						
						if (action.getAttribute ("visibility") == "unsichtbar") {
							
						
							qh.visible = false;


							if (qh.renderer != null) {

								qh.renderer.enabled = false;


							} 
							
						} else if (action.getAttribute ("visibility") == "sichtbar") {
							
							qh.visible = true;

							if (qh.renderer != null) {
								
								qh.renderer.enabled = true;

								
							} 
							
						} 
						
						
					}



				} else {

					questdb.debug ("Hotspot not found");

				}


			} else {


				questdb.debug ("no Hotspot specified");
 
			}






		}










	}

	public void addtoscore (QuestAction action)
	{



		if (action.hasAttribute ("value")) {


			string x = new string (action.getAttribute ("value").ToCharArray ()
				                        .Where (c => !Char.IsWhiteSpace (c))
				                        .ToArray ());

			int i = int.Parse (x);

			score += i;


		}
	}

	public void incvar (QuestAction action)
	{
			
		if (action.hasAttribute ("var")) {


			QuestVariable qv = getVariable (action.getAttribute ("var"));

			if (qv.type == "num") {

				double value = qv.getNumValue ();

				value += 1;

				variables.Remove (qv);

				variables.Add (new QuestVariable (action.getAttribute ("var"), value));

			}

		}

	}

	public void decvar (QuestAction action)
	{
		if (action.hasAttribute ("var")) {
			
			
			QuestVariable qv = getVariable (action.getAttribute ("var"));
			
			if (qv.type == "num") {
				
				double value = qv.getNumValue ();
				
				value -= 1;
				
				variables.Remove (qv);
				
				variables.Add (new QuestVariable (action.getAttribute ("var"), value));
				
			}
			
		}
	}

	public void vibrate (QuestAction action)
	{

#if UNITY_IPHONE || UNITY_WP8 || UNITY_ANDROID || UNITY_BLACKBERRY


		//int duration = int.Parse(action.getAttribute("duration"));

		Handheld.Vibrate ();
		//StartCoroutine(continueVibrating(duration));
#endif

	}

	public IEnumerator continueVibrating (int d)
	{
		yield return new WaitForSeconds (0.5f);
		#if UNITY_WEBPLAYER
		
		Debug.Log("cannot vibrate on web");
		
		# else 
		Handheld.Vibrate ();
#endif
		int b = d - 1;

		if (b > 0) {

			StartCoroutine (continueVibrating (b));

		}



	}

	public void ifCondition (QuestAction action)
	{



		if (action.condition.isfullfilled ()) {

			action.InvokeThen ();

		} else {
			action.InvokeElse ();
		}


	}

	public void Loop (QuestAction action)
	{



		loopcount += 1;
		int loopnumber = loopcount;


		int counter = 0;


		if (action.hasAttribute ("loopVariable") && action.hasAttribute ("from") && action.hasAttribute ("to")) {

			// FOR LOOP

			string var = action.getAttribute ("loopVariable");		
					

			int from;
			int to;

			bool a = int.TryParse (action.getAttribute ("from"), out from);
			bool b = int.TryParse (action.getAttribute ("to"), out to);
						

			if (a && b) {


				if (from < to) {
					for (int count = from; count <= to && loopcount >= loopnumber; count ++) {
						setVariable (var, (float)count);
						action.InvokeThen ();

					}
				} else {

					for (int count = from; count >= to && loopcount >= loopnumber; count --) {
						setVariable (var, (float)count);
						action.InvokeThen ();
						
					}
				}

			}






		} else {

			// WHILE LOOP


			while (action.condition.isfullfilled () && counter < 1000 && loopcount >= loopnumber) {


				action.InvokeThen ();

				if (action.hasAttribute ("unlimitedLoops")) {

					if (action.getAttribute ("unlimitedLoops") == "0" || action.getAttribute ("unlimitedLoops") == "false") {
						counter += 1;
					}


				} else {

					counter += 1;
				}


			} 
		}
		
	}

	void breakLoop ()
	{
		if (loopcount > 0) {
			//Debug.Log ("breaking loop");
			loopcount -= 1;
		}
	}

	public string formatString (string s)
	{





		string k = "";
		if (s.Contains ("@")) {


			bool lastwasvar = true;


			char[] splitter = "@".ToCharArray ();

			string[] splitted = s.Split (splitter);

			int c1 = 1;

			foreach (string x in splitted) {






				Debug.Log (x);

				string x2 = new string (x.ToCharArray ()
					                 .Where (c => !Char.IsWhiteSpace (c))
					                 .ToArray ());

				Debug.Log (x2);

				if (x == x2) {



					if (x2 == "score") {

						k += score.ToString ();
						lastwasvar = true;

					} else 
						if (getVariable (x2).ToString () != "[null]") {

						k += getVariable (x2).getStringValue ();
						lastwasvar = true;

					} else {


						if (lastwasvar) {
							k += x;
						} else {
							k += "@" + x;

						}
						lastwasvar = false;

					}


				} else {

					if (lastwasvar) {
						k += x;
					} else {
						k += "@" + x;
						
					}
					lastwasvar = false;

				}

							

				
						
				c1++;

			}




		} else {

			k += s;

		}



		k = k.Replace ("<br>", "\n");


		return k;

	}



	public void saveVariable (QuestAction action)
	{
		
	
		
		
		string key = action.getAttribute ("var");

		string varname = key;
		varname = new String (varname.Where (Char.IsLetter).ToArray ());
		
		varname = questdb.currentquest.id + "_" + varname;




		QuestVariable qv = getVariable (key);


		if (qv != null) {
			if (qv.type == "num") {

				PlayerPrefs.SetString (varname, "[GQ_NUM_VALUE]" + qv.num_value [0]);

			} else if (qv.type == "string") {

				PlayerPrefs.SetString (varname, qv.string_value [0]);

			} else if (qv.type == "bool") {


				if (qv.bool_value [0]) {
					PlayerPrefs.SetInt (varname, 1);
				} else {
					PlayerPrefs.SetInt (varname, 0);

				}

			}


		}

	}





	void removeVariable (string key)
	{

		
		List<QuestVariable> helplist = new List<QuestVariable> ();
		foreach (QuestVariable qa in variables) {
			helplist.Add (qa);
		}
		
		foreach (QuestVariable qa in helplist) { 
			if (qa.key == key) {
				variables.Remove (qa);
			}
		}


	}

	void loadVariable (QuestAction action)
	{



//		Debug.Log ("loading vars");

		string varname = action.getAttribute ("var");
		varname = new String (varname.Where (Char.IsLetter).ToArray ());

		varname = questdb.currentquest.id + "_" + varname;




	

		if (PlayerPrefs.GetString (varname, "[GEOQUEST_NO_VALUE]") != "[GEOQUEST_NO_VALUE]") {
		




			removeVariable (action.getAttribute ("var"));



			if (PlayerPrefs.GetString (varname).StartsWith ("[GQ_NUM_VALUE]")) {


				string s = PlayerPrefs.GetString (varname).Replace ("[GQ_NUM_VALUE]", "");
				double n = double.Parse (s);
				variables.Add (new QuestVariable (action.getAttribute ("var"), n));

				
			} else {

				variables.Add (new QuestVariable (action.getAttribute ("var"), PlayerPrefs.GetString (varname)));
		

			}

		} else if (PlayerPrefs.GetInt (varname, -99999) != -99999) {
			removeVariable (action.getAttribute ("var"));

			variables.Add (new QuestVariable (action.getAttribute ("var"), PlayerPrefs.GetInt (varname)));

		}

	}




	public void setVariable (string key, float f)
	{


		Debug.Log ("setting var " + key + " to " + f);

		List<QuestVariable> helplist = new List<QuestVariable> ();
		foreach (QuestVariable qa in variables) {
			helplist.Add (qa);
		}
		
		foreach (QuestVariable qa in helplist) { 
			if (qa.key == key) {
				variables.Remove (qa);
			}
		}


		variables.Add (new QuestVariable (key, f));

	}

	public void setVariable (string key, string s)
	{
			
		List<QuestVariable> helplist = new List<QuestVariable> ();
		foreach (QuestVariable qa in variables) {
			helplist.Add (qa);
		}
			
		foreach (QuestVariable qa in helplist) { 
			if (qa.key == key) {
				variables.Remove (qa);
			}
		}
			
			
		variables.Add (new QuestVariable (key, s));
			               
	}

	public void setVariable (string key, bool b)
	{
				
		List<QuestVariable> helplist = new List<QuestVariable> ();
		foreach (QuestVariable qa in variables) {
			helplist.Add (qa);
		}
				
		foreach (QuestVariable qa in helplist) { 
			if (qa.key == key) {
				variables.Remove (qa);
			}
		}
				
				
		variables.Add (new QuestVariable (key, b));
				               
	}

	public void setVariable (QuestAction action)
	{

		List<QuestVariable> helplist = new List<QuestVariable> ();
		foreach (QuestVariable qa in variables) {
			helplist.Add (qa);
		}

		foreach (QuestVariable qa in helplist) { 
			if (qa.key == action.getAttribute ("var")) {
				variables.Remove (qa);
			}
		}
			

		string key = action.getAttribute ("var");

//		Debug.Log ("trying to set var " + key);
		if (action.value != null) {

			if (key == "score" && action.value.num_value != null && action.value.num_value.Count > 0) {

				score = (int)action.value.num_value [0];

			} else if (action.value.bool_value != null && action.value.bool_value.Count > 0) {
				Debug.Log (key + " has bool value");
				variables.Add (new QuestVariable (key, action.value.bool_value [0]));
			} else if (action.value.num_value != null && action.value.num_value.Count > 0) {
				Debug.Log ("value found: " + action.value.num_value [0]);
				variables.Add (new QuestVariable (key, action.value.num_value [0]));
			} else if (action.value.string_value != null && action.value.string_value.Count > 0) {
				variables.Add (new QuestVariable (key, action.value.string_value [0]));
			} else if (action.value.var_value != null && action.value.var_value.Count > 0) {

				if (getVariable (action.value.var_value [0]) != null) {

					setVariable (key, getVariable (action.value.var_value [0]));

				} else {
				
					variables.Add (new QuestVariable (key, mathVariable (action.value.var_value [0])));


				}
			}

		}
	}

	public double mathVariable (string input)
	{




		double currentvalue = 0.0d;
		bool needsstartvalue = true;
		input = new string (input.ToCharArray ()
		                 .Where (c => !Char.IsWhiteSpace (c))
		                 .ToArray ());
		//	Debug.Log ("Rechnung:"+input);

		string arithmetics = "";


		foreach (Char c in input.ToCharArray()) {


			if (c == '+') {

				arithmetics = arithmetics + "+";
			}
			if (c == '-') {
				
				arithmetics = arithmetics + "-";
			}
			if (c == '*') {
				
				arithmetics = arithmetics + "*";
			}
			if (c == '/') {
				
				arithmetics = arithmetics + "/";
			}
			if (c == ':') {
				
				arithmetics = arithmetics + ":";
			}

		}


		
		//Debug.Log ("Rechnung:"+arithmetics);

		char[] splitter = "+-/*:".ToCharArray ();
		string[] splitted = input.Split (splitter);


		int count = 0;


		foreach (string s in splitted) {

			double n;
			bool isNumeric = double.TryParse (s, out n);
			if (isNumeric) {

				if (needsstartvalue) {

					currentvalue = n;
					needsstartvalue = false;
				} else {

					if (arithmetics.Substring (count, 1) == "+") {
						currentvalue += n;
					} else if (arithmetics.Substring (count, 1) == "-") {
						currentvalue -= n;
					} else if (arithmetics.Substring (count, 1) == "*") {
						currentvalue *= n;
					} else if ((arithmetics.Substring (count, 1) == "/") || (arithmetics.Substring (count, 1) == ":")) {

						currentvalue = currentvalue / n;
					} 


				}


			} else {

				QuestVariable qv = getVariable (s);
				if (!qv.isNull ()) {
					if (qv.num_value != null && qv.num_value.Count > 0) {
						if (needsstartvalue) {

							currentvalue = qv.num_value [0];
							Debug.Log (s + ":" + currentvalue.ToString ("F10"));

							needsstartvalue = false;
					
						} else {

							n = qv.num_value [0];

							Debug.Log (n);
							if (arithmetics.Substring (count, 1) == "+") {
								currentvalue += n;
							} else if (arithmetics.Substring (count, 1) == "-") {
								currentvalue -= n;
//								Debug.Log(currentvalue);
							} else if (arithmetics.Substring (count, 1) == "*") {
								currentvalue *= n;
							} else if ((arithmetics.Substring (count, 1) == "/") || (arithmetics.Substring (count, 1) == ":")) {
						
								currentvalue = currentvalue / n;
							}
							count += 1;

						}

					}
				}
			}


		}

		return currentvalue;
		
	}
	
	public void setVariable (string key, QuestVariable vari)
	{

		key = new string (key.ToCharArray ()
		                       .Where (c => !Char.IsWhiteSpace (c))
		                       .ToArray ());

		List<QuestVariable> helplist = new List<QuestVariable> ();
		foreach (QuestVariable qa in variables) {
			helplist.Add (qa);
		}
		
		foreach (QuestVariable qa in helplist) { 
			if (qa.key == key) {
				variables.Remove (qa);
			}
		}





		if (vari.bool_value != null && vari.bool_value.Count > 0) {
			//Debug.Log(key+" has bool value");
			variables.Add (new QuestVariable (key, vari.bool_value [0]));
		} else  
		if (vari.num_value != null && vari.num_value.Count > 0) {
			variables.Add (new QuestVariable (key, vari.num_value [0]));
			//Debug.Log(key+" has num value");

		} 
		if (vari.string_value != null && vari.string_value.Count > 0) {
			variables.Add (new QuestVariable (key, vari.string_value [0]));
			//Debug.Log(key+" has string value");

		} 
	}

	public QuestVariable getVariable (string k)
	{

		if (k != null) {



			string k2 = k;
			k = new string (k.ToCharArray ()
		                 .Where (c => !Char.IsWhiteSpace (c))
		                 .ToArray ());


			if (k.StartsWith ("date(")) {


				string d = k;
				d = d.Replace ("date(", "");
				d = d.Replace (")", "");

			
				if (getVariable (d).num_value != null) {

				
					double ergebnis = getVariable (d).num_value [0];
					Debug.Log (ergebnis);
					TimeSpan time = TimeSpan.FromSeconds (ergebnis);
				
					double seconds = time.Seconds;
					string seconds_str = seconds.ToString ();
				
					if (seconds < 10) {
						seconds_str = "0" + seconds;
					}
				
				
					double minutes = time.Minutes;
					string minutes_str = minutes.ToString ();
				
					if (minutes < 10) {
						minutes_str = "0" + (int)minutes;
					}
					if (minutes < 0) {
						minutes_str = "00";
					}
				
					int hours = time.Hours;
					string hours_str = hours.ToString ();
				
					if (hours < 10) {
						hours_str = "0" + hours;
					}
					if (hours < 0) {
						hours_str = "00";
					}
				
				
					int days = (int)time.TotalDays;
					string days_str = days.ToString ();
				
					if (days < 0) {
						days_str = "0";
					}
				
				
				
					string finaldate = "";
				
					if (days > 0) {
					
						finaldate = days_str + ":";
					
					}
				
					if (hours > 0) {
					
						finaldate = finaldate + "" + hours_str + ":";
					}
				
					finaldate = finaldate + "" + minutes_str + ":" + seconds_str;
				
				
				
					return new QuestVariable (k, finaldate);

				
				
			
				} else {

					return new QuestVariable (k, "[null]");

				}



			} else

		if (k.Contains ("+") || k.Contains ("-") || k.Contains ("*") || k.Contains (":") || k.Contains ("/")) {


				return new QuestVariable (k, mathVariable (k));


			} else if (k == "$date.now") {

				Debug.Log ("looking for date");
				DateTime Jan1St1970 = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

				double unixTime = ((DateTime.UtcNow - Jan1St1970).TotalSeconds);
				unixTime = Math.Round (unixTime, 0);
				return new QuestVariable ("$date.now", unixTime);



			} else if (k == "quest.name") {


				return new QuestVariable ("quest.name", questdb.currentquest.name);


			} else if (k == "score") {



				return new QuestVariable ("score", (float)score);

			
			
			
			} else if (k.StartsWith ("$_mission_") || k.StartsWith ("$_")) {


				k = k.Replace ("$_mission_", "");
				k = k.Replace ("$_", "");

				if (k.EndsWith (".result")) {

					k = k.Replace (".result", "");

					QuestPage qp = questdb.getPage (int.Parse (k));


					if (qp != null && qp.result != null && qp.result.Length > 0) {
						return new QuestVariable (k2, qp.result);
					} else if (Application.isWebPlayer) {

						return new QuestVariable (k2, "[MISSIONRESULT: " + k2 + " ]");
					
					} else {

						return new QuestVariable (k2, "");

					}

				} else if (k.EndsWith (".state")) {

					k = k.Replace (".state", "");
					QuestPage qp = questdb.getPage (int.Parse (k));



					if (qp != null && qp.state != null && qp.state.Length > 0) {
						return new QuestVariable (k2, qp.state);
					} else if (Application.isWebPlayer) {
						return new QuestVariable (k2, "[MISSIONSTATE: " + k2 + " ]");
					
					} else {
						return new QuestVariable (k2, "");


					}


				}



			} else {



				//Debug.Log("searching '"+k+"'");
				foreach (QuestVariable qa in variables) {
					//Debug.Log("found '"+qa.key+"'");
					if (qa.key == k) {

						return qa;
					}


				}

			}

		}

		questdb.debug ("Variable " + k + " wurde nicht gefunden.");
		return new QuestVariable (k, "[null]");


	}
	
	public void PlayNPCAudio (string path)
	{

		if (!path.EndsWith ("mp3")) {
			questdb.debug ("Audio-Datei muss im <b>MP3-Format</b> vorliegen.");
		} else {
			
			questdb.debug ("Audio-Datei (nur mobile): " + path);
			
		}
		if (npcaudio != null) {
			npcaudio.Stop ();
			Destroy (npcaudio.gameObject);
		}

		questaudio nqa = (questaudio)Instantiate (audio_prefab, transform.position, Quaternion.identity);
		nqa.transform.parent = questdb.currentquestdata.transform;

				
		npcaudio = nqa;

		string pre = "file: /";
		
		
		
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			
			pre = "file:";
		}

		string url = path;
		if (!url.StartsWith ("http:") && !url.StartsWith ("https:")) {
			url = pre + "" + path;
		}
		
		www = new WWW (url);

		StartCoroutine (waitforNPCAudio (nqa));
		

	}

	void PlayAudio (QuestAction action)
	{




		if (action.getAttribute ("file").StartsWith ("@_")) {

			foreach (QuestRuntimeAsset qra in  audioclips) {





				if (qra.key == action.getAttribute ("file")) {

					questaudio nqa1 = (questaudio)Instantiate (audio_prefab, transform.position, Quaternion.identity);
					nqa1.setAudio (qra.clip);
					nqa1.Play ();





				}

			}





		} else 
		if (!action.getAttribute ("file").EndsWith ("mp3")) {

			questdb.debug ("Audio-Datei muss im <b>MP3-Format</b> vorliegen.");
		} else {

			questdb.debug ("Audio-Datei (nur mobile): " + action.getAttribute ("file"));

		}
		questaudio nqa = (questaudio)Instantiate (audio_prefab, transform.position, Quaternion.identity);


		questdb.debug ("Stop other Audio? ->" + action.getAttribute ("stopOthers"));
		if (action.hasAttribute ("stopOthers") && action.getAttribute ("stopOthers") == "1") {

			questdb.debug ("stopping all sounds");
			foreach (questaudio qa in questaudiosources) {
				if (qa == null)
					continue;
				qa.Stop ();
				if (qa.gameObject != null)
					Destroy (qa.gameObject);

			}
			questaudiosources.Clear ();

		}



		questaudiosources.Add (nqa);

		Debug.Log ("nqa:" + nqa);
		Debug.Log ("nqa.transform:" + nqa.transform);
		Debug.Log ("questdb:" + questdb);
		Debug.Log ("questdb.currentquestdata:" + questdb.currentquestdata);

		nqa.transform.parent = questdb.currentquestdata.transform;
		bool looping = false;
		
		
		if (action.getAttribute ("loop") == "1") {
			looping = true;
		} else {
			looping = false;
		}
		//Debug.Log (looping);
		nqa.setLoop (looping);
		string pre = "file: /";
		
		
		
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			
			pre = "file:";
		}



		string url = action.getAttribute ("file");
		if (!url.StartsWith ("http:") && !url.StartsWith ("https:")) {
			url = pre + "" + action.getAttribute ("file");
		}
		
		www = new WWW (url);


		StartCoroutine (waitforAudio (nqa));


	}

	void showmessage (QuestAction action)
	{

		if (action.hasAttribute ("message")) {
			QuestMessage nqa = (QuestMessage)Instantiate (message_prefab, transform.position, Quaternion.identity);
		
			nqa.message = action.getAttribute ("message");

			if (action.hasAttribute ("buttontext") && action.getAttribute ("buttontext").Length > 0) {
				nqa.setButtonText (action.getAttribute ("buttontext"));
			}
			nqa.transform.SetParent (msgcanvas.transform, false);
			nqa.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);
		}
		
	}

	IEnumerator waitforAudio (questaudio audios)
	{
		
		yield return www;

		if (www.isDone) {
			if (www.error == null) {

				audios.setAudio (www.GetAudioClip (false));
				audios.Play ();
			} else {
				Debug.Log (www.error);
			}
		} else {
			StartCoroutine (waitforAudio (audios));
		}
	}

	IEnumerator waitforNPCAudio (questaudio audios)
	{
		
		yield return www;
		
		if (www.error == null) {
			audios.setAudio (www.audioClip);
			audios.Play ();
		} else {
			Debug.Log (www.error);
		}
		
	}






}

[System.Serializable]
public class QuestVariable
{


	public string key;
	public string type;
	public List<string> string_value;
	public List<double> num_value;
	public List<bool> bool_value;
	public List<double> double_value;

	public QuestVariable (string k, string s)
	{
		string_value = new List<string> ();
		string_value.Add (s);
		type = "string";
		key = k;
	}

	public QuestVariable (string k, double n)
	{

		num_value = new List<double> ();
		num_value.Add (n);
		type = "num";
		key = k;
	}


	public QuestVariable (string k, bool b)
	{
		bool_value = new List<bool> ();
		bool_value.Add (b);
		type = "bool";
		key = k;
	}

	public string getStringValue ()
	{

		if (string_value != null && string_value.Count > 0) {
			return string_value [0];
		} else if (bool_value != null && bool_value.Count > 0) {
			if (bool_value [0]) {
				return "true";
			} else {
				return "false";
			}
		} else if (num_value != null && num_value.Count > 0) {

			return num_value [0] + "";

		} else {

			return null;

		}


	}


	public string ToString ()
	{

		if (type == "bool") {

			return bool_value [0].ToString ();
		} else if (type == "num") {
			
			return num_value [0].ToString ();
		} else if (type == "string") {
			
			return string_value [0];
		} 

		return "";


	}


	public bool isNull ()
	{

		if (string_value != null && string_value.Count > 0) {

			if (string_value [0] == "[null]") {

				return true;
			} else {

				return false;

			}


		} else {

			return false;
		}

	}

	public double getNumValue ()
	{


		if (num_value != null && num_value.Count > 0) {
			
			return num_value [0];
			
		}

		return 0f;

	}
	
	
}

public class QuestRuntimeAsset
{

	public Texture2D texture;
	public AudioClip clip;
	public string key;

	public QuestRuntimeAsset (string k, Texture2D t2d)
	{
		key = k;
		texture = t2d;
	}

	public QuestRuntimeAsset (string k, AudioClip aclip)
	{
		key = k;
		clip = aclip;
	}



}