using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMove : MonoBehaviour
{
    public GameObject vrCamera;
    // Start is called before the first frame update
    Vector3 playerPos;
    Vector3 playerDirection;
    Quaternion playerRotation;
    public float spawnDistance = 5;
    Vector3 spawnPos;

    void Start()
    {
        if (vrCamera == null)
            vrCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    public void MoveCamera()
    {
        playerPos = vrCamera.transform.position;
        playerDirection = vrCamera.transform.forward;
        playerRotation = vrCamera.transform.rotation;
        playerRotation.x = 0;
        playerRotation.z = 0;
        spawnPos = playerPos + playerDirection * spawnDistance;

        transform.rotation = playerRotation;
        transform.position = spawnPos;
    }
}
