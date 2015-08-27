using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
    private Transform myTransform;
    public Vector3 Axis = Vector3.back;

    void Start()
    {
        myTransform = this.transform;
    }

    void Update()
    {


		if (Camera.main != null) {
			Transform cameraTransform = Camera.main.transform;
			myTransform.LookAt (myTransform.position + cameraTransform.rotation * Axis,
            cameraTransform.rotation * Vector3.up);
		}
    }
}