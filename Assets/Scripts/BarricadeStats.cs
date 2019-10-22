using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarricadeStats : MonoBehaviour
{
    public float healthMax = 100, health = 100, buildSpeed = 2;

    public PolygonCollider2D col;
    public LineRenderer lr;
    float width;
    [HideInInspector]
    public bool reinforced;
    public float value;

    public GameObject woodPrefab, metalPrefab;
    Vector3 midpoint = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        width = lr.widthMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        lr.widthMultiplier = width * (health / healthMax);
        ColliderUpdate();
        if (health <= 0)
            Destroyed();
    }

    public void Build()
    {
        if (value > 1)
            Split();
        StartCoroutine(BuildRoutine());
    }

    void Split()
    {
        Vector3 dir = (lr.GetPosition(1) - lr.GetPosition(0)).normalized;
        float dst = Vector3.Distance(lr.GetPosition(1), lr.GetPosition(0));

        Vector3[] newPoints = new Vector3[(int)value];
        newPoints[newPoints.Length - 1] = lr.GetPosition(1);
        for (int i = 0; i < newPoints.Length - 1; i++)
        {
            newPoints[i] = lr.GetPosition(0) + (dir * (dst / value) * (i + 1));
        }

        lr.SetPosition(1, newPoints[0]);

        for (int i = 0; i < value-1; i++)
        {
            GameObject newBarricade = Instantiate(gameObject, Vector3.zero, Quaternion.identity);
            newBarricade.GetComponent<BarricadeStats>().value = 1;
            LineRenderer nlr = newBarricade.GetComponent<LineRenderer>();
            nlr.SetPosition(0, newPoints[i]);
            nlr.SetPosition(1, newPoints[i+1]);
            newBarricade.SendMessage("Build");
        }        
    }

    IEnumerator BuildRoutine()
    {
        health = 5;
        for (int i = 5; i < healthMax; i++)
        {
            health++;
            yield return new WaitForSecondsRealtime(buildSpeed / healthMax);
        }
    }

    public void ColliderUpdate()
    {
        float halfWidth = lr.widthMultiplier / 2;

        Vector3 dirCol = lr.GetPosition(1) - lr.GetPosition(0);
        Vector3 newLeft = Vector3.Cross(Vector3.forward, dirCol.normalized);

        Vector2[] points = new Vector2[4];

        points[0] = lr.GetPosition(0) + newLeft * halfWidth;
        points[1] = lr.GetPosition(1) + newLeft * halfWidth;
        points[2] = lr.GetPosition(1) - newLeft * halfWidth;
        points[3] = lr.GetPosition(0) - newLeft * halfWidth;

        for (int i = 0; i < points.Length; i++)
        {
            midpoint += (Vector3)points[i];
        }
        midpoint = midpoint / 4;
        col.SetPath(0, points);
        col.isTrigger = false;
    }

    public void Attacked(float damage)
    {
        if (!reinforced)
            health -= damage;
        else
            health -= (damage / 3) * 2;
    }

    void Destroyed()
    {
        for (int i = 0; i < value; i++)
        {
            if (Random.Range(0, 2) == 0)
            {
                GameObject newWood = Instantiate(woodPrefab, midpoint, Quaternion.identity);
                newWood.name = "Wood";
            }
        }
        Destroy(gameObject);
    }
}
