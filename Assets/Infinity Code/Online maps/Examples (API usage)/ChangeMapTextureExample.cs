/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

public class ChangeMapTextureExample : MonoBehaviour
{
    // Original texture
    private Texture2D texture1;

    // Dynamic texture
    private Texture2D texture2;

    private void ChangeMapTexture()
    {
        // Change display texture
        Texture2D activeTexture = (guiTexture.texture == texture1) ? texture2 : texture1;
        guiTexture.texture = activeTexture;
        guiTexture.pixelInset = new Rect(activeTexture.width / -2, activeTexture.height / -2, activeTexture.width, activeTexture.height);

        // Change map texture
        OnlineMaps.instance.SetTexture(activeTexture);
    }

    void OnGUI()
    {
        // Change texture on button press
        if (GUI.Button(new Rect(5, 5, 100, 20),  "Change texture"))
        {
            ChangeMapTexture();
        }
    }

    void Start ()
	{
        // Get original texture
	    texture1 = (Texture2D)guiTexture.texture;

        // Create new dynamic texture
	    texture2 = new Texture2D(512, 256, TextureFormat.RGB24, false);

        // Add double click on map event
        OnlineMapsGUITextureControl.instance.OnMapDoubleClick += ChangeMapTexture;

        guiTexture.pixelInset = new Rect(texture1.width / -2, texture1.height / -2, texture1.width, texture1.height);
	}
}