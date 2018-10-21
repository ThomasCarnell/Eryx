using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BoostPadController : MonoBehaviour {

    public float boostMax;
    public float boostUpTime;
    public float boostdownTime;

    VehicleMovement vehicleMovement;

    [BoxGroup("Placement Helper")]
    public LayerMask whatIsGround;
    [BoxGroup("Placement Helper")]
    public Vector3 offset;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        vehicleMovement = player.GetComponent<VehicleMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            vehicleMovement.VehicleBoost(boostMax,boostUpTime,boostdownTime);
        }
    }

    [Button("Place on ground")]
    public void PlaceOnGround(){

        RaycastHit hit;

        if (Physics.Raycast(transform.position,-transform.up,out hit,10, whatIsGround))
        {

            transform.up = hit.normal;
            transform.position = hit.point+ offset;
        }

    }
}
