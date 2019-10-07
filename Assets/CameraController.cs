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
        // TODO: Try using movement inputs to control the camera directly. Overshoot the player to show where you are going, then snap back once the movement stops.

        playerPos = player.transform.position;
        transform.position = Vector3.Lerp(transform.position, playerPos + new Vector3(0, 0, -10), 2 * Time.deltaTime);
    }
}
