using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float bankingSpeed;
    public float movementSpeed;
    Rigidbody playerRigid;


    void Start () {
        playerRigid = GetComponent<Rigidbody>();
	}

    private void Update()
    {

        /*

        Vector3 dir = Vector3.zero;

        // we assume that the device is held parallel to the ground
        // and the Home button is in the right hand

        // remap the device acceleration axis to game coordinates:
        // 1) XY plane of the device is mapped onto XZ plane
        // 2) rotated 90 degrees around Y axis
        dir.x = Input.acceleration.x;
        //dir.z = Input.acceleration.x;

        //print("X: " + Input.acceleration.x + " Y: " + Input.acceleration.y + " Z: " + Input.acceleration.z);

        // clamp acceleration vector to the unit sphere
        if (dir.sqrMagnitude > 1)
            dir.Normalize();

        // Make it move 10 meters per second instead of 10 meters per frame...
        dir *= Time.deltaTime;


        Vector3 movement = new Vector3 (dir.x * bankingSpeed,0,movementSpeed*Time.deltaTime);

        // Move object
        transform.Translate(movement);

        //Debug.Log("Direction "+ dir);*/
    }

    void FixedUpdate () {

        Vector3 dir = Vector3.zero;
        dir.x = Input.acceleration.x;

        if (dir.sqrMagnitude > 1)
            dir.Normalize();

        dir *= Time.deltaTime;

        playerRigid.AddTorque(0,dir.x*bankingSpeed,0,ForceMode.);

        playerRigid.AddRelativeForce(new Vector3(0, 0, movementSpeed));

    }
}
