using GQ.Client.Conf;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPAdapter : MonoBehaviour
{
    public TextUsageType textUsageType;

    TextMeshProUGUI text;

    // Start is called before the first frame update
    void Reset()
    {
        text = GetComponent<TextMeshProUGUI>();

        text.font = Resources.Load("Font", typeof(TMP_FontAsset)) as TMP_FontAsset;
        text.color = ConfigurationManager.Current.mainFgColor;

        switch (textUsageType)
        {
            case TextUsageType.Title:
                text.alignment = TextAlignmentOptions.Center;
                text.enableAutoSizing = true;
                text.fontSizeMin = 50;
                text.fontSizeMax = 70;
                text.enableWordWrapping = true;
                break;
            case TextUsageType.Body:
                text.alignment = TextAlignmentOptions.TopJustified;
                text.enableAutoSizing = true;
                text.fontSizeMin = 40;
                text.fontSizeMax = 55;
                text.enableWordWrapping = true;
                break;
            case TextUsageType.CenteredBody:
                text.alignment = TextAlignmentOptions.Center;
                text.enableAutoSizing = true;
                text.fontSizeMin = 40;
                text.fontSizeMax = 55;
                text.enableWordWrapping = true;
                break;
            case TextUsageType.Button:
                text.alignment = TextAlignmentOptions.Center;
                text.enableAutoSizing = true;
                text.fontSizeMin = 45;
                text.fontSizeMax = 60;
                text.enableWordWrapping = true;
                break;
            case TextUsageType.Caption:
                text.alignment = TextAlignmentOptions.Center;
                text.enableAutoSizing = true;
                text.fontSizeMin = 35;
                text.fontSizeMax = 45;
                text.enableWordWrapping = true;
                break;
            case TextUsageType.Option:
                text.alignment = TextAlignmentOptions.Left;
                text.enableAutoSizing = true;
                text.fontSizeMin = 35;
                text.fontSizeMax = 45;
                text.enableWordWrapping = false;
                text.overflowMode = TextOverflowModes.Truncate;
                break;
            case TextUsageType.ListEntry:
                text.color = ConfigurationManager.Current.listEntryFgColor;
                text.alignment = TextAlignmentOptions.Left;
                text.enableAutoSizing = false;
                text.fontSize = 70;
                text.enableWordWrapping = true;
                text.overflowMode = TextOverflowModes.Ellipsis;
                text.maxVisibleLines = ConfigurationManager.Current.listEntryUseTwoLines ? 2 : 1;
                break;
            case TextUsageType.MenuEntry:
                text.color = ConfigurationManager.Current.menuFGColor;
                text.alignment = TextAlignmentOptions.Left;
                text.enableAutoSizing = true;
                text.fontSizeMin = 40;
                text.fontSizeMax = 60;
                text.enableWordWrapping = false;
                text.overflowMode = TextOverflowModes.Ellipsis;
                break;
            case TextUsageType.TextChunk:
                text.alignment = TextAlignmentOptions.Justified;
                text.enableAutoSizing = false;
                text.fontSize = ConfigurationManager.Current.mainFontSize;
                text.enableWordWrapping = true;
                text.overflowMode = TextOverflowModes.Overflow;
                break;
            default:
                break;
        }
    }

    private void OnValidate()
    {
        Reset();
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
