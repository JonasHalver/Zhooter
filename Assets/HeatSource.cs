using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatSource : MonoBehaviour
{
    public Camera refCam;
    public float temperature;
    public LayerMask hotspotMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckPositionOnGrid();
    }

    public void CheckPositionOnGrid()
    {
        Ray ray = refCam.ScreenPointToRay(refCam.WorldToScreenPoint(transform.position));
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, hotspotMask);
        if (hit.collider != null)
        {
            HotSpotObject hso = hit.collider.GetComponent<HotSpotObject>();

            foreach(Hotspot hs in HeatMap.grid)
            {
                if (hs.hotSpotObj.GetComponent<HotSpotObject>().heatSources.Contains(this))
                {
                    hs.hotSpotObj.GetComponent<HotSpotObject>().heatSources.Remove(this);
                }
            }

            hso.heatSources.Add(this);
        }
    }
}
