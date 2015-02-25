/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

#if !UNITY_3_5 && !UNITY_3_5_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
using UnityEngine;

/// <summary>
/// Class control the map for the SpriteRenderer.
/// </summary>
[AddComponentMenu("Infinity Code/Online Maps/Controls/SpriteRenderer")]
[RequireComponent(typeof(SpriteRenderer))]
// ReSharper disable once UnusedMember.Global
public class OnlineMapsSpriteRendererControl:OnlineMapsControlBase2D
{
    private SpriteRenderer spriteRenderer;

#if !UNITY_4_3
    public override Vector2 GetCoords(Vector2 position)
    {
        Vector2 coords2D = GetCoords2D(position);
        return (coords2D != Vector2.zero)? coords2D: GetCoords3D(position);
    }

    private Vector2 GetCoords2D(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(position), Mathf.Infinity);
        if (hit.collider == null || hit.collider.gameObject != gameObject) return Vector2.zero;
        if (collider2D == null) return Vector2.zero;

        Vector3 size = (collider2D.bounds.max - new Vector3(hit.point.x, hit.point.y));
        size.x = size.x / collider2D.bounds.size.x;
        size.y = size.y / collider2D.bounds.size.y;

        Vector2 r = new Vector3((size.x - .5f), (size.y - .5f));

        int countX = api.width / OnlineMapsUtils.tileSize;
        int countY = api.height / OnlineMapsUtils.tileSize;

        Vector2 p = OnlineMapsUtils.LatLongToTilef(api.position, api.zoom);
        p.x -= countX * r.x;
        p.y += countY * r.y;

        return OnlineMapsUtils.TileToLatLong(p, api.zoom);
    }
#else
    public override Vector2 GetCoords(Vector2 position)
    {
        return GetCoords3D(position);
    }
#endif

    private Vector2 GetCoords3D(Vector2 position)
    {
        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(position), out hit))
            return Vector2.zero;

        if (hit.collider.gameObject != gameObject) return Vector2.zero;

        Vector3 size = (collider.bounds.max - hit.point);
        size.x = size.x / collider.bounds.size.x;
        size.y = size.y / collider.bounds.size.y;

        Vector2 r = new Vector3((size.x - .5f), (size.y - .5f));

        int countX = api.width / OnlineMapsUtils.tileSize;
        int countY = api.height / OnlineMapsUtils.tileSize;

        Vector2 p = OnlineMapsUtils.LatLongToTilef(api.position, api.zoom);
        p.x -= countX * r.x;
        p.y += countY * r.y;
        
        return OnlineMapsUtils.TileToLatLong(p, api.zoom);
    }

    protected override void OnEnableLate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Can not find SpriteRenderer.");
            Destroy(this);
        }
    }

    public override void SetTexture(Texture2D texture)
    {
        base.SetTexture(texture);
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.AddTexture("_MainTex", texture);
        spriteRenderer.SetPropertyBlock(props);
    }
}
#endif