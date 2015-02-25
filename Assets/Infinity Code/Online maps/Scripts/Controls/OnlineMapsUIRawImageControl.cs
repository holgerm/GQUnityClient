/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class control the map for the uGUI UI RawImage.
/// </summary>
[AddComponentMenu("Infinity Code/Online Maps/Controls/UI RawImage")]
// ReSharper disable once UnusedMember.Global
public class OnlineMapsUIRawImageControl : OnlineMapsControlBase2D
{
    private RawImage image;

    protected override void BeforeUpdate()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        int touchCount = Input.GetMouseButton(0) ? 1 : 0;
        if (touchCount != lastTouchCount)
        {
            if (touchCount == 1) OnMapBasePress();
            else if (touchCount == 0) OnMapBaseRelease();
        }
        lastTouchCount = touchCount;
#else
        if (Input.touchCount != lastTouchCount)
        {
            if (Input.touchCount == 1) OnMapBasePress();
            else if (Input.touchCount == 0) OnMapBaseRelease();
        }
        lastTouchCount = Input.touchCount;
#endif
    }

    public override Vector2 GetCoords(Vector2 position)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, position, image.camera))
            return Vector2.zero;

        Vector2 point;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, position, image.camera, out point);

        Rect rect = image.GetPixelAdjustedRect();

        Vector2 size = (rect.max - point);
        size.x = size.x / rect.size.x;
        size.y = size.y / rect.size.y;

        Vector2 r = new Vector2((size.x - .5f), (size.y - .5f));

        int countX = api.width / OnlineMapsUtils.tileSize;
        int countY = api.height / OnlineMapsUtils.tileSize;

        Vector2 p = OnlineMapsUtils.LatLongToTilef(api.position, api.zoom);
        p.x -= countX * r.x;
        p.y += countY * r.y;

        return OnlineMapsUtils.TileToLatLong(p, api.zoom);
    }

    public override Rect GetRect()
    {
        RectTransform rectTransform = image.rectTransform;
        Rect rect = RectTransformUtility.PixelAdjustRect(rectTransform, image.canvas);
        rect.x += rectTransform.position.x;
        rect.y += rectTransform.position.y;
        return rect;
    }

    protected override bool HitTest()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, Input.mousePosition, image.camera);
    }

    protected override void OnEnableLate()
    {
        image = GetComponent<RawImage>();
        if (image == null)
        {
            Debug.LogError("Can not find Image.");
            Destroy(this);
        }
    }

    public override void SetTexture(Texture2D texture)
    {
        base.SetTexture(texture);
        image.texture = texture;
    }
}
#endif