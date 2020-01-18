using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace QM.UI
{

    [RequireComponent(typeof(RectTransform), typeof(LayoutElement))]
    public class AdaptHeight : MonoBehaviour
    {
        public RectTransform enclosedContent;
        public float MaxShareInParent = .5f;

        public IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            LayoutElement layoutEl = GetComponent<LayoutElement>();
            float parentHeight = transform.parent.GetComponent<RectTransform>().rect.height;
            layoutEl.preferredHeight = Mathf.Min(enclosedContent.rect.height, parentHeight * MaxShareInParent);
            Debug.LogFormat("Set height of {0} to {1}", GetType(), layoutEl.preferredHeight);
        }
    }
}