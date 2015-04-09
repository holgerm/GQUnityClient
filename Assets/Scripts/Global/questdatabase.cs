using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System;
using System.Linq;
using System.Text;

public class questdatabase : MonoBehaviour
{
	public int startingQuest;
	public bool newxml = true;
	public Quest currentquest;
	public Transform questdataprefab;
	public Transform currentquestdata;
	public List<Quest> allquests;
	public List<Quest> localquests;
	private List<int> predeployedQuests;
	private WWW www;
	public List<WWW> filedownloads;
	public Text downloadmsg;
	public Image publicquestslist;
	public actions actioncontroller;
	public QuestMessage message_prefab;
	public List<QuestRuntimeHotspot> hotspots;
	public Image questmilllogo;
	public Text webloadingmessage;
	public List<String> loadedfiles;
	public string webxml;
	public bool fixedposition = true;

	ScreenOrientation originalOrientation = ScreenOrientation.Portrait;


	void Start ()
	{
		//AllowAutoRotation (false);


		if (GameObject.Find ("QuestDatabase") != gameObject) {
			Destroy (gameObject);		
		} else {
			DontDestroyOnLoad (gameObject);
			Debug.Log (Application.persistentDataPath);
		}


		if (Application.isWebPlayer) {
			webloadingmessage.enabled = true;
			questmilllogo.enabled = true;
		}

		// collect local quests in list
		localquests = GetLocalQuests ();

		// TODO: filter predeployed but missing quests and copy them to the right place (emulate loading them)
		InitPredeployedQuests ();

		if (startingQuest != 0) {
			StartQuest ();
		}

		// TODO: if no starting quest is given show foyer lists


	}

	void InitPredeployedQuests ()
	{
		// TODO: get path to folder where predeployed quests are stored

		// TODO: find all predeployed quest ids

		// TODO: for each predeployed quest check if it is not already initialized
		//		 if not initialize it, i.e. copy it to the right place

		throw new NotImplementedException ();
	}

	void StartQuest ()
	{
		// TODO: if a starting quest is given try to start it. first locally, otherwise download it and start it

		webloadingmessage.enabled = true;
		questmilllogo.enabled = true;
		bool connected = CheckConnection ();
		if (connected) {
			webloadingmessage.text = "Downloading content ...";
			downloadQuest (startingQuest);
		}
		else {
			webloadingmessage.text = "You need to be connected to the internet!";
		}
	}

	bool CheckConnection ()
	{
		WWW www = new WWW ("http://www.google.com");
		while (!www.isDone) {
		}

		return (www.responseHeaders.Count () > 0);
	}
	
	public List<QuestRuntimeHotspot> getActiveHotspots ()
	{
		List<QuestRuntimeHotspot> activehs = new List<QuestRuntimeHotspot> ();
		
		foreach (QuestRuntimeHotspot qrh in hotspots) {



			if (qrh.active) {

				activehs.Add (qrh);
			}


		}


		return activehs;
	}

	void Update ()
	{



		if (Input.GetKey (KeyCode.G) && Input.GetKey (KeyCode.E) && Input.GetKey (KeyCode.O) && Input.GetKey (KeyCode.Q)) {
			Debug.Log ("Destroying GameObject");

			Destroy (gameObject);
			endQuest ();
		

		}

	}

	public void debug (string s)
	{
		
		Debug.Log (s);
		if (Application.isWebPlayer) {
			

				
			Application.ExternalCall ("unitydebug", s + "<br/><br/>");
				

		}
		
	}

	public void passWebXml (string x)
	{


		if (currentquestdata != null) {

			Destroy (currentquestdata.gameObject);
						
					
				
		} 

		actioncontroller.reset ();
		actioncontroller.sendVartoWeb ();
		
		if (webloadingmessage != null) {
			webloadingmessage.text = "Loading...";
			webloadingmessage.enabled = true;
						
		}

		www = new WWW (x);
		StartCoroutine (waitForWebXml ());
				
	}

	public void resetPlayer (string x)
	{
		Debug.Log ("Destroying GameObject");
		Destroy (gameObject);
		Application.LoadLevel (0);

	
		
	}

	IEnumerator waitForWebXml ()
	{

		yield return www;

		if (www.error == null) {

			currentquest = new Quest ();


		
			currentquestdata = (Transform)Instantiate (questdataprefab, transform.position, Quaternion.identity);

			currentquest.xmlcontent = UTF8Encoding.UTF8.GetString (www.bytes); 
			//ASCIIEncoding.ASCII.GetString (Encoding.Convert (Encoding.UTF32, Encoding.ASCII, www.bytes)); 


			installQuest (currentquest, false, false);

		} else {
			debug (www.error);

		}
	}

	public void startQuest (Quest q)
	{
		currentquest = q;

		currentquestdata = (Transform)Instantiate (questdataprefab, transform.position, Quaternion.identity);

		//Debug.Log (currentquest.id);




		if (!localquests.Contains (q)) {

			downloadQuest (q);
		} else {

			installQuest (q, true, true);
		}


	}

