using System;
using Code.GQClient.Conf;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Model.pages;
using Code.GQClient.UI.pages.videoplayer;
using Code.QM.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.pages.interactiveSphericalImage
{
    public class InteractiveSphericalImageController : PageController
    {
        #region Inspector Fields

        public GameObject videoPlayerPanel;
        public Text infoText;
        public TextMeshProUGUI forwardButtonText;
        public RawImage videoImage;
        public AudioSource audioSource;
        public GameObject videoControllerPanelNormal;
        internal GameObject videoControllerPanel;
        public Camera camera360;
        public TouchLook touchLook360;
        public MouseLook mouseLook360;
        public GameObject container360;
        public Image arrow360;
        protected Camera cameraMain;
        public GameObject header;
        public GameObject footer;
        public Image background;
        public GameObject canvas;

        #endregion


        #region Runtime API

        protected PageInteractiveSphericalImage myPage;

        protected ScreenOrientation orientationBefore;

        /// <summary>
        /// Is called during Start() of the base class, which is a MonoBehaviour.
        /// </summary>
        public override void InitPage_TypeSpecific()
        {
            myPage = (PageInteractiveSphericalImage)page;
            cameraMain = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            forwardButtonText.text = "Ok";

            videoControllerPanel = videoControllerPanelNormal;
            videoControllerPanel.SetActive(false);
            // switch to 360 Cam and show arrow hint:
            cameraMain.enabled = false;
            QuestManager.Instance.LoadImageToTexture(
                myPage.SphericalImage,
                texture =>
                    container360.GetComponent<Renderer>().material.mainTexture = texture
            );
            // switch to sphere:
            canvas.SetActive(false);
            camera360.enabled = true;
            container360.SetActive(true);

            // switch to Autorotation:
            orientationBefore = Screen.orientation;
            Screen.orientation = ScreenOrientation.AutoRotation;

            // rotate image to make the horizontal middle in north position and starting position:
            // the image starts otherwise at the 1/4 horizontal position, hence we rotate by 90 degrees
            touchLook360.ResetRotation(Quaternion.Euler(0f, 90f, 0f));
            mouseLook360.ResetRotation(Quaternion.Euler(0f, 90f, 0f));

            // controller for user interaction to camera rotation:
            MouseLook mouselook = camera360.GetComponent<MouseLook>();
            TouchLook touchlook = camera360.GetComponent<TouchLook>();
#if UNITY_EDITOR
            if (mouselook) mouselook.enabled = true;
            if (touchlook) touchlook.enabled = false;
#else
            if (mouselook) mouselook.enabled = false;
            if (touchlook) touchlook.enabled = true;
#endif

            // setting interactive sprites into the sphere:
            // first: remove any existing (e.g. from previous scene of same kind)
            for (int i = 0; i < container360.transform.childCount; i++)
            {
                Transform oldInteractionT = container360.transform.GetChild(i);
                if (oldInteractionT.GetComponent<InteractionCtrl>())
                    Destroy(oldInteractionT.gameObject);
            }

            // second create new gameobjects:
            foreach (var interaction in myPage.Interactions)
            {
                if (!String.IsNullOrEmpty(interaction.Icon))
                    InteractionCtrl.Create(container360, interaction, camera360);
            }
        }

        /// <summary>
        /// Video page does NOT offer leave quest, because it is confusing with leave video.
        /// </summary>
        internal override bool OfferLeaveQuest
        {
            get { return false; }
        }


        private void showPageControls(bool show)
        {
            header.SetActive(show);
            footer.SetActive(show);
            if (show)
                background.color = Config.Current.contentBackgroundColor;
            else
                background.color = Color.black;
        }

        private static DeviceOrientation orientation;

        private void SizeImageToFitInside(DeviceOrientation curOrientation)
        {
            orientation = curOrientation;
            rotateVideo(orientation);
            ScaleVideo(orientation);
        }

        private void rotateVideo(DeviceOrientation orient)
        {
            // --> RotateImage ???
            // if (myPage.VideoType == GQML.PAGE_VIDEOPLAY_VIDEOTYPE_360)
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
                // return;
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
            // --> ScaleImage
            // if (myPage.VideoType == GQML.PAGE_VIDEOPLAY_VIDEOTYPE_360)
            {
                if (orient == DeviceOrientation.Portrait || orient == DeviceOrientation.PortraitUpsideDown)
                {
                    orient = DeviceOrientation.LandscapeRight;
                }
            }

            Texture imageTexture = container360.GetComponent<Material>().mainTexture;
            var videoRatio =
                (float)imageTexture.width / (float)imageTexture.height;
            var screenRatio = 1f;
            /*
                (float) containerNormal.GetComponent<RectTransform>().rect.width /
                (float) containerNormal.GetComponent<RectTransform>().rect.height;
*/
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

        // public void Update()
        // {
        //     if (Device.Orientation != orientation)
        //     {
        //         SizeImageToFitInside(Device.Orientation);
        //     }
        // }

        public override void CleanUp()
        {
            base.CleanUp();

            // switch back to main camera:
            if (canvas)
                canvas.SetActive(true);
            if (camera360)
                camera360.enabled = false;
            if (cameraMain)
                cameraMain.enabled = true;

            // Back to orientation as ist was before we started this page:
            Screen.orientation = orientationBefore;
        }

        #endregion
    }
}