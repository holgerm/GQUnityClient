/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityEngine;

/// <summary>
/// Helper class, which contains all the basic methods.
/// </summary>
public static class OnlineMapsUtils
{
    /// <summary>
    /// Arcseconds in meters.
    /// </summary>
    public const float angleSecond = 1 / 3600f;

    /// <summary>
    /// Bytes per megabyte.
    /// </summary>
    public const int mb = 1024 * 1024;

    /// <summary>
    /// Size of the tile texture in pixels.
    /// </summary>
    public const short tileSize = 256;

    /// <summary>
    /// The second in ticks.
    /// </summary>
    public const long second = 10000000;

    public const string serviceURL = "http://service.infinity-code.com/redirect.php?";

    /// <summary>
    /// tileSize squared, to accelerate the calculations.
    /// </summary>
    public const int sqrTileSize = tileSize * tileSize;

    /// <summary>
    /// The angle between the two points in radians.
    /// </summary>
    /// <param name="point1">Point 1</param>
    /// <param name="point2">Point 2</param>
    /// <param name="offset">Result offset in degrees.</param>
    /// <returns>Angle in radians</returns>
    public static float Angle2DRad(Vector3 point1, Vector3 point2, float offset = 0)
    {
        return Mathf.Atan2((point2.z - point1.z), (point2.x - point1.x)) + offset * Mathf.Deg2Rad;
    }

    public static void ApplyColorArray(ref Color[] result, int px, int py, int width, int height, ref Color[] color, int rx = 0, int ry = 0, int mx = 1, int my = 1)
    {
        const int s = tileSize;
        int hs = s / 2 * my;
        for (int cy = 0; cy < height; cy++)
        {
            int scy = (cy + py) * s;
            int hcy = (cy + ry) * hs;
            for (int cx = 0; cx < width; cx++) result[scy + cx + px] = color[hcy + (cx + rx) * mx];
        }
    }

    /// <summary>
    /// Clamps a value between a minimum double and maximum double value.
    /// </summary>
    /// <param name="n">Value</param>
    /// <param name="minValue">Minimum</param>
    /// <param name="maxValue">Maximum</param>
    /// <returns>Value between a minimum and maximum.</returns>
    public static double Clip(double n, double minValue, double maxValue)
    {
        return Math.Min(Math.Max(n, minValue), maxValue);
    }

