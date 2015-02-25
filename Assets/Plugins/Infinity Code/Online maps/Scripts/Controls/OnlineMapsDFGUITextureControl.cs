/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

/// <summary>
/// Class control the map for the DF-GUI.
/// </summary>
[System.Serializable]
[AddComponentMenu("Infinity Code/Online Maps/Controls/DF-GUI Texture")]
public class OnlineMapsDFGUITextureControl : OnlineMapsControlBase2D
{
#if DFGUI
    private dfTextureSprite sprite;

    public override Vector2 GetCoords(Vector2 position)
    {
        Rect rect = GetRect();
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
        return sprite.GetScreenRect();
    }

    // ReSharper disable once UnusedMember.Local
    protected override void OnEnableLate()
    {
        sprite = GetComponent<dfTextureSprite>();
        if (sprite == null)
        {
            Debug.LogError("Can not find dfTextureSprite.");
            Destroy(this);
        }
    }

    public override void SetTexture(Texture2D texture)
    {
        base.SetTexture(texture);
        sprite.Texture = texture;
    }
#endif
}