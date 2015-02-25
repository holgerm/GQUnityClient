/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

#if UNITY_3_5 || UNITY_3_5_5
#define UNITY_OLD
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class implements the basic functionality of drawing on the map.
/// </summary>
[Serializable]
public class OnlineMapsDrawingElement
{
    private static readonly Vector2 halfVector = new Vector2(0.5f, 0.5f);
    private static readonly Vector2 sampleV1 = new Vector2(0.25f, 0.5f);
    private static readonly Vector2 sampleV2 = new Vector2(0.75f, 0.5f);
    private static readonly Vector2 sampleV3 = new Vector2(0.5f, 0.25f);
    private static readonly Vector2 sampleV4 = new Vector2(0.5f, 0.75f);

    protected static OnlineMaps api
    {
        get { return _api ?? (_api = OnlineMaps.instance); }
    }

    private static OnlineMaps _api;

    protected Mesh mesh;
    protected GameObject gameObject;

    protected virtual bool active
    {
        get
        {
#if UNITY_OLD
            return gameObject.active;
#else
            return gameObject.activeSelf;
#endif
        }
        set
        {
#if UNITY_OLD
            gameObject.active = value;
#else
            gameObject.SetActive(value);
#endif
        }
    }

    protected OnlineMapsDrawingElement()
    {
        
    }

    /// <summary>
    /// Draw element on the map.
    /// </summary>
    /// <param name="buffer">Backbuffer</param>
    /// <param name="bufferPosition">Backbuffer position</param>
    /// <param name="bufferWidth">Backbuffer width</param>
    /// <param name="bufferHeight">Backbuffer height</param>
    /// <param name="zoom">Zoom of the map</param>
    public virtual void Draw(Color[] buffer, OnlineMapsVector2i bufferPosition, int bufferWidth, int bufferHeight, int zoom)
    {
        
    }

    protected void DrawActivePoints(ref List<Vector2> activePoints, ref List<Vector3> verticles, ref List<Vector3> normals, ref List<int> triangles, ref List<Vector2> uv, float weight)
    {
        if (activePoints.Count < 2)
        {
            activePoints.Clear();
            return;
        }
        List<Vector3> side1 = new List<Vector3>();
        List<Vector3> side2 = new List<Vector3>();

        for (int i = 0; i < activePoints.Count; i++)
        {
            Vector3 p = new Vector3(-activePoints[i].x, 0, activePoints[i].y);
            if (i == 0)
            {
                float a = OnlineMapsUtils.Angle2DRad(p, new Vector3(-activePoints[i + 1].x, 0, activePoints[i + 1].y), 90);
                Vector3 off = new Vector3(Mathf.Cos(a) * weight, 0, Mathf.Sin(a) * weight);
                side1.Add(p + off);
                side2.Add(p - off);
            }
            else if (i == activePoints.Count - 1)
            {
                float a = OnlineMapsUtils.Angle2DRad(new Vector3(-activePoints[i - 1].x, 0, activePoints[i - 1].y), p, 90);
                Vector3 off = new Vector3(Mathf.Cos(a) * weight, 0, Mathf.Sin(a) * weight);
                side1.Add(p + off);
                side2.Add(p - off);
            }
            else
            {
                Vector3 p1 = new Vector3(-activePoints[i - 1].x, 0, activePoints[i - 1].y);
                Vector3 p2 = new Vector3(-activePoints[i + 1].x, 0, activePoints[i + 1].y);
                float a1 = OnlineMapsUtils.Angle2DRad(p1, p, 90);
                float a2 = OnlineMapsUtils.Angle2DRad(p, p2, 90);
                Vector3 off1 = new Vector3(Mathf.Cos(a1) * weight, 0, Mathf.Sin(a1) * weight);
                Vector3 off2 = new Vector3(Mathf.Cos(a2) * weight, 0, Mathf.Sin(a2) * weight);
                Vector3 p21 = p + off1;
                Vector3 p22 = p - off1;
                Vector3 p31 = p + off2;
                Vector3 p32 = p - off2;
                int state1, state2;
                Vector2 is1 = OnlineMapsUtils.GetIntersectionPointOfTwoLines(p1 + off1, p21, p31, p2 + off2, out state1);
                Vector2 is2 = OnlineMapsUtils.GetIntersectionPointOfTwoLines(p1 - off1, p22, p32, p2 - off2, out state2);
                if (state1 == 1) side1.Add(new Vector3(is1.x, (p21.y + p31.y) / 2, is1.y));
                if (state2 == 1) side2.Add(new Vector3(is2.x, (p22.y + p32.y) / 2, is2.y));
            }
        }

        for (int i = 0; i < side1.Count - 1; i++)
        {
            int ti = verticles.Count;

            verticles.Add(side1[i]);
            verticles.Add(side1[i + 1]);
            verticles.Add(side2[i + 1]);
            verticles.Add(side2[i]);

            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);

            uv.Add(new Vector2(0, 0));
            uv.Add(new Vector2(1, 0));
            uv.Add(new Vector2(1, 1));
            uv.Add(new Vector2(0, 1));

            triangles.Add(ti);
            triangles.Add(ti + 1);
            triangles.Add(ti + 2);
            triangles.Add(ti);
            triangles.Add(ti + 2);
            triangles.Add(ti + 3);
        }

