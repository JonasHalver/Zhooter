using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeatMap : MonoBehaviour
{
    public static Hotspot[,] grid;
    float nodeDiameter;
    public float nodeRadius;
    public Vector2 gridWorldSize;

    public GameObject hotspotPrefab;

    int gridSizeX, gridSizeY;

    public static float lowestTemp = -50f, highestTemp = 100f;

    List<int> indices = new List<int>();

    [Tooltip("Per 100 frames")]
    [Range(1, 100)]
    public int windSpeed = 1;

    [Tooltip("Distance per shift")]
    [Range(0, 5)]
    public float windForce = 0.5f;

    public bool wind;
    public float windDirection;
    List<Hotspot> localHS = new List<Hotspot>();
    List<float> hsDsts = new List<float>();
    

    // Start is called before the first frame update
    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        indices.Add(1); indices.Add(0); indices.Add(3); indices.Add(2);

        CreateGrid();
        RandomizeTemperatures();
        StartCoroutine(HeatExchanceProcess());
        StartCoroutine(WindProcess());
    }

    // Update is called once per frame
    void Update()
    {
        // Find a way to average heat values from all sources within a box
        windSpeed = Mathf.Clamp(windSpeed, 1, 100);

        AngleClamp();
        Debug.DrawLine(Vector3.zero, Vector3.zero + AngleToDir(windDirection) * 10);
    }

    public void CreateGrid()
    {
        grid = new Hotspot[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                GameObject newHotSpot = Instantiate(hotspotPrefab, worldPoint, Quaternion.Euler(0, 0, 0), transform);
                newHotSpot.transform.localScale = Vector3.one * nodeDiameter;
                grid[x, y] = new Hotspot(worldPoint, newHotSpot, x, y);
                newHotSpot.GetComponent<HotSpotObject>().hs = grid[x, y];
            }
        }
    }

    public void RandomizeTemperatures()
    {
        foreach(Hotspot hs in grid)
        {
            hs.temperature = Random.Range(lowestTemp, 0);
        }
    }

    IEnumerator HeatExchanceProcess()
    {
        while (true)
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 0)
                    NormalizeTemperatureGlobal();
                if (i == 5)
                    NormalizeTemperatureLocal();
                if (i == 9)
                    AirCooling();
                yield return null;
            }
        }
    }

    public void AirCooling()
    {
        foreach (Hotspot hs in grid)
        {
            hs.temperature = Mathf.Lerp(hs.temperature, hs.temperature - 20, 0.05f);
        }
    }

    public void NormalizeTemperatureGlobal()
    {
        foreach(Hotspot hs in grid)
        {
            if (hs.temperature < 50 || hs.hotSpotObj.GetComponent<HotSpotObject>().heatSources.Count == 0)
            {
                RandomizeDirectionCheck(hs);
            }
        }
    }

    public void NormalizeTemperatureLocal()
    {
        foreach(Hotspot hs in grid)
        {
            List<HeatSource> heatList = hs.hotSpotObj.GetComponent<HotSpotObject>().heatSources;
            foreach (HeatSource source in heatList)
            {
                HeatExchange(hs, source.temperature, 1);
            }
        }
    }

    public void HeatExchange(Hotspot affectee, float otherTemp, float rate)
    {
        float maxDif = highestTemp - lowestTemp;
        float thisDif = Mathf.Abs(affectee.temperature - otherTemp);
        float t = thisDif / maxDif;
        affectee.temperature = Mathf.Lerp(affectee.temperature, otherTemp, t * rate);
    }

    void RandomizeDirectionCheck(Hotspot hs)
    {
        int startIndex = Random.Range(0,4);
        bool reverse = false;
        if (Random.Range(0, 2) == 0)
            reverse = true;
        for (int i = 0; i < 4; i++)
        {
            int index = indices[startIndex];
            switch (index)
            {
                case 0:
                    if (hs.y != 0)
                        HeatExchange(hs, grid[hs.x, hs.y - 1].temperature, 1);
                    break;
                case 1:
                    if (hs.x < gridSizeX - 1)
                        HeatExchange(hs, grid[hs.x + 1, hs.y].temperature, 1);
                    break;
                case 2:
                    if (hs.y < gridSizeY - 1)
                        HeatExchange(hs, grid[hs.x, hs.y + 1].temperature, 1);
                    break;
                case 3:
                    if (hs.x != 0)
                        HeatExchange(hs, grid[hs.x - 1, hs.y].temperature, 1);
                    break;
            }
            if (!reverse)
                startIndex--;
            else
                startIndex++;

            if (startIndex < 0)
                startIndex = 3;
            if (startIndex > 3)
                startIndex = 0;
        }
    }

    IEnumerator WindProcess()
    {
        while (true)
        {
            for (int i = 0; i < 100 / windSpeed; i++)
            {
                if (i == 0)
                    Wind();
                yield return null;
            }
        }
    }

    public void Wind()
    {
        if (wind)
        {
            foreach (Hotspot hs in grid)
            {
                if (hs.windPoint == null)
                    hs.windPoint = hs.worldPos;

                hsDsts.Clear();
                localHS.Clear();

                localHS.Add(hs);
                //Left
                if (hs.y != 0)
                    localHS.Add(grid[hs.x, hs.y - 1]);
                //Top Left
                if (hs.y != 0 && hs.x < gridSizeX - 1)
                    localHS.Add(grid[hs.x + 1, hs.y - 1]);
                //Top
                if (hs.x < gridSizeX - 1)
                    localHS.Add(grid[hs.x + 1, hs.y]);
                //Top Right
                if (hs.x < gridSizeX - 1 && hs.y < gridSizeY - 1)
                    localHS.Add(grid[hs.x + 1, hs.y + 1]);
                //Right
                if (hs.y < gridSizeY - 1)
                    localHS.Add(grid[hs.x, hs.y + 1]);
                //Bot Right
                if (hs.y < gridSizeY - 1 && hs.x != 0)
                    localHS.Add(grid[hs.x - 1, hs.y + 1]);
                //Bot
                if (hs.x != 0)
                    localHS.Add(grid[hs.x - 1, hs.y]);
                //Bot Left
                if (hs.x != 0 && hs.y != 0)
                    localHS.Add(grid[hs.x - 1, hs.y - 1]);

                Vector3 newWindPoint = hs.windPoint + AngleToDir(windDirection) * windForce;
                Debug.DrawLine(hs.windPoint, newWindPoint, color: Color.red, 1);

                foreach (Hotspot spot in localHS)
                {
                    hsDsts.Add(Vector3.Distance(newWindPoint, spot.worldPos));
                }
                hsDsts.Sort();

                Hotspot affectedHotspot = null;

                foreach (Hotspot s in localHS)
                {
                    if (Vector3.Distance(newWindPoint, s.worldPos) == hsDsts[0])
                    {
                        affectedHotspot = s;
                    }
                }
                if (affectedHotspot != null)
                {
                    affectedHotspot.futureTemp = hs.temperature;
                    affectedHotspot.windPoint = newWindPoint;
                }
            }
            foreach (Hotspot hs in grid)
            {
                hs.temperature = (hs.temperature + hs.futureTemp)/2;
            }
        }
    }

    Vector3 AngleToDir(float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
    }

    void AngleClamp()
    {
        if (windDirection < 0)
            windDirection += 360;
        else if (windDirection > 360)
            windDirection -= 360;
    }
}

public class Hotspot
{
    public Vector3 worldPos;
    public GameObject hotSpotObj;
    public float temperature, futureTemp = HeatMap.lowestTemp;
    public Vector3 windPoint;
    public int x, y;

    public Hotspot(Vector3 _worldPos, GameObject _hotSpotObj, int _x, int _y)
    {
        worldPos = _worldPos;
        hotSpotObj = _hotSpotObj;
        x = _x;
        y = _y;
    }
}