	public void removeQuest (Quest q)
	{


		if (localquests.Contains (q)) {

						
			#if UNITY_WEBPLAYER

			Debug.Log("cannot remove on web");

			# else 
			if (Directory.Exists (Application.persistentDataPath + "/quests/" + q.id)) {
				Directory.Delete (Application.persistentDataPath + "/quests/" + q.id, true);


			}
#endif
			localquests.Remove (q);


		}

	

		if (GameObject.Find ("List") != null) {

			if (GameObject.Find ("List").GetComponent<createquestbuttons> ()) {

			
				GameObject.Find ("List").GetComponent<createquestbuttons> ().resetList ();

			}
		}


	}

	public void endQuest ()
	{

//		Debug.Log ("what?");
		debug ("Quest beendet");

		if (currentquestdata != null) {
			Destroy (currentquestdata.gameObject);
		}
		Debug.Log ("Destroying GameObject");
		Destroy (actioncontroller.msgcanvas.gameObject);
		Destroy (gameObject);

		Application.LoadLevel (0);


	}

	public void downloadQuest (Quest q)
	{

		if (newxml) {
			string url = "http://www.qeevee.org:9091/editor/" + q.id + "/clientxml";
			www = new WWW (url);
			downloadmsg.enabled = true;
			downloadmsg.text = "Getting Quest-Definition ... ";
			StartCoroutine (DownloadFinished (q));
			
		} else {
			string url = "http://www.qeevee.org:9091/game/download/" + q.id;
		
			www = new WWW (url);
			//Debug.Log (url);
		

			downloadmsg.enabled = true;
			downloadmsg.text = "Downloading ... " + (www.progress * 100) + " %";

		
			StartCoroutine (DownloadPercentage ());
			StartCoroutine (DownloadFinished (q));

		}
	}

	public void downloadQuest (int id)
	{
		Quest q = new Quest ();
		q.id = id;
		currentquest = q;
		downloadQuest (q);

	}

	public void downloadAsset (string url, string filename)
	{


		WWW wwwfile = new WWW (url);

		if (filedownloads == null) {
			filedownloads = new List<WWW> ();
		}
		filedownloads.Add (wwwfile);

		StartCoroutine (downloadAssetFinished (wwwfile, filename, 0f));



	}

	public IEnumerator downloadAssetFinished (WWW wwwfile, string filename, float timeout)
	{

		yield return new WaitForSeconds (0.3f);
		timeout += 0.3f;

		if (wwwfile.isDone) {



			if (!Directory.Exists (Path.GetDirectoryName (filename))) {
				
				Debug.Log ("creating folder:" + Path.GetDirectoryName (filename));
				
				Directory.CreateDirectory (Path.GetDirectoryName (filename));
			}

			FileStream fs = File.Create (filename);
			fs.Write (wwwfile.bytes, 0, wwwfile.size);
			fs.Close ();
			Debug.Log ("file saved: " + filename);


		} else {

			if (wwwfile.error == null) {

				//Debug.Log(timeout+" - "+wwwfile.progress);

				if (wwwfile.progress < 0.1f && timeout > 5f) {

					Debug.Log ("Error: " + www.url + " - " + timeout);




					
					Debug.Log ("redoing www");
					
					questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
					
					questdb.filedownloads.Remove (wwwfile);
					
					
					questdb.downloadAsset (wwwfile.url, filename);
					
					wwwfile.Dispose ();








				} else {

					StartCoroutine (downloadAssetFinished (wwwfile, filename, timeout));

				}


			} else {


				//TODO: Handle WWW Error -> REDO?

				Debug.Log ("Error: " + www.url + " - " + www.error);
				if (www.error.Contains ("peer")) {
					
					
					
					Debug.Log ("redoing www");
					
					questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
					
					questdb.filedownloads.Remove (wwwfile);
					
					
					questdb.downloadAsset (wwwfile.url, filename);
					
					wwwfile.Dispose ();
					
					



				}

			}





		}

	}

	public List<Quest> GetLocalQuests ()
	{

#if !UNITY_WEBPLAYER
		if (!Application.isWebPlayer) {

			localquests.Clear ();



			DirectoryInfo info = new DirectoryInfo (Application.persistentDataPath + "/quests/");
			if (!info.Exists) {
				info.Create();
				return localquests;
			}

			var fileInfo = info.GetDirectories ();




			foreach (DirectoryInfo folder in fileInfo) { 
						


				if (File.Exists (folder.ToString () + "/game.xml")) {

					Quest n = new Quest ();



					string[] splitted = folder.ToString ().Split ('/');

					n.id = int.Parse (splitted [splitted.Length - 1]);
//			Debug.Log("folder found:"+splitted[splitted.Length - 1]);

					n.filepath = folder.ToString () + "/";
					n = n.LoadFromText (int.Parse (splitted [splitted.Length - 1]), true);
					//n.deserializeAttributes();
					localquests.Add (n);
					//Debug.Log(folder.ToString());
				}
			}



		}

#endif

		return localquests;

	}

	public void AllowAutoRotation (bool status)
	{
		if (status == true) {
			Screen.autorotateToLandscapeLeft = true;
			Screen.autorotateToLandscapeRight = true;
			originalOrientation = Screen.orientation;
			Screen.orientation = ScreenOrientation.AutoRotation;
		} else {

			Debug.Log ("rotatin' 1");
			if (originalOrientation != null) {
				Debug.Log ("rotatin' 1.5");

				Screen.orientation = originalOrientation;
			}
			Debug.Log ("rotatin' 2");

			Screen.autorotateToLandscapeLeft = false;
			Screen.autorotateToLandscapeRight = false;
			Debug.Log ("rotatin' 3");


		}
	}

