using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{   
    public GameObject firepoint;
    public GameObject fx;   
    public Rotate rotate;   
    private GameObject effectToSpawn;    
    private float timeToFire = 0;

    void Start()
    {
        // Set the initial visual effect to be spawned to the assigned prefab.
        effectToSpawn = fx;
    }

    void Update()
    {
        // Check if the right mouse button is pressed and if enough time has passed since the last spawn.
        if (Input.GetMouseButton(1) && Time.time >= timeToFire)
        {
            // Calculate the next allowed spawn time based on the fire rate of the visual effect.
            timeToFire = Time.time + 1 / effectToSpawn.GetComponent<Fire>().fireRate;

            // Trigger the method to spawn the visual effect.
            SpawnVFX();
        }
    }

    // Method responsible for spawning the visual effect.
    void SpawnVFX()
    {
        // Local variable to store the instantiated visual effect.
        GameObject fx;

        // Check if a firepoint is assigned.
        if (firepoint != null)
        {
            // Instantiate the visual effect at the position of the firepoint with no rotation.
            fx = Instantiate(effectToSpawn, firepoint.transform.position, Quaternion.identity);

            // Check if a rotate script is assigned for additional rotation adjustments.
            if (rotate != null)
            {
                // Apply rotation to the spawned visual effect based on the Rotate script.
                fx.transform.localRotation = rotate.GetRotation();
            }
        }
        else
        {
            // Log a warning if no firepoint is assigned.
            Debug.Log("No Fire Point");
        }
    }
}

