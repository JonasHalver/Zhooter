using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    GameObject player;
    Vector2 dirToTarget;

    public float health = 100f, moveSpeed = 2;

    public GameObject target;

    public int rays = 10;
    public float radius = 1, viewAngle = 90, rotateSpeed = 180f;
    public LayerMask obstacleMask;
    bool obstacleInView;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(ForwardCheck());
    }

    // Update is called once per frame
    void Update()
    {
        //if (target == null)        
        //    target = player;

        if (target != null)
            dirToTarget = target.GetComponent<Rigidbody2D>().position - rb.position;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
        else
        {
            rb.MovePosition(rb.position + new Vector2(transform.up.x, transform.up.y) * moveSpeed * Time.fixedDeltaTime);

            if (obstacleInView)
            {
                if (FindValidPath())
                    transform.Rotate(new Vector3(0, 0, 1) * Time.fixedDeltaTime * rotateSpeed);
                else
                    transform.Rotate(new Vector3(0, 0, -1) * Time.fixedDeltaTime * rotateSpeed);
            }
            else
            {
                
            }
        }
    }

    public void Hit(float damage)
    {
        StartCoroutine(HitColor(damage));
    }

    IEnumerator HitColor(float damage)
    {
        Color c = sr.color;

        sr.color = Color.red;
        health -= damage;
        yield return new WaitForSeconds(0.01f);

        sr.color = new Color(c.r * 0.5f, c.g * 0.5f, c.b * 0.5f);
    }



    IEnumerator ForwardCheck()
    {
        while (true)
        {
            obstacleInView = false;
            FieldOfView(45);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RayCast(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, radius, obstacleMask);
        
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Obstacle")
            {
                print("Detecting obstacle");
                obstacleInView = true;
            }
        }
    }

    public void FieldOfView(float angle)
    {
        for (int i = 0; i < Mathf.RoundToInt(rays/2); i++)
        {
            float newAngle = angle / rays + (i * (angle / rays));
            RayCast(DirFromAngle(newAngle));
        }
        for (int i = 0; i < Mathf.RoundToInt(rays / 2); i++)
        {
            float newAngle = angle / rays + (i * (angle / rays)) * -1;
            RayCast(DirFromAngle(newAngle));
        }
    }

    public bool FindValidPath()
    {
        int left = 0, right = 0;
        Vector2 leftDir = Vector2.up, rightDir = Vector2.up;
        for (int i = 0; i < rays; i++)
        {
            float angle = 360 / rays + (i * (360 / rays));
            if (!Physics2D.Raycast(rb.position, DirFromAngle(angle), radius, obstacleMask))
            {
                right = i;
                rightDir = DirFromAngle(angle);
            }
        }
        for (int i = 0; i < rays; i++)
        {
            float angle = 360 / rays + (i * (360 / rays)) * -1;
            if (!Physics2D.Raycast(rb.position, DirFromAngle(angle), radius, obstacleMask))
            {
                left = i;
                leftDir = DirFromAngle(angle);
            }
        }

        if (left < right)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public Vector2 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.z + 90f;
        return new Vector2(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad));
    }
}
