using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Model;
using System;
using System.Globalization;
using GQ.Client.Util;
using System.IO;
using GQ.Client.FileIO;
using GQ.Client.Err;
using GQ.Client.Conf;

namespace GQ.Client.UI
{

    public class ImageCaptureController : PageController
    {

        #region Inspector Features

        public Text text;
        public Image textbg;

        public Button button;

        bool camIsRotated;
        WebCamTexture cameraTexture;
        public RawImage camRawImage;

        #endregion



        #region Runtime API

        protected PageImageCapture myPage;

        /// <summary>
        /// Is called during Start() of the base class, which is a MonoBehaviour.
        /// </summary>
        public override void InitPage_TypeSpecific()
        {
            myPage = (PageImageCapture)page;

            // show the task and button:
            if (myPage.Task != null && myPage.Task != "")
            {
                text.text = myPage.Task;

                /*  Adjust the text size after, because it is rotated by 
                    -90 degrees relative to its parent, but it should be 
                    same position: */
                Rect textRect = text.rectTransform.rect;
                Vector2 textSizeDelta = text.rectTransform.sizeDelta;
                text.rectTransform.sizeDelta = new Vector2(
                    textRect.height - textRect.width,
                    textRect.width - textRect.height
                );
            }
            else
            {
                text.gameObject.SetActive(false);
            }

            // init web cam;
            Base.Instance.StartCoroutine(initWebCam());
        }

        IEnumerator initWebCam()
        {
            string deviceName = null;

            // look for a cam in the preferred direction:
            foreach (WebCamDevice wcd in WebCamTexture.devices)
            {
                if (wcd.isFrontFacing == myPage.PreferFrontCam)
                {
                    deviceName = wcd.name;
                    break;
                }
            }
            // if we did not find a right cam, we use the first cam available:
            if (deviceName == null)
            {
                deviceName = WebCamTexture.devices[0].name;
                Log.SignalErrorToUser(
                    "Your device does not offer a {0} camera, os we can only use what we get: the default camera.",
                    myPage.PreferFrontCam ? "front" : "rear"
                    );
            }

            cameraTexture = new WebCamTexture(deviceName);

            cameraTexture.requestedHeight = 2000;
            cameraTexture.requestedWidth = 3000;

            cameraTexture.Play();

            // wait for web cam to be ready which is guaranteed after first image update:
            while (!cameraTexture.didUpdateThisFrame)
                yield return null;

            // rotate if needed:
            camRawImage.transform.rotation *= Quaternion.AngleAxis(cameraTexture.videoRotationAngle, Vector3.back);

            camIsRotated = Math.Abs(cameraTexture.videoRotationAngle) == 90 || Math.Abs(cameraTexture.videoRotationAngle) == 270;

            float camHeight = (camIsRotated ? cameraTexture.width : cameraTexture.height);
            float camWidth = (camIsRotated ? cameraTexture.height : cameraTexture.width);

            float panelHeight = camRawImage.rectTransform.rect.height;
            float panelWidth = camRawImage.rectTransform.rect.width;

            float heightScale = panelHeight / camHeight;
            float widthScale = panelWidth / camWidth;
            float fitScale = Math.Min(heightScale, widthScale);

            float goalHeight = cameraTexture.height * fitScale;
            float goalWidth = cameraTexture.width * fitScale;

            heightScale = goalHeight / panelHeight;
            widthScale = goalWidth / panelWidth;

            float mirrorAdjustment = cameraTexture.videoVerticallyMirrored ? -1F : 1F;
            // TODO adjust mirror also correct if cam is not rotated:

            camRawImage.transform.localScale = new Vector3(widthScale, heightScale * mirrorAdjustment, 1F);

            camRawImage.texture = cameraTexture;

        }

        public void TakeSnapshot()
        {

            Texture2D photo;

            // we add 360 degrees to avoid any negative values:
            int rotatedClockwiseQuarters = 360 - cameraTexture.videoRotationAngle;

            switch (Input.deviceOrientation)
            {
                case DeviceOrientation.LandscapeLeft:
                    rotatedClockwiseQuarters += 90;
                    break;
                case DeviceOrientation.PortraitUpsideDown:
                    rotatedClockwiseQuarters += 180;
                    break;
                case DeviceOrientation.LandscapeRight:
                    rotatedClockwiseQuarters += 270;
                    break;
                case DeviceOrientation.Portrait:
                case DeviceOrientation.FaceUp:
                case DeviceOrientation.FaceDown:
                default:
                    break;
            }

            rotatedClockwiseQuarters /= 90;  // going from degrees to quarters
            rotatedClockwiseQuarters %= 4; // reducing to 0, 1 ,2 or 3 quarters

            cameraTexture.Pause();
            Color[] pixels = cameraTexture.GetPixels();
            cameraTexture.Stop();

            switch (rotatedClockwiseQuarters)
            {
                case 1:
                    photo = new Texture2D(cameraTexture.height, cameraTexture.width);
                    photo.SetPixels(pixels.Rotate90(cameraTexture.height, cameraTexture.width));
                    break;
                case 2:
                    photo = new Texture2D(cameraTexture.width, cameraTexture.height);
                    photo.SetPixels(pixels.Rotate180(cameraTexture.width, cameraTexture.height));
                    break;
                case 3:
                    photo = new Texture2D(cameraTexture.height, cameraTexture.width);
                    photo.SetPixels(pixels.Rotate270(cameraTexture.height, cameraTexture.width));
                    break;
                case 0:
                default:
                    photo = new Texture2D(cameraTexture.width, cameraTexture.height);
                    photo.SetPixels(pixels);
                    break;
            }
            photo.Apply();

            SaveTextureToCamera(photo);

            OnForward();
        }

        void SaveTextureToCamera(Texture2D texture)
        {
            DateTime now = DateTime.Now;
            string filename = now.ToString("yyyy_MM_dd_HH_mm_ss_fff", CultureInfo.InvariantCulture) + ".jpg";
            string filepath = Files.CombinePath(QuestManager.GetRuntimeMediaPath(myPage.Quest.Id), filename);

            byte[] bytes = texture.EncodeToJPG();

            File.WriteAllBytes(filepath, bytes);
            Variables.SetVariableValue(myPage.File, new Value(filename));

            // save media info for local file under the pseudo variable (e.g. @_imagecapture):
            myPage.Quest.MediaStore[GQML.PREFIX_RUNTIME_MEDIA + myPage.File] =
                new MediaInfo(
                myPage.Quest.Id,
                GQML.PREFIX_RUNTIME_MEDIA + myPage.File,
                QuestManager.GetRuntimeMediaPath(myPage.Quest.Id),
                filename
            );

            // TODO save to mediainfos.json again

            if (File.Exists(filepath))
                Debug.Log("CAMERA: Shot saved to file: " + filepath);
            else
                Debug.Log("CAMERA: ERROR tring to save shot to file: " + filepath);

            NativeGallery.Permission permission = NativeGallery.RequestPermission();
            if (permission == NativeGallery.Permission.Denied && NativeGallery.CanOpenSettings())
            {
                NativeGallery.OpenSettings();
            }
            permission = NativeGallery.SaveImageToGallery(texture, ConfigurationManager.Current.name, filename);
            Debug.Log("PHOTO EXPORTED: " + permission.ToString());

            Destroy(texture); // avoid memory leaks
        }
        #endregion
    }
}
