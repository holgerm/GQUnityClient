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
            float fontSizeFactor = 1f;
            switch (TopicGridLayout.NumberOfColumns)
            {
                case 1:
                    fontSizeFactor = 1f;
                    break;
                case 2:
                    fontSizeFactor = 0.75f;
                    break;
                case 3:
                    fontSizeFactor = 0.60f;
                    break;
                default: // 4 and above:
                    fontSizeFactor = 0.45f;
                    break;
            }

            TextElement.fontSize = Config.mainFontSize * fontSizeFactor;
            TextElement.fontStyle = FontStyles.Bold;
            TextElement.enableWordWrapping = true;
            TextElement.lineSpacing = Config.lineSpacing;
        }
    }
}