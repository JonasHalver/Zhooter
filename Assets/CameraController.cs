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
        
    }

    void LateUpdate()
    {
        playerPos = player.transform.position;
        transform.position = Vector3.Lerp(transform.position, playerPos + new Vector3(0, 0, -10), 2 * Time.deltaTime);
    }
}