	public void installQuest (Quest q, bool reload, bool localload)
	{


		if (filedownloads != null) {
			filedownloads.Clear ();
		}
		if (loadedfiles != null) {
			loadedfiles.Clear ();
		}




		bool x = false;


		if (localload) {
			Debug.Log ("LOCAL");
			x = true;
		}



//				Debug.Log ("installing..."+reload);
		currentquest = q.LoadFromText (q.id, x);

		//q.deserializeAttributes ();
//		Debug.Log ("done installing...");



		if (newxml && !localload) {

			// resave xml
			string exportLocation = Application.persistentDataPath + "/quests/" + currentquest.id + "/";
			
			
			
			#if !UNITY_WEBPLAYER

			if (!Application.isWebPlayer && (!Directory.Exists (exportLocation) || reload)) {



				if (Directory.Exists (exportLocation)) {

					Directory.Delete (exportLocation, true);

				}
				Directory.CreateDirectory (exportLocation);


			
				var stream = new FileStream (exportLocation + "game.xml", FileMode.Create);
				// WRITE FILES TO XML -> NOT WORKING ON IOS
				//var serializer = new XmlSerializer (typeof(Quest));
				//serializer.Serialize (stream, currentquest);
				stream.Close ();
				var stream2 = new StreamWriter (exportLocation + "game.xml");




				stream2.Write (q.xmlcontent);

				stream2.Close ();

				Debug.Log ("WRITING XML FILE: " + exportLocation + "game.xml");
				if (File.Exists (exportLocation + "game.xml")) {
					Debug.Log ("...exists");

				}



			}
#endif
		}

				




		bool did = false;
		foreach (QuestPage qp in currentquest.pages) {

			if (did == false) {
				currentquest.currentpage = qp;
				did = true;
			}

		}

		hotspots = new List<QuestRuntimeHotspot> ();
		foreach (QuestHotspot qh in currentquest.hotspots) {




			bool a = false;
			if (qh.hasAttribute ("initialActivity")) {

				if (qh.getAttribute ("initialActivity") == "true") {
					a = true;
				}

			}

			bool v = false;
			if (qh.hasAttribute ("initialVisibility")) {
				
				if (qh.getAttribute ("initialVisibility") == "true") {
					v = true;
				}
				
			}

			hotspots.Add (new QuestRuntimeHotspot (qh, a, v, qh.latlon));
		

		}






		if (canPlayQuest (currentquest)) {

			Debug.Log ("WAITING FOR QUEST ASSETS");
			downloadmsg.text = "Downloading Quest Assets ... 0 %";

			StartCoroutine (waitforquestassets (currentquest.currentpage.id, 0f));
						

		} else {

			showmessage ("Entschuldigung! Die Quest kann in dieser Beta-Version nicht abgespielt werden.");
			GameObject.Find ("List").GetComponent<createquestbuttons> ().resetList ();

		}

	}

	IEnumerator waitforquestassets (int pageid, float timeout)
	{
		//downloadmsg.text = "Downloading Quest Assets ... 0 %";


		timeout += 0.5f;
		yield return new WaitForSeconds (0.5f);
		bool done = true;

		int percent = 100;
		int downloadsundone = 0;



		if (filedownloads != null) {
			foreach (WWW www in filedownloads) {

				if (!www.isDone) {
					done = false;
					downloadsundone += 1;
				}

			}




			percent = 100 - (downloadsundone * 100 / filedownloads.Count);

		}
		Debug.Log ("percent done: " + percent);

		downloadmsg.text = "Downloading Quest Assets ... " + percent + " %";


		if (done) {

			changePage (pageid);
		} else {

			StartCoroutine (waitforquestassets (pageid, timeout));

		}
	}

	public bool canPlayQuest (Quest q)
	{

		bool playable = true;
		foreach (QuestPage qp in q.pages) {


			if (qp.type != "StartAndExitScreen" && 
				qp.type != "NPCTalk" && 
				qp.type != "MultipleChoiceQuestion" && 
				qp.type != "VideoPlay" && 
				qp.type != "TagScanner" && 
				qp.type != "ImageCapture" && 
				qp.type != "AudioRecord" && 
				qp.type != "TextQuestion" && 
				qp.type != "MapOSM") {



				Debug.Log ("Can't play because it includes mission of type " + qp.type);
				playable = false;
			}

			

		}
		return playable;

	}

	IEnumerator DownloadPercentage ()
	{
		
		yield return new WaitForSeconds (0.1f);
		
		
		if (www.progress < 1f && www.error == null) {
			
			downloadmsg.text = "Downloading Quest ... " + (www.progress * 100) + " %";
			StartCoroutine (DownloadPercentage ());
			
		} else {
			
			
		}
		
		
	}

	public QuestPage getPage (int id)
	{
		QuestPage resultpage = null;



		foreach (QuestPage qp in currentquest.pages) {
			
			
			if (qp.id == id) {

				resultpage = qp;

			}


		}


		return resultpage;

	}

