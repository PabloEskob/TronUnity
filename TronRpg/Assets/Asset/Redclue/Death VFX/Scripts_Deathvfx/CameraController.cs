using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Reference to the main camera GameObject.
    public GameObject mainCamera;

    // Transform points representing the two camera positions for zooming.
    public Transform point1;
    public Transform point2;

    // Variables controlling camera movement and rotation.
    private Vector3 xyz;
    private float far = 0.5f;
    private float lerp = 0.5f;
    private Quaternion rotate;
    private bool isRotating = false;
    private float a;
    private float b;

    void Start()
    {
        // Set the initial local rotation and position values.
        rotate = gameObject.transform.localRotation;
        xyz = new Vector3(0f, 0f, 0f);
    }

    void Update()
    {
        // Call methods responsible for camera zooming and rotation.
        cameraZoom();
        cameraRotation();
    }

    // Method controlling the zooming behavior of the camera.
    void cameraZoom()
    {
        // Check for mouse scroll input to zoom in or out.
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            // Increase the zoom factor if within the allowed range.
            if (far < 1f)
            {
                far += 0.1f;
            }
            // Clamp the zoom factor to the maximum value.
            if (far > 1f)
            {
                far = 1f;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            // Decrease the zoom factor if within the allowed range.
            if (far > 0f)
            {
                far -= 0.1f;
            }
            // Clamp the zoom factor to the minimum value.
            if (far < 0f)
            {
                far = 0f;
            }
        }

        // Smoothly interpolate between two camera positions based on the zoom factor.
        lerp = Mathf.Lerp(lerp, far, Time.deltaTime * 10f);
        mainCamera.transform.position = Vector3.Lerp(point2.position, point1.position, lerp);
    }

    // Method controlling the rotation behavior of the camera.
    void cameraRotation()
    {
        // Check for left mouse button input to initiate or stop camera rotation.
        if (Input.GetMouseButton(0))
        {
            isRotating = true;
        }
        else
        {
            isRotating = false;
        }

        // If camera rotation is active, update the local rotation based on mouse input.
        if (isRotating == true)
        {
            rotate = gameObject.transform.localRotation;
            a = rotate.eulerAngles.x + Input.GetAxis("Mouse Y") * 2f;

            // Clamp the pitch angle to prevent over-rotation.
            if (a > 40f && a < 180f)
            {
                a = 40f;
            }
            if (a < 360f && a > 180f)
            {
                a = 360f;
            }

            // Update the yaw angle based on mouse input.
            b = rotate.eulerAngles.y + Input.GetAxis("Mouse X") * 2f;
            xyz.Set(a, b, 0f);
            rotate.eulerAngles = xyz;
            gameObject.transform.localRotation = rotate;
        }
    }
}
























