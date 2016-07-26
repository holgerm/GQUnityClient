#if UNITY_EDITOR


using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace QM.NFC {

	/// <summary>
	/// This Editor Window is avalibale at runtime only to enable it to emulate the Writing Behavior of the Game using the NFC4Unity Plugin.
	/// </summary>
	public class NFC_Emulator : EditorWindow {
		string id, payload, techs;

		static NFC_Emulator nfcEmulator;

		private const string PAYLOAD_TEXTFIELD_NAME = "PayloadTextField";

		public static void emulateNFCWrite (string writtenText) {
			if ( nfcEmulator != null ) {
				nfcEmulator.payload = writtenText;
				nfcEmulator.Repaint();
				EditorGUI.FocusTextInControl(PAYLOAD_TEXTFIELD_NAME);
			}
		}

		[MenuItem("Window/NFC Emulator")]
		public static void  ShowWindow () {
			EditorWindow.GetWindow(typeof(NFC_Emulator));
		}

		void OnEnable () {
			nfcEmulator = this;

			// ID:
			id = EditorPrefs.GetString("nfc.id");
			if ( id == null || id.Equals("") )
				id = "id goes here ...";

			// Payload:
			payload = EditorPrefs.GetString("nfc.payload");
			if ( payload == null )
				payload = "payload goes here ...";

			// Techs: // TODO make list of string instead to prevent errors
			techs = EditorPrefs.GetString("nfc.techs");
			if ( techs == null )
				techs = "(optional techs)";
		}

		void OnGUI () {
			GUILayout.Label("NFC Emulation", EditorStyles.boldLabel);

			// ID: 
			string idOld = id;
			id = EditorGUILayout.TextField("ID", id);
			// must not be empty:
			if ( id.Equals("") )
				id = idOld; // take old value if set empty.
			// save changes:
			if ( !id.Equals(idOld) )
				EditorPrefs.SetString("nfc.id", id);

			// Payload: 
			GUI.SetNextControlName(PAYLOAD_TEXTFIELD_NAME);
			string payloadOld = payload;
			payload = EditorGUILayout.TextField("Payload", payload);
			// save changes:
			if ( !payload.Equals(payloadOld) )
				EditorPrefs.SetString("nfc.payload", payload);
			
			// Techs: TODO should be a list of strings
			// TODO make in meaningful, e.g. by only accepting certain features on certain tech types
			string techsOld = techs;
			techs = EditorGUILayout.TextField("Tech Descriptions", techs);
			// save changes:
			if ( !techs.Equals(techsOld) )
				EditorPrefs.SetString("nfc.techs", techs);
			
			// Read NFC Button:
			GUI.enabled = Application.isPlaying;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Read NFC Chip");
			if ( GUILayout.Button("Simple mode") ) {
				emulateNFCRead(true);
			}
			if ( GUILayout.Button("Event mode") ) {
				emulateNFCRead(false);
			}
			EditorGUILayout.EndHorizontal();
			GUI.enabled = true;
		}

		private void emulateNFCRead (bool simpleMode) {
			// grab the nfc receiver: 
			GameObject nfcReceiver = GameObject.Find("/NFC_Connector");
			if ( nfcReceiver == null ) {
				Debug.LogError("NFCReceiver missing. The NFC Plugin does not find the NFCReceiver GameObject, but it needs it.");
				return;
			}
			NFC_Connector nfcConnector = nfcReceiver.GetComponent<NFC_Connector>();
			if ( nfcConnector == null ) {
				Debug.LogError("NFC_Connector missing. The NFC Plugin finds the NFCReceiver GameObject, but it lacks the NFC_Connector Script Component.");
				return;
			}

			if ( simpleMode )
				nfcConnector.NFCReadPayload(payload);
			else {
				string[] techsArray = techs.Split(new char[] {
					','
				});
				NFC_Info info = new NFC_Info(id, payload, techsArray);
				nfcConnector.NFCReadDetails(info.marshall());
			}
		}

		void OnDisable () {
			nfcEmulator = null;

			EditorPrefs.SetString("nfc.id", id);
			EditorPrefs.SetString("nfc.payload", payload);
			EditorPrefs.SetString("nfc.techs", techs);
		}
	}
}

#endif