/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 3D marker class.\n
/// <strong>Can be used only when the source display - Texture or Tileset.</strong>\n
/// To create a new 3D marker use OnlineMapsControlBase3D.AddMarker3D.
/// </summary>
[Serializable]
public class OnlineMapsMarker3D : OnlineMapsMarkerBase
{
    /// <summary>
    /// Marker prefab GameObject.
    /// </summary>
    public GameObject prefab;

    public OnlineMapsControlBase3D control;

    /// <summary>
    /// Event that occurs when the marker position changed.
    /// </summary>
    public Action<OnlineMapsMarker3D> OnPositionChanged;

    /// <summary>
    /// The instance.
    /// </summary>
    [HideInInspector]
    public GameObject instance;

    /// <summary>
    /// Specifies whether the marker is initialized.
    /// </summary>
    [HideInInspector] 
    public bool inited = false;

    public float scale = 1;

    private GameObject _prefab;
    private Vector3 _relativePosition;

    /// <summary>
    /// Gets or sets marker enabled.
    /// </summary>
    /// <value>
    /// true if enabled, false if not.
    /// </value>
    public override bool enabled
    {
        set
        {
            if (_enabled != value)
            {
                _enabled = value;
#if UNITY_3_5 || UNITY_3_5_5
                instance.SetActiveRecursively(value);
#else
                instance.SetActive(value);
#endif
                if (OnEnabledChange != null) OnEnabledChange(this);
            }
        }
    }

    /// <summary>
    /// Returns the position of the marker relative to Texture.
    /// </summary>
    /// <value>
    /// The relative position.
    /// </value>
    public Vector3 relativePosition
    {
        get
        {
            return enabled ? _relativePosition : Vector3.zero;
        }
    }

    /// <summary>
    /// Gets the instance transform.
    /// </summary>
    /// <value>
    /// The transform.
    /// </value>
    public Transform transform
    {
        get { return instance.transform; }
    }

    public OnlineMapsMarker3D()
    {
        range = new OnlineMapsRange(3, 20);
    }

    /// <summary>
    /// Initialises this object.
    /// </summary>
    /// <param name="parent">
    /// The parent transform.
    /// </param>
    public void Init(Transform parent)
    {
        if (instance != null) Object.DestroyImmediate(instance);

        if (prefab == null)
        {
            instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
            instance.transform.localScale = Vector3.one;
        }
        else instance = (GameObject)Object.Instantiate(prefab);

        _prefab = prefab;
        
        instance.transform.parent = parent;
        instance.AddComponent<OnlineMapsMarker3DInstance>().marker = this;
        enabled = false;
        inited = true;
        scale = control.marker3DScale;
        OnlineMaps api = OnlineMaps.instance;
        Update(api.topLeftPosition, api.bottomRightPosition, api.zoom);
    }

    public override void LookToCoordinates(Vector2 coordinates)
    {
        
    }

    /// <summary>
    /// Reinitialises this object.
    /// </summary>
    /// <param name="topLeft">
    /// The top left.
    /// </param>
    /// <param name="bottomRight">
    /// The bottom right.
    /// </param>
    /// <param name="zoom">
    /// The zoom.
    /// </param>
    public void Reinit(Vector2 topLeft, Vector2 bottomRight, int zoom)
    {
        if (instance)
        {
            Transform parent = instance.transform.parent;
            Object.Destroy(instance);
            Init(parent);
        }
        Update(topLeft, bottomRight, zoom);
    }

    /// <summary>
    /// Updates this object.
    /// </summary>
    /// <param name="topLeft">
    /// The top left coordinates.
    /// </param>
    /// <param name="bottomRight">
    /// The bottom right coordinates.
    /// </param>
    /// <param name="zoom">
    /// The zoom.
    /// </param>
    public void Update(Vector2 topLeft, Vector2 bottomRight, int zoom)
    {
        if (instance == null)
        {
            Debug.Log("No instance");
            return;
        }

        if (_prefab != prefab) Reinit(topLeft, bottomRight, zoom);

        if (!range.InRange(zoom)) enabled = false;
        else if (position.y > topLeft.y || position.y < bottomRight.y) enabled = false;
        else if (topLeft.x < bottomRight.x && (position.x < topLeft.x || position.x > bottomRight.x)) enabled = false;
        else if (topLeft.x > bottomRight.x && position.x < topLeft.x && position.x > bottomRight.x) enabled = false;
        else enabled = true;

        if (!enabled) return;

        Vector2 mp = OnlineMapsUtils.LatLongToTilef(position, zoom);
        Vector2 tl = OnlineMapsUtils.LatLongToTilef(topLeft, zoom);
        Vector2 br = OnlineMapsUtils.LatLongToTilef(bottomRight, zoom);

        int maxX = (2 << zoom) / 2;

        Bounds bounds = OnlineMaps.instance.collider.bounds;

        float sx = br.x - tl.x;
        if (sx < 0) sx += maxX;

        float mpx = mp.x - tl.x;
        if (mpx < 0) mpx += maxX;

        float px = mpx / sx;
        float pz = (tl.y - mp.y) / (tl.y - br.y);

        _relativePosition = new Vector3(px, 0, pz);
        px = bounds.center.x - (px - 0.5f) * bounds.size.x - OnlineMaps.instance.transform.position.x;
        pz = bounds.center.z + (pz - 0.5f) * bounds.size.z - OnlineMaps.instance.transform.position.z;

        Vector3 oldPosition = instance.transform.localPosition;
        float y = 0;

        if (OnlineMapsControlBase.instance is OnlineMapsTileSetControl)
        {
            OnlineMapsTileSetControl control = (OnlineMapsTileSetControl) OnlineMapsControlBase.instance;
            y = control.GetElevationValue(px, pz, control.GetBestElevationYScale(topLeft, bottomRight), topLeft,
                bottomRight);
        }

        Vector3 newPosition = new Vector3(px, y, pz);
        instance.transform.localPosition = newPosition;
        if (oldPosition != newPosition && OnPositionChanged != null) OnPositionChanged(this);
    }
}
