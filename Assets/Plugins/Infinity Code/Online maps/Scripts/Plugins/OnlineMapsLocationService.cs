/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System;
using UnityEngine;

/// <summary>
/// Controls map using GPS.
/// </summary>
[Serializable]
[AddComponentMenu("Infinity Code/Online Maps/Plugins/Location Service")]
// ReSharper disable once UnusedMember.Global
public class OnlineMapsLocationService : MonoBehaviour
{
    /// <summary>
    /// This event is called when the user rotates the device.
    /// </summary>
    public Action<float> OnCompassChanged;

    /// <summary>
    /// This event is called when changed your GPS location.
    /// </summary>
    public Action<Vector2> OnLocationChanged;

    /// <summary>
    /// Update stop position when user input.
    /// </summary>
    public bool autoStopUpdateOnInput = true;

    /// <summary>
    /// Desired service accuracy in meters. 
    /// </summary>
    public float desiredAccuracy = 10;

    /// <summary>
    /// Current GPS coordinates.
    /// </summary>
    public Vector2 position = Vector2.zero;

    /// <summary>
    /// Use the GPS coordinates after seconds of inactivity.
    /// </summary>
    public int restoreAfter = 10;

    /// <summary>
    ///  The minimum distance (measured in meters) a device must move laterally before location is updated.
    /// </summary>
    public float updateDistance = 10;

    /// <summary>
    /// Specifies whether the script will automatically update the location.
    /// </summary>
    public bool updatePosition = true;

    private OnlineMaps api;
    private long lastPositionChangedTime;
    private bool lockDisable;
    private float trueHeading = 0;

    private void OnChangePosition()
    {
        if (lockDisable) return;

        lastPositionChangedTime = DateTime.Now.Ticks;
        if (autoStopUpdateOnInput) updatePosition = false;
    }

// ReSharper disable once UnusedMember.Local
    private void OnEnable()
    {
        if (api == null) api = GetComponent<OnlineMaps>();
        if (api) api.OnChangePosition += OnChangePosition;
    }

// ReSharper disable once UnusedMember.Local
    private void Start() 
    {
	    if (!Input.location.isEnabledByUser)
	    {
            Debug.Log("Location service is not available.");
            Destroy(this);
	        return;
	    }

        Input.compass.enabled = true;
        Input.location.Start(desiredAccuracy, updateDistance);

	}
	
// ReSharper disable once UnusedMember.Local
	private void Update () 
    {
        if (api == null) return;
	    
	    if (Input.location.status == LocationServiceStatus.Failed)
	    {
            Debug.Log("Failed start Location service.");
            Destroy(this);
            return;
	    }

	    if (Input.location.status != LocationServiceStatus.Running) return;

	    LocationInfo info = Input.location.lastData;

	    if (trueHeading != Input.compass.trueHeading)
	    {
            trueHeading = Input.compass.trueHeading;
	        if (OnCompassChanged != null) OnCompassChanged(trueHeading / 360);
	    }

	    bool changed = false;
	    if (position.x != info.longitude)
	    {
	        position.x = info.longitude;
	        changed = true;
	    }
	    if (position.y != info.latitude)
	    {
	        position.y = info.latitude;
	        changed = true;
	    }

	    if (changed && OnLocationChanged != null) OnLocationChanged(position);

        if (updatePosition){
    	    UpdatePosition();
        } else {
            if (!updatePosition && restoreAfter > 0
            &&  DateTime.Now.Ticks > lastPositionChangedTime + OnlineMapsUtils.second * restoreAfter) {
    	        updatePosition = true;
                UpdatePosition();
            }
	    }
    }

    /// <summary>
    /// Sets map position using GPS coordinates.
    /// </summary>
    public void UpdatePosition()
    {  
        if (position == Vector2.zero) return;

        lockDisable = true;

        Vector2 p = api.position;
        bool changed = false;

        Debug.Log("GPS: " + position + "MAP: " + p);


        if (p.x != position.x)
        {
            p.x = position.x;
            changed = true;
        }
        if (p.y != position.y)
        {
            p.y = position.y;
            changed = true;
        }
        if (changed)
        {
            Debug.Log(position);
            api.position = p;
            api.Redraw();
        }

        lockDisable = false;
    }
}
