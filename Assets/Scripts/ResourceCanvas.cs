using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCanvas : MonoBehaviour
{
    public Text wood, metal, health;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        wood.text = "Wood = " + Manager.woodCount.ToString();
        metal.text = "Metal = " + Manager.metalCount.ToString();
        health.text = "Health = " + PlayerStats.health.ToString() + "/100";
    }
}
