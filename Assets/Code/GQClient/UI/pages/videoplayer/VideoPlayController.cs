using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using UnityEngine.Video;
using GQ.Client.Err;

namespace GQ.Client.UI
{
    public class VideoPlayController : PageController
    {

        #region Inspector Fields

        public GameObject videoPlayerPanel;
        public Text infoText;
        public Text forwardButtonText;
        public RawImage videoImage;
        public AudioSource audioSource;
        public VideoPlayer videoPlayerNormal;
        public GameObject containerNormal;
        public VideoPlayer videoPlayer360;
        public Camera camera360;
        public GameObject container360;
        public GameObject containerWebPlayer;
        public RectTransform webPlayerContent;
        protected Camera cameraMain;

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
            forwardButtonText.text = "Ok";

            containerWebPlayer.SetActive(false);
            // DIRECT MP4 LINK TO VIDEO: 
            // setup according to videotype:
            switch (myPage.VideoType)
            {
                case GQML.PAGE_VIDEOPLAY_VIDEOTYPE_NORMAL:
                    videoPlayer = videoPlayerNormal;
                    // enable camera & canvas:
                    containerNormal.SetActive(true);
                    videoImage.enabled = true;
                    containerWebPlayer.SetActive(false);
                    container360.SetActive(false);
                    CoroutineStarter.Run(playVideo());
                    break;
                case GQML.PAGE_VIDEOPLAY_VIDEOTYPE_360:
                    videoPlayer = videoPlayer360;
                    // switch to 360 Cam:
                    cameraMain.enabled = false;
                    camera360.enabled = true;
                    // switch to sphere:
                    //containerNormal.SetActive(false);
                    containerWebPlayer.SetActive(false);
                    container360.SetActive(true);
                    CoroutineStarter.Run(playVideo());
                    break;
               default:
                    //containerNormal.SetActive(false);
                    videoImage.enabled = false;
                    container360.SetActive(false);
                    VideoPlayerExtraModes.Initialize(myPage, containerWebPlayer);
                    break;
            }
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
                Log.WarnDeveloper("Video file was not loaded into media store, so we let the VideoPlayer load it ... " +
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

            videoPlayer.loopPointReached += (VideoPlayer source) =>
            {
                OnForward();
            };

            // set the rawimage texture:
            //videoImage.rectTransform.localScale = new Vector3(1f, videoPlayer.texture.height / videoPlayer.texture.width, 1f);
            videoImage.texture = videoPlayer.texture;

            SizeVideoToFitInside();

            // start Playing:
            videoPlayer.Play();
        }

        private void SizeVideoToFitInside()
        {
            float screenWidth = containerNormal.GetComponent<RectTransform>().rect.width;
            float screenHeight = containerNormal.GetComponent<RectTransform>().rect.height;


            if (videoPlayer.texture.width / videoPlayer.texture.height >= screenWidth / screenHeight)
            {
                // movie is too wide:
                float targetWidth = screenWidth;
                float targetHeight = videoPlayer.texture.height * screenWidth / videoPlayer.texture.width; 

                videoImage.rectTransform.SetInsetAndSizeFromParentEdge(
                    RectTransform.Edge.Left, 
                    0f, 
                    targetWidth);

                videoImage.rectTransform.SetInsetAndSizeFromParentEdge(
                    RectTransform.Edge.Top,
                    (videoImage.transform.parent.GetComponent<RectTransform>().rect.height - targetHeight) / 2,
                    targetHeight);
            }
            else {
                // movie is too tall:
                float targetWidth = videoPlayer.texture.width * screenHeight / videoPlayer.texture.height;
                float targetHeight = screenHeight;

                videoImage.rectTransform.SetInsetAndSizeFromParentEdge(
                    RectTransform.Edge.Left,
                    (videoImage.transform.parent.GetComponent<RectTransform>().rect.width - targetWidth) / 2,
                    targetWidth);

                videoImage.rectTransform.SetInsetAndSizeFromParentEdge(
                    RectTransform.Edge.Top,
                    0f,
                    targetHeight);
            }
        }

        public void OnRectTransformDimensionsChange(ScreenOrientation oldOrientation, ScreenOrientation newOrientation) {
            Debug.Log("OnRectTransformDimensionsChange() called. Orientation changed from: " + 
                      oldOrientation.ToString() + " to: " + newOrientation.ToString());
        }

        public override void CleanUp()
        {
            base.CleanUp();

            VideoPlayerExtraModes.CleanUp(containerWebPlayer);

            // switch back to main camera:
            camera360.enabled = false;
            cameraMain.enabled = true;
        }

        #endregion
    }
}
