using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public Vector3 playerPos;

    // Update is called once per frame
    void Update()
    {
        playerPos = player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = playerPos + new Vector3(0, 0, -10);
    }
}
