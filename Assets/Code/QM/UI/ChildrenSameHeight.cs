using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildrenSameHeight : MonoBehaviour
{

    bool sizeCalculated;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!sizeCalculated)
        {
            float maxSize = 0f;
            foreach (Transform child in transform)
            {
                float size = child.GetComponent<RectTransform>().rect.height;
                maxSize = Mathf.Max(maxSize, size);
                 Debug.Log("Child with h: " + size + " max: " + maxSize);
           }
            if (maxSize > 0.001f)
            {
                foreach (Transform child in transform)
                {
                    LayoutElement le = child.GetComponent<LayoutElement>();
                    if (le == null)
                    {
                        le = child.gameObject.AddComponent<LayoutElement>();
                    }

                    le.preferredHeight = maxSize;
                }

                sizeCalculated = true;
            }
        }
    }
}
