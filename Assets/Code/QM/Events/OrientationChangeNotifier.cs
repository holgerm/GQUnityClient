using QM.Util;
using UnityEngine;
using UnityEngine.Events;

namespace QM.Events
{
    /// <summary>
    /// Orientation changed event. Uses two arguments: firt the old and second the new orientation.
    /// </summary>
    [System.Serializable]
    public class OrientationChangedEvent : UnityEvent<ScreenOrientation, ScreenOrientation>
    {
    }

    public class OrientationChangeNotifier : MonoBehaviour
    {

        public OrientationChangedEvent oce;

        private ScreenOrientation orientation;

        // Use this for initialization
        void Start()
        {
            orientation = Device.orientation;
        }

        // Update is called once per frame
        void Update()
        {
            if (Screen.orientation != orientation) {
                ScreenOrientation oldOrientation = orientation;
                oce.Invoke(oldOrientation, Screen.orientation);
                orientation = Screen.orientation;
            }
        }
    }
}
