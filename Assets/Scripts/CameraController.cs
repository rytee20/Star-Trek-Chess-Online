using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Camera cameraObj;
    public GameObject myGameObj;
    public float speed = 2f;
    public float zoomSpeed = 5f;

    void Update()
    {
        RotateCamera();
    }

    void RotateCamera()
    {
        if (Input.GetMouseButton(1))
        {
            cameraObj.transform.RotateAround(myGameObj.transform.position,
                                            cameraObj.transform.up,
                                            -Input.GetAxis("Mouse X") * speed);

            cameraObj.transform.RotateAround(myGameObj.transform.position,
                                            cameraObj.transform.right,
                                            -Input.GetAxis("Mouse Y") * speed);
        }

        if (cameraObj.fieldOfView<=40 && cameraObj.fieldOfView>=20)
        {
            cameraObj.fieldOfView += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        }
        else
        {
            if (cameraObj.fieldOfView > 40) cameraObj.fieldOfView = 40;
            if (cameraObj.fieldOfView < 20) cameraObj.fieldOfView = 20;
        }
    }
}