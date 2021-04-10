using UnityEngine;

public class SafeAreaPanel : MonoBehaviour
{
    public bool simulate = false;
    
    private void Start() {
        Vector2 anchorMin = Screen.safeArea.position;
        Vector2 anchorMax = Screen.safeArea.position + Screen.safeArea.size;

        if (simulate)
        {
            anchorMin = new Vector2(0.0f, 68.0f);
            anchorMax = new Vector2(828.0f, 1696.0f);
        }

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        GetComponent<RectTransform>().anchorMin = anchorMin;
        GetComponent<RectTransform>().anchorMax = anchorMax;
    }

}
