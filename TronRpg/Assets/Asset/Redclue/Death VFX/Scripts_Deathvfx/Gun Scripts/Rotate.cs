using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Reference to the camera for raycasting
    public Camera cam;
    
    // Maximum length of the ray for raycasting
    public float maximumLenght;

    // Ray for mouse position
    private Ray rayMouse;

    // Position of the hit point or the maximum length point
    private Vector3 pos;

    // Direction from the object to the hit point
    private Vector3 direction;

    // Rotation quaternion for rotation calculation
    private Quaternion rotation;

    void Update()
    {
        // Check if the camera reference is not null
        if (cam != null)
        {
            RaycastHit hit;
            
            // Get mouse position in screen coordinates
            var mousePos = Input.mousePosition;

            // Convert mouse position to a ray in the scene
            rayMouse = cam.ScreenPointToRay(mousePos);

            // Check if the ray hits an object within the maximum length
            if (Physics.Raycast(rayMouse.origin, rayMouse.direction, out hit, maximumLenght))
            {
                // Rotate towards the hit point if an object is hit
                RotateToMouseDirection(gameObject, hit.point);
            }
            else
            {
                // If no object is hit, rotate towards the maximum length point
                pos = rayMouse.GetPoint(maximumLenght);
                RotateToMouseDirection(gameObject, pos);
            }
        }
        else
        {
            // Log a warning if no camera reference is provided
            Debug.Log("No Camera");
        }
    }

    // Rotate the object towards the specified destination point
    void RotateToMouseDirection(GameObject obj, Vector3 destination)
    {
        // Calculate the direction from the object to the destination point
        direction = destination - obj.transform.position;

        // Calculate the rotation quaternion to face the destination point
        rotation = Quaternion.LookRotation(direction);

        // Use Quaternion.Lerp for smooth rotation
        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
    }

    // Get the current rotation quaternion of the object
    public Quaternion GetRotation()
    {
        return rotation;
    }
}


