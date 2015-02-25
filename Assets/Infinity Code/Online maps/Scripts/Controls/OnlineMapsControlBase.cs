/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

#if !UNITY_3_5 && !UNITY_3_5_5
#define ALLOW_LONG_PRESS
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class implements the basic functionality control of the map.
/// </summary>
[Serializable]
[AddComponentMenu("")]
public class OnlineMapsControlBase : MonoBehaviour
{
#if ALLOW_LONG_PRESS
    /// <summary>
    /// Delay before invoking event OnMapLongPress.
    /// </summary>
    public static float longPressDelay = 1;
#endif

    protected static OnlineMapsControlBase _instance;

    /// <summary>
    /// Event that occurs when you click on the map.
    /// </summary>
    public Action OnMapClick;

    /// <summary>
    /// Event that occurs when you double-click on the map.
    /// </summary>
    public Action OnMapDoubleClick;

    /// <summary>
    /// Event that occurs when you drag the map.
    /// </summary>
    public Action OnMapDrag;

#if ALLOW_LONG_PRESS
    /// <summary>
    /// Event that occurs when you long press the map.
    /// </summary>
    public Action OnMapLongPress;
#endif

    /// <summary>
    /// Event that occurs when you press on the map.
    /// </summary>
    public Action OnMapPress;

    /// <summary>
    /// Event that occurs when you release the map.
    /// </summary>
    public Action OnMapRelease;

    /// <summary>
    /// Event that occurs when you zoom the map.
    /// </summary>
    public Action OnMapZoom;

    /// <summary>
    /// Texture, which will draw the map. \n
    /// To change the texture use OnlineMapsControlBase.SetTexture.
    /// </summary>
    [HideInInspector]
    public Texture2D activeTexture;

    /// <summary>
    /// Specifies whether to create a marker by pressing M under the cursor.
    /// </summary>
    public bool allowAddMarkerByM = true;

    /// <summary>
    /// Specifies whether the user can change zoom of the map.
    /// </summary>
    public bool allowZoom = true;

    /// <summary>
    /// Specifies whether the user can manipulate the map.
    /// </summary>
    public bool allowUserControl = true;

    /// <summary>
    /// Specifies whether to move map.
    /// </summary>
    [HideInInspector]
    public bool isMapDrag;

    /// <summary>
    /// Allows you to zoom the map when double-clicked.
    /// </summary>
    public bool zoomInOnDoubleClick = true;

    protected Rect _screenRect;
    protected OnlineMaps api;
    
    protected float lastGestureDistance;
    protected Vector2 lastMousePosition;
    protected Vector2 lastPosition;
    protected int lastTouchCount;

    private OnlineMapsMarker _dragMarker;
    private long[] lastClickTimes = {0, 0};
    private Vector3 pressPoint;
    private IEnumerator longPressEnumenator;

    /// <summary>
    /// Singleton instance of map control.
    /// </summary>
    public static OnlineMapsControlBase instance
    {
        get { return _instance; }
    }

    protected virtual bool allowTouchZoom
    {
        get { return true; }
    }

    /// <summary>
    /// Indicates whether it is possible to get the screen coordinates store. True - for 2D map, false - for the 3D map.
    /// </summary>
    public virtual bool allowMarkerScreenRect
    {
        get { return false; }
    }

    /// <summary>
    /// Marker that draged at the moment.
    /// </summary>
    public OnlineMapsMarker dragMarker
    {
        get { return _dragMarker; }
        set { _dragMarker = value; }
    }

    /// <summary>
    /// Screen area occupied by the map.
    /// </summary>
    public virtual Rect screenRect
    {
        get { return _screenRect; }
    }

    /// <summary>
    /// UV rectangle used by the texture of the map.
    /// NGUI: uiTexture.uvRect.
    /// Other: new Rect(0, 0, 1, 1);
    /// </summary>
    public virtual Rect uvRect
    {
        get { return new Rect(0, 0, 1, 1); }
    }

