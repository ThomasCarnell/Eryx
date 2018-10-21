using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine.Utility;
using Cinemachine;

public class RacingLineSeeker : MonoBehaviour {

    public bool showRacingLineDebug;

    public float seekerForce;
    public float distanceToCenterLine;
    public GameObject VisualPointOnRacingLine;
    GameObject visualPointIngame;

    //Rigidbody vehicleRigidbody;

    Vector3 closestPointOnRacingLine;

    private void Start()
    {
        //vehicleRigidbody = GetComponent<Rigidbody>();

#if UNITY_EDITOR
        if (showRacingLineDebug)
        {
            visualPointIngame = Instantiate(VisualPointOnRacingLine);
            visualPointIngame.SetActive(true);
        }
#endif
    }

    private void FixedUpdate()
    {
        closestPointOnRacingLine = FindClosestPointOnRacingLine();



#if UNITY_EDITOR
        if (showRacingLineDebug)
        {
            visualPointIngame.transform.position = closestPointOnRacingLine;
        }
#endif
    }

    void CalculateSeekerForce(){



    }

    Vector3 FindClosestPointOnRacingLine(){

        return Vector3.zero;
    }

}
