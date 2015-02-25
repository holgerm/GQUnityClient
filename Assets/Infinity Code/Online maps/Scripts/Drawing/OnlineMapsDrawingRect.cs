/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that draws a rectangle on the map.
/// </summary>
[Serializable]
public class OnlineMapsDrawingRect : OnlineMapsDrawingElement
{
    /// <summary>
    /// Background color of the rectangle.
    /// </summary>
    public Color backgroundColor = new Color(1, 1, 1, 0);

    /// <summary>
    /// Border color of the rectangle.
    /// </summary>
    public Color borderColor = Color.black;

    /// <summary>
    /// Border weight of the rectangle.
    /// </summary>
    public float borderWeight = 1;
    
    private List<Vector2> points;
    private float _height = 1;
    private float _width = 1;
    private float _x = 0;
    private float _y = 0;

    /// <summary>
    /// Gets or sets the width of the rectangle. Geographic coordinates.
    /// </summary>
    public float width
    {
        get { return _width; }
        set
        {
            _width = value;
            InitPoints();
            OnlineMaps.instance.needRedraw = true;
        }
    }

    /// <summary>
    /// Gets or sets the height of the rectangle. Geographic coordinates.
    /// </summary>
    public float height
    {
        get { return _height; }
        set
        {
            _height = value;
            InitPoints();
            OnlineMaps.instance.needRedraw = true;
        }
    }

    /// <summary>
    /// Gets or sets the x position of the rectangle. Geographic coordinates.
    /// </summary>
    public float x
    {
        get { return _x; }
        set
        {
            _x = value;
            InitPoints();
            OnlineMaps.instance.needRedraw = true;
        }
    }

    /// <summary>
    /// Gets or sets the y position of the rectangle. Geographic coordinates.
    /// </summary>
    public float y
    {
        get { return _y; }
        set
        {
            _y = value;
            InitPoints();
            OnlineMaps.instance.needRedraw = true;
        }
    }

    /// <summary>
    /// Creates a new rectangle.
    /// </summary>
    /// <param name="x">Position X. Geographic coordinates.</param>
    /// <param name="y">Position Y. Geographic coordinates.</param>
    /// <param name="width">Width. Geographic coordinates.</param>
    /// <param name="height">Height. Geographic coordinates.</param>
    public OnlineMapsDrawingRect(float x, float y, float width, float height)
    {
        _x = x;
        _y = y;
        _width = width;
        _height = height;

        InitPoints();
    }

    /// <summary>
    /// Creates a new rectangle.
    /// </summary>
    /// <param name="position">The position of the rectangle. Geographic coordinates.</param>
    /// <param name="size">The size of the rectangle. Geographic coordinates.</param>
    public OnlineMapsDrawingRect(Vector2 position, Vector2 size):this(position.x, position.y, size.x, size.y)
    {
        
    }

    /// <summary>
    /// Creates a new rectangle.
    /// </summary>
    /// <param name="rect">Rectangle. Geographic coordinates.</param>
    public OnlineMapsDrawingRect(Rect rect): this(rect.x, rect.y, rect.width, rect.height)
    {
        
    }

    /// <summary>
    /// Creates a new rectangle.
    /// </summary>
    /// <param name="x">Position X. Geographic coordinates.</param>
    /// <param name="y">Position Y. Geographic coordinates.</param>
    /// <param name="width">Width. Geographic coordinates.</param>
    /// <param name="height">Height. Geographic coordinates.</param>
    /// <param name="borderColor">Border color.</param>
    public OnlineMapsDrawingRect(float x, float y, float width, float height, Color borderColor)
        : this(x, y, width, height)
    {
        this.borderColor = borderColor;
    }

    /// <summary>
    /// Creates a new rectangle.
    /// </summary>
    /// <param name="position">The position of the rectangle. Geographic coordinates.</param>
    /// <param name="size">The size of the rectangle. Geographic coordinates.</param>
    /// <param name="borderColor">Border color.</param>
    public OnlineMapsDrawingRect(Vector2 position, Vector2 size, Color borderColor)
        : this(position.x, position.y, size.x, size.y)
    {
        this.borderColor = borderColor;
    }

    /// <summary>
    /// Creates a new rectangle.
    /// </summary>
    /// <param name="rect">Rectangle. Geographic coordinates.</param>
    /// <param name="borderColor">Border color.</param>
    public OnlineMapsDrawingRect(Rect rect, Color borderColor)
        : this(rect)
    {
        this.borderColor = borderColor;
    }

    /// <summary>
    /// Creates a new rectangle.
    /// </summary>
    /// <param name="x">Position X. Geographic coordinates.</param>
    /// <param name="y">Position Y. Geographic coordinates.</param>
    /// <param name="width">Width. Geographic coordinates.</param>
    /// <param name="height">Height. Geographic coordinates.</param>
    /// <param name="borderColor">Border color.</param>
    /// <param name="borderWeight">Border weight.</param>
    public OnlineMapsDrawingRect(float x, float y, float width, float height, Color borderColor, float borderWeight)
        : this(x, y, width, height, borderColor)
    {
        this.borderWeight = borderWeight;
    }