    /// <summary>
    /// Function, which is executed after map updating.
    /// </summary>
    protected virtual void AfterUpdate()
    {
        
    }

    /// <summary>
    /// Function, which is executed before map updating.
    /// </summary>
    protected virtual void BeforeUpdate()
    {
        
    }

    /// <summary>
    /// Creates a marker at the location of the cursor.
    /// </summary>
    protected void CreateMarker()
    {
        OnlineMapsMarker m = new OnlineMapsMarker
        {
            position = GetCoords(),
            texture = api.defaultMarkerTexture
        };
        m.Init();
        List<OnlineMapsMarker> markerList = api.markers.ToList();
        markerList.Add(m);
        api.markers = markerList.ToArray();
        api.Redraw();
    }

    /// <summary>
    /// Moves the marker to the location of the cursor.
    /// </summary>
    protected void DragMarker()
    {
        Vector2 curPos = GetCoords();
        if (curPos != Vector2.zero)
        {
            Vector2 offset = curPos - lastPosition;
            if (offset.magnitude > 0)
            {
                dragMarker.position += offset;
                api.Redraw();
            }
            lastPosition = curPos;
        }
    }

    /// <summary>
    /// Returns the geographical coordinates of the location where the cursor is.
    /// </summary>
    /// <returns>Geographical coordinates</returns>
    public virtual Vector2 GetCoords()
    {
        return GetCoords(Input.mousePosition);
    }

    /// <summary>
    /// Returns the geographical coordinates at the specified coordinates of the screen.
    /// </summary>
    /// <param name="position">Screen coordinates</param>
    /// <returns>Geographical coordinates</returns>
    public virtual Vector2 GetCoords(Vector2 position)
    {
        return Vector2.zero;
    }

    /// <summary>
    /// Converts geographical coordinate to position in the scene relative to the top-left corner of the map.
    /// </summary>
    /// <param name="coords">Geographical coordinate</param>
    /// <returns>Scene position</returns>
    public virtual Vector2 GetPosition(Vector2 coords)
    {
        Vector2 pos = OnlineMapsUtils.LatLongToTilef(coords, api.zoom);
        Vector2 topLeft = OnlineMapsUtils.LatLongToTilef(api.topLeftPosition, api.zoom);
        pos -= topLeft;
        return new Vector2(pos.x * api.width, pos.y * api.height);
    }

    /// <summary>
    /// Screen area occupied by the map.
    /// </summary>
    /// <returns>Screen rectangle</returns>
    public virtual Rect GetRect()
    {
        return new Rect();
    }

    /// <summary>
    /// Checks whether the cursor over the map.
    /// </summary>
    /// <returns>True - if the cursor over the map, false - if not.</returns>
    protected virtual bool HitTest()
    {
        return true;
    }

    /// <summary>
    /// Event that occurs before Awake.
    /// </summary>
    public virtual void OnAwakeBefore()
    {
        
    }

    private void OnDestroy()
    {
        OnDestroyLate();
    }

    protected virtual void OnDestroyLate()
    {
        
    }

    private void OnEnable()
    {
        _instance = this;
        dragMarker = null;
        api = GetComponent<OnlineMaps>();
        activeTexture = api.texture;
        if (api == null)
        {
            Debug.LogError("Can not find a script OnlineMaps.");
            Destroy(this);
            return;
        }
        api.control = this;
        OnEnableLate();
    }

    /// <summary>
    /// Function that is called after control of the map enabled.
    /// </summary>
    protected virtual void OnEnableLate()
    {
        
    }

