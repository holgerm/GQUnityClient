using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Conf;
using System;

namespace GQ.Client.UI
{
    public class AudioRecordController : PageController
    {

        #region Inspector Fields

        public GameObject contentPanel;
        public Text infoText;
        public Text forwardButtonText;
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
            infoText.text = myPage.PromptText;
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

            InitMicrophone();
        }

        private void InitMicrophone()
        {
            //Check if there is at least one microphone connected  
            if (Microphone.devices.Length <= 0)
            {
                //TOTO Throw a warning message at the console if there isn't  
            }
            else
            {
                //Get the default microphone recording capabilities  
                Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

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
                IsPlaying = false;
            }
            else
            {
                IsPlaying = true;
            }
            UpdateView();
        }

        public void PressRecord()
        {
            if (IsRecording)
            {
                IsRecording = false;
            }
            else
            {
                IsRecording = true;
                HasRecorded = true;
            }
            UpdateView();
        }

        public void PressDelete()
        {
            HasRecorded = false;
            IsRecording = false;
            IsPlaying = false;
            UpdateView();
        }
        #endregion


    }
}
