/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class implements the basic functionality control of the 3D map.
/// </summary>
[System.Serializable]
public class OnlineMapsControlBase3D: OnlineMapsControlBase
{
    /// <summary>
    /// The camera you are using to display the map.
    /// </summary>
    public Camera activeCamera;

    /// <summary>
    /// Mode of 2D markers. Bake in texture or Billboard.
    /// </summary>
    public OnlineMapsMarker2DMode marker2DMode = OnlineMapsMarker2DMode.flat;

    /// <summary>
    /// Size of billboard markers.
    /// </summary>
    public float marker2DSize = 100;

    /// <summary>
    /// List of 3D markers.
    /// </summary>
    public OnlineMapsMarker3D[] markers3D;

    /// <summary>
    /// Scaling of 3D markers by default.
    /// </summary>
    public float marker3DScale = 1;

    /// <summary>
    /// Specifies whether to create a 3D marker by pressing N under the cursor.
    /// </summary>
    public bool allowAddMarker3DByN = true;

    protected GameObject markersGameObject;
    protected Mesh markersMesh;
    protected Renderer markersRenderer;

    private List<OnlineMapsMarkerBillboard> markerBillboards;

    /// <summary>
    /// Adds a new 3D marker on the map.
    /// </summary>
    /// <param name="markerPosition">Vector2. X - Longituge, Y - Latitude.</param>
    /// <param name="prefab">Marker prefab.</param>
    /// <returns>Marker instance.</returns>
    public OnlineMapsMarker3D AddMarker3D(Vector2 markerPosition, GameObject prefab)
    {
        List<OnlineMapsMarker3D> ms = markers3D.ToList();
        OnlineMapsMarker3D marker = new OnlineMapsMarker3D { position = markerPosition, prefab = prefab, control = this };
        marker.Init(transform);
        ms.Add(marker);
        markers3D = ms.ToArray();
        return marker;
    }

    protected override void AfterUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(activeCamera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            OnlineMapsMarkerInstanceBase markerInstance = hit.collider.gameObject.GetComponent<OnlineMapsMarkerInstanceBase>();
            if (markerInstance != null)
            {
                api.tooltip = markerInstance.marker.label;
                api.tooltipMarker = markerInstance.marker;
            }


        }