    /// <summary>
    /// Function that is called when you press the map.
    /// </summary>
    protected void OnMapBasePress()
    {
        if (!allowUserControl) return;

        dragMarker = null;
        if (!HitTest()) return;
        if (GUIUtility.hotControl != 0) return;

        if (OnMapPress != null) OnMapPress();

        lastClickTimes[0] = lastClickTimes[1];
        lastClickTimes[1] = DateTime.Now.Ticks;
        lastPosition = GetCoords();
        lastMousePosition = pressPoint = Input.mousePosition;
        if (lastPosition == Vector2.zero) return;

        OnlineMapsMarker marker = api.GetMarkerFromScreen(lastPosition);
        if (marker != null)
        {
            if (marker.OnPress != null) marker.OnPress(marker);
            if (Input.GetKey(KeyCode.LeftControl)) dragMarker = marker;
        }

        if (dragMarker == null)
        {
            isMapDrag = true;

#if ALLOW_LONG_PRESS
            longPressEnumenator = WaitLongPress();
            StartCoroutine(longPressEnumenator);
#endif
        }
        else lastClickTimes[0] = 0;

        OnlineMaps.isUserControl = true;
    }

    /// <summary>
    /// Function that is called when you release the map.
    /// </summary>
    protected void OnMapBaseRelease()
    {
        if (!allowUserControl) return;
        if (GUIUtility.hotControl != 0) return;

        bool isClick = (pressPoint - Input.mousePosition).magnitude < 20;
        isMapDrag = false;
        dragMarker = null;

        if (longPressEnumenator != null)
        {
#if ALLOW_LONG_PRESS
            StopCoroutine(longPressEnumenator);
#endif
            longPressEnumenator = null;
        }

        OnlineMaps.isUserControl = false;
        if (OnMapRelease != null) OnMapRelease();

        OnlineMapsMarker marker = api.GetMarkerFromScreen(GetCoords());

        if (marker != null)
        {
            if (marker.OnRelease != null) marker.OnRelease(marker);
            if (isClick && marker.OnClick != null) marker.OnClick(marker);
        }

        if (isClick && DateTime.Now.Ticks - lastClickTimes[0] < 5000000)
        {
            if (marker != null && marker.OnDoubleClick != null) marker.OnDoubleClick(marker);
            else
            {
                if (OnMapDoubleClick != null) OnMapDoubleClick();

                if (allowZoom && zoomInOnDoubleClick) ZoomOnPoint(1, Input.mousePosition);
            }
            
            lastClickTimes[0] = 0;
            lastClickTimes[1] = 0;
        }
        else if (isClick)
        {
            if (OnMapClick != null) OnMapClick();
        }

        if (api.bufferStatus == OnlineMapsBufferStatus.wait) api.needRedraw = true;
    }

#if !UNITY_ANDROID
// ReSharper disable once UnusedMember.Global
    protected void OnMouseDown()
    {
        OnMapBasePress();
    }

// ReSharper disable once UnusedMember.Global
    protected void OnMouseUp()
    {
        OnMapBaseRelease();
    }
#endif

    /// <summary>
    /// Specifies the texture, which will draw the map. In texture must be enabled "Read / Write Enabled".
    /// </summary>
    /// <param name="texture">Texture</param>
    public virtual void SetTexture(Texture2D texture)
    {
        activeTexture = texture;
    }

    // ReSharper disable once UnusedMember.Local
    protected void Update()
    {
        BeforeUpdate();
        _screenRect = GetRect();
        if (allowAddMarkerByM && Input.GetKeyUp(KeyCode.M)) CreateMarker();
#if UNITY_ANDROID
#if !UNITY_EDITOR
        if (allowTouchZoom && Input.touchCount != lastTouchCount)
        {
            if (Input.touchCount == 1) OnMapBasePress();
            else if (Input.touchCount == 0) OnMapBaseRelease();
        }
        lastTouchCount = Input.touchCount;
#else
        int touchCount = Input.GetMouseButton(0) ? 1 : 0;
        if (allowTouchZoom && touchCount != lastTouchCount)
        {
            if (touchCount == 1) OnMapBasePress();
            else if (touchCount == 0) OnMapBaseRelease();
        }
        lastTouchCount = touchCount;
#endif
#endif
        if (isMapDrag) UpdatePosition();

        if (allowZoom)
        {
            UpdateZoom();
            UpdateGestureZoom();
        }

        if (dragMarker != null) DragMarker();
        else if (HitTest())
        {
            Vector2 pos = GetCoords();
            if (pos != Vector2.zero) api.ShowMarkersTooltip(pos);
        }
        else
        {
            api.tooltip = string.Empty;
            api.tooltipMarker = null;
        }
        AfterUpdate();
    }