    /// <summary>
    /// Creates a new rectangle.
    /// </summary>
    /// <param name="position">The position of the rectangle. Geographic coordinates.</param>
    /// <param name="size">The size of the rectangle. Geographic coordinates.</param>
    /// <param name="borderColor">Border color.</param>
    /// <param name="borderWeight">Border weight.</param>
    public OnlineMapsDrawingRect(Vector2 position, Vector2 size, Color borderColor, float borderWeight)
        : this(position, size, borderColor)
    {
        this.borderWeight = borderWeight;
    }

    /// <summary>
    /// Creates a new rectangle.
    /// </summary>
    /// <param name="rect">Rectangle. Geographic coordinates.</param>
    /// <param name="borderColor">Border color.</param>
    /// <param name="borderWeight">Border weight.</param>
    public OnlineMapsDrawingRect(Rect rect, Color borderColor, float borderWeight)
        : this(rect, borderColor)
    {
        this.borderWeight = borderWeight;
    }

    /// <summary>
    /// Creates a new rectangle.
    /// </summary>
    /// <param name="x">Position X. Geographic coordinates.</param>
    /// <param name="y">Position Y. Geographic coordinates.</param>
    /// <param name="width">Width. Geographic coordinates.</param>
    /// <param name="height">Height. Geographic coordinates.</param>
    /// <param name="borderColor">Border color.</param>
    /// <param name="borderWeight">Border weight.</param>
    /// <param name="backgroundColor">Background color.</param>
    public OnlineMapsDrawingRect(float x, float y, float width, float height, Color borderColor, float borderWeight, Color backgroundColor)
        : this(x, y, width, height, borderColor, borderWeight)
    {
        this.backgroundColor = backgroundColor;
    }

    /// <summary>
    /// Creates a new rectangle.
    /// </summary>
    /// <param name="position">The position of the rectangle. Geographic coordinates.</param>
    /// <param name="size">The size of the rectangle. Geographic coordinates.</param>
    /// <param name="borderColor">Border color.</param>
    /// <param name="borderWeight">Border weight.</param>
    /// <param name="backgroundColor">Background color.</param>
    public OnlineMapsDrawingRect(Vector2 position, Vector2 size, Color borderColor, float borderWeight, Color backgroundColor)
        : this(position, size, borderColor, borderWeight)
    {
        this.backgroundColor = backgroundColor;
    }

    /// <summary>
    /// Creates a new rectangle.
    /// </summary>
    /// <param name="rect">Rectangle. Geographic coordinates.</param>
    /// <param name="borderColor">Border color.</param>
    /// <param name="borderWeight">Border weight.</param>
    /// <param name="backgroundColor">Background color.</param>
    public OnlineMapsDrawingRect(Rect rect, Color borderColor, float borderWeight, Color backgroundColor)
        : this(rect, borderColor, borderWeight)
    {
        this.backgroundColor = backgroundColor;
    }

    public override void Draw(Color[] buffer, OnlineMapsVector2i bufferPosition, int bufferWidth, int bufferHeight, int zoom)
    {
        FillPoly(buffer, bufferPosition, bufferWidth, bufferHeight, zoom, points, backgroundColor);
        DrawLineToBuffer(buffer, bufferPosition, bufferWidth, bufferHeight, zoom, points, borderColor, borderWeight, true);
    }

