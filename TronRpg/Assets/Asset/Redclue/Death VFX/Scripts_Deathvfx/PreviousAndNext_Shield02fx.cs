using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PreviousAndNext_Shield02fx : MonoBehaviour
{
    public GameObject[] Prefab;
    public float respawnDelay = 1f;
    private int number;
    private GameObject currentInstance;
    private bool isPressed;
    private Coroutine respawnCoroutine;

    void Start()
    {
        ChangeCurrent(0);
    }

    void Update()
    {
        if (currentInstance == null && respawnCoroutine == null)
        {
            respawnCoroutine = StartCoroutine(RespawnAfterDelay());
        }

        if (Input.GetKeyDown(KeyCode.A) && !isPressed)
        {
            isPressed = true;
            ChangeCurrent(-1);
        }

        if (Input.GetKeyDown(KeyCode.D) && !isPressed)
        {
            isPressed = true;
            ChangeCurrent(+1);
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            isPressed = false;
        }
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (enabled)
        {
            ChangeCurrent(0);
        }

        respawnCoroutine = null;
    }

    void ChangeCurrent(int delta)
    {
        number += delta;

        if (number > Prefab.Length - 1)
            number = 0;
        else if (number < 0)
            number = Prefab.Length - 1;

        if (currentInstance != null)
        {
            Destroy(currentInstance);
        }

        currentInstance = Instantiate(Prefab[number], transform.position, Quaternion.identity);
    }
}

