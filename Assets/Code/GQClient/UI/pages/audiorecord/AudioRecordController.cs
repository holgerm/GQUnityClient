using System;
using System.Collections;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using Code.GQClient.Model.expressions;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Model.pages;
using Code.GQClient.Util;
using GQClient.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.pages.audiorecord
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
            myPage = (PageAudioRecord) page;

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
            get { return true; }
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

#if !UNITY_WEBGL // https://forum.unity.com/threads/webgl-and-microphone.308197/
            //Check if there is at least one microphone connected  
            if (Microphone.devices.Length <= 0)
            {
                //TODO Throw a warning message at the console if there isn't  
                Debug.Log("NO MICROPHONE FOUND!");
                Log.SignalErrorToUser($"No microphone found. Can not record audio. Occurred on device of type {SystemInfo.deviceModel}.");
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
#endif
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
            get { return StopRecordingImage.activeSelf; }
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
            get { return StopPlayingImage.activeSelf; }
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

        public bool HasRecorded { get; private set; }

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

        readonly SaveWav saver = new SaveWav();

        public void PressRecord()
        {
#if !UNITY_WEBGL // https://forum.unity.com/threads/webgl-and-microphone.308197/
            if (IsRecording)
            {
                // pressed Record again while playíng ends the recoding and saves the file:
                Microphone.End(microphoneName);
                myPage.PageCtrl.FooterButtonPanel.SetActive(true);
                audioSource.Play();
                // TODO save audio to file!
                string filename = Quest.GetRuntimeMediaFileName(".wav");
                string path = Files.CombinePath(QuestManager.GetRuntimeMediaPath(myPage.Quest.Id), filename);
                Variables.SetVariableValue(myPage.FileName, new Value(filename));

                Debug.Log("TODO save audio to file: " + path);
                saver.Save(path, saver.TrimSilence(audioSource.clip, 0.012f));

                // save media info for local file under the pseudo variable (e.g. @_imagecapture):
                string relDir = Files.CombinePath(QuestInfoManager.QuestsRelativeBasePath, myPage.Quest.Id.ToString(), "runtime");
                myPage.Quest.MediaStore[GQML.PREFIX_RUNTIME_MEDIA + myPage.FileName] =
                    new MediaInfo(
                        myPage.Quest.Id,
                        GQML.PREFIX_RUNTIME_MEDIA + myPage.FileName,
                        relDir, 
                        filename
                    );
 
                // TODO save to mediainfos.json again
                
                IsRecording = false;
            }
            else
            {
                int freq = Math.Min(maxFreq, 44100);
                Debug.Log("MaxFreq: " + freq);
                myPage.PageCtrl.FooterButtonPanel.SetActive(false);
                audioSource.clip = 
                    Microphone.Start(
                        microphoneName, 
                        true, 
                        myPage.MaxRecordTime, 
                        freq);
                IsRecording = true;
                HasRecorded = true;
            }
#endif
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
#if !UNITY_WEBGL // https://forum.unity.com/threads/webgl-and-microphone.308197/
            if (Microphone.IsRecording(microphoneName))
            {
                length += Time.deltaTime;
            }
#endif
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