    public override void DrawOnTileset(OnlineMapsTileSetControl control)
    {
        InitMesh(control, "Rect", borderColor, backgroundColor);

        List<Vector2> localPoints = GetLocalPoints(points, true);

        if (localPoints.All(p => p.x < 0))
        {
            active = false;
            return;
        }
        if (localPoints.All(p => p.x > api.tilesetSize.x))
        {
            active = false;
            return;
        }
        if (localPoints.All(p => p.y < 0))
        {
            active = false;
            return;
        }
        if (localPoints.All(p => p.y > api.tilesetSize.y))
        {
            active = false;
            return;
        }

        if (!active) active = true;

        bool ignoreLeft = false;
        bool ignoreRight = false;
        bool ignoreTop = false;
        bool ignoreBottom = false;
        int countIgnore = 0;

        for (int i = 0; i < localPoints.Count; i++)
        {
            Vector2 point = localPoints[i];
            if (point.x < 0)
            {
                point.x = 0;
                if (!ignoreLeft) countIgnore++;
                ignoreLeft = true;
            }
            if (point.y < 0)
            {
                point.y = 0;
                if (!ignoreTop) countIgnore++;
                ignoreTop = true;
            }
            if (point.x > api.tilesetSize.x)
            {
                point.x = api.tilesetSize.x;
                if (!ignoreRight) countIgnore++;
                ignoreRight = true;
            }
            if (point.y > api.tilesetSize.y)
            {
                point.y = api.tilesetSize.y;
                if (!ignoreBottom) countIgnore++;
                ignoreBottom = true;
            }

            localPoints[i] = point;
        }

        List<Vector3> verticles = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> backTriangles = new List<int>();
        List<int> borderTriangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        verticles.Add(new Vector3(-localPoints[0].x, -0.05f, localPoints[0].y));
        verticles.Add(new Vector3(-localPoints[1].x, -0.05f, localPoints[1].y));
        verticles.Add(new Vector3(-localPoints[2].x, -0.05f, localPoints[2].y));
        verticles.Add(new Vector3(-localPoints[3].x, -0.05f, localPoints[3].y));

        if (!ignoreTop)
        {
            verticles[2] += new Vector3(0, 0, borderWeight);
            verticles[3] += new Vector3(0, 0, borderWeight);
        }

        if (!ignoreBottom)
        {
            verticles[0] -= new Vector3(0, 0, borderWeight);
            verticles[1] -= new Vector3(0, 0, borderWeight);
        }

        if (!ignoreLeft)
        {
            verticles[0] -= new Vector3(borderWeight, 0, 0);
            verticles[3] -= new Vector3(borderWeight, 0, 0);
        }

        if (!ignoreRight)
        {
            verticles[1] += new Vector3(borderWeight, 0, 0);
            verticles[2] += new Vector3(borderWeight, 0, 0);
        }

        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);

        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(0, 1));
        uv.Add(new Vector2(1, 1));
        uv.Add(new Vector2(1, 0));

        backTriangles.Add(0);
        backTriangles.Add(2);
        backTriangles.Add(1);
        backTriangles.Add(0);
        backTriangles.Add(3);
        backTriangles.Add(2);

        List<Vector2> activePoints = new List<Vector2>();

        if (countIgnore == 0)
        {
            activePoints.Add(localPoints[0] + new Vector2(borderWeight, 0));
            activePoints.Add(localPoints[1]);
            activePoints.Add(localPoints[2]);
            activePoints.Add(localPoints[3]);
            activePoints.Add(localPoints[0] + new Vector2(0, borderWeight));
            DrawActivePoints(ref activePoints, ref verticles, ref normals, ref borderTriangles, ref uv, borderWeight);
        }
        else if (countIgnore == 1)
        {
            int off = 0;
            if (ignoreTop) off = 3;
            else if (ignoreRight) off = 2;
            else if (ignoreBottom) off = 1;

            for (int i = 0; i < 4; i++)
            {
                int ci = i + off;
                if (ci > 3) ci -= 4;
                activePoints.Add(localPoints[ci]);
            }
            DrawActivePoints(ref activePoints, ref verticles, ref normals, ref borderTriangles, ref uv, borderWeight);
        }
        else if (countIgnore == 2)
        {
            if (ignoreBottom && ignoreTop)
            {
                activePoints.Add(localPoints[1]);
                activePoints.Add(localPoints[2]);
                DrawActivePoints(ref activePoints, ref verticles, ref normals, ref borderTriangles, ref uv, borderWeight);
                activePoints.Add(localPoints[3]);
                activePoints.Add(localPoints[0]);
                DrawActivePoints(ref activePoints, ref verticles, ref normals, ref borderTriangles, ref uv, borderWeight);
            }
            else if (ignoreLeft && ignoreRight)
            {
                activePoints.Add(localPoints[0]);
                activePoints.Add(localPoints[1]);
                DrawActivePoints(ref activePoints, ref verticles, ref normals, ref borderTriangles, ref uv, borderWeight);
                activePoints.Add(localPoints[2]);
                activePoints.Add(localPoints[3]);
                DrawActivePoints(ref activePoints, ref verticles, ref normals, ref borderTriangles, ref uv, borderWeight);
            }
            else
            {
                DrawActivePointsCI3(ignoreTop, ignoreRight, ignoreBottom, activePoints, localPoints, ref verticles, ref normals, ref borderTriangles, ref uv);
            }
        }
        else if (countIgnore == 3)
        {
            DrawActivePointsCI3(ignoreTop, ignoreRight, ignoreBottom, activePoints, localPoints, ref verticles, ref normals, ref borderTriangles, ref uv);
        }

        mesh.Clear();
        mesh.vertices = verticles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        mesh.subMeshCount = 2;

        mesh.SetTriangles(borderTriangles.ToArray(), 0);
        mesh.SetTriangles(backTriangles.ToArray(), 1);
    }

    private void DrawActivePointsCI3(bool ignoreTop, bool ignoreRight, bool ignoreBottom, List<Vector2> activePoints,
        List<Vector2> localPoints, ref List<Vector3> verticles, ref List<Vector3> normals, ref List<int> borderTriangles, ref List<Vector2> uv)
    {
        int off = 0;

        if (ignoreTop) off = 3;
        else if (ignoreRight) off = 2;
        else if (ignoreBottom) off = 1;

        for (int i = 0; i < 2; i++)
        {
            int ci = i + off;
            if (ci > 3) ci -= 4;
            activePoints.Add(localPoints[ci]);
        }
        DrawActivePoints(ref activePoints, ref verticles, ref normals, ref borderTriangles, ref uv, borderWeight);
    }

    private void InitPoints()
    {
        points = new List<Vector2>
        {
            new Vector2(_x, _y),
            new Vector2(_x + _width, _y),
            new Vector2(_x + _width, _y + _height),
            new Vector2(_x, _y + _height)
        };
    }
}