	public void changePage (int id)
	{
		


		foreach (QuestPage qp in currentquest.pages) {
		

			if (qp.id == id) {

				currentquest.currentpage = qp;



				currentquest.currentpage.state = "running";

				if (qp.type == "NPCTalk") {
					Application.LoadLevel (1);
				} else if (qp.type == "StartAndExitScreen") {
					Application.LoadLevel (2);

				} else if (qp.type == "MultipleChoiceQuestion") {
					Application.LoadLevel (3);

				} else if (qp.type == "VideoPlay") {
					Application.LoadLevel (4);

				} else if (qp.type == "TagScanner") {
					Application.LoadLevel (5);
					
				} else if (qp.type == "ImageCapture") {
					Application.LoadLevel (6);
					
				} else if (qp.type == "TextQuestion") {
					Application.LoadLevel (7);
				} else if (qp.type == "AudioRecord") {
					Application.LoadLevel (8);
				} else if (qp.type == "MapOSM") {
					Application.LoadLevel (9);
				}
				
				
				
			}
		
		}
		
		
	}
	
	void showmessage (string text)
	{
		

		QuestMessage nqa = (QuestMessage)Instantiate (message_prefab, transform.position, Quaternion.identity);
			
		nqa.message = text;
			
		nqa.transform.SetParent (GameObject.Find ("Canvas").transform, false);
		nqa.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);

		
	}

	public QuestRuntimeHotspot getHotspot (string str)
	{

		QuestRuntimeHotspot qh = null;
	
		foreach (QuestRuntimeHotspot qrh in hotspots) {



			if (qrh.hotspot.id == int.Parse (str)) {

				qh = qrh;


			}



		}

		return qh;

	}

	IEnumerator DownloadFinished (Quest q)
	{
		downloadmsg.enabled = true;

		localquests.Add (q);
		yield return www;
		if (www.error == null) {




			if (newxml) {

				downloadmsg.text = "Downloading Quest Assets...";


				currentquest = new Quest ();
				
				currentquest.id = q.id;
				
				currentquestdata = (Transform)Instantiate (questdataprefab, transform.position, Quaternion.identity);
				
				currentquest.xmlcontent = UTF8Encoding.UTF8.GetString (www.bytes); 

				bool b = false;



				foreach (Quest lq in localquests) {
					if (lq.id == q.id) {

						b = true;
					}
				}

//				Debug.Log(q.id+","+b);

				installQuest (currentquest, b, false);

				
			} else {
				//Debug.Log("WWW Finished");
				downloadmsg.text = "Installing Quest...";

	
				string fileName = Application.temporaryCachePath + "/quest" + q.id + ".zip";
				FileStream zip = File.Create (fileName);
				zip.Write (www.bytes, 0, www.size);

				zip.Close ();




				string exportLocation = Application.persistentDataPath + "/quests/" + q.id + "/";



				if (Directory.Exists (exportLocation)) {

					#if UNITY_WEBPLAYER
				
				Debug.Log("cannot delete local files on web");

				
					# else 
					Directory.Delete (exportLocation, true);

#endif
				}
				//	ZipUtil.Unzip (fileName, exportLocation);



				q.filepath = exportLocation;


				bool b = false;
				
				
				foreach (Quest lq in localquests) {
					if (lq.id == q.id) {
						
						b = true;
					}
				}
				installQuest (q, b, false);


			}
			
		} else {
			Debug.Log ("WWW Error: " + www.error);
			downloadmsg.text = www.error;

		}    
		
		
	}


	

}

[System.Serializable]
[XmlRoot("game")]
public class Quest  : IComparable<Quest>
{
	
	[XmlAttribute("name")]
	public string
		name;
	[XmlAttribute("id")]
	public int
		id;
	[XmlAttribute("xmlformat")]
	public int
		xmlformat;
	public string filepath;
	[XmlElement("mission")]
	public List<QuestPage>
		pages;
	[XmlElement("hotspot")]
	public List<QuestHotspot>
		hotspots;
	public bool hasData = false;
	public QuestPage currentpage;
	public string xmlcontent;

	public Quest ()
	{


	}

	public int CompareTo (Quest q)
	{



		if (q == null) {
			return 1;
		} else {

			return this.name.ToUpper ().CompareTo (q.name.ToUpper ());
		}

	}

	public  Quest LoadFromText (int id, bool redo)
	{
	
		string fp = filepath;
		string xmlfilepath = filepath;
		string xmlcontent_copy = xmlcontent;
		if (filepath == null) {
			xmlfilepath = " ";

		}

		if (xmlcontent_copy == null) {

			xmlcontent_copy = " ";
		}

		if (!xmlfilepath.EndsWith (".xml")) {
			xmlfilepath = filepath + "game.xml";
		}


		Encoding enc = System.Text.Encoding.UTF8;

		
		TextReader txr = new StringReader (xmlcontent_copy);



		if (xmlfilepath != null && xmlfilepath.Length > 9) {





			txr = new StreamReader (xmlfilepath, enc);

		}
		XmlSerializer serializer = new XmlSerializer (typeof(Quest));

		Quest q = serializer.Deserialize (txr) as Quest; 
	
	


		q.filepath = fp;
		q.hasData = true;
	
		q.id = id;
//		Debug.Log ("my id is " + id + " -> " + q.id);
		q.deserializeAttributes (redo);


		return q;
	}

