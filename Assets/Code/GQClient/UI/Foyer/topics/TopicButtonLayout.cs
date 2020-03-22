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
            TextElement.fontSize = 0.75f * Config.mainFontSize;
            TextElement.fontStyle = FontStyles.Bold;
            TextElement.enableWordWrapping = true;
            TextElement.lineSpacing = Config.lineSpacing;
        }
    }
}
