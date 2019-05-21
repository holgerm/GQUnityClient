using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace QM.NFC
{

    public class NFC_Reader : MonoBehaviour, NFC_Reader_I
    {
        // TODO automatically search for suitable TEXT fields in Inspector (cf. old code)

        [NFCReadTextPropertyAttribute]
        public Text NFCPayloadText;
        public NFCReadPayloadEvent OnNFCReadPayload;

        public Text NFCIdText;

        public NFCReadDetailsEvent OnNFCReadDetails;

        // Set the Reader UI Text Element:
        void Start()
        {
            if (NFCPayloadText == null)
                setReaderText();

        }

        void setReaderText()
        {
            Text myText = gameObject.GetComponent<Text>();
            if (myText != null)
            {
                NFCPayloadText = myText;
            }
            else
            {
                NFCPayloadText = (Text)FindObjectOfType(typeof(Text));
            }
        }

        void OnEnable()
        {
            NFC_Connector.Connector.RegisterReaderUI(this);
        }

        void OnDisable()
        {
            NFC_Connector.Connector.UnregisterReaderUI(this);
        }

        void OnDestroy()
        {
            NFC_Connector.Connector.UnregisterReaderUI(this);
        }

        /// <summary>
        /// Called by Android Java Plugin when an NFC Tag is read. The read payload is given as parameter. Called via NFC_Connector.
        /// </summary>
        /// <param name="payload">Payload.</param>
        public void OnNFCPayloadRead(string payload)
        {
#if UNITY_ANDROID
            if (NFCPayloadText != null)
            {
                NFCPayloadText.text = payload;
            }
            if (OnNFCReadPayload != null)
            {
                OnNFCReadPayload.Invoke(payload);
            }
#elif UNITY_EDITOR
		Debug.LogWarning ("NFC Plugin only works on Android Platform.");
#endif
        }

        /// <summary>
        /// Called by Android Java Plugin when an NFC Tag is read. Called via NFC_Connector.
        /// The read details are given as parameter and are unmarshalled first. 
        /// Then the different contents are made available to the game by triggering an event.
        /// </summary>
        /// <param name="id">ID.</param>
        public void OnNFCDetailsRead(NFC_Info info)
        {
#if UNITY_ANDROID
            if (NFCIdText != null)
            {
                NFCIdText.text = info.Id;
            }
            if (NFCPayloadText != null)
            {
                NFCPayloadText.text = info.Payload;
            }
            if (OnNFCReadDetails != null)
                OnNFCReadDetails.Invoke(info);
#elif UNITY_EDITOR
		Debug.LogWarning ("NFC Plugin only works on Android Platform.");
#endif
        }

    }
}