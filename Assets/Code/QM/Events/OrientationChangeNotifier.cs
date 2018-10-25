using QM.Util;
using UnityEngine;
using UnityEngine.Events;

namespace QM.Events
{

    [System.Serializable]
    public class OrientationChangedEvent : UnityEvent<ScreenOrientation, ScreenOrientation>
    {
    }

    public class OrientationChangeNotifier : MonoBehaviour
    {

        public OrientationChangedEvent orientationChangedEvent;

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
                orientationChangedEvent.Invoke(oldOrientation, Screen.orientation);
                orientation = Screen.orientation;
            }
        }
    }
}