	public void deserializeAttributes (bool redo)
	{


		if (pages != null) {
			foreach (QuestPage qp in pages) {
				qp.deserializeAttributes (id, redo);
			}
		} else {

			Debug.Log ("no pages");
		}
		if (hotspots != null) {

			foreach (QuestHotspot qh in hotspots) {
				qh.deserializeAttributes (id, redo);
			}
		}

	}
	
	
}

[System.Serializable]
public class QuestPage
{


	[XmlAttribute("id")]
	public int
		id;
	[XmlAttribute("type")]
	public string
		type;
	[XmlAnyAttribute()]
	public XmlAttribute[]
		help_attributes;
	public List<QuestAttribute> attributes;
	[XmlElement("dialogitem")]
	public List<QuestContent>
		contents_dialogitems;
	[XmlElement("expectedCode")]
	public List<QuestContent>
		contents_expectedcode;
	[XmlElement("answer")]
	public List<QuestContent>
		contents_answers;
	[XmlElement("question")]
	public QuestContent
		contents_question;
	[XmlElement("answers")]
	public List<QuestContent>
		contents_answersgroup;
	[XmlElement("onEnd")]
	public QuestTrigger
		onEnd;
	[XmlElement("onStart")]
	public QuestTrigger
		onStart;
	[XmlElement("onTap")]
	public QuestTrigger
		onTap;
	[XmlElement("onSuccess")]
	public QuestTrigger
		onSuccess;
	[XmlElement("onFail")]
	public QuestTrigger
		onFailure;
	public string state;
	public string result;

	public QuestPage ()
	{

		state = "new";
		result = null;
	}

	public string getAttribute (string k)
	{
		
		foreach (QuestAttribute qa in attributes) {


			if (qa.key.Equals (k)) {
				return qa.value;
			}
			
		}


		return "";
		
	}

	public bool hasAttribute (string k)
	{

		bool h = false;
		foreach (QuestAttribute qa in attributes) {
			
			
			if (qa.key.Equals (k)) {
				h = true;
			}
			
		}
		
		
		return h;
		
	}
	
