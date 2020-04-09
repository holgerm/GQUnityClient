using Code.QM.Util;
using TMPro;
using UnityEngine;

namespace Code.GQClient.UI.layout
{
    public class TopicButtonLayout : TextElementCtrl
    {
        protected override void SpecialLayout()
        {
            TextElement.color = Config.paletteFGColor;
            TextElement.alignment = TextAlignmentOptions.Center;
            TextElement.enableAutoSizing = false;
            TextElement.fontSize = Config.mainFontSize * (Device.DisplaySize <= Device.Size.Medium ? 0.75f : 0.5f);
            TextElement.fontStyle = FontStyles.Bold;
            TextElement.enableWordWrapping = true;
            TextElement.lineSpacing = Config.lineSpacing;
        }
    }
}
