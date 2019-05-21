using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

namespace QM.NFC
{

    public interface NFC_Reader_I
    {
        void OnNFCPayloadRead(string payload);

        void OnNFCDetailsRead(NFC_Info info);
    }


    public class NFC_Connector : MonoBehaviour
    {
        public const string NAME = "NFC_Connector";

        private static NFC_Connector _connector;
        private static GameObject connectorGO;

        public static NFC_Connector Connector
        {
            get
            {
                if (_connector == null)
                {
                    connectorGO = new GameObject(NAME);
                    connectorGO.AddComponent<NFC_Connector>();
                    _connector = connectorGO.GetComponent<NFC_Connector>();
                }
                return _connector;
            }
        }
        private static List<NFC_Reader_I> registeredReaderUIs;

        public bool RegisterReaderUI(NFC_Reader_I newNFCReaderUI)
        {
            if (registeredReaderUIs == null)
            {
                registeredReaderUIs = new List<NFC_Reader_I>();
            }
            if (registeredReaderUIs.Contains(newNFCReaderUI))
                return false;
            else
            {
                registeredReaderUIs.Add(newNFCReaderUI);
                return true;
            }
        }

        public bool UnregisterReaderUI(NFC_Reader_I nfcReaderUI)
        {
            if (registeredReaderUIs == null)
            {
                return false;
            }
            else
            {
                return registeredReaderUIs.Remove(nfcReaderUI);
            }
        }

        /// <summary>
        /// Called by Android Java Plugin when an NFC Tag is read. The read payload is given as parameter. 
        /// </summary>
        /// <param name="payload">Payload.</param>
        public void NFCReadPayload(string payload)
        {
#if UNITY_ANDROID
            foreach (NFC_Reader_I reader in registeredReaderUIs)
            {
                reader.OnNFCPayloadRead(payload);
            }

#elif UNITY_EDITOR
		Debug.LogWarning ("NFC Plugin only works on Android Platform.");
#endif
        }

        /// <summary>
        /// Called by Android Java Plugin when an NFC Tag is read.
        /// The read details are given as parameter and are unmarshalled first. 
        /// Then the different contents are made available to the game by triggering an event.
        /// </summary>
        /// <param name="id">ID.</param>
        public void NFCReadDetails(string marshalledContent)
        {
#if UNITY_ANDROID
            NFC_Info info = new NFC_Info(marshalledContent);

            foreach (NFC_Reader_I reader in registeredReaderUIs.ToArray())
            {
                reader.OnNFCDetailsRead(info);
            }

#elif UNITY_EDITOR
			Debug.LogWarning ("NFC Plugin only works on Android Platform.");
#endif
        }

        public void NFCWrite(string payload)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass javaNFCPluginClass = new AndroidJavaClass("com.questmill.nfc.NFCPlugin");

			javaNFCPluginClass.CallStatic("write", new object[] {
				payload
			});

			Debug.Log("Unity Side write called: " + payload);

#elif UNITY_ANDROID && UNITY_EDITOR
            NFC_Emulator.emulateNFCWrite(payload);

#else
			Debug.LogWarning ("NFC Plugin only works on Android Platform.");
#endif
        }

        private void OnDisable()
        {
            Debug.Log("Destroying NFC_Connector");
            Destroy(connectorGO);
        }

    }

}


