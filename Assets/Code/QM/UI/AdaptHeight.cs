using UnityEngine;
using UnityEngine.UI;

namespace Code.QM.UI
{

    [RequireComponent(typeof(RectTransform), typeof(LayoutElement))]
    public class AdaptHeight : MonoBehaviour
    {
        public RectTransform referredContent;
        public float MaxShareInParent = .5f;

        LayoutElement layoutEl;
        RectTransform parentRT;

        public void Start()
        {
            layoutEl = GetComponent<LayoutElement>();
            parentRT = transform.parent.GetComponent<RectTransform>();
        }

        public void LateUpdate()
        {
            layoutEl.preferredHeight =
                Mathf.Min(referredContent.rect.height, parentRT.rect.height * MaxShareInParent);
        }
    }
}