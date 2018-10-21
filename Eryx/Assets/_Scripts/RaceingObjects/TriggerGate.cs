using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGate : MonoBehaviour {

   public bool didTrigger;

    FinishLine finishLine;

    private void Start()
    {
        finishLine = FindObjectOfType<FinishLine>();
        didTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !didTrigger)
        {
            finishLine.TriggerGatePassed();
            didTrigger = true;
        }
    }
}
