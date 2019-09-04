using UnityEngine;
using Candlelight.UI;
using System.Text.RegularExpressions;
using GQ.Client.Util;
using GQ.Client.Conf;
using TMPro;
using UnityEngine.EventSystems;

namespace GQ.Client.UI
{

    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextchunkCtrl : MonoBehaviour, IPointerClickHandler
    {

        #region Unity Inspektor

        public TextMeshProUGUI DialogItemText;

        public void OnLinkClicked(HyperText text, Candlelight.UI.HyperText.LinkInfo linkInfo)
        {
            string href = extractHREF(linkInfo);
            if (href != null)
            {
                Application.OpenURL(href);
            }
        }

        #endregion


        public void Initialize(string itemText, bool supportHtmlLinks)
        {
            this.DialogItemText.text = itemText.Decode4HyperText(supportHtmlLinks: supportHtmlLinks);
            this.DialogItemText.color = ConfigurationManager.Current.mainFgColor;
            this.DialogItemText.fontSize = ConfigurationManager.Current.mainFontSize;
        }

        public static TextchunkCtrl Create(Transform rootTransform, string text, bool supportHtmlLinks = true)
        {
            GameObject go = (GameObject)Instantiate(
                Resources.Load("TextChunk"),
                rootTransform,
                false
            );
            go.SetActive(true);

            TextchunkCtrl diCtrl = go.GetComponent<TextchunkCtrl>();
            diCtrl.Initialize(text, supportHtmlLinks);

            return diCtrl;
        }

        private string extractHREF(Candlelight.UI.HyperText.LinkInfo info)
        {
            string href = null;

            string pattern = @".*?href=""(?'href'[^""]*?)(?:["" \s]|$)";
            Match match = Regex.Match(info.Name, pattern);
            if (match.Success)
            {
                href = match.Groups["href"].ToString();
                if (!href.StartsWith("http://") && !href.StartsWith("https://"))
                {
                    href = "http://" + href;
                }
            }
            return href;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Camera camera = null;
            GameObject camGo = GameObject.FindWithTag("MainCamera");
            if (camGo != null)
                camera = camGo.GetComponent<Camera>();
            if (camera == null)
                return;

            int linkIndex = TMP_TextUtilities.FindIntersectingLink(DialogItemText, Input.mousePosition, null);
            if (linkIndex != -1)
            { // was a link clicked?
                TMP_LinkInfo linkInfo = DialogItemText.textInfo.linkInfo[linkIndex];

                // open the link id as a url, which is the metadata we added in the text field
                Application.OpenURL(linkInfo.GetLinkID());
            }
        }

    }

}
