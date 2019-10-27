using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSpotObject : MonoBehaviour
{
    public Hotspot hs;
    public SpriteRenderer sr;

    public Color coldest, warmest;
    public Gradient coldToWarm;

    public float temperature;

    public List<HeatSource> heatSources = new List<HeatSource>();
    public bool ignore;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ignore = hs.ignoreDuringLoops;
        hs.temperature = Mathf.Clamp(hs.temperature, HeatMap.lowestTemp, HeatMap.highestTemp);
        temperature = hs.temperature;
        if (hs.affectedByTemp)
            sr.color = TemperatureColor();
        else
            sr.color = Color.black;
    }

    public Color TemperatureColor()
    {
        float dif = HeatMap.highestTemp - HeatMap.lowestTemp;
        float point = hs.temperature - HeatMap.lowestTemp;
        float t = point / dif;
        return coldToWarm.Evaluate(t);
        //return Color.Lerp(coldest, warmest, t);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Obstacle"))
            hs.affectedByTemp = false;
    }
}
