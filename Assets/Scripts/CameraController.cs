using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static Camera cam;
    Camera refCam, usedCam;
    public GameObject player;
    public Vector3 playerPos;
    Vector3 mousePos;
    public float camNear = 5, camFar = 10;

    public bool cameraAimOn;
    float aimAmount = 5;
    public Vector3 offset;

    public bool isReference;

    void Start()
    {
        if (!isReference)
        {
            cam = transform.GetChild(0).GetComponent<Camera>();
            usedCam = cam;
        }
        else
        {
            refCam = GetComponent<Camera>();
            usedCam = refCam;
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(1))
            cameraAimOn = true;
        else
            cameraAimOn = false;

        // TODO: Try using movement inputs to control the camera directly. Overshoot the player to show where you are going, then snap back once the movement stops.

        playerPos = player.transform.position;
        mousePos = usedCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseDir = (mousePos - playerPos).normalized;

        if (!cameraAimOn)
            transform.position = playerPos + offset + new Vector3(mouseDir.x, mouseDir.y, -10);
        else
            transform.position = playerPos + offset + new Vector3(mouseDir.x * aimAmount, mouseDir.y * aimAmount, -10);

        if (Input.mouseScrollDelta.y < 0)
        {
            usedCam.orthographicSize = camFar;
            aimAmount = camFar;
        }
        else if (Input.mouseScrollDelta.y > 0)
        {
            usedCam.orthographicSize = camNear;
            aimAmount = camNear;
        }
    }
}