    /// <summary>
    /// The distance between two geographical coordinates.
    /// </summary>
    /// <param name="point1">Coordiate (X - Lng, Y - Lat)</param>
    /// <param name="point2">Coordiate (X - Lng, Y - Lat)</param>
    /// <returns>Distance (km).</returns>
    public static Vector2 DistanceBetweenPoints(Vector2 point1, Vector2 point2)
    {
        const float R = 6371;

        Vector2 range = point1 - point2;

        double scfY = Math.Sin(point1.y * Mathf.Deg2Rad);
        double sctY = Math.Sin(point2.y * Mathf.Deg2Rad);
        double ccfY = Math.Cos(point1.y * Mathf.Deg2Rad);
        double cctY = Math.Cos(point2.y * Mathf.Deg2Rad);
        double cX = Math.Cos(range.x * Mathf.Deg2Rad);
        double sizeX1 = Math.Abs(R * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
        double sizeX2 = Math.Abs(R * Math.Acos(sctY * sctY + cctY * cctY * cX));
        float sizeX = (float)((sizeX1 + sizeX2) / 2.0);
        float sizeY = (float)(R * Math.Acos(scfY * sctY + ccfY * cctY));
        return new Vector2(sizeX, sizeY);
    }

    public static Vector2 FixAngle(this Vector2 v)
    {
        float y = v.y;
        if (y < -90) y = -90;
        else if (y > 90) y = 90;
        return new Vector2(Mathf.Repeat(v.x + 180, 360) - 180, y);
    }

    public static void FlipNegative(this Rect r)
    {
        if (r.width < 0) r.x -= (r.width *= -1);
        if (r.height < 0) r.y -= (r.height *= -1);
    }

    /// <summary>
    /// Get the center point and best zoom for the array of markers.
    /// </summary>
    /// <param name="markers">Array of markers.</param>
    /// <param name="center">Center point.</param>
    /// <param name="zoom">Best zoom.</param>
    public static void GetCenterPointAndZoom(OnlineMapsMarker[] markers, out Vector2 center, out int zoom)
    {
        float minX = Single.MaxValue;
        float minY = Single.MaxValue;
        float maxX = Single.MinValue;
        float maxY = Single.MinValue;

        foreach (OnlineMapsMarker marker in markers)
        {
            if (marker.position.x < minX) minX = marker.position.x;
            if (marker.position.y < minY) minY = marker.position.y;
            if (marker.position.x > maxX) maxX = marker.position.x;
            if (marker.position.y > maxY) maxY = marker.position.y;
        }

        float rx = maxX - minX;
        float ry = maxY - minY;
        center = new Vector2(rx / 2 + minX, ry / 2 + minY);

        int width = OnlineMaps.instance.width;
        int height = OnlineMaps.instance.height;

        float countX = width / (float)tileSize / 2;
        float countY = height / (float)tileSize / 2;

        for (int z = 20; z > 4; z--)
        {
            bool success = true;

            foreach (OnlineMapsMarker marker in markers)
            {
                Vector2 p = LatLongToTilef(marker.position, z);
                Vector2 bufferPosition = LatLongToTilef(center, z);
                p.x -= bufferPosition.x - countX;
                p.y -= bufferPosition.y - countY;
                OnlineMapsVector2i ip = marker.GetAlignedPosition(new OnlineMapsVector2i((int)(p.x * tileSize), (int)(p.y * tileSize)));
                if (ip.x < 0 || ip.y < 0 || ip.x + marker.width > width || ip.y + marker.height > height)
                {
                    success = false;
                    break;
                }
            }
            if (success)
            {
                zoom = z - 1;
                return;
            }
        }

        zoom = 3;
    }

    /// <summary>
    /// Get float value from child of node.
    /// </summary>
    /// <param name="node">Node</param>
    /// <param name="subNodeName">Sub node name.</param>
    /// <returns>Float value.</returns>
    public static float GetFloat(this XmlNode node, string subNodeName)
    {
        return Single.Parse(node.SelectSingleNode(subNodeName).InnerText);
    }

    /// <summary>
    /// Get int value from child of node.
    /// </summary>
    /// <param name="node">Node</param>
    /// <param name="subNodeName">Sub node name.</param>
    /// <returns>Integer value.</returns>
    public static int GetInt(this XmlNode node, string subNodeName)
    {
        return Int32.Parse(node.SelectSingleNode(subNodeName).InnerText);
    }

    public static Vector2 GetIntersectionPointOfTwoLines(Vector2 p11, Vector2 p12, Vector2 p21, Vector2 p22,
        out int state)
    {
        state = -2;
        Vector2 result = new Vector2();
        float m = ((p22.x - p21.x) * (p11.y - p21.y) - (p22.y - p21.y) * (p11.x - p21.x));
        float n = ((p22.y - p21.y) * (p12.x - p11.x) - (p22.x - p21.x) * (p12.y - p11.y));

        float Ua = m / n;

        if (n == 0 && m != 0) state = -1;
        else if (m == 0 && n == 0) state = 0;
        else
        {
            result.x = p11.x + Ua * (p12.x - p11.x);
            result.y = p11.y + Ua * (p12.y - p11.y);

            if (((result.x >= p11.x || result.x <= p11.x) && (result.x >= p21.x || result.x <= p21.x))
                && ((result.y >= p11.y || result.y <= p11.y) && (result.y >= p21.y || result.y <= p21.y))) state = 1;
        }
        return result;
    }

    public static Vector2 GetIntersectionPointOfTwoLines(Vector3 p11, Vector3 p12, Vector3 p21, Vector3 p22,
        out int state)
    {
        return GetIntersectionPointOfTwoLines(new Vector2(p11.x, p11.z), new Vector2(p12.x, p12.z),
            new Vector2(p21.x, p21.z), new Vector2(p22.x, p22.z), out state);
    }

    public static Vector2 GetLatLng(this XmlNode node, string subNodeName)
    {
        XmlNode subNode = node.SelectSingleNode(subNodeName);
        return new Vector2(subNode.GetFloat("lng"), subNode.GetFloat("lat"));
    }

    public static string GetMD5(byte[] bytes)
    {
        byte[] hashBytes = MD5.Create().ComputeHash(bytes);
        string hash = hashBytes.Aggregate("", (current, b) => current + b.ToString("x2"));
        return hash;
    }

    public static bool Intersect(this Rect a, Rect b)
    {
        a.FlipNegative();
        b.FlipNegative();
        if (a.xMin >= b.xMax) return false;
        if (a.xMax <= b.xMin) return false;
        if (a.yMin >= b.yMax) return false;
        if (a.yMax <= b.yMin) return false;

        return true;
    }

    public static Vector2 Crossing(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        if (p3.x == p4.x)
        {
            float y = p1.y + ((p2.y - p1.y) * (p3.x - p1.x)) / (p2.x - p1.x);
            if (y > Mathf.Max(p3.y, p4.y) || y < Mathf.Min(p3.y, p4.y) || y > Mathf.Max(p1.y, p2.y) || y < Mathf.Min(p1.y, p2.y)) return Vector2.zero;
            Debug.Log("Cross Vertical");
            return new Vector2(p3.x, y);
        }
        float x = p1.x + ((p2.x - p1.x) * (p3.y - p1.y)) / (p2.y - p1.y);
        if (x > Mathf.Max(p3.x, p4.x) || x < Mathf.Min(p3.x, p4.x) || x > Mathf.Max(p1.x, p2.x) || x < Mathf.Min(p1.x, p2.x)) return Vector2.zero;
        return new Vector2(x, p3.y);
    }

    public static Vector2 LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num, offset;
        float x1lo, x1hi, y1lo, y1hi;

        Ax = p2.x - p1.x;
        Bx = p3.x - p4.x;

        if (Ax < 0)
        {
            x1lo = p2.x; x1hi = p1.x;
        }
        else
        {
            x1hi = p2.x; x1lo = p1.x;
        }

        if (Bx > 0)
        {
            if (x1hi < p4.x || p3.x < x1lo) return Vector2.zero;
        }
        else
        {
            if (x1hi < p3.x || p4.x < x1lo) return Vector2.zero;
        }

        Ay = p2.y - p1.y;
        By = p3.y - p4.y;

        if (Ay < 0)
        {
            y1lo = p2.y; y1hi = p1.y;
        }
        else
        {
            y1hi = p2.y; y1lo = p1.y;
        }

        if (By > 0)
        {
            if (y1hi < p4.y || p3.y < y1lo) return Vector2.zero;
        }
        else
        {
            if (y1hi < p3.y || p4.y < y1lo) return Vector2.zero;
        }

        Cx = p1.x - p3.x;
        Cy = p1.y - p3.y;
        d = By * Cx - Bx * Cy;
        f = Ay * Bx - Ax * By;

        if (f > 0)
        {
            if (d < 0 || d > f) return Vector2.zero;
        }
        else
        {
            if (d > 0 || d < f) return Vector2.zero;
        }

        e = Ax * Cy - Ay * Cx;

        if (f > 0)
        {
            if (e < 0 || e > f) return Vector2.zero;
        }
        else
        {
            if (e > 0 || e < f) return Vector2.zero;
        }

        if (f == 0) return Vector2.zero;

        Vector2 intersection;

        num = d * Ax;
        offset = same_sign(num, f) ? f * 0.5f : -f * 0.5f;
        intersection.x = p1.x + (num + offset) / f;

        num = d * Ay;
        offset = same_sign(num, f) ? f * 0.5f : -f * 0.5f;
        intersection.y = p1.y + (num + offset) / f;

        return intersection;
    }

