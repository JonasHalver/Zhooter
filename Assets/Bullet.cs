using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Color hitColor = Color.red;
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
        {
            if (other.gameObject.tag == "Zombie")
            {
                other.gameObject.SendMessage("Hit", damage);
            }
            Destroy(gameObject);
        }
    }

}
