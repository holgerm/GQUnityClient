//#define DEBUG_LOG

using UnityEngine;
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
        private static List<NFC_Reader_I> _registeredReaderUIs;
        private static List<NFC_Reader_I> RegisteredReaderUIs
        {
            get
            {
                if (_registeredReaderUIs == null)
                {
                    _registeredReaderUIs = new List<NFC_Reader_I>();
                }
                return _registeredReaderUIs;
            }
        }


        public bool RegisterReaderUI(NFC_Reader_I newNFCReaderUI)
        {
            if (RegisteredReaderUIs.Contains(newNFCReaderUI))
                return false;
            else
            {
                RegisteredReaderUIs.Add(newNFCReaderUI);
                return true;
            }
        }

        public bool UnregisterReaderUI(NFC_Reader_I nfcReaderUI)
        {
            if (RegisteredReaderUIs == null)
            {
                return false;
            }
            else
            {
                return RegisteredReaderUIs.Remove(nfcReaderUI);
            }
        }

        /// <summary>
        /// Called by Android Java Plugin when an NFC Tag is read. The read payload is given as parameter. 
        /// </summary>
        /// <param name="payload">Payload.</param>
        public void NFCReadPayload(string payload)
        {
            string nonNullPayLoad = (payload == null ? "" : payload);
#if UNITY_ANDROID
            foreach (NFC_Reader_I reader in RegisteredReaderUIs)
            {
                reader.OnNFCPayloadRead(nonNullPayLoad);
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
#if DEBUG_LOG
            Debug.LogFormat("GQ: NFC CONNECTOR: marshalledCOntent: {0}", marshalledContent);
            Debug.LogFormat("GQ: NFC CONNECTOR: info.payload: {0}", info.Payload);
#endif

            foreach (NFC_Reader_I reader in RegisteredReaderUIs.ToArray())
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
            Destroy(connectorGO);
        }

    }

}


