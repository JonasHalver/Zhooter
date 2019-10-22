using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour
{
    public Camera cam;
    public GameObject barricadePrefab;
    LineRenderer lr = null;

    bool startPlaced;
    Vector3 startPos, endPos;
    public Vector3 dir;

    public bool snap;
    public int snapCount;

    public static float width;
    public static bool building;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Manager.currentState == Manager.State.Building)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                    if (Manager.woodCount != 0)
                        SetStartPostion();
            }
            if (startPlaced)
            {
                width = Vector3.Distance(startPos, cam.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10));

                if (snap)
                    PlaceWithSnap();
                else
                    lr.SetPosition(1, cam.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10));                    
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (startPlaced)
                    SetEndPosition();
            }
        }
    }

    void SetStartPostion()
    {
        building = true;
        startPos = cam.ScreenToWorldPoint(Input.mousePosition + (Vector3.forward * 10));
        GameObject newBarricade = Instantiate(barricadePrefab, Vector3.zero, Quaternion.identity);
        lr = newBarricade.GetComponent<LineRenderer>();
        lr.SetPosition(0, startPos);
        startPlaced = true;
    }

    void PlaceWithSnap()
    {
        dir = (cam.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10) - lr.GetPosition(0)).normalized;
        int length = Mathf.FloorToInt(width / Manager.costPerMeter) + 1;
        length = Mathf.Clamp(length, 1, Manager.woodCount);
        lr.SetPosition(1, lr.GetPosition(0) + dir * length * Manager.costPerMeter);
        snapCount = length;
    }

    void SetEndPosition()
    {
        if (snap)
            endPos = lr.GetPosition(0) + dir * snapCount * Manager.costPerMeter;
        else
            endPos = cam.ScreenToWorldPoint(Input.mousePosition + (Vector3.forward*10));

        startPlaced = false;
        lr.SetPosition(1, endPos);
        lr.gameObject.GetComponent<BarricadeStats>().value = snapCount;
        lr.gameObject.SendMessage("Build");
        
        //AddCollider();
        lr = null;
        building = false;
        Manager.woodCount -= snapCount;
    }

    void AddCollider()
    {
        PolygonCollider2D col = lr.gameObject.GetComponent<PolygonCollider2D>();
        float halfWidth = lr.startWidth / 2;

        Vector3 dirCol = lr.GetPosition(1) - lr.GetPosition(0);
        Vector3 newLeft = Vector3.Cross(Vector3.forward, dirCol.normalized);

        Vector2[] points = new Vector2[4];

        points[0] = lr.GetPosition(0) + newLeft * halfWidth;
        points[1] = lr.GetPosition(1) + newLeft * halfWidth;
        points[2] = lr.GetPosition(1) - newLeft * halfWidth;
        points[3] = lr.GetPosition(0) - newLeft * halfWidth;

        col.SetPath(0, points);
        col.isTrigger = false;
        col.gameObject.SendMessage("Build");
    }
}
