using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSpotObject : MonoBehaviour
{
    public Hotspot hs;
    public SpriteRenderer sr;

    public Color coldest, warmest;

    public float temperature;

    public List<HeatSource> heatSources = new List<HeatSource>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hs.temperature = Mathf.Clamp(hs.temperature, HeatMap.lowestTemp, HeatMap.highestTemp);
        temperature = hs.temperature;
        sr.color = TemperatureColor();
    }

    public Color TemperatureColor()
    {
        float dif = HeatMap.highestTemp - HeatMap.lowestTemp;
        float point = hs.temperature - HeatMap.lowestTemp;
        float t = point / dif;
        return Color.Lerp(coldest, warmest, t);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<HeatSource>() != null)
        {
            print("Entered hotspot");
            HeatSource hs = col.GetComponent<HeatSource>();
            if (!heatSources.Contains(hs))
                heatSources.Add(hs);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<HeatSource>() != null)
        {
            HeatSource hs = col.GetComponent<HeatSource>();
            if (heatSources.Contains(hs))
                heatSources.Remove(hs);
        }
    }
}
