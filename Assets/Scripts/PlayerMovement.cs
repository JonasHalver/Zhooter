using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f, rollDst = 5f, rollCoolDown = 1f;
    public Rigidbody2D rb;

    Vector2 movement;
    bool rolling, cd;


    void Update()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Roll"))
        {
            if (!cd)
                StartCoroutine(Roll(movement));
        }
    }

    void FixedUpdate()
    {
        if (!rolling)
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    IEnumerator Roll(Vector2 direction)
    {
        rolling = true;
        Vector2 prevPos = rb.position;
        while (true)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * 3 * Time.fixedDeltaTime);
            Vector2 newPos = rb.position;
            yield return null;
            if (Vector2.SqrMagnitude(newPos - prevPos) >= rollDst)
            {
                break;
            }
        }
        StartCoroutine(CoolDown());
        rolling = false;
    }

    IEnumerator CoolDown()
    {
        cd = true;
        yield return new WaitForSeconds(rollCoolDown);
        cd = false;
    }
}
