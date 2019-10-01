using UnityEngine;
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
            Config config = ConfigurationManager.Current;
            TextElement = GetComponent<TextMeshProUGUI>();
#if UNITY_EDITOR
            UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(TextElement, false);
#endif

            TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Font");
            if (font != null)
            {
                TextElement.font = font;
            }

            TextElement.color = Color.black;

            switch (textUsageType)
            {
                case TextUsageType.Title:
                    TextElement.alignment = mapAlignment(config.textAlignment);
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.5f * config.mainFontSize;
                    TextElement.fontSizeMax = 0.7f * config.mainFontSize;
                    TextElement.enableWordWrapping = true;
                    TextElement.lineSpacing = config.lineSpacing;
                    break;
                case TextUsageType.Paragraph:
                    TextElement.alignment = mapAlignment(config.textAlignment);
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.4f * config.mainFontSize;
                    TextElement.fontSizeMax = 0.55f * config.mainFontSize;
                    TextElement.enableWordWrapping = true;
                    TextElement.lineSpacing = config.lineSpacing;
                    break;
                 case TextUsageType.Caption:
                    TextElement.alignment = TextAlignmentOptions.Center;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.35f * config.mainFontSize;
                    TextElement.fontSizeMax = 0.45f * config.mainFontSize;
                    TextElement.enableWordWrapping = true;
                    break;
                case TextUsageType.Option:
                    TextElement.alignment = TextAlignmentOptions.Left;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.35f * config.mainFontSize;
                    TextElement.fontSizeMax = 0.45f * config.mainFontSize;
                    TextElement.enableWordWrapping = true;
                    TextElement.lineSpacing = config.lineSpacing;
                    TextElement.overflowMode = TextOverflowModes.Overflow;
                    TextElement.raycastTarget = false;
                    break;
                case TextUsageType.MenuEntry:
                    TextElement.alignment = TextAlignmentOptions.Left;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.4f * config.mainFontSize;
                    TextElement.fontSizeMax = 0.6f * config.mainFontSize;
                    TextElement.enableWordWrapping = false;
                    TextElement.overflowMode = TextOverflowModes.Ellipsis;
                    break;
                case TextUsageType.Button:
                    TextElement.alignment = TextAlignmentOptions.Center;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.45f * config.mainFontSize;
                    TextElement.fontSizeMax = 0.6f * config.mainFontSize;
                    TextElement.enableWordWrapping = true;
                    TextElement.raycastTarget = false;
                    break;
                case TextUsageType.FoyerListEntry:
                    TextElement.alignment = TextAlignmentOptions.Left;
                    TextElement.enableAutoSizing = false;
                    TextElement.fontSize = 0.7f * config.mainFontSize;
                    TextElement.enableWordWrapping = true;
                    TextElement.overflowMode = TextOverflowModes.Ellipsis;
                    TextElement.maxVisibleLines = ConfigurationManager.Current.listEntryUseTwoLines ? 2 : 1;
                    break;
                case TextUsageType.DialogTitle:
                    TextElement.alignment = TextAlignmentOptions.Center;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.5f * config.mainFontSize;
                    TextElement.fontSizeMax = 0.7f * config.mainFontSize;
                    TextElement.enableWordWrapping = true;
                    break;
                case TextUsageType.DialogMessage:
                    TextElement.alignment = TextAlignmentOptions.Center;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.4f * config.mainFontSize;
                    TextElement.fontSizeMax = 0.55f * config.mainFontSize;
                    TextElement.enableWordWrapping = true;
                    TextElement.lineSpacing = config.lineSpacing;
                    break;
                default:
                    break;
            }

        }

        private TextAlignmentOptions mapAlignment(AlignmentOption configAligment)
        {
            switch (configAligment)
            {
                case
                    AlignmentOption.Left:
                    return TextAlignmentOptions.Left;
                case AlignmentOption.Center:
                    return TextAlignmentOptions.Center;
                case AlignmentOption.Right:
                    return TextAlignmentOptions.Right;
                case AlignmentOption.Justified:
                    return TextAlignmentOptions.Justified;
                default:
                    return TextAlignmentOptions.Left;
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
        Paragraph = 1,
        Caption = 2,
        Button = 3,
        Option = 4,
        MenuEntry = 5,
        FoyerListEntry = 6,
        DialogTitle = 7,
        DialogMessage = 8,
    }
}
