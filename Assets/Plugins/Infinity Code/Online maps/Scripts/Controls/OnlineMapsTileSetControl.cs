/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Class control the map for the Tileset.
/// Tileset - a dynamic mesh, created at runtime.
/// </summary>
[Serializable]
[AddComponentMenu("Infinity Code/Online Maps/Controls/Tileset")]
public class OnlineMapsTileSetControl : OnlineMapsControlBase3D
{
    /// <summary>
    /// The event, which occurs when the changed texture tile maps.
    /// </summary>
    public Action<OnlineMapsTile, Material> OnChangeMaterialTexture;

    /// <summary>
    /// The event, which intercepts the request to BingMaps Elevation API.
    /// </summary>
    public Action<Vector2, Vector2> OnGetElevation;

    /// <summary>
    /// Bing Maps API key
    /// </summary>
    public string bingAPI = "";

    /// <summary>
    /// Container for drawing elements.
    /// </summary>
    public GameObject drawingsGameObject;

    /// <summary>
    /// Material that will be used for tile.
    /// </summary>
    public Material tileMaterial;

    /// <summary>
    /// Specifies that you want to build a map with the elevetions.
    /// </summary>
    public bool useElevation = false;

    private bool _useElevation;
    private OnlineMapsVector2i bufferPosition;
    private WWW elevationRequest;
    private Rect elevationRequestRect;
    private short[,] elevationData;
    private Rect elevationRect;
    private MeshCollider meshCollider;
    private bool ignoreGetElevation;
    private Mesh tilesetMesh;
    private int[] triangles;
    private Vector2[] uv;
    private Vector3[] vertices;

    [HideInInspector]
    public Shader tilesetShader;

    protected override void AfterUpdate()
    {
        base.AfterUpdate();

        if (elevationRequest != null) CheckElevationRequest();
    }

    private void CheckElevationRequest()
    {
        if (elevationRequest == null || !elevationRequest.isDone) return;

        if (string.IsNullOrEmpty(elevationRequest.error))
        {
            elevationRect = elevationRequestRect;
            string response = elevationRequest.text;

            Match match = Regex.Match(response, "\"elevations\":\\[(.*?)]");
            if (match.Success)
            {
                short[] heights = match.Groups[1].Value.Split(new[] {','}).Select(v => short.Parse(v)).ToArray();
                elevationData = new short[32,32];

                for (int i = 0; i < heights.Length; i++)
                {
                    int x = i % 32;
                    int y = i / 32;
                    elevationData[x, y] = heights[i];
                }
            }

            UpdateControl();
        }
        else
        {
            Debug.LogWarning(elevationRequest.error);
        }
        elevationRequest = null;

        if (ignoreGetElevation) GetElevation();
    }

    public override float GetBestElevationYScale(Vector2 topLeftPosition, Vector2 bottomRightPosition)
    {
        Vector2 realDistance = OnlineMapsUtils.DistanceBetweenPoints(topLeftPosition, bottomRightPosition);
        return Mathf.Min(api.width / realDistance.x, api.height / realDistance.y) / 1000;
    }

    public override Vector2 GetCoords(Vector2 position)
    {
        if (!HitTest()) return Vector2.zero;

        RaycastHit hit;
        if (!collider.Raycast(activeCamera.ScreenPointToRay(position), out hit, 100000))
            return Vector2.zero;

        Vector3 size = (collider.bounds.max - hit.point);
        size.x = size.x / collider.bounds.size.x;
        size.z = size.z / collider.bounds.size.z;

        Vector2 r = new Vector3((size.x - .5f), (size.z - .5f));

        int countX = api.width / OnlineMapsUtils.tileSize;
        int countY = api.height / OnlineMapsUtils.tileSize;

        Vector2 p = OnlineMapsUtils.LatLongToTilef(api.position, api.zoom);
        p.x += countX * r.x;
        p.y -= countY * r.y;
        return OnlineMapsUtils.TileToLatLong(p, api.zoom);
    }