    /// <summary>
    /// Pinch to zoom for iOS and Android
    /// </summary>
    protected void UpdateGestureZoom()
    {
        if (!allowUserControl) return;

        if (Input.touchCount == 2)
        {
            Vector2 p1 = Input.GetTouch(0).position;
            Vector2 p2 = Input.GetTouch(1).position;
            float distance = Vector2.Distance(p1, p2);

            Vector2 center = Vector2.Lerp(p1, p2, 0.5f);

            if (lastGestureDistance == 0)
            {
                lastGestureDistance = distance;
                return;
            }
            int z = 0;
            if (lastGestureDistance * 1.2f < distance) z = 1;
            else if (distance * 1.2f < lastGestureDistance) z = -1;

            if (ZoomOnPoint(z, center)) lastGestureDistance = distance;
        }
        else lastGestureDistance = 0;
    }

    /// <summary>
    /// Updates the map coordinates for the actions of the user.
    /// </summary>
    protected void UpdatePosition()
    {
        if (!allowUserControl) return;

        if (Input.touchCount > 1) return;

        Vector2 curPos = GetCoords();
        Vector2 mousePosition = Input.mousePosition;
        if (curPos != Vector2.zero && lastMousePosition != mousePosition)
        {
            Vector2 offset = curPos - lastPosition;
            if (offset.magnitude > 0)
            {
                api.position -= offset;
                api.needRedraw = true;

                if (longPressEnumenator != null)
                {
#if ALLOW_LONG_PRESS
                    StopCoroutine(longPressEnumenator);
#else
                    StopCoroutine("WaitLongPress");
#endif
                    longPressEnumenator = null;
                }

                if (OnMapDrag != null) OnMapDrag();
            }
            lastPosition = GetCoords();
            lastMousePosition = mousePosition;
        }
    }

    /// <summary>
    /// Updates the map zoom for mouse wheel.
    /// </summary>
    protected void UpdateZoom()
    {
        if (!allowUserControl) return;

        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel == 0) return;

        ZoomOnPoint((wheel > 0) ? 1 : -1, Input.mousePosition);
    }

#if ALLOW_LONG_PRESS
    private IEnumerator WaitLongPress()
    {
        yield return new WaitForSeconds(longPressDelay);

        OnlineMapsMarker marker = api.GetMarkerFromScreen(lastPosition);

        if (marker != null && marker.OnLongPress != null) marker.OnLongPress(marker);

        if (OnMapLongPress != null)
        {
            OnMapLongPress();
            isMapDrag = false;
        }

        longPressEnumenator = null;
    }
#endif 

    /// <summary>
    /// Changes the zoom keeping a specified point on same place.
    /// </summary>
    /// <param name="zoomOffset">Positive - zoom in, Negative - zoom out</param>
    /// <param name="screenPosition">Screen position</param>
    /// <returns>True - if zoom changed, False - if zoom not changed</returns>
    public bool ZoomOnPoint(int zoomOffset, Vector2 screenPosition)
    {
        int newZoom = Mathf.Clamp(api.zoom + zoomOffset, 3, 20);
        if (newZoom == api.zoom) return false;

        Vector2 mouseCoords = GetCoords(screenPosition);
        if (mouseCoords == Vector2.zero) return false;

        api.dispatchEvents = false;
        api.position = mouseCoords;
        api.zoom = newZoom;
        api.position += api.position - GetCoords(screenPosition);
        api.dispatchEvents = true;
        api.DispatchEvent(OnlineMapsEvents.changedPosition, OnlineMapsEvents.changedZoom);

        if (OnMapZoom != null) OnMapZoom();
        return true;
    }
}