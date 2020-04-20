// #define DEBUG_LOG

using System;
using System.Collections;
using System.Collections.Generic;
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
                Debug.Log(("InvalidCastException: NPCTalk Page found a page of type " + page.GetType()));
            }

            // show the content:
            ShowImage();
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
                NpcPage.End();
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

        private void ShowImage()
        {
            // allow for variables inside the image url:
            var rtImageUrl = NpcPage.ImageUrl.MakeReplacements();

            // show (or hide completely) image:
            if (rtImageUrl == "")
            {
                imagePanel.SetActive(false);
                layout.TopMargin.SetActive(true);
                return;
            }
            else
            {
                layout.TopMargin.SetActive(false);
            }

            AbstractDownloader loader;
            if (rtImageUrl.StartsWith(GQML.PREFIX_RUNTIME_MEDIA))
            {
                if (page.Parent.MediaStore.TryGetValue(rtImageUrl, out var rtMediaInfo))
                {
                    loader = new LocalFileLoader(rtMediaInfo.LocalPath);
                }
                else
                {
                    Log.SignalErrorToAuthor($"Runtime media {rtImageUrl} not found in quest {page.Parent.Id}");
                    imagePanel.SetActive(false);
                    layout.TopMargin.SetActive(true);
                    return;
                }
            }
            else
            {
                // not runtime media case, i.e. ordinary url case:
                if (QuestManager.Instance.MediaStore.TryGetValue(rtImageUrl, out var mediaInfo))
                {
                    loader = new LocalFileLoader(mediaInfo.LocalPath);
                }
                else
                {
                    loader = new Downloader(
                        url: rtImageUrl,
                        timeout: ConfigurationManager.Current.timeoutMS,
                        maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS
                    );
                }
            }

            loader.OnSuccess += (AbstractDownloader d, DownloadEvent e) =>
            {
                var imageAreaHeight = image == null ? 0f : FitInAndShowImage(d.Www.texture);

                imagePanel.GetComponent<LayoutElement>().flexibleHeight = LayoutConfig.Units2Pixels(imageAreaHeight);
                contentPanel.GetComponent<LayoutElement>().flexibleHeight = CalculateMainAreaHeight(imageAreaHeight);

                // Dispose www including it s Texture and take some logs for performance surveillance:
                d.Www.Dispose();
            };
            loader.Start();
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
                imageAreaHeight = ConfigurationManager.Current.imageAreaHeightMaxUnits;
            }

            if (ImageRatioMaximum < imageRatio)
            {
                // image too wide to fit:
                imageAreaHeight = ConfigurationManager.Current.imageAreaHeightMinUnits;
            }

            //imagePanel.GetComponent<LayoutElement> ().flexibleHeight = LayoutConfig.Units2Pixels (imageAreaHeight);
            //contentPanel.GetComponent<LayoutElement> ().flexibleHeight = CalculateMainAreaHeight (imageAreaHeight);

            fitter.aspectRatio = imageRatio; // i.e. the adjusted image area aspect ratio
            fitter.aspectMode =
                ConfigurationManager.Current.fitExceedingImagesIntoArea
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
                bgImg.color = ConfigurationManager.Current.mainBgColor;
            }
        }

        private void AddCurrentText()
        {
            // decode text for HyperText Component:
            var currentText = NpcPage.CurrentDialogItem.Text.Decode4TMP();

            // create dialog item GO from prefab:
            TextElementCtrl.Create(dialogItemContainer, currentText);

            // play audio if specified:
            var duration = 0f;
            if (!string.IsNullOrEmpty(NpcPage.CurrentDialogItem.AudioURL))
                duration = Audio.PlayFromMediaStore(NpcPage.CurrentDialogItem.AudioURL);

            if (ConfigurationManager.Current.autoScrollNewText)
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