    private void GetElevation()
    {
        ignoreGetElevation = true;

        if (elevationRequest != null) return;

        ignoreGetElevation = false;

        bufferPosition = api.buffer.bufferPosition;
        if (bufferPosition == null) return;

        const int s = OnlineMapsUtils.tileSize;
        int countX = api.width / s + 2;
        int countY = api.height / s + 2;

        Vector2 startCoords = OnlineMapsUtils.TileToLatLong(bufferPosition.x, bufferPosition.y, api.buffer.apiZoom);
        Vector2 endCoords = OnlineMapsUtils.TileToLatLong(bufferPosition.x + countX, bufferPosition.y + countY, api.buffer.apiZoom);

        elevationRequestRect = new Rect(startCoords.x, startCoords.y, endCoords.x - startCoords.x, endCoords.y - startCoords.y);

        if (OnGetElevation == null)
        {
            const string urlPattern =
                "http://dev.virtualearth.net/REST/v1/Elevation/Bounds?bounds={0},{1},{2},{3}&rows=32&cols=32&key={4}";
            string url = string.Format(urlPattern, endCoords.y, startCoords.x, startCoords.y, endCoords.x, bingAPI);

#if !UNITY_WEBPLAYER
            elevationRequest = new WWW(url);
#else
            elevationRequest = new WWW(OnlineMapsUtils.serviceURL + url);
#endif
        }
        else OnGetElevation(startCoords, endCoords);
    }

    public override float GetElevationValue(float x, float z, float yScale, Vector2 topLeftPosition, Vector2 bottomRightPosition)
    {
        if (elevationData == null) return 0;

        x /= -api.tilesetSize.x;
        z /= api.tilesetSize.y;

        float cx = Mathf.Lerp(topLeftPosition.x, bottomRightPosition.x, x);
        float cz = Mathf.Lerp(topLeftPosition.y, bottomRightPosition.y, z);

        float rx = (cx - elevationRect.x) / elevationRect.width * 31;
        float ry = (cz - elevationRect.y) / elevationRect.height * 31;

        rx = Mathf.Clamp(rx, 0, 31);
        ry = Mathf.Clamp(ry, 0, 31);

        int x1 = (int)rx;
        int x2 = x1 + 1;
        int y1 = (int)ry;
        int y2 = y1 + 1;
        if (x2 > 31) x2 = 31;
        if (y2 > 31) y2 = 31;

        float p1 = Mathf.Lerp(elevationData[x1, 31 - y1], elevationData[x2, 31 - y1], rx - x1);
        float p2 = Mathf.Lerp(elevationData[x1, 31 - y2], elevationData[x2, 31 - y2], rx - x1);

        return Mathf.Lerp(p1, p2, ry - y1) * yScale;
    }

    protected override bool HitTest()
    {
#if NGUI
        if (UICamera.Raycast(Input.mousePosition)) return false;
#endif
        RaycastHit hit;

        return collider.Raycast(activeCamera.ScreenPointToRay(Input.mousePosition), out hit, 100000);
    }

    private void InitDrawingsMesh()
    {
        drawingsGameObject = new GameObject("Drawings");
        drawingsGameObject.transform.parent = transform;
        drawingsGameObject.transform.localPosition = new Vector3(0, 0.1f, 0);
    }

    private void InitMapMesh()
    {
        _useElevation = useElevation;

        Shader tileShader = Shader.Find("Infinity Code/Online Maps/Tileset");

        MeshFilter meshFilter;

        if (tilesetMesh == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            meshCollider = gameObject.AddComponent<MeshCollider>();

            tilesetMesh = new Mesh {name = "Tileset"};
        }
        else
        {
            meshFilter = GetComponent<MeshFilter>();
            //tilesetMesh.Clear();
            elevationData = null;
            elevationRequest = null;
            if (useElevation)
            {
                ignoreGetElevation = false;
                bufferPosition = null;
            }
        }

        int w1 = api.tilesetWidth / OnlineMapsUtils.tileSize;
        int h1 = api.tilesetHeight / OnlineMapsUtils.tileSize;

        int subMeshVX = 1;
        int subMeshVZ = 1;

        if (useElevation)
        {
            if (w1 < 32) subMeshVX = 32 % w1 == 0 ? 32 / w1 : 32 / w1 + 1;
            if (h1 < 32) subMeshVZ = 32 % h1 == 0 ? 32 / h1 : 32 / h1 + 1;
        }

        Vector2 subMeshSize = new Vector2(api.tilesetSize.x / w1, api.tilesetSize.y / h1);

        int w = w1 + 2;
        int h = h1 + 2;

        vertices = new Vector3[w * h * subMeshVX * subMeshVZ * 4];
        uv = new Vector2[w * h * subMeshVX * subMeshVZ * 4];
        Vector3[] normals = new Vector3[w * h * subMeshVX * subMeshVZ * 4];
        Material[] materials = new Material[w * h];
        tilesetMesh.subMeshCount = w * h;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                InitMapSubMesh(ref normals, x, y, w, h, subMeshSize, subMeshVX, subMeshVZ);
            }
        }

        tilesetMesh.vertices = vertices;
        tilesetMesh.uv = uv;
        tilesetMesh.normals = normals;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                InitMapSubMeshTriangles(ref materials, x, y, w, h, subMeshVX, subMeshVZ, tileShader);
            }
        }

        triangles = null;

        gameObject.renderer.materials = materials;

