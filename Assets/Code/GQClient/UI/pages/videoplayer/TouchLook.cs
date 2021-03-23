using UnityEngine;
using UnityEngine.Events;

namespace Code.GQClient.UI.pages.videoplayer
{
    [AddComponentMenu("Camera-Control/Touch Look")]
    public class TouchLook : MonoBehaviour
    {
        public float sensitivityX = 5.0f;
        public float sensitivityY = 5.0f;

        public float upperLimit = 310f;
        public float lowerLimit = 40f;
        public UnityEvent swiped;


        public bool invertX = false;
        public bool invertY = false;

        private float epsilon = 0.01f;

        private Vector2 positionAtBegin;
        private bool canBecomeSingleTouch;
        private bool canSwipeCamera;

        public VideoPlayController videoPlayController;

        // Update is called once per frame
        private void Update()
        {
            if (Input.touches.Length > 0)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    positionAtBegin = Input.touches[0].position;
                    canBecomeSingleTouch =
                        !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(
                            Input.GetTouch(0).fingerId);
                    canSwipeCamera =
                        canBecomeSingleTouch;
                }

                if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    var cumulatedDelta = Input.touches[0].position - positionAtBegin;
                    if (cumulatedDelta.x > epsilon || cumulatedDelta.y > epsilon)
                    {
                        // too far from initial touch position to make a single touch:
                        canBecomeSingleTouch = false;
                    }

                    if (canSwipeCamera)
                    {
                        var delta = Input.touches[0].deltaPosition;
                        var rotationZ = delta.x * sensitivityX * Time.deltaTime;
                        rotationZ = invertX ? rotationZ : rotationZ * -1;
                        var rotationX = delta.y * sensitivityY * Time.deltaTime;
                        rotationX = invertY ? rotationX : rotationX * -1;
 
                        transform.localEulerAngles += new Vector3(-rotationZ, -rotationX, 0);
                        if (Mathf.Abs(rotationZ) > 0.1f)
                        {
                            swiped.Invoke();
                        }
                        
                        if (transform.localEulerAngles.x > 180f && rotationZ > 0f &&
                            transform.localEulerAngles.x < upperLimit)
                        {
                            // upper limit reached:
                            transform.localEulerAngles = new Vector3(upperLimit, transform.localEulerAngles.y,
                                transform.localEulerAngles.z);
                        }

                        if (transform.localEulerAngles.x < 180f && rotationZ < 0f && transform.localEulerAngles.x > lowerLimit)
                        {
                            // lower limit reached:
                            transform.localEulerAngles = new Vector3(lowerLimit, transform.localEulerAngles.y,
                                transform.localEulerAngles.z);
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