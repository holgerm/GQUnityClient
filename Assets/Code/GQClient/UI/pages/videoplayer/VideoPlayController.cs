using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using GQ.Client.Util;
using UnityEngine.Video;
using GQ.Client.Err;
using QM.Util;
using GQ.Client.Conf;

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
        public GameObject header;
        public GameObject footer;
        public Image background;

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
                    containerNormal.SetActive(false);
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
                showControls(true);

                //OnForward();
            };

            // set the rawimage texture:
            //videoImage.rectTransform.localScale = new Vector3(1f, videoPlayer.texture.height / videoPlayer.texture.width, 1f);
            videoImage.texture = videoPlayer.texture;

            // If the device is faceUp or down at start, we use portrait:
            if (Device.Orientation == DeviceOrientation.FaceDown || Device.Orientation == DeviceOrientation.FaceUp)
            {
                SizeVideoToFitInside(DeviceOrientation.Portrait);
            }
            else
            {
                SizeVideoToFitInside(Device.Orientation);
            }

            videoPlayer.SetTargetAudioSource(0, audioSource);
            play();
        }

        private void play()
        {
            showControls(false);

            // start Playing:
            videoPlayer.Play();
        }

        private void showControls(bool show) {
            header.SetActive(show);
            footer.SetActive(show);
            if (show)
                background.color = ConfigurationManager.Current.contentBackgroundColor;
            else
                background.color = Color.black;
        }

        private static DeviceOrientation orientation;

        private void SizeVideoToFitInside(DeviceOrientation curOrientation)
        {
            orientation = curOrientation;
            rotateVideo(orientation);
            scaleVideo(orientation);
        }

        private void rotateVideo(DeviceOrientation orient)
        {
            switch (orient)
            {
                case DeviceOrientation.Portrait:
                    videoImage.rectTransform.eulerAngles = new Vector3(0, 0, 0);
                    break;
                case DeviceOrientation.Unknown: // TEST
                case DeviceOrientation.LandscapeRight:
                    videoImage.rectTransform.eulerAngles = new Vector3(0, 0, 90);
                    break;
                case DeviceOrientation.PortraitUpsideDown:
                    videoImage.rectTransform.eulerAngles = new Vector3(0, 0, 180);
                    break;
                case DeviceOrientation.LandscapeLeft:
                    videoImage.rectTransform.eulerAngles = new Vector3(0, 0, 270);
                    break;
                default:
                    break;
            }
        }

        void scaleVideo(DeviceOrientation orient) {
            float videoRatio = (float)videoPlayer.texture.width / (float)videoPlayer.texture.height;
            float screenRatio =
                (float)containerNormal.GetComponent<RectTransform>().rect.width /
                (float)containerNormal.GetComponent<RectTransform>().rect.height;

            float xScale = 1.0f;
            float yScale = 1.0f;

            switch (orient)
            {
                case DeviceOrientation.Portrait:
                case DeviceOrientation.PortraitUpsideDown:
                    if (videoRatio >= screenRatio)
                    {
                        // CASE 1:
                        xScale = 1.0f;
                        yScale = screenRatio / videoRatio;
                    }
                    else
                    {
                        // CASE 2:
                        xScale = videoRatio / screenRatio;
                        yScale = 1.0f;
                    }
                    videoImage.rectTransform.localScale = new Vector3(xScale, yScale, 1.0f);
                    break;
                case DeviceOrientation.Unknown: // TEST
                case DeviceOrientation.LandscapeRight:
                case DeviceOrientation.LandscapeLeft:
                    if (videoRatio >= 1 / screenRatio)
                    {
                        // CASE 3:
                        xScale = 1 / screenRatio;
                        yScale = xScale * (screenRatio / videoRatio);
                    }
                    else
                    {
                        // CASE 4:
                        xScale = videoRatio;
                        yScale = screenRatio;
                    }
                    videoImage.rectTransform.localScale = new Vector3(xScale, yScale, 1.0f);
                    break;
                default:
                    break;
            }
        }

        public void Update()
        {
            if (Device.Orientation != orientation) {
                SizeVideoToFitInside(Device.Orientation);
            }
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
