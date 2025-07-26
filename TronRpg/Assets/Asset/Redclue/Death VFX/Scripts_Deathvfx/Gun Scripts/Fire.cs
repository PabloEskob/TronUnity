using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public float speed;     // Speed at which the Bullet moves
    public float fireRate;  // Rate at which the bullet can be fired(Other scripts using this as reference)

    void Update()
    {
        // Check if the speed is not equal to 0
        if (speed != 0)
        {
            // Move the object in the forward direction
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
        else
        {
            // If speed is zero, log a message
            Debug.Log("No Speed");
        }

        // Destroy the GameObject after 6 seconds
        Destroy(gameObject, 6f);
    }

    // Called when the GameObject collides with another object
    void OnCollisionEnter(Collision collision)
    {
        // Stop the movement and destroy the fire projectile upon collision
        speed = 0;
        Destroy(gameObject);
    }
}



