using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour {

    //enum PlayerState { onNormalTrack, offTrack };

    //static PlayerState playerState;

    public static int playerState;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NormalTrack"))
        {
            playerState = 0;

            print("Player on track");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NormalTrack"))
        {
            playerState = 1;

            print("Player off track");
        }
    }
}
