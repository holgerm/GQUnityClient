using System.Collections;
using System.Collections.Generic;
using System.Text;
using Code.QM.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchDetector : MonoBehaviour
{
    public Camera cam;

    public void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PointerEventData m_PointerEventData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(m_PointerEventData, results);

            StringBuilder sb = new StringBuilder($"WATCH Hit @ {m_PointerEventData.position}: ");
            sb.Append("\n  on go: ");
            if (m_PointerEventData.pointerPress == null)
            {
                sb.Append($"pointerpress is null");
            }
            else
            {
                if (m_PointerEventData.pointerPress.transform == null)
                {
                    sb.Append(
                        $"pointerpress.transform is null, name is: {m_PointerEventData.pointerPress.gameObject.name}");
                }
                else
                {
                    sb.Append($"\n  on go: {m_PointerEventData.pointerPress.transform.GetPath()}");
                }
            }

            foreach (RaycastResult result in results)
            {
                sb.Append(
                    $"\n  {result.gameObject.transform.GetPath()} activeInHierarchy: {result.gameObject.activeInHierarchy}");
                Rect r = result.gameObject.GetComponent<RectTransform>().rect;
                sb.Append($"\n    minX: ({r.xMin}, {r.yMin}) --> ({r.xMax}, {r.yMax})");
            }

            Debug.Log(sb.ToString());
        }
    }
}