#if !UNITY_3_5 && !UNITY_3_5_5
        tilesetMesh.MarkDynamic();
#endif
        tilesetMesh.RecalculateBounds();
        meshFilter.sharedMesh = tilesetMesh;
        meshCollider.sharedMesh = Instantiate(tilesetMesh) as Mesh;
    }

    private void InitMapSubMesh(ref Vector3[] normals, int x, int y, int w, int h, Vector2 subMeshSize, int subMeshVX, int subMeshVZ)
    {
        int i = (x + y * w) * subMeshVX * subMeshVZ * 4;

        Vector2 cellSize = new Vector2(subMeshSize.x / subMeshVX, subMeshSize.y / subMeshVZ);

        float sx = (x > 0 && x < w - 1) ? cellSize.x : 0;
        float sy = (y > 0 && y < h - 1) ? cellSize.y : 0;

        float nextY = subMeshSize.y * (y - 1);

        float uvX = 1f / subMeshVX;
        float uvZ = 1f / subMeshVZ;

        for (int ty = 0; ty < subMeshVZ; ty++)
        {
            float nextX = -subMeshSize.x * (x - 1);

            for (int tx = 0; tx < subMeshVX; tx++)
            {
                int ci = (tx + ty * subMeshVX) * 4 + i;

                vertices[ci] = new Vector3(nextX, 0, nextY);
                vertices[ci + 1] = new Vector3(nextX - sx, 0, nextY);
                vertices[ci + 2] = new Vector3(nextX - sx, 0, nextY + sy);
                vertices[ci + 3] = new Vector3(nextX, 0, nextY + sy);
                
                uv[ci] = new Vector2(1 - uvX * (tx + 1), 1 - uvZ * ty);
                uv[ci + 1] = new Vector2(1 - uvX * tx, 1 - uvZ * ty);
                uv[ci + 2] = new Vector2(1 - uvX * tx, 1 - uvZ * (ty + 1));
                uv[ci + 3] = new Vector2(1 - uvX * (tx + 1), 1 - uvZ * (ty + 1));

                normals[ci] = Vector3.up;
                normals[ci + 1] = Vector3.up;
                normals[ci + 2] = Vector3.up;
                normals[ci + 3] = Vector3.up;

                nextX -= sx;
            }

            nextY += sy;
        }
    }

    private void InitMapSubMeshTriangles(ref Material[] materials, int x, int y, int w, int h, int subMeshVX, int subMeshVZ, Shader tileShader)
    {
        if (triangles == null) triangles = new int[subMeshVX * subMeshVZ * 6];
        int i = (x + y * w) * subMeshVX * subMeshVZ * 4;

        for (int ty = 0; ty < subMeshVZ; ty++)
        {
            for (int tx = 0; tx < subMeshVX; tx++)
            {
                int ci = (tx + ty * subMeshVX) * 4 + i;
                int ti = (tx + ty * subMeshVX) * 6;

                triangles[ti] = ci;
                triangles[ti + 1] = ci + 1;
                triangles[ti + 2] = ci + 2;
                triangles[ti + 3] = ci;
                triangles[ti + 4] = ci + 2;
                triangles[ti + 5] = ci + 3;
            }
        }

        tilesetMesh.SetTriangles(triangles, x + y * w);
        Material material;

        if (tileMaterial != null) material = (Material)Instantiate(tileMaterial);
        else material = new Material(tileShader);

        if (api.defaultTileTexture != null) material.mainTexture = api.defaultTileTexture;
        materials[x + y * w] = material;
    }

    public override void OnAwakeBefore()
    {
        base.OnAwakeBefore();

        api = GetComponent<OnlineMaps>();

        InitMapMesh();

        if (useElevation) GetElevation();
    }

    protected override void OnDestroyLate()
    {
        base.OnDestroyLate();

        elevationData = null;
        elevationRequest = null;
        meshCollider = null;
        tilesetMesh = null;
        triangles = null;
        uv = null;
        vertices = null;
    }

    /// <summary>
    /// Allows you to set the current values ​​of elevation.
    /// </summary>
    /// <param name="data">Elevation data [32x32]</param>
    public void SetElevationData(short[,] data)
    {
        elevationData = data;
        elevationRect = elevationRequestRect;
        UpdateControl();
    }

    public override void UpdateControl()
    {
        base.UpdateControl();

        if (OnlineMapsTile.tiles == null) return;

        if (useElevation != _useElevation)
        {
            triangles = null;
            InitMapMesh();
        }
        UpdateMapMesh();

        if (drawingsGameObject == null) InitDrawingsMesh();
        foreach (OnlineMapsDrawingElement drawingElement in api.drawingElements) drawingElement.DrawOnTileset(this);

        if (marker2DMode == OnlineMapsMarker2DMode.flat) UpdateMarkersMesh();
        else UpdateMarkersBillboard();
    }

    private void UpdateMapMesh()
    {
        if (useElevation && !ignoreGetElevation && api.buffer.bufferPosition != bufferPosition) GetElevation();

        int w1 = api.tilesetWidth / OnlineMapsUtils.tileSize;
        int h1 = api.tilesetHeight / OnlineMapsUtils.tileSize;

        int subMeshVX = 1;
        int subMeshVZ = 1;

        if (useElevation)
        {
            if (w1 < 32) subMeshVX = 32 % w1 == 0 ? 32 / w1 : 32 / w1 + 1;
            if (h1 < 32) subMeshVZ = 32 % h1 == 0 ? 32 / h1 : 32 / h1 + 1;
        }

        Vector2 subMeshSize = new Vector2(api.tilesetSize.x / w1, api.tilesetSize.y / h1);

        Vector2 topLeftPosition = api.topLeftPosition;
        Vector2 bottomRightPosition = api.bottomRightPosition;

        Vector2 tlPos = OnlineMapsUtils.LatLongToTilef(topLeftPosition, api.buffer.apiZoom);
        Vector2 pos = tlPos - api.buffer.bufferPosition;

        int maxX = (2 << api.buffer.apiZoom) / 2;
        if (pos.x >= maxX) pos.x -= maxX;
        
        Vector3 startPos = new Vector3(subMeshSize.x * pos.x, 0, -subMeshSize.y * pos.y);

        float yScale = GetBestElevationYScale(topLeftPosition, bottomRightPosition);

        int w = w1 + 2;
        int h = h1 + 2;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                UpdateMapSubMesh(x, y, w, h, subMeshSize, subMeshVX, subMeshVZ, startPos, yScale, topLeftPosition, bottomRightPosition);
            }
        }

        tilesetMesh.vertices = vertices;
        tilesetMesh.uv = uv;

        for (int i = 0; i < tilesetMesh.subMeshCount; i++) tilesetMesh.SetTriangles(tilesetMesh.GetTriangles(i), i);

        tilesetMesh.RecalculateBounds();

        if (useElevation) meshCollider.sharedMesh = Instantiate(tilesetMesh) as Mesh;
    }

    private void UpdateMapSubMesh(int x, int y, int w, int h, Vector2 subMeshSize, int subMeshVX, int subMeshVZ, Vector3 startPos, float yScale, Vector2 topLeftPosition, Vector2 bottomRightPosition)
    {
        int mi = x + y * w;
        int i = mi * subMeshVX * subMeshVZ * 4;

        Vector2 cellSize = new Vector2(subMeshSize.x / subMeshVX, subMeshSize.y / subMeshVZ);

        float uvX = 1f / subMeshVX;
        float uvZ = 1f / subMeshVZ;

        for (int ty = 0; ty < subMeshVZ; ty++)
        {
            float uvY1 = 1 - uvZ * ty;
            float uvY2 = 1 - uvZ * (ty + 1);

            float z1 = startPos.z + y * subMeshSize.y + ty * cellSize.y;
            float z2 = z1 + cellSize.y;

            if (z1 < 0) z1 = 0;
            if (z1 > api.tilesetSize.y) z1 = api.tilesetSize.y;

            if (z2 < 0) z2 = 0;
            if (z2 > api.tilesetSize.y) z2 = api.tilesetSize.y;

            if (z1 == 0 && z2 > 0) uvY1 = Mathf.Lerp(uvY1, uvY2, 1 - z2 / cellSize.y);
                else if (z1 < api.tilesetSize.y && z2 == api.tilesetSize.y)
                    uvY2 = Mathf.Lerp(uvY1, uvY2, (api.tilesetSize.y - z1) / cellSize.y);

            for (int tx = 0; tx < subMeshVX; tx++)
            {
                float uvX1 = uvX * tx;
                float uvX2 = uvX * (tx + 1);

                float x1 = startPos.x - x * subMeshSize.x - tx * cellSize.x;
                float x2 = x1 - cellSize.x;
                
                if (x1 > 0) x1 = 0;
                if (x1 < -api.tilesetSize.x) x1 = -api.tilesetSize.x;

                if (x2 > 0) x2 = 0;
                if (x2 < -api.tilesetSize.x) x2 = -api.tilesetSize.x;

                if (x1 == 0 && x2 < 0) uvX1 = Mathf.Lerp(uvX2, uvX1, -x2 / cellSize.x);
                else if (x1 > -api.tilesetSize.x && x2 == -api.tilesetSize.x)
                    uvX2 = Mathf.Lerp(uvX2, uvX1, 1 - (x1 + api.tilesetSize.x) / cellSize.x);

                float y1 = 0;
                float y2 = 0;
                float y3 = 0;
                float y4 = 0;

                if (useElevation)
                {
                    y1 = GetElevationValue(x1, z1, yScale, topLeftPosition, bottomRightPosition);
                    y2 = GetElevationValue(x2, z1, yScale, topLeftPosition, bottomRightPosition);
                    y3 = GetElevationValue(x2, z2, yScale, topLeftPosition, bottomRightPosition);
                    y4 = GetElevationValue(x1, z2, yScale, topLeftPosition, bottomRightPosition);
                }

                int ci = (tx + ty * subMeshVX) * 4 + i;

                vertices[ci] = new Vector3(x1, y1, z1);
                vertices[ci + 1] = new Vector3(x2, y2, z1);
                vertices[ci + 2] = new Vector3(x2, y3, z2);
                vertices[ci + 3] = new Vector3(x1, y4, z2);

                uv[ci] = new Vector2(uvX1, uvY1);
                uv[ci + 1] = new Vector2(uvX2, uvY1);
                uv[ci + 2] = new Vector2(uvX2, uvY2);
                uv[ci + 3] = new Vector2(uvX1, uvY2);
            }
        }

        int bx = x + api.buffer.bufferPosition.x;
        int by = y + api.buffer.bufferPosition.y;

        int maxX = (2 << api.buffer.apiZoom) / 2;

        if (bx >= maxX) bx -= maxX;
        if (bx < 0) bx += maxX;

        OnlineMapsTile tile =
            OnlineMapsTile.tiles.FirstOrDefault(
                t => t.x == bx && t.y == by);

        if (tile != null)
        {
            if (renderer.materials[mi].mainTexture != tile.texture)
            {
                renderer.materials[mi].mainTexture = tile.texture;
                if (OnChangeMaterialTexture != null) OnChangeMaterialTexture(tile, renderer.materials[mi]);
            }
            if (renderer.materials[mi].GetTexture("_TrafficTex") != tile.trafficTexture)
            {
                renderer.materials[mi].SetTexture("_TrafficTex", tile.trafficTexture);
            }
        }
    }

    private void UpdateMarkersMesh()
    {
        if (markersGameObject == null) InitMarkersMesh();

        List<OnlineMapsMarker> usedMarkers =
            api.markers.Where(m => m.enabled && m.range.InRange(api.buffer.apiZoom))
                .OrderByDescending(m => m.position.y)
                .ToList();

        Vector2 startPos = api.topLeftPosition;
        Vector2 endPos = api.bottomRightPosition;
        if (endPos.x < startPos.x) endPos.x += 360;

        int maxX = (2 << api.buffer.apiZoom) / 2;

        Vector2 startTilePos = OnlineMapsUtils.LatLongToTilef(startPos, api.buffer.apiZoom);

        List<Vector3> markersVerticles = new List<Vector3>();
        List<Vector2> markersUV = new List<Vector2>();
        List<Vector3> markersNormals = new List<Vector3>();
        List<Texture> markersTextures = new List<Texture>();

        float yScale = GetBestElevationYScale(startPos, endPos);

        foreach (OnlineMapsMarker marker in usedMarkers)
        {
            float mx = marker.position.x;
            if (((mx > startPos.x && mx < endPos.x) || (mx + 360 > startPos.x && mx + 360 < endPos.x) ||
                 (mx - 360 > startPos.x && mx - 360 < endPos.x)) &&
                marker.position.y < startPos.y && marker.position.y > endPos.y)
            {
                Vector2 p = OnlineMapsUtils.LatLongToTilef(marker.position, api.buffer.apiZoom);
                p -= startTilePos;
                p.x = Mathf.Repeat(p.x, maxX);
                OnlineMapsVector2i ip =
                    marker.GetAlignedPosition(new OnlineMapsVector2i((int) (p.x * OnlineMapsUtils.tileSize),
                        (int) (p.y * OnlineMapsUtils.tileSize)));

                float rx1 = ip.x / (float) api.tilesetWidth * api.tilesetSize.x;
                float ry1 = ip.y / (float) api.tilesetHeight * api.tilesetSize.y;
                float rx2 = (ip.x + marker.width) / (float) api.tilesetWidth * api.tilesetSize.x;
                float ry2 = (ip.y + marker.height) / (float) api.tilesetHeight * api.tilesetSize.y;

                float y = GetElevationValue((-rx1 - rx2) / 2, (ry1 + ry2) / 2, yScale, startPos, endPos);

                markersVerticles.Add(new Vector3(-rx1, y, ry1));
                markersVerticles.Add(new Vector3(-rx2, y, ry1));
                markersVerticles.Add(new Vector3(-rx2, y, ry2));
                markersVerticles.Add(new Vector3(-rx1, y, ry2));

                markersNormals.Add(Vector3.up);
                markersNormals.Add(Vector3.up);
                markersNormals.Add(Vector3.up);
                markersNormals.Add(Vector3.up);

                markersUV.Add(new Vector2(1, 1));
                markersUV.Add(new Vector2(0, 1));
                markersUV.Add(new Vector2(0, 0));
                markersUV.Add(new Vector2(1, 0));

                if (marker.texture != null) markersTextures.Add(marker.texture);
                else markersTextures.Add(api.defaultMarkerTexture);
            }
        }

        markersMesh.Clear();
        markersMesh.vertices = markersVerticles.ToArray();
        markersMesh.uv = markersUV.ToArray();
        markersMesh.normals = markersNormals.ToArray();

        if (markersRenderer.materials.Length != markersTextures.Count)
            markersRenderer.materials = new Material[markersTextures.Count];

        markersMesh.subMeshCount = markersTextures.Count;

        int[] markersTriangles = new int[6];
        Shader shader = Shader.Find("Transparent/Diffuse");

        for (int i = 0; i < markersTextures.Count; i++)
        {
            int vi = i * 4;

            markersTriangles[0] = vi;
            markersTriangles[1] = vi + 1;
            markersTriangles[2] = vi + 2;
            markersTriangles[3] = vi;
            markersTriangles[4] = vi + 2;
            markersTriangles[5] = vi + 3;

            markersMesh.SetTriangles(markersTriangles, i);

            if (markersRenderer.materials[i] == null) markersRenderer.materials[i] = new Material(shader);

            if (markersRenderer.materials[i].mainTexture != markersTextures[i])
            {
                markersRenderer.materials[i].mainTexture = markersTextures[i];
                markersRenderer.materials[i].shader = shader;
                markersRenderer.materials[i].color = Color.white;
            }
        }
    }
}