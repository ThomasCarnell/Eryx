using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float bankingSpeed;
    public float movementSpeed;
    public float accel = 5f;
    public float maxSpeed;
    public float globalGravityAmount = -9.81f;
    public float magneticGravityAmount = -9.81f;
    Rigidbody playerRigid;

    int playerState;

    void Start () {
        playerRigid = GetComponent<Rigidbody>();
	}

    void Update () {

        if (Input.GetKey(KeyCode.W))
        {
            //Kør fremaf når den er under max speed
            if (playerRigid.velocity.magnitude < maxSpeed)
            {
                playerRigid.AddForce(transform.forward * accel, ForceMode.Impulse);
            }

        }else // Brems
        {
            if (playerRigid.velocity.magnitude > 0f)
            {
                playerRigid.velocity *= 0.95f;
            }
        }

        switch (GroundChecker.playerState)
        {
            case 0: //Player on track

                //Lock player to road surface, unless velocity gets too high.

                

                break;

            case 1: //Player off track - Flight movement
               
                //Turn player around if upside-down and apply global gravity

                

                break;

            default:
                break;
        }

        //Vector3 dir = Vector3.zero;
        //dir.x = Input.acceleration.x;

        //if (dir.sqrMagnitude > 1)
        //    dir.Normalize();

        //dir *= Time.deltaTime;

        //playerRigid.AddTorque(0,dir.x*bankingSpeed,0,ForceMode.Force);

        //playerRigid.AddRelativeForce(new Vector3(0, 0, movementSpeed));

    }

    void ApplyGravity(Vector3 dir){

    }
}   
