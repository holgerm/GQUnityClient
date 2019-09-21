using UnityEngine;
using System.Text.RegularExpressions;
using GQ.Client.Util;
using GQ.Client.Conf;
using TMPro;
using UnityEngine.EventSystems;

namespace GQ.Client.UI
{

    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextElementCtrl : MonoBehaviour, IPointerClickHandler
    {

        public TextMeshProUGUI TextElement;
        public TextUsageType textUsageType;

        private void OnValidate()
        {
            Reset();
        }

        public void Reset()
        {
            TextElement = GetComponent<TextMeshProUGUI>();
            UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(TextElement, false);

            TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Font");
            if (font != null)
            {
                TextElement.font = font;
            }

            TextElement.color = Color.black;

            switch (textUsageType)
            {
                case TextUsageType.Title:
                    TextElement.alignment = TextAlignmentOptions.Center;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 50;
                    TextElement.fontSizeMax = 70;
                    TextElement.enableWordWrapping = true;
                    break;
                case TextUsageType.Body:
                    TextElement.alignment = TextAlignmentOptions.TopJustified;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 40;
                    TextElement.fontSizeMax = 55;
                    TextElement.enableWordWrapping = true;
                    break;
                case TextUsageType.CenteredBody:
                    TextElement.alignment = TextAlignmentOptions.Center;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 40;
                    TextElement.fontSizeMax = 55;
                    TextElement.enableWordWrapping = true;
                    break;
                case TextUsageType.Button:
                    TextElement.alignment = TextAlignmentOptions.Center;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 45;
                    TextElement.fontSizeMax = 60;
                    TextElement.enableWordWrapping = true;
                    break;
                case TextUsageType.Caption:
                    TextElement.alignment = TextAlignmentOptions.Center;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 35;
                    TextElement.fontSizeMax = 45;
                    TextElement.enableWordWrapping = true;
                    break;
                case TextUsageType.Option:
                    TextElement.alignment = TextAlignmentOptions.Left;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 35;
                    TextElement.fontSizeMax = 45;
                    TextElement.enableWordWrapping = false;
                    TextElement.overflowMode = TextOverflowModes.Truncate;
                    break;
                case TextUsageType.ListEntry:
                    TextElement.alignment = TextAlignmentOptions.Left;
                    TextElement.enableAutoSizing = false;
                    TextElement.fontSize = 70;
                    TextElement.enableWordWrapping = true;
                    TextElement.overflowMode = TextOverflowModes.Ellipsis;
                    TextElement.maxVisibleLines = ConfigurationManager.Current.listEntryUseTwoLines ? 2 : 1;
                    break;
                case TextUsageType.MenuEntry:
                    TextElement.alignment = TextAlignmentOptions.Left;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 40;
                    TextElement.fontSizeMax = 60;
                    TextElement.enableWordWrapping = false;
                    TextElement.overflowMode = TextOverflowModes.Ellipsis;
                    break;
                case TextUsageType.TextChunk:
                    TextElement.alignment = TextAlignmentOptions.Justified;
                    TextElement.enableAutoSizing = false;
                    TextElement.fontSize = ConfigurationManager.Current.mainFontSize;
                    TextElement.enableWordWrapping = true;
                    TextElement.overflowMode = TextOverflowModes.Overflow;
                    break;
                default:
                    break;
            }

        }

        public void Initialize(string itemText, bool supportHtmlLinks)
        {
            this.TextElement.text = itemText.Decode4TMP(supportHtmlLinks: supportHtmlLinks);
            this.TextElement.color = ConfigurationManager.Current.mainFgColor;
            this.TextElement.fontSize = ConfigurationManager.Current.mainFontSize;
        }

        public static TextElementCtrl Create(Transform rootTransform, string text, bool supportHtmlLinks = true)
        {
            GameObject go = (GameObject)Instantiate(
                Resources.Load("TextChunk"),
                rootTransform,
                false
            );
            go.SetActive(true);

            TextElementCtrl diCtrl = go.GetComponent<TextElementCtrl>();
            diCtrl.Initialize(text, supportHtmlLinks);

            return diCtrl;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int linkIndex =
                TMP_TextUtilities.FindIntersectingLink(TextElement, Input.mousePosition, null);

            if (linkIndex != -1)
            { // was a link clicked?
                TMP_LinkInfo linkInfo = TextElement.textInfo.linkInfo[linkIndex];

                // open the link id as a url, which is the metadata we added in the text field
                Application.OpenURL(linkInfo.GetLinkID());
            }
        }

    }

    public enum TextUsageType
    {
        Title = 0,
        Body = 1,
        CenteredBody = 8,
        Caption = 3,
        Button = 2,
        Option = 4,
        ListEntry = 5,
        MenuEntry = 6,
        TextChunk = 7
        // nextitem = 9
    }
}
