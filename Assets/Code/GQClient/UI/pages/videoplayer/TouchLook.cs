using QM.Util;
using UnityEngine;

namespace GQ.Client.UI
{

    [AddComponentMenu("Camera-Control/Touch Look")]
    public class TouchLook : MonoBehaviour
    {

        public float sensitivityX = 5.0f;
        public float sensitivityY = 5.0f;

        public bool invertX = false;
        public bool invertY = false;

        private float epsilon = 0.01f;

        private Vector2 positionAtBegin;
        private bool canBecomeSingleTouch;
        private bool canSwipeCamera;

        public VideoPlayController videoPlayController;

        // Update is called once per frame
        void Update()
        {
            if (Input.touches.Length > 0)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    positionAtBegin = Input.touches[0].position;
                    canBecomeSingleTouch = 
                        !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
                    canSwipeCamera = 
                        canBecomeSingleTouch;
                }

                if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    Vector2 cumulatedDelta = Input.touches[0].position - positionAtBegin;
                    if (cumulatedDelta.x > epsilon || cumulatedDelta.y > epsilon)
                    {
                        // too far from initial touch position to make a single touch:
                        canBecomeSingleTouch = false;
                    }

                    if (canSwipeCamera)
                    {
                        Vector2 delta = Input.touches[0].deltaPosition;
                        float rotationZ = delta.x * sensitivityX * Time.deltaTime;
                        rotationZ = invertX ? rotationZ : rotationZ * -1;
                        float rotationX = delta.y * sensitivityY * Time.deltaTime;
                        rotationX = invertY ? rotationX : rotationX * -1;
                        if (Device.Orientation == DeviceOrientation.LandscapeLeft)
                        {
                            transform.localEulerAngles += new Vector3(-rotationZ, -rotationX, 0);
                        }
                        else
                        {
                            transform.localEulerAngles += new Vector3(rotationZ, rotationX, 0);
                        }
                    }
                }

                if (Input.touches[0].phase == TouchPhase.Ended)
                {
                    if (canBecomeSingleTouch)
                    {
                        videoPlayController.ToggleVideoController();
                    }
                }
            }
        }
    }
}
