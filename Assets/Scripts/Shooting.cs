using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    public float shotForce = 30f;
    public GameObject shotPrefab;
    public Transform shootPoint;

    public Transform ammoPanel;

    public int ammoMax = 8, ammoSpent = 0;
    public float damage = 40f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (ammoSpent != ammoMax)
                Shoot();
            else
                Reload();
        }
        if (Input.GetButtonDown("Reload"))
            if (ammoSpent != 0)
                Reload();
    }

    public void Shoot()
    {
        GameObject newBullet = Instantiate(shotPrefab, shootPoint.position, Quaternion.identity);
        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
        newBullet.GetComponent<Bullet>().damage = damage;
        rb.AddForce(-shootPoint.up * shotForce, ForceMode2D.Impulse);
        Image img = ammoPanel.GetChild(ammoSpent).GetComponent<Image>();
        var alpha = img.color;
        alpha.a = 0;
        img.color = alpha;

        ammoSpent++;
        Destroy(newBullet, 2f);
    }

    public void Reload()
    {
        for (int i = 0; i < ammoPanel.childCount; i++)
        {
            Image img = ammoPanel.GetChild(i).GetComponent<Image>();
            var alpha = img.color;
            alpha.a = 1;
            img.color = alpha;
        }
        ammoSpent = 0;
    }
}
