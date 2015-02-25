/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

#if IGUI
using iGUI;
#endif
using UnityEngine;

/// <summary>
/// Class control the map for the iGUI.
/// </summary>
[System.Serializable]
[AddComponentMenu("Infinity Code/Online Maps/Controls/iGUI Texture")]
public class OnlineMapsIGUITextureControl : OnlineMapsControlBase2D
{
#if IGUI
    private iGUIImage image;
    private bool mousePressed;

    protected override void BeforeUpdate()
    {
        bool mouseButton = Input.GetMouseButton(0);
        if (mouseButton != mousePressed)
        {
            if (mouseButton) OnMapBasePress();
            else OnMapBaseRelease();
            mousePressed = mouseButton;
        }
    }

    public override Vector2 GetCoords(Vector2 position)
    {
        Rect rect = image.getAbsoluteRect();
        rect.y = Screen.height - rect.yMax;

        int countX = api.texture.width / OnlineMapsUtils.tileSize;
        int countY = api.texture.height / OnlineMapsUtils.tileSize;
        Vector2 p = OnlineMapsUtils.LatLongToTilef(api.position, api.zoom);
        float rx = (rect.center.x - position.x) / rect.width * 2;
        float ry = (rect.center.y - position.y) / rect.height * 2;
        p.x -= countX / 2f * rx;
        p.y += countY / 2f * ry;
        return OnlineMapsUtils.TileToLatLong(p, api.zoom);
    }

	public override Rect GetRect()
    {
        return image.getAbsoluteRect();
    }

    protected override bool HitTest()
    {
        Rect rect = image.getAbsoluteRect();
        rect.y = Screen.height - rect.yMax;
        return rect.Contains(Input.mousePosition);
    }

    protected override void OnEnableLate()
    {
        image = GetComponent<iGUIImage>();
        if (image == null)
        {
            Debug.LogError("Can not find iGUIImage.");
            Destroy(this);
        }
    }

    public override void SetTexture(Texture2D texture)
    {
        base.SetTexture(texture);
        image.image = texture;
    }
#endif
}