	public void deserializeAttributes (int id, bool redo)
	{

		attributes = new List<QuestAttribute> ();

		if (help_attributes != null) {
			foreach (XmlAttribute xmla in help_attributes) {



							
				if (xmla.Value.StartsWith ("http://") || xmla.Value.StartsWith ("https://")) {


					string[] splitted = xmla.Value.Split ('/');


					questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();


					string filename = "files/" + splitted [splitted.Length - 1];

					int i = 0;
					while (questdb.loadedfiles.Contains(filename)) {
						i++;
						filename = "files/" + i + "_" + splitted [splitted.Length - 1];

					}

					questdb.loadedfiles.Add (filename);




					if (!Application.isWebPlayer) {

					
				
						if (!redo) {
							questdb.downloadAsset (xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
						}
						if (splitted.Length > 3) {
							
							xmla.Value = Application.persistentDataPath + "/quests/" + id + "/" + filename;
							
						}
					}


				}	
								
				attributes.Add (new QuestAttribute (xmla.Name, xmla.Value));

			
			}
		}


		foreach (QuestContent qcdi in contents_dialogitems) {
			qcdi.deserializeAttributes (id, redo);
		}

		foreach (QuestContent qcdi in contents_answers) {
			qcdi.deserializeAttributes (id, redo);
		}

		if (contents_question != null) {
			contents_question.deserializeAttributes (id, redo);
		}
		foreach (QuestContent qcdi in contents_answersgroup) {
			qcdi.deserializeAttributes (id, redo);
		}


			
			

		foreach (QuestContent qcdi in contents_expectedcode) {
			qcdi.deserializeAttributes (id, redo);
		}

		if (onEnd != null) {
			foreach (QuestAction qa in onEnd.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
		if (onStart != null) {
			foreach (QuestAction qa in onStart.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
		if (onTap != null) {
			foreach (QuestAction qa in onTap.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
		if (onSuccess != null) {
			foreach (QuestAction qa in onSuccess.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
		if (onFailure != null) {
			foreach (QuestAction qa in onFailure.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
	}

}

[System.Serializable]
public class QuestHotspot
{


	[XmlAttribute("id")]
	public int
		id;
	[XmlAttribute("latlong")]
	public string
		latlon;
	[XmlAnyAttribute()]
	public XmlAttribute[]
		help_attributes;
	public List<QuestAttribute> attributes;
	[XmlElement("onEnter")]
	public QuestTrigger
		onEnter;
	[XmlElement("onLeave")]
	public QuestTrigger
		onLeave;
	[XmlElement("onTap")]
	public QuestTrigger
		onTap;

	public string getAttribute (string k)
	{
		
		foreach (QuestAttribute qa in attributes) {
			
			
			if (qa.key.Equals (k)) {
				return qa.value;
			}
			
		}
		
		
		return "";
		
	}
	
	public bool hasAttribute (string k)
	{
		
		bool h = false;
		foreach (QuestAttribute qa in attributes) {
			
			
			if (qa.key.Equals (k)) {
				h = true;
			}
			
		}
		
		
		return h;
		
	}

	public void deserializeAttributes (int id, bool redo)
	{
		
		attributes = new List<QuestAttribute> ();
		
		if (help_attributes != null) {
			foreach (XmlAttribute xmla in help_attributes) {
				
				if (xmla.Value.StartsWith ("http://") || xmla.Value.StartsWith ("https://")) {
								
								
					string[] splitted = xmla.Value.Split ('/');
								
								
					questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
								
								
					string filename = "files/" + splitted [splitted.Length - 1];
								
					int i = 0;
					while (questdb.loadedfiles.Contains(filename)) {
						i++;
						filename = "files/" + i + "_" + splitted [splitted.Length - 1];
									
					}
								
					questdb.loadedfiles.Add (filename);
								
								
								
								
					if (!Application.isWebPlayer) {
									
									
									
						if (!redo) {
							questdb.downloadAsset (xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
						}
						if (splitted.Length > 3) {
										
							xmla.Value = Application.persistentDataPath + "/quests/" + id + "/" + filename;
										
						}
					}
								
								
				}

				attributes.Add (new QuestAttribute (xmla.Name, xmla.Value));
				
				
			}
		}
		if (onEnter != null) {
			foreach (QuestAction qa in onEnter.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
		if (onLeave != null) {
			foreach (QuestAction qa in onLeave.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
		if (onTap != null) {
			foreach (QuestAction qa in onTap.actions) {
				qa.deserializeAttributes (id, redo);
			}
		}
		
		
		
		
	}
	
}

[System.Serializable]
public class QuestContent
{

	[XmlAttribute("id")]
	public int
		id;
	[XmlText()]
	public string
		content;
	[XmlAnyAttribute()]
	public XmlAttribute[]
		help_attributes;
	public List<QuestAttribute> attributes;
	[XmlElement("questiontext")]
	public QuestContent
		questiontext;
	[XmlElement("answer")]
	public List<QuestContent>
		answers;

	public string getAttribute (string k)
	{
		
		foreach (QuestAttribute qa in attributes) {
			if (qa.key.Equals (k)) {
				return qa.value;
			}
		}
		return "";
	}

	public void deserializeAttributes (int id, bool redo)
	{

		foreach (QuestContent qcdi in answers) {
			qcdi.deserializeAttributes (id, redo);
		}


		if (questiontext != null) {
			questiontext.deserializeAttributes (id, redo);
		}
		
		attributes = new List<QuestAttribute> ();
		
		if (help_attributes != null) {
			foreach (XmlAttribute xmla in help_attributes) {



				if (xmla.Value.StartsWith ("http://") || xmla.Value.StartsWith ("https://")) {
									
									
					string[] splitted = xmla.Value.Split ('/');
									
									
					questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
									
									
					string filename = "files/" + splitted [splitted.Length - 1];
									
					int i = 0;
					while (questdb.loadedfiles.Contains(filename)) {
						i++;
						filename = "files/" + i + "_" + splitted [splitted.Length - 1];
										
					}
									
					questdb.loadedfiles.Add (filename);
									
									
									
									
					if (!Application.isWebPlayer) {
										
										

						if (!redo) {
							questdb.downloadAsset (xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
						}
						if (splitted.Length > 3) {
											
							xmla.Value = Application.persistentDataPath + "/quests/" + id + "/" + filename;
											
						}
					}
									
									
				}
				
				attributes.Add (new QuestAttribute (xmla.Name, xmla.Value));
				
				
			}
		}



		
		
	}

}

[System.Serializable]
public class QuestValue
{



}

[System.Serializable]
public class QuestVariableValue
{
	

	[XmlElement("string")]
	public List<string>
		string_value;
	[XmlElement("num")]
	public List<float>
		num_value;
	[XmlElement("boolean")]
	public List<bool>
		bool_value;
	[XmlElement("var")]
	public List<string>
		var_value;

	
	
}

[System.Serializable]
public class QuestAction
{


	[XmlAttribute("type")]
	public string
		type;
	[XmlElement("value")]
	public QuestVariableValue
		value;
	[XmlAnyAttribute()]
	public XmlAttribute[]
		help_attributes;
	public List<QuestAttribute> attributes;
	[XmlArray("then"),XmlArrayItem("action")]
	public List<QuestAction>
		thenactions;
	[XmlArray("else"),XmlArrayItem("action")]
	public List<QuestAction>
		elseactions;
	[XmlElement("condition")]
	public QuestConditionGrouper
		condition;

	public string getAttribute (string k)
	{
		
		foreach (QuestAttribute qa in attributes) {
			
			
			if (qa.key.Equals (k)) {
				return qa.value;
			}
			
		}
		
		
		return "";
		
	}

	public bool hasMissionAction ()
	{

		bool b = false;



		if (type == "StartMission") {
			b = true;
		} else if (thenactions.Count > 0 || elseactions.Count > 0) {


			foreach (QuestAction qa in thenactions) {
				if (qa.hasMissionAction ()) {
					b = true;
				} 
			}
			foreach (QuestAction qa in elseactions) {
				if (qa.hasMissionAction ()) {
					b = true;
				} 
			}

		}

				

		return b;
	}

	public bool hasAttribute (string k)
	{
		
		bool h = false;
		foreach (QuestAttribute qa in attributes) {
			
			
			if (qa.key.Equals (k)) {
				h = true;
			}
			
		}
		
		
		return h;
		
	}

	public void Invoke ()
	{

		//Debug.Log (type);
		GameObject.Find ("QuestDatabase").GetComponent<actions> ().doAction (this);

	}

	public void InvokeThen ()
	{
		
		
		if (thenactions != null && thenactions.Count > 0) {
			foreach (QuestAction qa in thenactions) {
				
				qa.Invoke ();
				
			}
		}
		
		
	}

	public void InvokeElse ()
	{
		
		
		if (elseactions != null && thenactions.Count > 0) {
			foreach (QuestAction qa in elseactions) {
				
				qa.Invoke ();
				
			}
		}
		
		
	}

	public void deserializeAttributes (int id, bool redo)
	{

		attributes = new List<QuestAttribute> ();
		
		if (help_attributes != null) {
			foreach (XmlAttribute xmla in help_attributes) {
				

				if (xmla.Value.StartsWith ("http://") || xmla.Value.StartsWith ("https://")) {
									
									
					string[] splitted = xmla.Value.Split ('/');
									
									
					questdatabase questdb = GameObject.Find ("QuestDatabase").GetComponent<questdatabase> ();
									
									
					string filename = "files/" + splitted [splitted.Length - 1];
									
					int i = 0;
					while (questdb.loadedfiles.Contains(filename)) {
						i++;
						filename = "files/" + i + "_" + splitted [splitted.Length - 1];
										
					}
									
					questdb.loadedfiles.Add (filename);
									
									
									
									
					if (!Application.isWebPlayer) {
										
										
										
						if (!redo) {
							questdb.downloadAsset (xmla.Value, Application.persistentDataPath + "/quests/" + id + "/" + filename);
						}
						if (splitted.Length > 3) {
											
							xmla.Value = Application.persistentDataPath + "/quests/" + id + "/" + filename;
											
						}
					}
									
									
				}

				attributes.Add (new QuestAttribute (xmla.Name, xmla.Value));
				
				
			}
		}
		


		foreach (QuestAction qa in thenactions) {
			qa.deserializeAttributes (id, redo);
		}

		foreach (QuestAction qa in elseactions) {
			qa.deserializeAttributes (id, redo);
		}

	}
	
	

}

[System.Serializable]
public class QuestConditionGrouper
{


	[XmlElement("and")]
	public QuestConditionGrouper
		and;
	[XmlElement("or")]
	public QuestConditionGrouper
		or;
	[XmlElement("not")]
	public QuestConditionGrouper
		not;
	[XmlElement("lt")]
	public List<QuestConditionComparer>
		lt;
	[XmlElement("gt")]
	public List<QuestConditionComparer>
		gt;
	[XmlElement("leq")]
	public List<QuestConditionComparer>
		leq;
	[XmlElement("geq")]
	public List<QuestConditionComparer>
		geq;
	[XmlElement("eq")]
	public List<QuestConditionComparer>
		eq;

	public bool isfullfilled ()
	{

		return isfullfilled ("and");

	}

	public bool isfullfilled (string type)
	{


		List<bool> allbools = new List<bool> ();

		if (and != null) {
			allbools.Add (and.isfullfilled ("and"));
		}

		if (or != null) {
			allbools.Add (or.isfullfilled ("or"));
		}

		if (not != null) {
			allbools.Add (not.isfullfilled ("not"));
		}

		if (eq != null) {

			foreach (QuestConditionComparer qcc in eq) {

				allbools.Add (qcc.isFullfilled ("eq"));

			}

		}

		if (lt != null) {
				
			foreach (QuestConditionComparer qcc in lt) {
					
				allbools.Add (qcc.isFullfilled ("lt"));
					
			}

		}

		if (gt != null) {
				
			foreach (QuestConditionComparer qcc in gt) {
					
				allbools.Add (qcc.isFullfilled ("gt"));
					
			}
				
		}

		if (leq != null) {
					
			foreach (QuestConditionComparer qcc in leq) {
						
				allbools.Add (qcc.isFullfilled ("leq"));
						
			}
					
		}
				
		if (geq != null) {
					
			foreach (QuestConditionComparer qcc in geq) {
						
				allbools.Add (qcc.isFullfilled ("geq"));
						
			}
					
		}
	
			
		if (allbools.Count > 0) {

			if (type == "and") {

				bool ands = true;

				foreach (bool b in allbools) {

					if (!b) {
						ands = false;
					}

				}

				return ands;


			} else if (type == "or") {

				bool ors = false;
						
				foreach (bool b in allbools) {
							
					if (b) {
						ors = true;
					}
							
				}
						
				return ors;

			} else if (type == "not") {
						
				bool nots = true;
						
				foreach (bool b in allbools) {
							
					if (!b) {
						nots = false;
					}
							
				}
						
				return nots;
						
			}

		} 


			



		return true;

	}






}

[System.Serializable]
public class QuestConditionComparer
{

	[XmlElement("string")]
	public List<string>
		string_value;
	[XmlElement("num")]
	public List<float>
		num_value;
	[XmlElement("boolean")]
	public List<bool>
		bool_value;
	[XmlElement("var")]
	public List<string>
		var_value;

	public bool isFullfilled (string type)
	{



		if (type == "eq") {

			if (stringcomponents ().Count > 1) {
				bool equals = true;
				string last = null;
				foreach (string current in stringcomponents()) {

					if (last == null) {
						last = current;
					} else {

						if (current != last) {

							equals = false;

						}

					}

				}

				return equals;
			} else {

				return false;

			}


		} else  if (type == "lt") {



			if (intcomponents ().Count > 1) {

				bool lessthan = true;
				int last = int.MinValue;

				foreach (int i in intcomponents()) {

					if (last >= i) {
						lessthan = false;
					}
											
									


					last = i;

				}


				return lessthan;


			} else {

				return false;

			}

		} else  if (type == "leq") {
					
					
					
			if (intcomponents ().Count > 1) {
						
				bool lessthan = true;
				int last = int.MinValue;
						
				foreach (int i in intcomponents()) {
							
					if (last > i) {
						lessthan = false;
					}
							
							
							
							
					last = i;
							
				}
						
						
				return lessthan;
						
				
			} else {
				
				return false;
				
			}
			


		} else  if (type == "gt") {




			if (intcomponents ().Count > 1) {
							
				bool greaterthan = true;
				int last = int.MaxValue;
							
							
				foreach (int i in intcomponents()) {
								
									
					if (last <= i) {
						greaterthan = false;
					}
									
								
								
								
					last = i;
								
				}
							
							
				return greaterthan;
							
							
			} else {
							
				return false;
							
			}


		} else  if (type == "geq") {
			
			
			
			
			if (intcomponents ().Count > 1) {
				
				bool greaterthan = true;
				int last = int.MaxValue;
				
				
				foreach (int i in intcomponents()) {
					
					
					if (last < i) {
						greaterthan = false;
					}
					
					
					
					
					last = i;
					
				}
				
				
				return greaterthan;
				
				
			} else {
				
				return false;
				
			}
			

				
				
		} else {

			return false;

		}
	
	}

	public List<float> intcomponents ()
	{

		List<float> comp = new List<float> ();



		if (var_value != null) {


			foreach (string s in var_value) {

				QuestVariable qv = GameObject.Find ("QuestDatabase").GetComponent<actions> ().getVariable (s);

				if (qv != null) {
					if (qv.num_value != null && qv.num_value.Count > 0) {
						comp.Add (qv.num_value [0]);
					}
				}

			}




		} 

		if (num_value != null) {

			comp.AddRange (num_value);
		} 


		return comp;
	}

	public List<string> stringcomponents ()
	{
		
		List<string> comp = new List<string> ();

		if (string_value != null) {
			comp.AddRange (string_value);
		}
		if (num_value != null) {
			foreach (float n in num_value) {
				comp.Add ("" + n);
			}
		}
		if (bool_value != null) {

			foreach (bool b in bool_value) {
					
				if (b) {
					comp.Add ("true"); 
				} else {
					comp.Add ("false");
				}

			}
		}
		if (var_value != null && var_value.Count > 0) {

			foreach (string k in var_value) {



				string kk = new string (k.ToCharArray ()
				                       .Where (c => !Char.IsWhiteSpace (c))
				                       .ToArray ());

				//Debug.Log("-----starting to look for '"+kk+"'");

				QuestVariable qv = GameObject.Find ("QuestDatabase").GetComponent<actions> ().getVariable (kk);

				if (qv != null) {

					//Debug.Log("found");
					if (qv.getStringValue () != null) {
						comp.Add (qv.getStringValue ());
					}
				} else {

					Debug.Log ("couldn't find var " + kk);

				}	

			}

		}
		
		return comp;
		
	}




}

[System.Serializable]
public class QuestAttribute
{

	public string key;
	public string value;

	public QuestAttribute ()
	{

	}

	public QuestAttribute (string k, string v)
	{

		key = k;
		value = v;
	}
	
}

[System.Serializable]
public class QuestTrigger
{


	[XmlArray("rule"),XmlArrayItem("action")]
	public List<QuestAction>
		actions;

	public void Invoke ()
	{


		if (actions != null) {
			foreach (QuestAction qa in actions) {

				qa.Invoke ();

			}
		}


	}

	public bool hasMissionAction ()
	{
		
		bool b = false;
		foreach (QuestAction a in actions) {


			if (a.hasMissionAction ()) {

				b = true;

			}

		}

		return b;

	}


	
}

[System.Serializable]
public class QuestRuntimeHotspot
{
	
	
	public QuestHotspot hotspot;
	public bool active;
	public bool visible;
	public float lon;
	public float lat;
	public MeshRenderer renderer;
	public bool entered = false;
	
	public QuestRuntimeHotspot (QuestHotspot hp, bool a, bool v, string ll)
	{
		
		hotspot = hp;
		active = a;
		visible = v;


		if (ll.Contains (",")) {
			
			
			
			char[] splitter = ",".ToCharArray ();
			
			string[] splitted = ll.Split (splitter);

			foreach (string x in splitted) {

				if (lon == null || lon == 0.0f) {

					lon = float.Parse (x);


				} else {

					lat = float.Parse (x);

				}



			}


		} else {

			lon = 0.0f;
			lat = 0.0f;

		}


		
	}
	
	
	
}

