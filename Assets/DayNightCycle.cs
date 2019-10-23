using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Color morning, noon, evening;
    public enum TimeOfDay { Night, Morning, Noon, Evening }
    public TimeOfDay currentTOD = TimeOfDay.Night;

    public float timeScale = 1;

    public float t;

    public new Light light;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PassageOfTime());
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentTOD)
        {
            case TimeOfDay.Night:
                light.intensity = 0;
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, -90, 0);
                break;
            case TimeOfDay.Morning:
                light.intensity = Mathf.Lerp(0, 1, t);
                light.color = Color.Lerp(evening, morning, t);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Lerp(-90, -30, t), 0);
                break;
            case TimeOfDay.Noon:
                light.color = Color.Lerp(morning, noon, t);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Lerp(-30, 30, t), 0);
                break;
            case TimeOfDay.Evening:
                light.color = Color.Lerp(noon, evening, t);
                light.intensity = Mathf.Lerp(1, 0, t);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Lerp(30, 90, t), 0);
                break;
        }
    }

    IEnumerator PassageOfTime()
    {
        while (true)
        {
            for (int i = 0; i < 360; i++)
            {
                t = (float)i / 360;
                yield return new WaitForSecondsRealtime(1 / timeScale);
            }
            switch (currentTOD)
            {
                case TimeOfDay.Night:
                    currentTOD = TimeOfDay.Morning;
                    break;
                case TimeOfDay.Morning:
                    currentTOD = TimeOfDay.Noon;
                    break;
                case TimeOfDay.Noon:
                    currentTOD = TimeOfDay.Evening;
                    break;
                case TimeOfDay.Evening:
                    currentTOD = TimeOfDay.Night;
                    break;
            }
        }
    }
}
