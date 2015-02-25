/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

/// <summary>
/// Class control the map for the GUITexture.
/// </summary>
[AddComponentMenu("Infinity Code/Online Maps/Controls/GUITexture")]
[RequireComponent(typeof(GUITexture))]
// ReSharper disable once UnusedMember.Global
public class OnlineMapsGUITextureControl : OnlineMapsControlBase2D
{
    public override Vector2 GetCoords(Vector2 position)
    {
        Rect rect = screenRect;
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
        return guiTexture.GetScreenRect();
    }

    protected override bool HitTest()
    {
        return guiTexture.HitTest(Input.mousePosition);
    }

    protected override void OnEnableLate()
    {
        if (guiTexture == null)
        {
            Debug.LogError("Can not find GUITexture.");
            Destroy(this);
        }
    }

    public override void SetTexture(Texture2D texture)
    {
        base.SetTexture(texture);
        guiTexture.texture = texture;
    }
}