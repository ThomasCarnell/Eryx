using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPadController : MonoBehaviour {

    public float healingAmountPerUpdate;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.HealPlayer(healingAmountPerUpdate);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerIsGettingHealed = false;
        }
    }

}