    private static bool same_sign(float a, float b)
    {
        return ((a * b) >= 0f);
    }

    public static bool IsPointInPolygon(List<Vector2> poly, float x, float y)
    {
        int i, j;
        bool c = false;
        for (i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
        {
            if (((poly[i].y <= y && y < poly[j].y) || (poly[j].y <= y && y < poly[i].y)) && 
                x < (poly[j].x - poly[i].x) * (y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x)
                c = !c;
        }
        return c;
    }

    /// <summary>
    /// Converts geographic coordinates to Mercator coordinates.
    /// </summary>
    /// <param name="x">Longitude</param>
    /// <param name="y">Latitude</param>
    /// <returns>Mercator coordinates</returns>
    public static Vector2 LatLongToMercat(float x, float y)
    {
        float sy = Mathf.Sin(y * Mathf.Deg2Rad);
        return new Vector2((x + 180) / 360, 0.5f - Mathf.Log((1 + sy) / (1 - sy)) / (4 * Mathf.PI));
    }

    /// <summary>
    /// Converts geographic coordinates to the index of the tile.
    /// What is the tiles, and how it works, you can read here:
    /// https://developers.google.com/maps/documentation/javascript/v2/overlays?csw=1#Google_Maps_Coordinates
    /// </summary>
    /// <param name="x">Longitude</param>
    /// <param name="y">Latitude</param>
    /// <param name="zoom">Zoom</param>
    /// <returns>Tile index</returns>
    private static OnlineMapsVector2i LatLongToTile(float x, float y, int zoom)
    {
        Vector2 mPos = LatLongToMercat(x, y);
        uint mapSize = (uint) tileSize << zoom;
        int px = (int) Clip(mPos.x * mapSize + 0.5, 0, mapSize - 1);
        int py = (int) Clip(mPos.y * mapSize + 0.5, 0, mapSize - 1);
        int ix = px / tileSize;
        int iy = py / tileSize;

        return new OnlineMapsVector2i(ix, iy);
    }

    /// <summary>
    /// Converts geographic coordinates to the index of the tile.
    /// What is the tiles, and how it works, you can read here:
    /// https://developers.google.com/maps/documentation/javascript/v2/overlays?csw=1#Google_Maps_Coordinates
    /// </summary>
    /// <param name="p">Geographic coordinates (X - Lng, Y - Lat)</param>
    /// <param name="zoom">Zoom</param>
    /// <returns>Tile index</returns>
    public static OnlineMapsVector2i LatLongToTile(Vector2 p, int zoom)
    {
        return LatLongToTile(p.x, p.y, zoom);
    }

    /// <summary>
    /// Converts geographic coordinates to tile coordinates.
    /// What is the tiles, and how it works, you can read here:
    /// https://developers.google.com/maps/documentation/javascript/v2/overlays?csw=1#Google_Maps_Coordinates
    /// </summary>
    /// <param name="p">Geographic coordinates (X - Lng, Y - Lat)</param>
    /// <param name="zoom">Zoom</param>
    /// <returns>Tile coordinates</returns>
    public static Vector2 LatLongToTilef(Vector2 p, int zoom)
    {
        Vector2 mPos = LatLongToMercat(p.x, p.y);
        uint mapSize = (uint) tileSize << zoom;
        int px = (int) Clip(mPos.x * mapSize + 0.5, 0, mapSize - 1);
        int py = (int) Clip(mPos.y * mapSize + 0.5, 0, mapSize - 1);
        float fx = px / (float) tileSize;
        float fy = py / (float) tileSize;

        return new Vector2(fx, fy);
    }

    /// <summary>
    /// Converts geographic coordinates to tile coordinates.
    /// What is the tiles, and how it works, you can read here:
    /// https://developers.google.com/maps/documentation/javascript/v2/overlays?csw=1#Google_Maps_Coordinates
    /// </summary>
    /// <param name="x">Longitude</param>
    /// <param name="y">Latitude</param>
    /// <param name="zoom">Zoom</param>
    /// <returns>Tile coordinates</returns>
    public static Vector2 LatLongToTilef(float x, float y, int zoom)
    {
        Vector2 mPos = LatLongToMercat(x, y);
        uint mapSize = (uint)tileSize << zoom;
        int px = (int)Clip(mPos.x * mapSize + 0.5, 0, mapSize - 1);
        int py = (int)Clip(mPos.y * mapSize + 0.5, 0, mapSize - 1);
        float fx = px / (float)tileSize;
        float fy = py / (float)tileSize;

        return new Vector2(fx, fy);
    }

    public static Vector2 NearestPointStrict(this Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        var fullDirection = lineEnd - lineStart;
        var lineDirection = fullDirection.normalized;
        var closestPoint = Vector2.Dot((point - lineStart), lineDirection) / Vector2.Dot(lineDirection, lineDirection);
        return lineStart + (Mathf.Clamp(closestPoint, 0, fullDirection.magnitude) * lineDirection);
    }

    private static double Repeat(double n, double minValue, double maxValue)
    {
        double range = maxValue - minValue;
        while (n < minValue || n > maxValue)
        {
            if (n < minValue) n += range;
            else if (n > maxValue) n -= range;
        }
        return n;
    }

    /// <summary>
    /// Converts tile coordinates to geographic coordinates.
    /// What is the tiles, and how it works, you can read here:
    /// https://developers.google.com/maps/documentation/javascript/v2/overlays?csw=1#Google_Maps_Coordinates
    /// </summary>
    /// <param name="x">Tile X</param>
    /// <param name="y">Tile Y</param>
    /// <param name="zoom">Zoom</param>
    /// <returns>Geographic coordinates (X - Lng, Y - Lat)</returns>
    public static Vector2 TileToLatLong(int x, int y, int zoom)
    {
        double mapSize = tileSize << zoom;
        double lx = 360 * ((Repeat(x * tileSize, 0, mapSize - 1) / mapSize) - 0.5);
        double ly = 90 -
                    360 * Math.Atan(Math.Exp(-(0.5 - (Clip(y * tileSize, 0, mapSize - 1) / mapSize)) * 2 * Math.PI)) /
                    Math.PI;
        return new Vector2((float) lx, (float) ly);
    }

    /// <summary>
    /// Converts tile coordinates to geographic coordinates.
    /// What is the tiles, and how it works, you can read here:
    /// https://developers.google.com/maps/documentation/javascript/v2/overlays?csw=1#Google_Maps_Coordinates
    /// </summary>
    /// <param name="x">Tile X</param>
    /// <param name="y">Tile Y</param>
    /// <param name="zoom">Zoom</param>
    /// <returns>Geographic coordinates (X - Lng, Y - Lat)</returns>
    public static Vector2 TileToLatLong(float x, float y, int zoom)
    {
        double mapSize = tileSize << zoom;
        double lx = 360 * ((Repeat(x * tileSize, 0, mapSize - 1) / mapSize) - 0.5);
        double ly = 90 -
                    360 * Math.Atan(Math.Exp(-(0.5 - (Clip(y * tileSize, 0, mapSize - 1) / mapSize)) * 2 * Math.PI)) /
                    Math.PI;
        return new Vector2((float) lx, (float) ly);
    }

    /// <summary>
    /// Converts tile coordinates to geographic coordinates.
    /// What is the tiles, and how it works, you can read here:
    /// https://developers.google.com/maps/documentation/javascript/v2/overlays?csw=1#Google_Maps_Coordinates
    /// </summary>
    /// <param name="p">Tile coordinates</param>
    /// <param name="zoom">Zoom</param>
    /// <returns>Geographic coordinates (X - Lng, Y - Lat)</returns>
    public static Vector2 TileToLatLong(Vector2 p, int zoom)
    {
        return TileToLatLong(p.x, p.y, zoom);
    }

    /// <summary>
    /// Converts tile index to quadkey.
    /// What is the tiles and quadkey, and how it works, you can read here:
    /// http://msdn.microsoft.com/en-us/library/bb259689.aspx
    /// </summary>
    /// <param name="x">Tile X</param>
    /// <param name="y">Tile Y</param>
    /// <param name="zoom">Zoom</param>
    /// <returns>Quadkey</returns>
    public static string TileToQuadKey(int x, int y, int zoom)
    {
        StringBuilder quadKey = new StringBuilder();
        for (int i = zoom; i > 0; i--)
        {
            char digit = '0';
            int mask = 1 << (i - 1);
            if ((x & mask) != 0) digit++;
            if ((y & mask) != 0)
            {
                digit++;
                digit++;
            }
            quadKey.Append(digit);
        }
        return quadKey.ToString();
    }

    public static IEnumerable<int> Triangulate(List<Vector2> points)
    {
        List<int> indices = new List<int>();

        int n = points.Count;
        if (n < 3) return indices;

        int[] V = new int[n];
        if (TriangulateArea(points) > 0) for (int v = 0; v < n; v++) V[v] = v;
        else for (int v = 0; v < n; v++) V[v] = (n - 1) - v;

        int nv = n;
        int count = 2 * nv;
        for (int v = nv - 1; nv > 2; )
        {
            if ((count--) <= 0) return indices;

            int u = v;
            if (nv <= u) u = 0;
            v = u + 1;
            if (nv <= v) v = 0;
            int w = v + 1;
            if (nv <= w) w = 0;

            if (TriangulateSnip(points, u, v, w, nv, V))
            {
                int s, t;
                indices.Add(V[u]);
                indices.Add(V[v]);
                indices.Add(V[w]);
                for (s = v, t = v + 1; t < nv; s++, t++) V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices;
    }

    private static float TriangulateArea(List<Vector2> points)
    {
        int n = points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = points[p];
            Vector2 qval = points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private static bool TriangulateInsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float bp = (C.x - B.x) * (P.y - B.y) - (C.y - B.y) * (P.x - B.x);
        float ap = (B.x - A.x) * (P.y - A.y) - (B.y - A.y) * (P.x - A.x);
        float cp = (A.x - C.x) * (P.y - C.y) - (A.y - C.y) * (P.x - C.x);
        return ((bp >= 0.0f) && (cp >= 0.0f) && (ap >= 0.0f));
    }

    private static bool TriangulateSnip(List<Vector2> points, int u, int v, int w, int n, int[] V)
    {
        Vector2 A = points[V[u]];
        Vector2 B = points[V[v]];
        Vector2 C = points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x)))) return false;
        for (int p = 0; p < n; p++)
        {
            if (p == u || p == v || p == w) continue;
            if (TriangulateInsideTriangle(A, B, C, points[V[p]])) return false;
        }
        return true;
    }
}