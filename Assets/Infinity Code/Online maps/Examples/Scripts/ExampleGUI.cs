/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

// ReSharper disable once UnusedMember.Global
public class ExampleGUI : MonoBehaviour
{
    private OnlineMaps api;
    private GUIStyle rowStyle;
    private string search = "";

// ReSharper disable once UnusedMember.Local
    private void OnEnable()
    {
        api = GetComponent<OnlineMaps>();
    }

// ReSharper disable once UnusedMember.Local
    private void OnGUI()
    {
        if (rowStyle == null)
        {
            rowStyle = new GUIStyle(GUI.skin.button);
            RectOffset margin = rowStyle.margin;
            rowStyle.margin = new RectOffset(margin.left, margin.right, 1, 1);
        }

        //GUILayout.BeginArea(new Rect(5, 40, 30, 255), GUI.skin.box);

        if (GUILayout.Button("-")) api.zoom--;

     

        if (GUILayout.Button("+")) api.zoom++;

        //GUILayout.EndArea();

       
    }
}