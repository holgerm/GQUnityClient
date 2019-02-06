using QM.Util;
using UnityEngine;
using UnityEngine.Events;

namespace QM.Events
{
    /// <summary>
    /// Orientation changed event. Uses two arguments: firt the old and second the new orientation.
    /// </summary>
    [System.Serializable]
    public class OrientationChangedEvent : UnityEvent<DeviceOrientation, DeviceOrientation>
    {
    }

    public class OrientationChangeNotifier : MonoBehaviour
    {

        public OrientationChangedEvent oce;

        private DeviceOrientation orientation;

        // Use this for initialization
        void Start()
        {
            orientation = Device.Orientation;
        }

        // Update is called once per frame
        void Update()
        {
            if (Device.Orientation != orientation) {
                DeviceOrientation oldOrientation = orientation;
                oce.Invoke(oldOrientation, Device.Orientation);
                orientation = Device.Orientation;
            }
        }
    }
}
