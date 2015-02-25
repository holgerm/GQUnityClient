/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

public class ModifyTooltipStyleExample : MonoBehaviour 
{
    private void OnEnable()
    {
        // Subscribe to the event preparation of tooltip style.
        OnlineMaps.instance.OnPrepareTooltipStyle += OnPrepareTooltipStyle;
    }
    private void OnPrepareTooltipStyle(ref GUIStyle style)
    {
        // Change the style settings.
        style.fontSize = Screen.width / 50;
    }
}
