using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public enum State { Idle, Shooting, Building, Reinforcing }
    public static State currentState = State.Idle;

    public static float costPerMeter = 1, costOfCurrentBuild;
    [Range(0, 10)]
    public int costPerMeterInspector =1;

    public static int woodCount, metalCount;
    [Range(0,10)]
    public int wood, metal;

    public GameObject spawnerPrefab;
    public int nightLengthMinutes = 3;
    public static int minutes, seconds;
    public static bool nightStarted;
    public static int spawnDelay = 10;
    public static float reinforceArmor = 10f;

    // Start is called before the first frame update
    void Start()
    {
        woodCount = wood;
        metalCount = metal;
    }

    // Update is called once per frame
    void Update()
    {
        #region States
        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (currentState)
            {
                case State.Idle:
                    currentState = State.Shooting;
                    break;
                case State.Shooting:
                    currentState = State.Building;
                    break;
                case State.Building:
                    currentState = State.Reinforcing;
                    break;
                case State.Reinforcing:
                    currentState = State.Idle;
                    break;
            }
        }
        #endregion States

        costPerMeter = costPerMeterInspector;
        if (Building.building)
        {
            costOfCurrentBuild = Mathf.Clamp(Mathf.Floor(Building.width / costPerMeter) + 1, 1, woodCount);
        }
        else
        {
            costOfCurrentBuild = 0;
        }

        if (Input.GetKeyDown(KeyCode.P))
            StartCoroutine(StartNight());
    }

    IEnumerator StartNight()
    {
        nightStarted = true;
        for (int i = nightLengthMinutes -1; i > -1; i--)
        {
            minutes = i;
            for (int s = 59; s > -1; s--)
            {
                if (s % 20 == 0)
                    spawnDelay--;
                seconds = s;
                yield return new WaitForSecondsRealtime(1);
            }
            GameObject newSpawner = Instantiate(spawnerPrefab, Vector3.zero, Quaternion.identity);
            newSpawner.GetComponent<OffScreenSpawner>().cam = CameraController.cam;
        }
        nightStarted = false;
    }

    public void SetStateBuilding()
    {
        currentState = State.Building;
    }
}
