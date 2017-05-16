using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine.Networking;
using GQ.Client.Net;
using GQ.Client.Conf;
using QM.NFC;
using GQ.Client.Util;
using GQ.Client.Model;

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
	// TODO move to Variables into Instance
	public Dictionary<string, QuestVariable> variables;
	public List<questaudio> questaudiosources;
	public questaudio npcaudio;
	public List<AudioSource> audiosources;
	public List<QuestRuntimeAsset> photos;
	public List<QuestRuntimeAsset> audioclips;
	public networkactions networkActionsObject;
	private QuestAction gpsRoute;
	private bool updateGPSRoute = false;
	public float gPSRouteUpdateInterval = 10f;
	private float gPSRouteUpdateInterval_save = 10f;
	public bool test;

	public string lastServerIp;

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
		variables = new Dictionary<string, QuestVariable> ();
		questaudiosources = new List<questaudio> ();
		photos = new List<QuestRuntimeAsset> ();
		audioclips = new List<QuestRuntimeAsset> ();

	}

	public void setNetworkIdentity (networkactions na)
	{

		networkActionsObject = na;

	}

	public void reset ()
	{

		variables = new Dictionary<string, QuestVariable> ();
		questaudiosources = new List<questaudio> ();
		photos = new List<QuestRuntimeAsset> ();

		loopcount = 0;
		score = 0;

	}

	public void sendVartoWeb ()
	{

		if (Application.isWebPlayer) {

			string myvars = "";
			if (variables != null && variables.Count > 0) {
				foreach (QuestVariable qv in variables.Values) {

				
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
		} else if (action.type == "PlayAudio") {
			PlayAudio (action);
		} else if (action.type == "SetVariable") {
			setVariable (action);
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
			string varName = action.getAttribute ("var");
			QuestVariable questVar = Variables.LoadVariableFromStore (varName);
			if (questVar != null) {
				variables [questVar.key] = questVar;
			}
		} else if (action.type == "LoadVariable") {
			string varName = action.getAttribute ("var");
			QuestVariable questVar = Variables.LoadVariableFromStore (varName);
			if (questVar != null) {
				variables [questVar.key] = questVar;
				sendVartoWeb ();
			}
		} else if (action.type == "SaveVar") {
			QuestVariable qv = getVariable (action.getAttribute ("var"));
			Variables.SaveVariableToStore (qv);
		} else if (action.type == "ShowVar") {
			showVariableOverlay (action);
		} else if (action.type == "HideVar") {
			hideVariableOverlay (action);
		} else if (action.type == "StartQuest") {
			startQuest (action);
		} else if (action.type == "SendVarToServer") {
			sendVarToServer (action);
		} else if (action.type == "ParseVariables") {
			parseVariable (action);
		} else if (action.type == "WriteToNFC") {
			writeToNFC (action);
		}
		
		
	}

	public void startQuest (QuestAction action)
	{

		if (!action.hasAttribute ("quest"))
			return;
				
		int questID;

		try {
			questID = int.Parse (action.getAttribute ("quest"));
		} catch (Exception exc) {
			QuestVariable questVar = getVariable (action.getAttribute ("quest"));
			if (questVar != null) {

				questID = (int)questVar.getNumValue ();
			} else {
				return;
			}
		}

		questdb.StartQuest (questID);
	}

	public string getServerIp (string s)
	{

		//lastServerIp = PlayerPrefs.GetString ("lastServerIp");

		if (s != null && s.Length > 2) {

			lastServerIp = s;
			//	PlayerPrefs.SetString ("lastServerIp", lastServerIp);

			Debug.Log ("getIP: " + s);
			return s;

		} else if (lastServerIp != null && lastServerIp.Length > 2) {

			Debug.Log ("getIP: " + lastServerIp);
			return lastServerIp;

		}


		lastServerIp = "192.168.178.67";
		//	PlayerPrefs.SetString ("lastServerIp", lastServerIp);

		Debug.Log ("getIP: " + lastServerIp);
		return lastServerIp;

	}

	void addFileBytesToSendQueue (QuestAction action, string filetype, List<byte[]> sendbytes)
	{
		string ip = getServerIp (action.getAttribute ("ip"));
		string var = action.getAttribute ("var");

		if (sendbytes == null || sendbytes.Count <= 0)
			return;

		// Start part:
		GetComponent<ConnectionClient> ().addFileMessage (ip, var, filetype, sendbytes [0], 0,
			questdb.currentquest.Id);

		// Middle Parts:
		for (int i = 1; i < sendbytes.Count; i++) {
			GetComponent<ConnectionClient> ().addFileMessage (ip, var, filetype, sendbytes [i], i,
				questdb.currentquest.Id);
		}

		// FINISH Part:
		GetComponent<ConnectionClient> ().addFileFinishMessage (ip, var, filetype,
			questdb.currentquest.Id);
	}


	void sendVarToServer (QuestAction action)
	{

		bool doit = true;

		if (Configuration.instance.showMessageForDatasendAction && !questdb.currentquest.acceptedDS) {

			doit = false;

		}

		if (doit) {

			if (action.hasAttribute ("var")) {
				
				if (getVariable (action.getAttribute ("var")).getStringValue () != "[null]") {
					// Cases String Bool Number variables (has string representation):
					GetComponent<ConnectionClient> ().addTextMessage (
						getServerIp (action.getAttribute ("ip")), 
						action.getAttribute ("var"), 
						getVariable (action.getAttribute ("var")).getStringValue (),
						questdb.currentquest.Id);
				} else {
					bool filefound = false;
					string deviceid = SystemInfo.deviceUniqueIdentifier;
				
					List<byte> filebytes = new List<byte> ();
				
					// PHOTOS

					string filetype = "image/jpg";
				
					QuestRuntimeAsset qra = null;
				
					foreach (QuestRuntimeAsset qrat in photos) {
					
						if (!filefound && qrat.key == action.getAttribute ("var")) {
							filefound = true;
							qra = qrat;
						
						}
					}
				
					if (filefound) {
					
						filebytes = qra.texture.EncodeToJPG (90).ToList ();
					
					} else {
						filetype = "audio";
					
						foreach (QuestRuntimeAsset qrat in audioclips) {
						
							if (!filefound && qrat.key == action.getAttribute ("var")) {
								filefound = true;
								qra = qrat;
							
							}
						}
					
						if (filefound) {
						
							int length = qra.clip.samples * qra.clip.channels;
							var samples = new float[length];
							qra.clip.GetData (samples, 0);
						
							int length2 = samples.Count () * 4;
							var filebytes2 = new byte[length2];
							Buffer.BlockCopy (samples, 0, filebytes2, 0, filebytes2.Count ());
						
							filebytes = filebytes2.ToList ();
						}
					}

					List<byte[]> sendbytes = SendQueueHelper.prepareToSend (filebytes);

					// jetzt ist die datei in byte arrays zerlegt (liegen in sendbytes)

					addFileBytesToSendQueue (action, filetype, sendbytes);
			
				
					if (!filefound) {
						Debug.Log ("var not found");
						questdb.debug ("Die Variable " + action.getAttribute ("var") + " konnte nicht gefunden werden.");
					
					}
				}
			}
		}
	}

	/// <summary>
	/// Writes the content as payload to an NFC chip. The content can conatin variables etc. that will be replaced.
	/// </summary>
	/// <param name="action">Action.</param>
	void writeToNFC (QuestAction action)
	{
		string payload = TextHelper.makeReplacements (action.getAttribute ("content"));

		if (payload.Equals ("[null]"))
			return;

		// TODO woher kommen die umgebenden Anfürhungszeichen beim NFC Spiel in München? (hm)

		if (payload.StartsWith ("\"") && payload.EndsWith ("\"")) {
			payload = payload.Substring (1, payload.Length - 2);
		}

		NFC_Connector.Connector.NFCWrite (payload);
	}

	/// <summary>
	/// Parses the variable in case it has a canonical key-value form like this: key:value,key:value,... 
	/// into separate variables with the keys as names and the values as values.
	/// 
	/// KEYS MUST ONLY CONTAIN A SINGLE ALPHANUMERIC CHARACTER.
	/// 
	/// VALUE MUST MASK COMMAS by doubling them.
	/// 
	/// If the given FromVar varibale contains something else, nothing happens.
	/// </summary>
	/// <param name="action">Action.</param>
	void parseVariable (QuestAction action)
	{
		const char DELIMITER = ',';

		string rawValue = getVariable (action.getAttribute ("FromVar")).getStringValue ();

		if (rawValue == null || rawValue.Equals ("[null]")) {
			return;
		}

		char[] receivedChars = rawValue.ToCharArray ();

		int curIndex = 0;
		char key = '\0';
		System.Text.StringBuilder valueBuilder;

		while (curIndex <= receivedChars.Length - 2) {
			// index must leave a rest of at least two chars: the key and the ':'

			// parse key:
			key = receivedChars [curIndex];
			curIndex += 2; // skip the key char and the ':'

			// parse value:
			valueBuilder = new System.Text.StringBuilder ();

			while (curIndex <= receivedChars.Length - 1) {

				if (receivedChars [curIndex] != DELIMITER) {
					// ordinary content:
					valueBuilder.Append (receivedChars [curIndex]);
					curIndex++;
					continue; // do NOT store key-value but proceed to gather the value
				}

				if (curIndex == receivedChars.Length - 1) {
					// the current is a ',' and the last in the array we interpret it as an empty value
					// e.g. [i:,] => id = ""
					// we proceed one char and do not have to do anything since the while loop will terminate
					curIndex++;
				} else {
					// now we look at the char after the first ',':
					curIndex++;

					if (receivedChars [curIndex] == DELIMITER) {
						// we found a double ',,' which is just a ',' within the content
						// hence we add just one ',' ignore the second and go on parsing the value further
						// e.g. [p:me,, you and him] -> payload = "me, you and him"
						valueBuilder.Append (receivedChars [curIndex]);
						curIndex++; // remember this is the second increase!
						continue; // ready to step one char further
					} else {
						// we found a single ',' which signifies the end of the value
						// we keep the index pointing at the next key and finish with this value
						// e.g. [i:123,p:hello] -> id = "123"; payload = "hello"
						break; // leaving value gathering and go on with next key-value pair
					}
				}
			}
			// end of KV pair reached: store it
			// if value can be parsed as number we store it as number typed var:
			string stringValue = valueBuilder.ToString ();
			double numValue;
			if (Double.TryParse (stringValue, out numValue)) {
				setVariable (key.ToString (), new QuestVariable (key.ToString (), numValue));
			} else {
				setVariable (key.ToString (), new QuestVariable (key.ToString (), stringValue));
			}
		}

	}

	IEnumerator waitForClientStart (QuestAction action, int tries)
	{
		tries += 1;
		Debug.Log ("waiting for client to start");


		yield return null;

		if (networkActionsObject != null) {

			Debug.Log ("waiting for client to start #2");

			if (getVariable (action.getAttribute ("var")).getStringValue () != "[null]") {


				Debug.Log ("var send succesfully");
				string deviceid = SystemInfo.deviceUniqueIdentifier;
				//networkActionsObject.CmdSendVar(deviceid,action.getAttribute("var"),getVariable(action.getAttribute("var")).getStringValue());


				yield return new WaitForSeconds (10f);
				NetworkManager.singleton.StopClient ();


			} else {



				bool filefound = false;
				string deviceid = SystemInfo.deviceUniqueIdentifier;



				// PHOTOS


				List<byte> filebytes = new List<byte> ();

				string filetype = "image/jpg";

				QuestRuntimeAsset qra = null;

				foreach (QuestRuntimeAsset qrat in photos) {
					
					if (!filefound && qrat.key == action.getAttribute ("var")) {
						filefound = true;
						qra = qrat;

					}
						
						
				}
					


				if (filefound) {

					filebytes = qra.texture.EncodeToJPG (90).ToList ();

				} else {



					filetype = "audio";
					

					foreach (QuestRuntimeAsset qrat in audioclips) {
						
						if (!filefound && qrat.key == action.getAttribute ("var")) {
							filefound = true;
							qra = qrat;
							
						}
						
						
					}


					if (filefound) {

						int length = qra.clip.samples * qra.clip.channels;
						var samples = new float[length];
						qra.clip.GetData (samples, 0);

						int length2 = samples.Count () * 4;
						var filebytes2 = new byte[length2];
						Buffer.BlockCopy (samples, 0, filebytes2, 0, filebytes2.Count ());
						

						filebytes = filebytes2.ToList ();



					}





				}




				Debug.Log (filebytes.Count);

				int size = 1300;



				List<byte[]> sendbytes = new List<byte[]> ();

				for (int i = 0; i < filebytes.Count; i += size) {
					var list = new List<byte> ();

					if ((i + size) > filebytes.Count) {
						Debug.Log ("last: " + i + "*" + size + "=" + (i * size));
						size = filebytes.Count - (i);
						Debug.Log (size);
					}

					list.AddRange (filebytes.GetRange (i, size));
					sendbytes.Add (list.ToArray ());
				}


				//networkActionsObject.CmdSendFile(deviceid,action.getAttribute("var"),filetype,sendbytes[0]);
				int y = sendbytes [0].Count ();
				Debug.Log ("send chunk #1: " + y);


				yield return new WaitForEndOfFrame ();


				int k = 1;

				foreach (byte[] b in sendbytes) {

					if (k <= sendbytes.Count) {
					
						if (k > 1) {
							//networkActionsObject.CmdAddToFile(deviceid,action.getAttribute("var"),filetype,b);

							int x = b.Count ();
							Debug.Log ("send chunk #" + k + ": " + x);

						
							if (k % 150 == 0) {
							
								yield return new WaitForSeconds (2f);
							}

						}

						k++;
					}
				}



				//			networkActionsObject.CmdFinishFile(deviceid,action.getAttribute("var"),filetype);

				Debug.Log ("finish File");

						
			

					
					

				




				if (!filefound) {
					Debug.Log ("var not found");
					questdb.debug ("Die Variable " + action.getAttribute ("var") + " konnte nicht gefunden werden.");

				}


			}
				

		} else {
			yield return new WaitForSeconds (0.2f);

			if (tries > 20) {
				sendVarToServer (action);
			} else {
				StartCoroutine (waitForClientStart (action, tries));
			}
		}




	}

	public void addPhoto (QuestRuntimeAsset tqra)
	{



		List<QuestRuntimeAsset> temp = new List<QuestRuntimeAsset> ();


		foreach (QuestRuntimeAsset qra in photos) {
		
			temp.Add (qra);
		
		
		}


		foreach (QuestRuntimeAsset qra in temp) {
			
			if (qra.key == tqra.key) {
				Debug.Log ("removed old one");
				photos.Remove (qra);

			}
			
			
		}

		photos.Add (tqra);

	
	
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

				





					string url = "http://qeevee.org:9091/routes/gosmore.php-" +
					             "format=kml" +
					             "_flat=" + lon1 +
					             "_flon=" + lat1 +
					             "_tlat=" + lon2 +
					             "_tlon=" + lat2 +
					             "_v=foot" +
					             "_fast=1" +
					             "_layer=mapnik" +
					             "_instructions=1" +
					             "_lang=de";

					Debug.Log (url);
					WWW routewww = new WWW (url);

					StartCoroutine (waitForRouteFile (routewww));



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

				Debug.Log ("doing new Route");


				string[] coordinates = routefile.Split (new string[] {
					Environment.NewLine
				}, StringSplitOptions.None);

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

		if (action.hasAttribute ("allowReturn")) {
			quest.AllowReturn = action.getBoolAttribute ("allowReturn");
		} else {
			quest.AllowReturn = false;
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

		if (GameObject.Find ("PageController_Map") != null) {
			GameObject.Find ("PageController_Map").GetComponent<page_map> ().updateMapMarkerInQuest ();
			
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


			string varName = action.getAttribute ("var");
			QuestVariable qv = getVariable (varName);

			if (qv.type == "num") {

				double value = qv.getNumValue ();

				value += 1;

				variables [varName] = new QuestVariable (varName, value);

			}

		}

	}

	public void decvar (QuestAction action)
	{
		if (action.hasAttribute ("var")) {
			
			
			string varName = action.getAttribute ("var");
			QuestVariable qv = getVariable (varName);
			
			if (qv.type == "num") {
				
				double value = qv.getNumValue ();
				
				value -= 1;

				variables [varName] = new QuestVariable (varName, value);

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
		#if UNITY_IPHONE || UNITY_WP8 || UNITY_ANDROID || UNITY_BLACKBERRY
		Handheld.Vibrate ();
# else 
		Debug.Log("cannot vibrate on web");
		


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
					for (int count = from; count <= to && loopcount >= loopnumber; count++) {
						setVariable (var, (float)count);
						action.InvokeThen ();

					}
				} else {

					for (int count = from; count >= to && loopcount >= loopnumber; count--) {
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


	public void localizeStringToDictionary (string s)
	{
		
		//TODO: for more than DE and EN
		

		string german = "";
		string english = "";

		
		if (s.Contains ("[---DE---]")) {
			
			
			string help = s.Substring (s.IndexOf ("[---DE---]") + ("[---DE---]").Length);
			
			german = help;
			
			if (help.IndexOf ("[---") != -1) {
				german = help.Substring (0, help.IndexOf ("[---"));
				
			}

			
		}


		if (s.IndexOf ("[---EN---]") != -1) {
			// use english
			string help = s.Substring (s.IndexOf ("[---EN---]") + ("[---EN---]").Length);
				
			english = help;
				
			if (help.IndexOf ("[---") != -1) {
				english = help.Substring (0, help.IndexOf ("[---"));
					
			}
				
				

			
		}
		
		

//		Debug.Log("new dictionary entry: " + german + "," + english);
		
		GetComponent<Dictionary> ().translations.Add (new Translation (german, english));
		
		
	}

	public String localizeString (string s)
	{

//		Debug.Log (s);



		string lang = GetComponent<Dictionary> ().language;


		if (s.Contains ("[---" + lang.ToUpper () + "---]")) {


			string help = s.Substring (s.IndexOf ("[---" + lang.ToUpper () + "---]") + ("[---" + lang.ToUpper () + "---]").Length);

			string final = help;

			if (help.IndexOf ("[---") != -1) {
				final = help.Substring (0, help.IndexOf ("[---"));

			}
			//Debug.Log (final);
			return final;

		} else {

			// use either english or s

			if (s.IndexOf ("[---EN---]") != -1) {
				// use english
				string help = s.Substring (s.IndexOf ("[---EN---]") + ("[---EN---]").Length);
				
				string final = help;
				
				if (help.IndexOf ("[---") != -1) {
					final = help.Substring (0, help.IndexOf ("[---"));
					
				}
				
				return final;

			} else {
				// no fallback language defined

				return s;
			}

		}







	}

	public string formatString (string s)
	{
		if (s == null) {
			return "";
		}




		string k = "";
		if (s.Contains ("@")) {


			bool lastwasvar = true;


			char[] splitter = "@".ToCharArray ();

			string[] splitted = s.Split (splitter);

			int c1 = 1;

			foreach (string x in splitted) {






//				Debug.Log (x);

				string x2 = new string (x.ToCharArray ()
					                 .Where (c => !Char.IsWhiteSpace (c))
					                 .ToArray ());

//				Debug.Log (x2);

				if (x.Equals (x2)) {



					if (x2.Equals ("score")) {

						k += score.ToString ();
						lastwasvar = true;

					} else if (!getVariable (x2).getStringValue ().Equals ("[null]")) {

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

	//	void removeVariable (string key) {
	//
	//
	//		List<QuestVariable> helplist = new List<QuestVariable>();
	//		foreach ( QuestVariable qa in variables ) {
	//			helplist.Add(qa);
	//		}
	//
	//		foreach ( QuestVariable qa in helplist ) {
	//			if ( qa.key == key ) {
	//				variables.Remove(qa);
	//			}
	//		}
	//
	//
	//	}

	public void setVariable (string key, float f)
	{

		variables [key] = new QuestVariable (key, f);
	}

	public void setVariable (string key, string s)
	{
			
		variables [key] = new QuestVariable (key, s);
	}

	public void setVariable (string key, bool b)
	{
				
		variables [key] = new QuestVariable (key, b);
	}

	public void setVariable (QuestAction action)
	{

		string key = action.getAttribute ("var");

		if (action.value != null) {

			if (key == "score" && action.value.num_value != null && action.value.num_value.Count > 0) {

				score = (int)action.value.num_value [0];
			} else if (action.value.bool_value != null && action.value.bool_value.Count > 0) {
				variables [key] = new QuestVariable (key, action.value.bool_value [0]);
			} else if (action.value.num_value != null && action.value.num_value.Count > 0) {
				variables [key] = new QuestVariable (key, action.value.num_value [0]);
			} else if (action.value.string_value != null && action.value.string_value.Count > 0) {
				string unformattedContent = action.value.string_value [0];
				string formattedContent = TextHelper.makeReplacements (unformattedContent);
				variables [key] = new QuestVariable (key, formattedContent);
			} else if (action.value.var_value != null && action.value.var_value.Count > 0) {

				if (getVariable (action.value.var_value [0]) != null) {

					setVariable (key, getVariable (action.value.var_value [0]));

				} else {

					variables [key] = new QuestVariable (key, mathVariable (action.value.var_value [0]));
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
							needsstartvalue = false;
						} else {
							n = qv.num_value [0];

							if (arithmetics.Substring (count, 1) == "+") {
								currentvalue += n;
							} else if (arithmetics.Substring (count, 1) == "-") {
								currentvalue -= n;
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

		if (vari.bool_value != null && vari.bool_value.Count > 0) {
			variables [key] = new QuestVariable (key, vari.bool_value [0]);
		} else if (vari.num_value != null && vari.num_value.Count > 0) {
			variables [key] = new QuestVariable (key, vari.num_value [0]);
		} 
		if (vari.string_value != null && vari.string_value.Count > 0) {
			variables [key] = new QuestVariable (key, vari.string_value [0]);
		} 
	}

	public QuestVariable getVariable (string varName)
	{
		Debug.Log ("looking up var: " + varName);

		string originalVarName = varName;

		if (varName == null) {
			questdb.debug ("Variable " + varName + " wurde nicht gefunden.");
			return new QuestVariable (varName, "[null]");
		}


		string k2 = varName;
		varName = new string (varName.ToCharArray ()
		                 .Where (c => !Char.IsWhiteSpace (c))
		                 .ToArray ());


		if (varName.StartsWith ("date(")) {

			return getDateVariable (varName);
		}

		// XML Tags filtered out with "<" and ">"
		if (!(varName.Contains ("<") && varName.Contains (">")) && (varName.Contains ("+") || varName.Contains ("-") || varName.Contains ("*") || varName.Contains (":") || varName.Contains ("/"))) {

			return new QuestVariable (varName, mathVariable (varName));
		}

		if (varName == "$date.now") {

			// Debug.Log ("looking for date");
			DateTime Jan1St1970 = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			double unixTime = ((DateTime.UtcNow - Jan1St1970).TotalSeconds);
			unixTime = Math.Round (unixTime, 0);
			return new QuestVariable ("$date.now", unixTime);
		}

		if (varName == "quest.name") {

			return new QuestVariable ("quest.name", questdb.currentquest.Name);
		}

		if (varName == "score") {

			return new QuestVariable ("score", (float)score);
		}

		if (varName.StartsWith ("$_mission_") || varName.StartsWith ("$_")) {

			varName = varName.Replace ("$_mission_", "");
			varName = varName.Replace ("$_", "");

			if (varName.EndsWith (".result")) {

				varName = varName.Replace (".result", "");

				Page qp = questdb.getPage (int.Parse (varName));

				if (qp != null && qp.result != null && qp.result.Length > 0) {
					return new QuestVariable (k2, qp.result);
				}
				if (Application.isWebPlayer) {

					return new QuestVariable (k2, "[MISSIONRESULT: " + k2 + " ]");
				}

				return new QuestVariable (k2, "");
			}

			if (varName.EndsWith (".state")) {

				varName = varName.Replace (".state", "");
				Page qp = questdb.getPage (int.Parse (varName));

				if (qp != null && qp.state != null && qp.state.Length > 0) {
					return new QuestVariable (k2, qp.state);
				}

				if (Application.isWebPlayer) {
					return new QuestVariable (k2, "[MISSIONSTATE: " + k2 + " ]");
					
				}
				return new QuestVariable (k2, "");
			}

			Debug.LogWarning ("Unknown mission variable type: " + originalVarName);
			return null;
		}

		// Location:
		if (varName.Equals ("$location.lat") || varName.Equals ("$location.long")) {
			if (Input.location.status != LocationServiceStatus.Running) {
				Debug.LogWarning ("Location DETECTION not Running!");
				return new QuestVariable (varName, 0d);
			} else {
				if (varName.Equals ("$location.lat")) {			
					Debug.LogWarning ("$location.lat: " + Input.location.lastData.latitude);
					return new QuestVariable ("$location.lat", Convert.ToString (Input.location.lastData.latitude));
				}
				if (varName.Equals ("$location.long")) {
					Debug.LogWarning ("$location.long: " + Input.location.lastData.longitude);
					return new QuestVariable ("$location.long", Convert.ToString (Input.location.lastData.longitude));
				}
				Debug.LogWarning ("Unknown location system varibale: " + varName);
				return new QuestVariable (varName, "[null]");
			}
		}

		// STANDARD CASE:
		QuestVariable resultVar;
		if (variables.TryGetValue (varName, out resultVar)) {
			Debug.Log ("getVariable(" + varName + ") --> " + resultVar.ToString ());
			return resultVar;
		} else {
			resultVar = new QuestVariable (varName, "[null]");

			questdb.debug ("Variable " + varName + " wurde nicht gefunden.");
			return new QuestVariable (varName, "[null]");

		}
	}

	private QuestVariable getDateVariable (string varName)
	{

		string d = varName;
		d = d.Replace ("date(", "");
		d = d.Replace (")", "");

		if (getVariable (d).num_value != null) {

			double ergebnis = getVariable (d).num_value [0];
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

			return new QuestVariable (varName, finaldate);
		} else {

			return new QuestVariable (varName, "[null]");
		}
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





		} else if (!action.getAttribute ("file").EndsWith ("mp3")) {

			questdb.debug ("Audio-Datei muss im <b>MP3-Format</b> vorliegen.");
		} else {

			questdb.debug ("Audio-Datei (nur mobile): " + action.getAttribute ("file"));

		}
		questaudio nqa = (questaudio)Instantiate (audio_prefab, transform.position, Quaternion.identity);


		questdb.debug ("Stop other Audio? ->" + action.getAttribute ("stopOthers"));
		if (action.hasAttribute ("stopOthers") && action.getAttribute ("stopOthers") == "1") {

			questdb.debug ("stopping all sounds");
			foreach (questaudio qa in questaudiosources) {
				if (qa == null) {
					continue;
				}
				qa.Stop ();
				if (qa.gameObject != null) {
					Destroy (qa.gameObject);
				}

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

		if (msgcanvas == null) {
			Debug.LogWarning ("Action 'showMessage' can not execute, because the MessageCanvas is null.");
		}

		if (action.hasAttribute ("message")) {
			QuestMessage nqa = (QuestMessage)Instantiate (message_prefab, transform.position, Quaternion.identity);
		
			nqa.message = formatString (action.getAttribute ("message"));

			if (action.hasAttribute ("buttontext") && action.getAttribute ("buttontext").Length > 0) {
				nqa.setButtonText (action.getAttribute ("buttontext"));
			}
			nqa.transform.SetParent (msgcanvas.transform, false);
			nqa.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);
		}
		
	}

	IEnumerator waitforAudio (questaudio audios)
	{
		
		while (!www.isDone) {
			yield return null;
		}

		if (www.error == null) {
			audios.setAudio (www.GetAudioClip (false));
			audios.Play ();
		} else {
			Debug.Log ("geoquest audio error: " + www.error);
		}
	}

	IEnumerator waitforNPCAudio (questaudio audios)
	{
		
		yield return www;
		
		if (www.error == null) {
			audios.setAudio (www.GetAudioClip (false));
			audios.Play ();
		} else {
			Debug.Log ("geoquest audio error: " + www.error);
		}
		
	}
}