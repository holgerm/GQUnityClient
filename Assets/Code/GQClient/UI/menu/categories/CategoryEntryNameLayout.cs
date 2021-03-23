using Code.GQClient.Conf;
using TMPro;
using UnityEngine;

namespace Code.GQClient.UI.layout
{

    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CategoryEntryNameLayout : MonoBehaviour
    {
        public TextMeshProUGUI TextElement;
        private static Config Config => ConfigurationManager.Current;

        private void OnValidate()
        {
            Layout();
        }

        public void Start()
        {
            Layout();
        }

        public void Reset()
        {
            Layout();
        }

        private void Layout()
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

            // TextElement.color = Config.menuFGColor;
            TextElement.alignment = TextAlignmentOptions.Left;
            TextElement.enableAutoSizing = true;
            TextElement.fontSizeMin = 0.45f * Config.mainFontSize;
            TextElement.fontSizeMax = 0.65f * Config.mainFontSize;
            TextElement.fontStyle = FontStyles.Bold;
            TextElement.enableWordWrapping = false;
            TextElement.overflowMode = TextOverflowModes.Ellipsis;
            TextElement.raycastTarget = false;
        }
    }
}
