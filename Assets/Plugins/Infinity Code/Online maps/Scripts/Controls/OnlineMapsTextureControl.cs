/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

/// <summary>
/// Class control the map for the Texture.
/// </summary>
[System.Serializable]
[AddComponentMenu("Infinity Code/Online Maps/Controls/Texture")]
[RequireComponent(typeof(MeshRenderer))]
// ReSharper disable once UnusedMember.Global
public class OnlineMapsTextureControl : OnlineMapsControlBase3D
{
    public override Vector2 GetCoords(Vector2 position)
    {
        RaycastHit hit;
        if (!Physics.Raycast(activeCamera.ScreenPointToRay(position), out hit))
            return Vector2.zero;

        if (hit.collider.gameObject != gameObject) return Vector2.zero;

        Renderer render = hit.collider.renderer;
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (render == null || render.sharedMaterial == null || render.sharedMaterial.mainTexture == null ||
            meshCollider == null)
            return Vector2.zero;

        Vector2 r = hit.textureCoord;

        r.x = r.x - 0.5f;
        r.y = r.y - 0.5f;

        int countX = api.width / OnlineMapsUtils.tileSize;
        int countY = api.height / OnlineMapsUtils.tileSize;

        Vector2 p = OnlineMapsUtils.LatLongToTilef(api.position, api.zoom);
        p.x += countX * r.x;
        p.y -= countY * r.y;
        return OnlineMapsUtils.TileToLatLong(p, api.zoom);
    }

    public override void SetTexture(Texture2D texture)
    {
        base.SetTexture(texture);
        renderer.sharedMaterial.mainTexture = texture;
    }
}