using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using GQ.Client.Conf;
using UnityEngine.Video;
using GQ.Client.Err;

namespace GQ.Client.UI
{
    public class VideoPlayController : PageController
    {

        #region Inspector Fields

        public GameObject contentPanel;
        public Text infoText;
        public Text forwardButtonText;
        public RawImage videoImage;
        public VideoPlayer videoPlayerNormal;
        public VideoPlayer videoPlayer360;
        public Camera camera360;
        protected Camera cameraMain;
        public GameObject containerNormal;
        public GameObject container360;
        public AudioSource audioSource;

        #endregion


        #region Runtime API

        protected PageVideoPlay myPage;
        protected VideoPlayer videoPlayer;

        /// <summary>
        /// Is called during Start() of the base class, which is a MonoBehaviour.
        /// </summary>
        public override void Initialize()
        {
            myPage = (PageVideoPlay)page;
            cameraMain = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

            // setup according to videotype:
            switch (myPage.VideoType) {
                case GQML.PAGE_VIDEOPLAY_VIDEOTYPE_NORMAL:
                    videoPlayer = videoPlayerNormal;
                    // enable camera & canvas:
                    break;
                case GQML.PAGE_VIDEOPLAY_VIDEOTYPE_360:
                    videoPlayer = videoPlayer360;
                    // switch to 360 Cam:
                    cameraMain.enabled = false;
                    camera360.enabled = true;
                    // switch to sphere:
                    containerNormal.SetActive(false);
                    container360.SetActive(true);
                    break;
                default:
                    Log.SignalErrorToAuthor("Unknown video type {0} used on page {1}", myPage.VideoType, myPage.Id);
                    break;
            }

            // show the content:
            showInfo();
            forwardButtonText.text = "Ok";

            CoroutineStarter.Run(playVideo());
        }

        IEnumerator playVideo()
        {
            videoPlayer.playOnAwake = false;
            audioSource.playOnAwake = false;
            audioSource.Pause();

            // set video path:
            if (myPage.Parent.MediaStore.ContainsKey(myPage.VideoFile))
            {
                MediaInfo mediaInfo;
                myPage.Parent.MediaStore.TryGetValue(myPage.VideoFile, out mediaInfo);
                videoPlayer.url = mediaInfo.LocalPath;
            }
            else
            {
                Debug.Log("Video file was not loaded into media store, so we let the VideoPlayer load it ... " +
                          myPage.VideoFile);
                videoPlayer.url = myPage.VideoFile;
            }

            videoPlayer.Prepare();

            int secondsWaited = 0;
            while (!videoPlayer.isPrepared && secondsWaited < 6)
            {
                yield return new WaitForSeconds(1);
                secondsWaited++;
            }

            videoPlayer.loopPointReached += (VideoPlayer source) => {
                OnForward();
            };
            // set the rawimage texture:
            videoImage.texture = videoPlayer.texture;

            // start Playing:
            videoPlayer.Play();

            // loop while playing:
            Debug.Log("VideoPlayer started.");
            //while (videoPlayer.isPlaying)
            //{
            //    Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));
            //}

            Debug.Log("Done with VideoPlaying.");
        }

        public override void CleanUp()
        {
            base.CleanUp();

            // switch back to main camera:
            camera360.enabled = false;
            cameraMain.enabled = true;
        }

        void showInfo()
        {
            infoText.text =
                "Diese Funktion steht leider noch nicht zur Verfügung. Hier werden als Test die Informationen angezeigt, die in der Quest-Seite gespeichert wurden:\n\n" +
                "type:\t\t\t" + myPage.PageType + "\n" +
                "id:\t\t\t\t\t" + myPage.Id + "\n" +
                "file:\t\t\t" + myPage.VideoFile + "\n" +
                "cotrollable:\t" + myPage.Controllable + "\n" +
                        "videotype:\t\t\t\t" + myPage.VideoType;
        }

        #endregion
    }
}
