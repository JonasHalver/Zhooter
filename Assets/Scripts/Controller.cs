using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    GameObject player;
    Vector2 dirToTarget;

    public GameObject target;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)        
            target = player;

        dirToTarget = target.GetComponent<Rigidbody2D>().position - rb.position;
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }

    public void Hit(Color hitColor)
    {
        StartCoroutine(HitColor(hitColor));
    }

    IEnumerator HitColor(Color hitColor)
    {
        Color c = sr.color;

        sr.color = hitColor;
        yield return new WaitForSeconds(0.01f);

        sr.color = new Color(c.r * 0.5f, c.g * 0.5f, c.b * 0.5f);
    }
}
