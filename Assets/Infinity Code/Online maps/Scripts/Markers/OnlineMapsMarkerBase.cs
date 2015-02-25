/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System;
using UnityEngine;

/// <summary>
/// The base class for markers.
/// </summary>
[Serializable]
public class OnlineMapsMarkerBase
{
    /// <summary>
    /// Default event caused to draw tooltip.
    /// </summary>
    public static Action<OnlineMapsMarkerBase> OnMarkerDrawTooltip;

    /// <summary>
    /// Events that occur when user click on the marker.
    /// </summary>
    public Action<OnlineMapsMarkerBase> OnClick;

    /// <summary>
    /// Events that occur when user double click on the marker.
    /// </summary>
    public Action<OnlineMapsMarkerBase> OnDoubleClick;

    /// <summary>
    /// Event caused to draw tooltip.
    /// </summary>
    public Action<OnlineMapsMarkerBase> OnDrawTooltip;

    /// <summary>
    /// Event occurs when the marker enabled change.
    /// </summary>
    public Action<OnlineMapsMarkerBase> OnEnabledChange;

    /// <summary>
    /// Events that occur when user long press on the marker.
    /// </summary>
    public Action<OnlineMapsMarkerBase> OnLongPress;

    /// <summary>
    /// Events that occur when user press on the marker.
    /// </summary>
    public Action<OnlineMapsMarkerBase> OnPress;

    /// <summary>
    /// Events that occur when user release on the marker.
    /// </summary>
    public Action<OnlineMapsMarkerBase> OnRelease;

    /// <summary>
    /// Events that occur when user roll out marker.
    /// </summary>
    public Action<OnlineMapsMarkerBase> OnRollOut;

    /// <summary>
    /// Events that occur when user roll over marker.
    /// </summary>
    public Action<OnlineMapsMarkerBase> OnRollOver;

    /// <summary>
    /// In this variable you can put any data that you need to work with markers.
    /// </summary>
    public object customData;

    /// <summary>
    /// Marker label.
    /// </summary>
    public string label = "";

    /// <summary>
    /// Marker coordinates.
    /// </summary>
    public Vector2 position;

    /// <summary>
    /// Zoom range, in which the marker will be displayed.
    /// </summary>
    public OnlineMapsRange range;

    protected bool _enabled = true;

    /// <summary>
    /// Gets or sets marker enabled.
    /// </summary>
    /// <value>
    /// true if enabled, false if not.
    /// </value>
    public virtual bool enabled
    {
        get { return _enabled; }
        set
        {
            if (_enabled != value)
            {
                _enabled = value;
                if (OnEnabledChange != null) OnEnabledChange(this);
            }
        }
    }

    /// <summary>
    /// Checks to display marker in current map view.
    /// </summary>
    public virtual bool inMapView
    {
        get
        {
            if (!enabled) return false;

            OnlineMaps api = OnlineMaps.instance;

            if (!range.InRange(api.zoom)) return false;

            Vector2 topLeft = api.topLeftPosition;
            Vector2 bottomRight = api.bottomRightPosition;

            if (position.x >= topLeft.x && position.x <= bottomRight.x && position.y >= bottomRight.y && position.y <= topLeft.y) return true;
            return false;
        }
    }

    /// <summary>
    /// Turns the marker in the direction specified coordinates.
    /// </summary>
    /// <param name="coordinates">
    /// The coordinates.
    /// </param>
    public virtual void LookToCoordinates(Vector2 coordinates)
    {
        
    }
}