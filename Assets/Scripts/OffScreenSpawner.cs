using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreenSpawner : MonoBehaviour
{
    public Camera cam;
    public GameObject zombiePrefab;
    public List<Vector3> spawnPoints = new List<Vector3>();
    bool spawning;
    public LayerMask obstacleMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Manager.nightStarted && !spawning)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    IEnumerator SpawnRoutine()
    {
        spawning = true;
        while (true)
        {
            yield return new WaitForSecondsRealtime(Manager.spawnDelay);
            int iteration = 0;
            while (true)
            {
                int rndIndex = Random.Range(0, spawnPoints.Count);
                if (CheckSpawnPoint(rndIndex))
                {
                    Spawn(rndIndex);
                    break;
                }
                else
                    iteration++;
                if (iteration >= 100)
                    break;
            }
            if (!Manager.nightStarted)
                break;
        }
        spawning = false;
    }

    bool CheckSpawnPoint(int index)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(cam.ViewportToWorldPoint(spawnPoints[index]), 1, obstacleMask);
        if (cols.Length > 0)
            return false;
        else
            return true;
    }

    void Spawn(int index)
    {
        Instantiate(zombiePrefab, cam.ViewportToWorldPoint(spawnPoints[index]), Quaternion.identity);
    }
}
