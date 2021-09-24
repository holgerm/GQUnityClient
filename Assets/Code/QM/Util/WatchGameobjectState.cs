using System.Collections;
using System.Collections.Generic;
using System.Text;
using Code.QM.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WatchGameobjectState : MonoBehaviour
{
    public bool watchActiveInHierarchy = true;
    public bool watchSize = false;
    public Button watchButton;

    private bool activeLastFrame;
    private bool buttonEnabledLastFrame;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        StringBuilder sb = new StringBuilder($"WATCH {transform.GetPath()} in frame {Time.frameCount}:");

        if (watchActiveInHierarchy)
        {
            activeLastFrame = gameObject.activeInHierarchy;
            sb.Append(
                $"\n  activeInHierarchy SET to {gameObject.activeInHierarchy}");
        }

        if (watchButton)
        {
            buttonEnabledLastFrame = watchButton.enabled;
            sb.Append($"\n  enablement SET to {watchButton.enabled}");
            sb.Append($"\n  persistent listeners# {watchButton.onClick.GetPersistentEventCount()}");

            watchButton.onClick.AddListener(() =>
            {
                Debug.Log($"WATCH Button {watchButton.transform.GetPath()} CLICKED");
            });
        }

        yield return new WaitForEndOfFrame();

        if (watchSize)
        {
            Rect rect = watchButton.GetComponent<RectTransform>().rect;
            sb.Append($"\n  height: {rect.height}, width: {rect.width}");
        }
        
        Debug.Log(sb.ToString());
    }

    void OnDisable()
    {
        StringBuilder sb = new StringBuilder($"WATCH {transform.GetPath()} in frame {Time.frameCount}:");

        sb.Append(
                $"\n  DISABLED. activeInHierarchy: {gameObject.activeInHierarchy}");
        
        Debug.Log(sb.ToString());
    }

    private static StringBuilder _sb;

    // Update is called once per frame
    void Update()
    {
        bool output = false;

        if (watchActiveInHierarchy && activeLastFrame != gameObject.activeInHierarchy)
        {
            _sb.Clear();
            output = true;
            _sb.Append($"\n  activeInHierarchy CHANGED to {gameObject.activeInHierarchy}");
            activeLastFrame = gameObject.activeInHierarchy;
        }

        if (watchButton)
        {
            if (buttonEnabledLastFrame != watchButton.enabled)
            {
                if (!output) _sb.Clear();
                _sb.Append(
                    $"\n  enablement CHANGED to {watchButton.enabled}");
                output = true;
            }
        }

        if (output) 
            Debug.Log($"WATCH {transform.GetPath()} in frame {Time.frameCount}: {_sb?.ToString()}");
        
        
        // Check if the left mouse button was clicked
        if (Input.GetMouseButtonDown(0) || Input.touches.Length > 0)
        {
            // Check if the mouse was clicked over a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("WATCH Clicked on the UI");
            }
        }

    }
}