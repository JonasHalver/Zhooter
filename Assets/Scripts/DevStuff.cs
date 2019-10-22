using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevStuff : MonoBehaviour
{
    public Text state, cost;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        state.text = Manager.currentState.ToString();
        cost.text = "Cost = " + Manager.costOfCurrentBuild.ToString();
    }
}
