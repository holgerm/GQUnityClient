/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using UnityEngine;

/// <summary>
/// 3D marker instance class.
/// </summary>
[System.Serializable]
[AddComponentMenu("")]
public class OnlineMapsMarker3DInstance : OnlineMapsMarkerInstanceBase
{
    private Vector2 _position;
    private float _scale;

    private OnlineMapsMarker3D marker3D
    {
        get { return marker as OnlineMapsMarker3D; }
    }

    private void Awake()
    {
        if (collider == null) gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    private void Start()
    {
        _position = marker.position;
        _scale = marker3D.scale;
        transform.localScale = new Vector3(_scale, _scale, _scale);
    }

    private void Update()
    {
        if (_position != marker.position)
        {
            OnlineMaps map = OnlineMaps.instance;
            marker3D.Update(map.topLeftPosition, map.bottomRightPosition, map.zoom);
        }

        if (_scale != marker3D.scale)
        {
            _scale = marker3D.scale;
            transform.localScale = new Vector3(_scale, _scale, _scale);
        }

    }
}