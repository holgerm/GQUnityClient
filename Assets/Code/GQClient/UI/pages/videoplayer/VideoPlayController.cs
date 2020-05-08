using System.Collections;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Model.pages;
using Code.GQClient.Util;
using Code.QM.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Code.GQClient.UI.pages.videoplayer
{
    public class VideoPlayController : PageController
    {
        #region Inspector Fields

        public GameObject videoPlayerPanel;
        public Text infoText;
        public TextMeshProUGUI forwardButtonText;
        public RawImage videoImage;
        public AudioSource audioSource;
        public VideoPlayer videoPlayerNormal;
        public GameObject containerNormal;
        public GameObject videoControllerPanelNormal;
        internal GameObject videoControllerPanel;
        private Slider videoControllerSlider;
        public VideoPlayer videoPlayer360;
        public Camera camera360;
        public GameObject container360;
        public Image arrow360;
        public GameObject containerWebPlayer;
        public RectTransform webPlayerContent;
        public GameObject webPlayerFooterButtonPanel;
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
        public override void InitPage_TypeSpecific()
        {
            myPage = (PageVideoPlay) page;
            cameraMain = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            forwardButtonText.text = "Ok";

            containerWebPlayer.SetActive(false);
            // DIRECT MP4 LINK TO VIDEO: 
            // setup according to videotype:
            switch (myPage.VideoType)
            {
                case GQML.PAGE_VIDEOPLAY_VIDEOTYPE_NORMAL:
                    videoPlayer = videoPlayerNormal;
                    videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
                    videoControllerPanel = videoControllerPanelNormal;
                    videoControllerSlider = videoControllerPanel.GetComponentInChildren<Slider>();
                    videoControllerPanel.SetActive(false);
                    // enable camera & canvas:
                    containerNormal.SetActive(true);
                    videoImage.enabled = true;
                    containerWebPlayer.SetActive(false);
                    container360.SetActive(false);
                    videoPlayer.started += (source) => { videoControllerPanel.SetActive(myPage.Controllable); };
                    
                    CoroutineStarter.Run(PlayVideo());
                    break;
                case GQML.PAGE_VIDEOPLAY_VIDEOTYPE_360:
                    videoPlayer = videoPlayer360;
                    videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
                    videoControllerPanel = videoControllerPanelNormal;
                    videoControllerSlider = videoControllerPanel.GetComponentInChildren<Slider>();
                    videoControllerPanel.SetActive(false);
                    // switch to 360 Cam and show arrow hint:
                    cameraMain.enabled = false;
                    camera360.enabled = true;
                    // switch to sphere:
                    containerNormal.SetActive(true);
                    HideNormalVideo(true);
                    containerWebPlayer.SetActive(false);
                    container360.SetActive(true);
                    videoPlayer.started += (source) =>
                    {
                        arrow360.enabled = true;
                        videoControllerPanel.SetActive(myPage.Controllable);
                    };
                    CoroutineStarter.Run(PlayVideo());
                    break;
                default:
                    //containerNormal.SetActive(false);
                    videoImage.enabled = false;
                    container360.SetActive(false);
                    WebViewExtras.Initialize(myPage, containerWebPlayer);
                    break;
            }
        }

        /// <summary>
        /// Video page does NOT offer leave quest, because it is confusing with leave video.
        /// </summary>
        internal override bool OfferLeaveQuest
        {
            get { return false; }
        }


        private void HideNormalVideo(bool hide)
        {
            containerNormal.transform.Find("Screen").GetComponent<Image>().enabled = !hide;
            videoImage.GetComponent<RawImage>().enabled = !hide;
            videoPlayerNormal.enabled = !hide;
        }

        private IEnumerator PlayVideo()
        {           
            videoPlayer.playOnAwake = false;
            audioSource.playOnAwake = false;
            audioSource.Pause();

            // set video path:
            if (myPage.Stream)
            {
                videoPlayer.url = myPage.VideoFile;
            }
            else
            {
                if (QuestManager.Instance.MediaStore.ContainsKey(myPage.VideoFile))
                {
                    QuestManager.Instance.MediaStore.TryGetValue(myPage.VideoFile, out var mediaInfo);
                    videoPlayer.url = mediaInfo.LocalPath;
                }
                else
                {
                    Log.WarnDeveloper(
                        "Video file was not loaded into media store, so we let the VideoPlayer load it ... " +
                        myPage.VideoFile);
                    videoPlayer.url = myPage.VideoFile;
                }
            }

            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
            {
                yield return new WaitForEndOfFrame();
            }

            videoPlayer.loopPointReached += (VideoPlayer source) =>
            {
                showPageControls(true);

                if (myPage.VideoType == GQML.PAGE_VIDEOPLAY_VIDEOTYPE_360)
                {
                    OnForward();
                }
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
            showPageControls(false);

            // start Playing:
            videoPlayer.Play();
        }

        private void showPageControls(bool show)
        {
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
            ScaleVideo(orientation);
        }

        private void rotateVideo(DeviceOrientation orient)
        {
            if (myPage.VideoType == GQML.PAGE_VIDEOPLAY_VIDEOTYPE_360)
            {
                Transform camTransform = container360.transform.Find("Camera").transform;
                float new360CamZAngle;
                float newControlsZAngle;
                if (orient == DeviceOrientation.LandscapeLeft)
                {
                    new360CamZAngle = 90f;
                    newControlsZAngle = 270f;
                }
                else
                {
                    new360CamZAngle = 270f;
                    newControlsZAngle = 90f;
                }

                camTransform.eulerAngles =
                    new Vector3(camTransform.eulerAngles.x, camTransform.eulerAngles.y, new360CamZAngle);
                // rotate "normal" video image to rotate controls:
                videoImage.rectTransform.eulerAngles = new Vector3(0, 0, newControlsZAngle);
                return;
            }

            switch (orient)
            {
                case DeviceOrientation.Portrait:
                    videoImage.rectTransform.eulerAngles = new Vector3(0, 0, 0);
                    break;
                case DeviceOrientation.LandscapeRight:
                    videoImage.rectTransform.eulerAngles = new Vector3(0, 0, 90);
                    break;
                case DeviceOrientation.Unknown: // TEST
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

        private void ScaleVideo(DeviceOrientation orient)
        {
            if (myPage.VideoType == GQML.PAGE_VIDEOPLAY_VIDEOTYPE_360)
            {
                if (orient == DeviceOrientation.Portrait || orient == DeviceOrientation.PortraitUpsideDown)
                {
                    orient = DeviceOrientation.LandscapeRight;
                }
            }

            var videoRatio = (float) videoPlayer.texture.width / (float) videoPlayer.texture.height;
            var screenRatio =
                (float) containerNormal.GetComponent<RectTransform>().rect.width /
                (float) containerNormal.GetComponent<RectTransform>().rect.height;

            var xScale = 1.0f;
            var yScale = 1.0f;

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
            if (myPage.VideoType == GQML.PAGE_VIDEOPLAY_VIDEOTYPE_YOUTUBE)
                return;

            if (Device.Orientation != orientation)
            {
                SizeVideoToFitInside(Device.Orientation);
            }

            if (!Input.GetMouseButton(0))
            {
                // auto-proceed the movie slider only when no interaction:
                videoControllerSlider.value = (float) videoPlayer.frame / videoPlayer.frameCount;
            }
        }

        public void ToggleVideoController()
        {
            videoControllerPanelNormal.SetActive(myPage.Controllable && !videoControllerPanelNormal.activeSelf);
        }

        public void ControlVideoBySlider(float newValue)
        {
            if (!videoControllerSlider.IsActive())
                return;

            if (!Input.GetMouseButton(0))
            {
                // prevent a normal update on the slider when the film runs to change frame, hence inhibit cycle.
                return;
            }

            videoPlayer.frame = (long) (videoPlayer.frameCount * newValue);
        }

        public override void CleanUp()
        {
            base.CleanUp();

            WebViewExtras.CleanUp(containerWebPlayer);

            // switch back to main camera:
            camera360.enabled = false;
            cameraMain.enabled = true;

            HideNormalVideo(false);
        }

        #endregion
    }
}