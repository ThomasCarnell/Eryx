using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour {

    int numberOfPassedTriggerGates;

    TriggerGate[] triggerGates;

    public bool debug;
    public bool overwriteTriggerGates;

    void Start () {
        triggerGates = FindObjectsOfType<TriggerGate>();

        ResetTriggerGates();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (numberOfPassedTriggerGates >= triggerGates.Length || overwriteTriggerGates)
            {
                GameManager.instance.PlayerCompletedLap();
                ResetTriggerGates();
                if (debug)
                {
                    print("Lap Completed");
                }
            }else
            {
                if (debug)
                {
                    int triggerGatesNeeded = triggerGates.Length - numberOfPassedTriggerGates;
                    print("Lap not Completed, need " + triggerGatesNeeded + " trigger gates to win");
                }
            }


        }
    }

    public void TriggerGatePassed(){
        numberOfPassedTriggerGates++;

        if (debug)
        {
            int triggerGatesNeeded = triggerGates.Length - numberOfPassedTriggerGates;
            print("Trigger Gate Passed, " + triggerGatesNeeded +" left to go!");
        }
    }

    public void ResetTriggerGates(){


        numberOfPassedTriggerGates = 0;

        foreach (var triggerGate in triggerGates)
        {
            triggerGate.didTrigger = false;
        }

        if (debug)
        {
            print("Resetting Triggers");
        }
    }
}
