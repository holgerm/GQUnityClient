﻿// #define DEBUG_LOG

using System;
using System.Collections;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Model.pages;
using Code.GQClient.UI.layout;
using Code.GQClient.Util;
using Code.GQClient.Util.http;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Code.GQClient.UI.pages.npctalk
{
    public class NPCTalkController : PageController
    {
        #region Inspector Fields

        public RawImage image;
        public GameObject imagePanel;
        public GameObject contentPanel;
        public Transform dialogItemContainer;

        #endregion

        #region Runtime API

        private PageNPCTalk NpcPage;

        /// <summary>
        /// Is called during Start() of the base class, which is a MonoBehaviour.
        /// </summary>
        public override void InitPage_TypeSpecific()
        {
            try
            {
                NpcPage = (PageNPCTalk) page;
            }
            catch (InvalidCastException)
            {
                Log.SignalErrorToDeveloper(
                    $"PAGE CAST PROBLEM: NPCTalkController found a page of type {page.GetType()}");
            }

            // show the content:
            ShowImage(NpcPage.ImageUrl, imagePanel, layout.TopMargin);
            ClearText();
            AddCurrentText();
            UpdateForwardButton();
        }

        public override void OnForward()
        {
            if (NpcPage.HasMoreDialogItems())
            {
                NpcPage.Next();
                // update the content:
                AddCurrentText();
                UpdateForwardButton();
            }
            else
            {
                base.OnForward();
            }
        }

        public override void CleanUp()
        {
            Destroy(image.texture);
#if DEBUG_LOG
            Debug.Log("NPCTalkController cleaned up image texture.".Red());
#endif
        }

        #endregion

        #region View Update Methods

        /// <summary>
        /// Shows top margin when no image is shown, i.e. text follows directly below header:
        /// </summary>
        public override bool ShowsTopMargin
        {
            get
            {
                if (NpcPage == null)
                {
                    return false;
                }
                else
                {
                    return string.IsNullOrEmpty(NpcPage.ImageUrl);
                }
            }
        }

        protected override void ImageDownloadCallback(AbstractDownloader d, DownloadEvent e)
        {
            DownloadHandlerTexture dhTexture = null;
            
            try
            {
                dhTexture = (DownloadHandlerTexture) d.DownloadHandler;
            }
            catch (InvalidCastException) {
                d.WebRequest.Dispose();
                return;
            }
            
            var imageAreaHeight = image == null ? 0f : FitInAndShowImage(dhTexture.texture);

            imagePanel.GetComponent<LayoutElement>().flexibleHeight = LayoutConfig.Units2Pixels(imageAreaHeight);
            contentPanel.GetComponent<LayoutElement>().flexibleHeight = CalculateMainAreaHeight(imageAreaHeight);

            // Dispose www including it s Texture and take some logs for performance surveillance:
            d.WebRequest.Dispose();
        }

        private float FitInAndShowImage(Texture texture)
        {
            var fitter = image.GetComponent<AspectRatioFitter>();
            var imageRatio = texture.width / (float) texture.height;
            var imageAreaHeight =
                ContentWidthUnits / imageRatio; // if image fits, so we use its height (adjusted to the area):

            if (imageRatio < ImageRatioMinimum)
            {
                // image too high to fit:
                imageAreaHeight = Config.Current.imageAreaHeightMaxUnits;
            }

            if (ImageRatioMaximum < imageRatio)
            {
                // image too wide to fit:
                imageAreaHeight = Config.Current.imageAreaHeightMinUnits;
            }

            //imagePanel.GetComponent<LayoutElement> ().flexibleHeight = LayoutConfig.Units2Pixels (imageAreaHeight);
            //contentPanel.GetComponent<LayoutElement> ().flexibleHeight = CalculateMainAreaHeight (imageAreaHeight);

            fitter.aspectRatio = imageRatio; // i.e. the adjusted image area aspect ratio
            fitter.aspectMode =
                Config.Current.fitExceedingImagesIntoArea
                    ? AspectRatioFitter.AspectMode.FitInParent
                    : AspectRatioFitter.AspectMode.EnvelopeParent;

            image.texture = texture;
            imagePanel.SetActive(true);

            return imageAreaHeight;
        }

        private void ClearText()
        {
            foreach (Transform dialogItem in dialogItemContainer)
            {
                GameObject.Destroy(dialogItem.gameObject);
            }

            var bgImg = contentPanel.GetComponent<Image>();
            if (bgImg != null)
            {
                bgImg.color = Config.Current.mainBgColor;
            }
        }

        private void AddCurrentText()
        {
            var currentText = NpcPage.CurrentDialogItem.Text;

            // create dialog item GO from prefab (also decodes text for HyperText Component):
            TextElementCtrl.Create(dialogItemContainer, currentText);

            // play audio if specified:
            var duration = 0f;
            if (!string.IsNullOrEmpty(NpcPage.CurrentDialogItem.AudioURL))
                duration = Audio.PlayFromMediaStore(NpcPage.CurrentDialogItem.AudioURL);

            if (Config.Current.autoScrollNewText)
            {
                if (Math.Abs(duration) < 0.01)
                    duration = currentText.Length / 14f;
                // ca. 130 words à 6,5 letters pro Minute cf. https://de.wikipedia.org/wiki/Lesegeschwindigkeit

                // scroll to bottom:
                Base.Instance.StartCoroutine(AdjustScrollRect(duration));
            }
        }

        private void UpdateForwardButton()
        {
            // update forward button text:
            var forwardButtonText = forwardButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            forwardButtonText.text = NpcPage.HasMoreDialogItems()
                ? NpcPage.NextDialogButtonText.Decode4TMP(false)
                : NpcPage.EndButtonText.Decode4TMP(false);
        }

        private IEnumerator AdjustScrollRect(float timespan)
        {
            yield return new WaitForEndOfFrame();

            var usedTime = 0f;
            var startPosition = contentPanel.GetComponent<ScrollRect>().verticalNormalizedPosition;
            float newPos;

            do
            {
                usedTime += Time.deltaTime;
                var share = timespan <= usedTime ? 1f : usedTime / timespan;
                newPos = Mathf.Lerp(startPosition, 0f, share);
                contentPanel.GetComponent<ScrollRect>().verticalNormalizedPosition = newPos;

                yield return null;
                if (contentPanel == null)
                    // if page already left:
                    yield break;
            } while (newPos > 0.0001 && Input.touchCount == 0 && !Input.GetMouseButtonDown(0));

            // when it was not touched scroll to the perfect button:
            if (Input.touchCount == 0 && !Input.GetMouseButtonDown(0))
                contentPanel.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
        }

        #endregion
    }
}