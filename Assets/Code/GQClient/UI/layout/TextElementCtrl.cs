using System;
using Code.GQClient.Conf;
using Code.GQClient.start;
using Code.GQClient.UI.Dialogs;
using Code.GQClient.Util;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.GQClient.UI.layout
{

    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextElementCtrl : MonoBehaviour, IPointerClickHandler
    {

        public TextMeshProUGUI TextElement;
        public TextUsageType textUsageType;

        private void OnValidate()
        {
            // Layout();
        }

        // public void Update()
        // {
        //     Debug.Log($"TextElementCtrl: {FullGOName()} Text font size: {TextElement.fontSize} in frame # {Time.frameCount}");
        // }
        //
        // private string FullGOName()
        // {
        //     GameObject go = gameObject;
        //     string goName = go.name;
        //     while (go.transform.parent != null) {
        //
        //         go = go.transform.parent.gameObject;
        //         goName = go.name + "/" + goName;
        //     }
        //     return goName;
        // }

        public void Start()
        {
            Layout();
        }

        public void Reset()
        {
            Layout();
        }

        protected static Config Config => Config.Current;

        private void Layout()
        {
            CommonLayout();
            SpecialLayout();
        }

        protected virtual void CommonLayout()
        {
            TextElement = GetComponent<TextMeshProUGUI>();
#if UNITY_EDITOR
            UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(TextElement, false);
#endif

            var font = Resources.Load<TMP_FontAsset>("Font");
            if (font != null)
            {
                TextElement.font = font;
            }

            TextElement.color = Config.mainFgColor;
        }

        protected virtual void SpecialLayout()
        {
            switch (textUsageType)
            {
                case TextUsageType.Title:
                    TextElement.alignment = mapAlignment(Config.textAlignment);
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.85f * Config.mainFontSize;
                    TextElement.fontSizeMax = 0.95f * Config.mainFontSize;
                    TextElement.fontStyle = FontStyles.Bold;
                    TextElement.enableWordWrapping = true;
                    TextElement.lineSpacing = Config.lineSpacing;
                    break;
                case TextUsageType.TitleCentered:
                    TextElement.alignment = TextAlignmentOptions.Center;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.85f * Config.mainFontSize;
                    TextElement.fontSizeMax = 0.95f * Config.mainFontSize;
                    TextElement.fontStyle = FontStyles.Bold;
                    TextElement.enableWordWrapping = true;
                    TextElement.lineSpacing = Config.lineSpacing;
                    break;
                case TextUsageType.Paragraph:
                    TextElement.alignment = mapAlignment(Config.textAlignment);
                    TextElement.enableAutoSizing = false;
                    TextElement.fontSize = 0.75f * Config.mainFontSize;
                    TextElement.enableWordWrapping = true;
                    TextElement.lineSpacing = Config.lineSpacing;
                    TextElement.characterSpacing = -3f;
                    break;
                case TextUsageType.Caption:
                    TextElement.alignment = TextAlignmentOptions.Center;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.35f * Config.mainFontSize;
                    TextElement.fontSizeMax = 0.45f * Config.mainFontSize;
                    TextElement.enableWordWrapping = true;
                    TextElement.characterSpacing = -3f;
                    break;
                case TextUsageType.CopyRight:
                    TextElement.alignment = TextAlignmentOptions.Left;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.35f * Config.mainFontSize;
                    TextElement.fontSizeMax = 0.45f * Config.mainFontSize;
                    TextElement.enableWordWrapping = true;
                    TextElement.color = Color.white;
                    TextElement.characterSpacing = -3f;
                    break;
                case TextUsageType.Option:
                    TextElement.alignment = TextAlignmentOptions.Left;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.5f * Config.mainFontSize;
                    TextElement.fontSizeMax = 0.65f * Config.mainFontSize;
                    TextElement.fontStyle = FontStyles.Bold;
                    TextElement.enableWordWrapping = true;
                    TextElement.lineSpacing = Config.lineSpacing;
                    TextElement.overflowMode = TextOverflowModes.Overflow;
                    TextElement.raycastTarget = false;
                    TextElement.characterSpacing = -3f;
                    break;
                case TextUsageType.SettingsOption:
                    TextElement.alignment = TextAlignmentOptions.Left;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.35f * Config.mainFontSize;
                    TextElement.fontSizeMax = 0.45f * Config.mainFontSize;
                    TextElement.enableWordWrapping = true;
                    TextElement.lineSpacing = Config.lineSpacing;
                    TextElement.overflowMode = TextOverflowModes.Overflow;
                    TextElement.raycastTarget = false;
                    TextElement.characterSpacing = -3f;
                    break;
                case TextUsageType.MenuEntry:
                    TextElement.color = Config.menuFGColor;
                    TextElement.alignment = TextAlignmentOptions.Left;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.45f * Config.mainFontSize;
                    TextElement.fontSizeMax = 0.65f * Config.mainFontSize;
                    TextElement.fontStyle = FontStyles.Bold;
                    TextElement.enableWordWrapping = false;
                    TextElement.overflowMode = TextOverflowModes.Ellipsis;
                    TextElement.raycastTarget = false;
                    TextElement.characterSpacing = -4f;
                    break;
                case TextUsageType.Button:
                    TextElement.alignment = TextAlignmentOptions.Center;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.45f * Config.mainFontSize;
                    TextElement.fontSizeMax = 0.65f * Config.mainFontSize;
                    TextElement.fontStyle = FontStyles.Bold;
                    TextElement.enableWordWrapping = true;
                    TextElement.raycastTarget = false;
                    TextElement.characterSpacing = -4f;
                    break;
                case TextUsageType.FoyerListEntry:
                    TextElement.color = Config.listEntryFgColor;
                    TextElement.alignment = TextAlignmentOptions.Left;
                    TextElement.enableAutoSizing = false;
                    TextElement.fontSize = 0.7f * Config.mainFontSize;
                    TextElement.fontStyle = FontStyles.Bold;
                    TextElement.enableWordWrapping = true;
                    TextElement.overflowMode = TextOverflowModes.Ellipsis;
                    TextElement.maxVisibleLines = Config.Current.listEntryUseTwoLines ? 2 : 1;
                    TextElement.raycastTarget = true;
                    TextElement.characterSpacing = -4f;
                    break;
                case TextUsageType.DialogTitle:
                    TextElement.alignment = TextAlignmentOptions.Center;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.4f * Config.mainFontSize;
                    TextElement.fontSizeMax = 0.8f * Config.mainFontSize;
                    TextElement.fontStyle = FontStyles.Bold;
                    TextElement.enableWordWrapping = true;
                    break;
                case TextUsageType.DialogMessage:
                    TextElement.alignment = TextAlignmentOptions.Center;
                    TextElement.enableAutoSizing = true;
                    TextElement.fontSizeMin = 0.4f * Config.mainFontSize;
                    TextElement.fontSizeMax = 0.55f * Config.mainFontSize;
                    TextElement.fontStyle = FontStyles.Bold;
                    TextElement.enableWordWrapping = true;
                    TextElement.lineSpacing = Config.lineSpacing;
                    TextElement.characterSpacing = -4f;
                    break;
                default:
                    break;
            }
        }

        protected TextAlignmentOptions mapAlignment(AlignmentOption configAligment)
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

        private void Initialize(string itemText, bool supportHtmlLinks)
        {
            TextElement.text = itemText.Trim().Decode4TMP(supportHtmlLinks: supportHtmlLinks);
            TextElement.color = Config.Current.mainFgColor;
            TextElement.fontSize = Config.Current.mainFontSize;
        }

        public static TextElementCtrl Create(Transform rootTransform, string text, bool supportHtmlLinks = true)
        {
            var go = (GameObject)Instantiate(
                AssetBundles.Asset("prefabs", "TextChunk"),
                rootTransform,
                false
            );
            go.SetActive(true);

            var diCtrl = go.GetComponent<TextElementCtrl>();
            diCtrl.Initialize(text, supportHtmlLinks);

            return diCtrl;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var linkIndex =
                TMP_TextUtilities.FindIntersectingLink(TextElement, Input.mousePosition, null);

            if (linkIndex != -1)
            { // was a link clicked?
                var linkInfo = TextElement.textInfo.linkInfo[linkIndex];

                // open the link id as a url, which is the metadata we added in the text field
                string linkUrl = linkInfo.GetLinkID();
                // Application.OpenURL(linkUrl);
                
                if (Config.Current.warnWhenLeavingQuest)
                {
                    void Act()
                    {
                        Application.OpenURL(linkUrl);
                    }

                    CancelableFunctionDialog.Show(
                        Config.Current.warnDialogTitleWhenLeavingApp,
                        Config.Current.warnDialogMessageWhenLeavingApp,
                        Act,
                        Config.Current.warnDialogOKWhenLeavingQuest,
                        Config.Current.warnDialogCancelWhenLeavingQuest);
                }
                else
                {
                    Application.OpenURL(linkUrl);
                }

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
        CopyRight = 9,
        SettingsOption = 10,
        TitleCentered = 11,
    }
}
