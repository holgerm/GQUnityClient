using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using System;
using GQ.Client.Err;
using TMPro;

namespace GQ.Client.UI
{
    public class AudioRecordController : PageController
    {

        #region Inspector Fields

        public GameObject contentPanel;
        public TextMeshProUGUI infoText;
        public TextMeshProUGUI forwardButtonText;
        public AudioSource audioSource;

        #endregion


        #region Runtime API
        protected PageAudioRecord myPage;

        private int minFreq;
        private int maxFreq;

        /// <summary>
        /// Is called during Start() of the base class, which is a MonoBehaviour.
        /// </summary>
        public override void InitPage_TypeSpecific()
        {
            myPage = (PageAudioRecord)page;

            // show the content:
            infoText.text = myPage.PromptText.Decode4TMP(true);
            forwardButtonText.text = "Ok";

            // init state:
            StartRecordingImage = RecordButton.transform.Find("StartRecordingImage").gameObject;
            StopRecordingImage = RecordButton.transform.Find("StopRecordingImage").gameObject;
            StartPlayingImage = PlayButton.transform.Find("StartPlayingImage").gameObject;
            StopPlayingImage = PlayButton.transform.Find("StopPlayingImage").gameObject;
            IsRecording = false;
            IsPlaying = false;
            HasRecorded = false;
            UpdateView();

            CoroutineStarter.Instance.StartCoroutine(InitMicrophone());
        }

        /// <summary>
        /// Shows top margin:
        /// </summary>
        public override bool ShowsTopMargin
        {
            get
            {
                return true;
            }
        }

        protected string microphoneName;

        private IEnumerator InitMicrophone()
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                //TODO Throw a warning message at the console that we can not record audio without permission:
                Debug.Log("NO MICROPHONE PERMISSION!");
                Log.SignalErrorToUser("No permission to use microphone. Can not record audio.");
            }
            else
            {
                Debug.Log("Permission received. Thank you!");
            }

            //Check if there is at least one microphone connected  
            if (Microphone.devices.Length <= 0)
            {
                //TODO Throw a warning message at the console if there isn't  
                Debug.Log("NO MICROPHONE FOUND!");
                Log.SignalErrorToUser("No microphone found. Can not record audio. Occurred on device of type {0}.", SystemInfo.deviceModel);
            }
            else
            {
                //Get the default microphone recording capabilities  
                microphoneName = Microphone.devices[0];
                Microphone.GetDeviceCaps(microphoneName, out minFreq, out maxFreq);

                //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
                if (minFreq == 0 && maxFreq == 0)
                {
                    //...meaning 44100 Hz can be used as the recording sampling rate  
                    maxFreq = 44100;
                }

            }
        }

        #endregion


        #region Runtime Status
        public Button RecordButton;
        public Button PlayButton;
        public Button DeleteButton;

        GameObject StartRecordingImage;
        GameObject StopRecordingImage;
        GameObject StartPlayingImage;
        GameObject StopPlayingImage;

        private void UpdateView()
        {
            RecordButton.interactable = IsRecording || !HasRecorded;
            PlayButton.interactable = HasRecorded && !IsRecording;
            DeleteButton.interactable = HasRecorded;
        }

        public bool IsRecording
        {
            get
            {
                return StopRecordingImage.activeSelf;
            }
            private set
            {
                if (value)
                {
                    StopRecordingImage.SetActive(true);
                    StartRecordingImage.SetActive(false);
                    RecordButton.targetGraphic = StopRecordingImage.GetComponent<Image>();
                }
                else
                {
                    StartRecordingImage.SetActive(true);
                    StopRecordingImage.SetActive(false);
                    RecordButton.targetGraphic = StartRecordingImage.GetComponent<Image>();
                }
                UpdateView();
            }
        }

        public bool IsPlaying
        {
            get
            {
                return StopPlayingImage.activeSelf;
            }
            private set
            {
                if (value)
                {
                    StopPlayingImage.SetActive(true);
                    StartPlayingImage.SetActive(false);
                    PlayButton.targetGraphic = StopPlayingImage.GetComponent<Image>();
                }
                else
                {
                    StartPlayingImage.SetActive(true);
                    StopPlayingImage.SetActive(false);
                    PlayButton.targetGraphic = StartPlayingImage.GetComponent<Image>();
                }
                UpdateView();
            }
        }

        public bool HasRecorded
        {
            get;
            private set;
        }

        public void PressPlay()
        {
            if (IsPlaying)
            {
                audioSource.Stop();
                IsPlaying = false;
            }
            else
            {
                audioSource.Play();
                IsPlaying = true;
            }
            UpdateView();

        }
        
        public void PressRecord()
        {
            if (IsRecording)
            {
                Microphone.End(microphoneName);
                audioSource.Play();
                // TODO save audio to file!
                IsRecording = false;
            }
            else
            {
                // Length is fixed to 60 seconds. This should be sepcified by author in quest.
                Debug.Log("Recording started with microphone: " + microphoneName + 
                    " ac.length: " + (audioSource.clip == null ? "null" : audioSource.clip.length.ToString()) + 
                    " samples: " + (audioSource.clip == null ? "null" : audioSource.clip.samples.ToString()));
                int freq = Math.Min(maxFreq, 44100);
                Debug.Log("MaxFreq: " + freq);
                audioSource.clip = Microphone.Start(microphoneName, true, 10, freq);
                IsRecording = true;
                HasRecorded = true;
            }
            UpdateView();
        }

        public void PressDelete()
        {
            // TODO: remove audio file.
            HasRecorded = false;
            IsRecording = false;
            IsPlaying = false;
            UpdateView();
        }

        private float length = 0f;
        private float playlength = 0f;

        void Update()
        {
            if (Microphone.IsRecording(microphoneName))
            {
                length += Time.deltaTime;
            }
            if (IsRecording)
            {
                Debug.Log("Recording ...");
                if (!Microphone.IsRecording(microphoneName))
                {
                    Debug.Log("RECORDING HAS ENDED.".Yellow());
                }
                else
                {
                    Debug.Log("@ pos: " + Microphone.GetPosition(microphoneName));
                }
            }
            if (IsPlaying)
            {
                playlength += Time.deltaTime;

                if (!audioSource.isPlaying)
                {
                    IsPlaying = false;
                }
            }
        }
        #endregion


    }
}
