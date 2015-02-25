/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

// ReSharper disable once UnusedMember.Global
public class GUITextureSize : MonoBehaviour
{
// ReSharper disable once UnusedMember.Local
    private void Start()
    {
        Rect pi = guiTexture.pixelInset;
        float sw = Screen.width / (float) guiTexture.texture.width;
        float sh = Screen.height / (float) guiTexture.texture.height;

        if (sw > sh)
        {
            pi.width = Screen.width;
            pi.height = sw * guiTexture.texture.height;
        }
        else
        {
            pi.height = Screen.height;
            pi.width = sh * guiTexture.texture.width;
        }

        pi.x = pi.width / -2;
        pi.y = pi.height / -2;

        guiTexture.pixelInset = pi;
    }
}