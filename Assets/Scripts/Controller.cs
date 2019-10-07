using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Controller : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    GameObject player;
    Vector2 dirToTarget;

    public enum State { Chasing, Idle, Searching }
    public State currentState = State.Idle;

    public float health = 100f, moveSpeed = 2;

    public GameObject target;

    public int rays = 10;
    public float radius = 1, viewAngle = 90, rotateSpeed = 180f;
    public LayerMask obstacleMask, playerAndObstacleMask;
    bool obstacleInView, lookingAtPlayer, investigateSound;

    Vector2 lastPos = new Vector2();
    Seeker seeker;
    Path path;
    int currentWayPoint = 0;
    public bool reachedEndOfPath;
    float nextWayPointDistance = .5f;
    Vector2 direction;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        seeker = GetComponent<Seeker>();
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
        if (path != null)
        {
            if (currentWayPoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                path = null;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
            if (distance < nextWayPointDistance)
            {
                currentWayPoint++;
            }
        }

        switch (currentState)
        {
            case State.Idle:
                IdleMovement();
                if (target != null)
                    currentState = State.Chasing;
                break;
            case State.Chasing:
                ChaseMovement();
                break;
            case State.Searching:
                SearchMovement(direction);
                if (rb.velocity.magnitude < 1)
                    currentState = State.Idle;
                break;
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void IdleMovement()
    {
        rb.MovePosition(rb.position + new Vector2(transform.up.x, transform.up.y) * moveSpeed * Time.fixedDeltaTime);

        if (obstacleInView)
        {
            if (FindValidPath())
                transform.Rotate(new Vector3(0, 0, 1) * Time.fixedDeltaTime * rotateSpeed);
            else
                transform.Rotate(new Vector3(0, 0, -1) * Time.fixedDeltaTime * rotateSpeed);
        }

        //Idea isn't bad, just give it a delay by adding to float t += time.deltattime
        //if (rb.velocity.magnitude < .01f)
        //{
        //    rb.rotation += 90;
        //}
    }
    
    public void ChaseMovement()
    {
        rb.MovePosition(rb.position + new Vector2(transform.up.x, transform.up.y) * moveSpeed * Time.fixedDeltaTime);
        float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
        if (!lookingAtPlayer)
            StartCoroutine(LookAtPlayer());
    }

    public void SearchMovement(Vector2 direction)
    {
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
        if (reachedEndOfPath)
            currentState = State.Idle;
    }

    public void Hit(float damage)
    {
        StartCoroutine(HitColor(damage));
    }

    IEnumerator LookAtPlayer()
    {
        lookingAtPlayer = true;
        
        while (true)
        {
            if (target == null)
            {
                lookingAtPlayer = false;
                currentState = State.Idle;
                break;
            }
            else
            {
                Vector2 dirToPlayer = target.transform.position - transform.position;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, radius, playerAndObstacleMask);
                if (hit.collider != null)
                {
                    if (!hit.collider.gameObject.CompareTag("Player"))
                    {
                        target = null;
                        lookingAtPlayer = false;
                        SearchArea(lastPos);
                        break;
                    }
                    else
                    {
                        lastPos = target.transform.position;
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                }
                else
                {
                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }
        }
    }

    public void SoundHeard(Vector2 pos)
    {
        if (currentState != State.Chasing)
            SearchArea(pos);
    }

    public void SearchArea(Vector2 pos)
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, pos, OnPathComplete);
        
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
            currentState = State.Searching;
        }
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
            FieldOfView(viewAngle/2);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RayCast(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, radius, playerAndObstacleMask);
        
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Obstacle")
            {
                obstacleInView = true;
            }
            else if (hit.collider.CompareTag("Player"))
            {
                print("Found Player");
                target = hit.collider.gameObject;
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
