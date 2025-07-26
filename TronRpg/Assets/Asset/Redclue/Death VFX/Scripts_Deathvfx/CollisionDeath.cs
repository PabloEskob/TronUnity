using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDeath : MonoBehaviour
{
    private Animator animator; 
    private Material dissolveMaterials;
    private bool press;
    private float number;


    public Renderer skinnedMeshRenderer;
    public ParticleSystem particles;
    public float waitBeforeParticles = 1f;
    public float waitBeforeDissolve = 2f;
    public float dissolveDuration = 1f;
    public float destroyDelay = 2.0f;

    private void Start()
    {
        animator = GetComponent<Animator>();  // Get the Animator component attached to this object.

        // Deactivate the particle system if it is not null.
        if (particles != null)
        {
            particles.gameObject.SetActive(false);
        }

        // Set the dissolve material if the skinned mesh renderer is not null.
        if (skinnedMeshRenderer != null)
        {
            dissolveMaterials = skinnedMeshRenderer.material;
        }
    }

    private void Update()
    {
        // If the press flag is true, gradually increase the dissolve effect over time.
        if (press == true)
        {
            number += Time.deltaTime / dissolveDuration;
            dissolveMaterials.SetFloat("Dissolve", number);
        }
    }

    // Collision detection when the object is hit by a "Bullet" object.
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is tagged as "Bullet".
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Trigger the Die animation and initiate the coroutine for particle activation and dissolve effect.
            Die();
            StartCoroutine(WaitParticles());
            StartCoroutine(WaitDissolve());
        }
    }

    // Method to trigger the "Die" animation and destroy the object after a delay.
    private void Die()
    {
        animator.SetTrigger("Die");  // Trigger the "Die" animation.

        // Destroy the game object after the specified delay.
        Destroy(gameObject, destroyDelay);
    }

    // Coroutine to wait for a specified time before activating the particle system.
    IEnumerator WaitParticles()
    {
        yield return new WaitForSeconds(waitBeforeParticles);  // Wait for the specified time.

        // Activate the particle system if it is not null.
        if (particles != null)
        {
            particles.gameObject.SetActive(true);
        }
    }

    // Coroutine to wait for a specified time before setting the press flag to initiate the dissolve effect.
    IEnumerator WaitDissolve()
    {
        yield return new WaitForSeconds(waitBeforeDissolve);  // Wait for the specified time.

        // Set the press flag to true, initiating the dissolve effect in the Update method.
        press = true;
    }
}