        activePoints.Clear();
    }

    protected void DrawLineToBuffer(Color[] buffer, OnlineMapsVector2i bufferPosition, int bufferWidth, int bufferHeight,
        int zoom, List<Vector2> points, Color color, float weight, bool closed)
    {
        if (color.a == 0) return;

        int w = Mathf.RoundToInt(weight);
        int countOffet = closed ? 0 : 1;

        for (int j = 0; j < points.Count - countOffet; j++)
        {
            int pj = j + 1;
            if (pj >= points.Count) pj = 0;
            Vector2 from = OnlineMapsUtils.LatLongToTilef(points[j], zoom) - bufferPosition;
            Vector2 to = OnlineMapsUtils.LatLongToTilef(points[pj], zoom) - bufferPosition;
            from *= OnlineMapsUtils.tileSize;
            to *= OnlineMapsUtils.tileSize;

            float stY = Mathf.Clamp(Mathf.Min(from.y, to.y) - w, 0, bufferHeight);
            float stX = Mathf.Clamp(Mathf.Min(from.x, to.x) - w, 0, bufferWidth);
            float endY = Mathf.Clamp(Mathf.Max(from.y, to.y) + w, 0, bufferHeight);
            float endX = Mathf.Clamp(Mathf.Max(from.x, to.x) + w, 0, bufferWidth);
            int strokeOuter2 = w * w;

            int sqrW = w * w;

            int lengthX = Mathf.RoundToInt(endX - stX);
            int lengthY = Mathf.RoundToInt(endY - stY);
            Vector2 start = new Vector2(stX, stY);

            for (int y = 0; y < lengthY; y++)
            {
                for (int x = 0; x < lengthX; x++)
                {
                    Vector2 p = new Vector2(x, y) + start;
                    Vector2 center = p + halfVector;
                    float dist = (center - center.NearestPointStrict(from, to)).sqrMagnitude;

                    if (dist <= strokeOuter2)
                    {
                        Color c = Color.black;

                        Vector2[] samples = {
	                        p + sampleV1,
	                        p + sampleV2,
	                        p + sampleV3,
	                        p + sampleV4
	                    };
                        int bufferIndex = (int)p.y * bufferWidth + (int)p.x;
                        Color pc = buffer[bufferIndex];
                        for (int i = 0; i < 4; i++)
                        {
                            dist = (samples[i] - samples[i].NearestPointStrict(from, to)).sqrMagnitude;
                            if (dist < sqrW) c += color;
                            else c += pc;
                        }
                        c /= 4;
                        buffer[bufferIndex] = c;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws element on a specified TilesetControl.
    /// </summary>
    /// <param name="control"></param>
    public virtual void DrawOnTileset(OnlineMapsTileSetControl control)
    {
        
    }

    protected List<Vector2> GetLocalPoints(List<Vector2> points, bool closed = false)
    {
        Vector2 startTilePos = OnlineMapsUtils.LatLongToTilef(api.topLeftPosition, api.buffer.apiZoom);

        List<Vector2> localPoints = new List<Vector2>();

        int off = closed ? 1 : 0;

        for (int i = 0; i < points.Count + off; i++)
        {
            int ci = i;
            if (ci >= points.Count) ci -= points.Count;
            Vector2 p = OnlineMapsUtils.LatLongToTilef(points[ci], api.buffer.apiZoom) - startTilePos;

            float rx1 = (p.x * OnlineMapsUtils.tileSize) / api.tilesetWidth * api.tilesetSize.x;
            float ry1 = (p.y * OnlineMapsUtils.tileSize) / api.tilesetHeight * api.tilesetSize.y;
            Vector2 np = new Vector2(rx1, ry1);
            localPoints.Add(np);
        }
        return localPoints;
    }

    protected void FillPoly(Color[] buffer, OnlineMapsVector2i bufferPosition, int bufferWidth, int bufferHeight,
        int zoom, List<Vector2> points, Color color)
    {
        if (color.a == 0) return;

        List<Vector2> bufferPoints = new List<Vector2>();
        
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (Vector2 point in points)
        {
            Vector2 bufferPoint = (OnlineMapsUtils.LatLongToTilef(point, zoom) - bufferPosition) * OnlineMapsUtils.tileSize;

            if (bufferPoint.x < minX) minX = bufferPoint.x;
            if (bufferPoint.x > maxX) maxX = bufferPoint.x;
            if (bufferPoint.y < minY) minY = bufferPoint.y;
            if (bufferPoint.y > maxY) maxY = bufferPoint.y;
            
            bufferPoints.Add(bufferPoint);
        }

        float stY = Mathf.Clamp(minY, 0, bufferHeight);
        float stX = Mathf.Clamp(minX, 0, bufferWidth);
        float endY = Mathf.Clamp(maxY, 0, bufferHeight);
        float endX = Mathf.Clamp(maxX, 0, bufferWidth);

        int lengthX = Mathf.RoundToInt(endX - stX);
        int lengthY = Mathf.RoundToInt(endY - stY);
        Vector2 start = new Vector2(stX, stY);

        Color clr = new Color(color.r, color.g, color.b, 1);

        for (int y = 0; y < lengthY; y++)
        {
            float bufferY = y + start.y;
            for (int x = 0; x < lengthX; x++)
            {
                float bufferX = x + start.x;
                if (OnlineMapsUtils.IsPointInPolygon(bufferPoints, bufferX, bufferY))
                {
                    int bufferIndex = (int)bufferY * bufferWidth + (int)bufferX;
                    if (color.a == 1) buffer[bufferIndex] = color;
                    else buffer[bufferIndex] = Color.Lerp(buffer[bufferIndex], clr, color.a);
                }
            }
        }
    }

    protected void InitLineMesh(List<Vector2> points, out List<Vector3> verticles, out List<Vector3> normals, out List<int> triangles, out List<Vector2> uv, float weight, bool closed = false)
    {
        List<Vector2> localPoints = GetLocalPoints(points, closed);

        List<Vector2> activePoints = new List<Vector2>();

        Rect mapRect = new Rect(0, 0, api.tilesetSize.x, api.tilesetSize.y);
        Vector2 lastPoint = Vector2.zero;

        Vector2 rightTop = new Vector2(api.tilesetSize.x, 0);
        Vector2 rightBottom = new Vector2(api.tilesetSize.x, api.tilesetSize.y);
        Vector2 leftBottom = new Vector2(0, api.tilesetSize.y);

        verticles = new List<Vector3>();
        normals = new List<Vector3>();
        triangles = new List<int>();
        uv = new List<Vector2>();

        for (int i = 0; i < localPoints.Count; i++)
        {
            Vector2 p = localPoints[i];

            if (lastPoint != Vector2.zero)
            {
                Vector2 crossTop = OnlineMapsUtils.LineIntersection(lastPoint, p, Vector2.zero, rightTop);
                Vector2 crossBottom = OnlineMapsUtils.LineIntersection(lastPoint, p, leftBottom, rightBottom);
                Vector2 crossLeft = OnlineMapsUtils.LineIntersection(lastPoint, p, Vector2.zero, leftBottom);
                Vector2 crossRight = OnlineMapsUtils.LineIntersection(lastPoint, p, rightTop, rightBottom);

                List<Vector2> intersections = new List<Vector2>();
                if (crossTop != Vector2.zero) intersections.Add(crossTop);
                if (crossBottom != Vector2.zero) intersections.Add(crossBottom);
                if (crossLeft != Vector2.zero) intersections.Add(crossLeft);
                if (crossRight != Vector2.zero) intersections.Add(crossRight);

                if (intersections.Count == 1)
                    activePoints.Add(intersections[0]);
                else if (intersections.Count == 2)
                {
                    int minIndex = ((lastPoint - intersections[0]).magnitude < (lastPoint - intersections[1]).magnitude)
                        ? 0
                        : 1;
                    activePoints.Add(intersections[minIndex]);
                    activePoints.Add(intersections[1 - minIndex]);
                }
            }

            if (mapRect.Contains(p)) activePoints.Add(p);
            else if (activePoints.Count > 0)
                DrawActivePoints(ref activePoints, ref verticles, ref normals, ref triangles, ref uv, weight);

            lastPoint = p;
        }

        if (activePoints.Count > 0)
            DrawActivePoints(ref activePoints, ref verticles, ref normals, ref triangles, ref uv, weight);
    }

    protected bool InitMesh(OnlineMapsTileSetControl control, string name, Color borderColor, Color backgroundColor = default(Color))
    {
        if (mesh != null) return false;
        gameObject = new GameObject(name);
        gameObject.transform.parent = control.drawingsGameObject.transform;
        gameObject.transform.localPosition = Vector3.zero;
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        mesh = new Mesh();
        mesh.name = name;
        meshFilter.mesh = mesh;
        renderer.materials = new Material[backgroundColor != default(Color)?2: 1];
        Shader shader = Shader.Find("Transparent/Diffuse");
        renderer.materials[0] = new Material(shader);
        renderer.materials[0].color = borderColor;
        renderer.materials[0].shader = shader;

        if (backgroundColor != default(Color))
        {
            renderer.materials[1] = new Material(shader);
            renderer.materials[1].color = backgroundColor;
            renderer.materials[1].shader = shader;
        }

        return true;
    }
}