        if (allowAddMarker3DByN && Input.GetKeyUp(KeyCode.N))
        {
            OnlineMapsMarker3D m = new OnlineMapsMarker3D
            {
                position = GetCoords(),
                control = this
            };
            m.Init(transform);
            m.Update(api.topLeftPosition, api.bottomRightPosition, api.zoom);
            List<OnlineMapsMarker3D> markerList = markers3D.ToList();
            markerList.Add(m);
            markers3D = markerList.ToArray();
        }
    }

    /// <summary>
    /// Returns best value yScale, based on the coordinates corners of map.
    /// </summary>
    /// <param name="topLeftPosition">Top-Left corner coordinates.</param>
    /// <param name="bottomRightPosition">Bottom-Right corner coordinates.</param>
    /// <returns>Best yScale</returns>
    public virtual float GetBestElevationYScale(Vector2 topLeftPosition, Vector2 bottomRightPosition)
    {
        return 0;
    }

    /// <summary>
    /// Returns the elevation value, based on the coordinates of the scene.
    /// </summary>
    /// <param name="x">Scene X</param>
    /// <param name="z">Scene Z</param>
    /// <param name="yScale">Scale factor for evevation value</param>
    /// <param name="topLeftPosition">Top-Left corner coordinates.</param>
    /// <param name="bottomRightPosition">Bottom-Right corner coordinates.</param>
    /// <returns></returns>
    public virtual float GetElevationValue(float x, float z, float yScale, Vector2 topLeftPosition, Vector2 bottomRightPosition)
    {
        return 0;
    }

    protected void InitMarkersMesh()
    {
        markersGameObject = new GameObject("Markers");
        markersGameObject.transform.parent = transform;
        markersGameObject.transform.localPosition = new Vector3(0, 0.2f, 0);

        markersRenderer = markersGameObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = markersGameObject.AddComponent<MeshFilter>();
        markersMesh = new Mesh();
        markersMesh.name = "MarkersMesh";
#if !UNITY_3_5 && !UNITY_3_5_5
        markersMesh.MarkDynamic();
#endif
        meshFilter.mesh = markersMesh;
    }

    private void OnChangePosition()
    {
        Vector2 topLeft = api.topLeftPosition;
        Vector2 bottomRight = api.bottomRightPosition;

        foreach (OnlineMapsMarker3D marker in markers3D) marker.Update(topLeft, bottomRight, api.zoom);
    }

    protected override void OnDestroyLate()
    {
        base.OnDestroyLate();

        markersMesh = null;
        markersRenderer = null;
    }

    protected override void OnEnableLate()
    {
        base.OnEnableLate();

        if (activeCamera == null) activeCamera = Camera.main;
    }

    /// <summary>
    /// Removes the specified 3D marker.
    /// </summary>
    /// <param name="marker">3D marker</param>
    public void RemoveMarker3D(OnlineMapsMarker3D marker)
    {
        List<OnlineMapsMarker3D> ms = markers3D.ToList();
        ms.Remove(marker);
        markers3D = ms.ToArray();
        if (marker.instance != null) Destroy(marker.instance);
    }

    private void Start()
    {
        api.OnChangePosition += OnChangePosition;

        foreach (OnlineMapsMarker3D marker in markers3D.Where(m => !m.inited))
        {
            marker.control = this;
            marker.Init(transform);
        }
        OnChangePosition();
    }

    /// <summary>
    /// Updates the current control.
    /// </summary>
    public virtual void UpdateControl()
    {
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
        if (marker2DMode == OnlineMapsMarker2DMode.billboard) UpdateMarkersBillboard();
#endif
    }

    /// <summary>
    /// Updates billboard markers.
    /// </summary>
    protected void UpdateMarkersBillboard()
    {
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
        if (markersGameObject == null) InitMarkersMesh();
        if (markerBillboards == null) markerBillboards = new List<OnlineMapsMarkerBillboard>();

        List<OnlineMapsMarker> usedMarkers =
            api.markers.Where(m => m.enabled && m.range.InRange(api.buffer.apiZoom))
                .OrderByDescending(m => m.position.y)
                .ToList();

        Vector2 startPos = api.topLeftPosition;
        Vector2 endPos = api.bottomRightPosition;
        if (endPos.x < startPos.x) endPos.x += 360;

        int maxX = (2 << api.buffer.apiZoom) / 2;

        Vector2 startTilePos = OnlineMapsUtils.LatLongToTilef(startPos, api.buffer.apiZoom);

        float yScale = GetBestElevationYScale(startPos, endPos);

        Bounds mapBounds = collider.bounds;
        Vector3 positionOffset = transform.position - mapBounds.min;
        if (api.target == OnlineMapsTarget.tileset) positionOffset.x -= mapBounds.size.x;

        foreach (OnlineMapsMarkerBillboard billboard in markerBillboards) billboard.used = false;

        foreach (OnlineMapsMarker marker in usedMarkers)
        {
            float mx = marker.position.x;
            if (((mx > startPos.x && mx < endPos.x) || (mx + 360 > startPos.x && mx + 360 < endPos.x) ||
                 (mx - 360 > startPos.x && mx - 360 < endPos.x)) &&
                marker.position.y < startPos.y && marker.position.y > endPos.y)
            {
                OnlineMapsMarkerBillboard markerBillboard = markerBillboards.FirstOrDefault(m => m.marker == marker);

                if (markerBillboard == null)
                {
                    markerBillboard = OnlineMapsMarkerBillboard.Create(marker);
                    markerBillboards.Add(markerBillboard);
                    markerBillboard.transform.parent = markersGameObject.transform;

                    float sx = mapBounds.size.x / api.width * marker2DSize;
                    float sz = mapBounds.size.z / api.height * marker2DSize;
                    float s = Mathf.Max(sx, sz);
                    markerBillboard.transform.localScale = new Vector3(s, s, s);
                }

                Vector2 p = OnlineMapsUtils.LatLongToTilef(marker.position, api.buffer.apiZoom);
                p -= startTilePos;
                p.x = Mathf.Repeat(p.x, maxX);

                float x = -p.x / api.width * OnlineMapsUtils.tileSize *  mapBounds.size.x + positionOffset.x;
                float z = p.y / api.height * OnlineMapsUtils.tileSize * mapBounds.size.z - positionOffset.z;

                float y = GetElevationValue(x, z, yScale, startPos, endPos);

                markerBillboard.transform.localPosition = new Vector3(x, y, z);
                markerBillboard.used = true;
            }
        }

        for (int i = 0; i < markerBillboards.Count; i++)
        {
            if (!markerBillboards[i].used)
            {
                markerBillboards[i].Dispose();
                markerBillboards[i] = null;
            }
        }

        markerBillboards.RemoveAll(m => m == null);
#endif
    }
}