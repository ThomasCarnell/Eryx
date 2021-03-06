﻿//This script handles all of the physics behaviors for the player's ship. The primary functions
//are handling the hovering and thrust calculations. 

using UnityEngine;
using UnityEditor;
using NaughtyAttributes;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;

public class VehicleMovement : MonoBehaviour
{
    public float speed;						//The current forward speed of the ship

    CharacterProfile selectedCharacter;

    [BoxGroup("Driver Settings")]
    public DriftingState driftingState;
    [BoxGroup("Driver Settings")]
    public float slowingVelFactor = .99f;   //The percentage of velocity the ship maintains when not thrusting (e.g., a value of .99 means the ship loses 1% velocity when not thrusting)
    [BoxGroup("Driver Settings")]
    public float brakingVelFactor = .95f;   //The percentage of velocty the ship maintains when braking
    [Tooltip("x = amount, y = min and max angle")]
    [BoxGroup("Driver Settings")]
    public Vector2 angleOfRoll;			//The angle that the ship "banks" into a turn

    [BoxGroup("Hover Settings")]
    public float checkForGroundDist = 5f;        //The distance the ship can be above the ground before it is "falling"
    [BoxGroup("Hover Settings")]
    public LayerMask whatIsGround;			//A layer mask to determine what layer the ground is on
    public PIDController hoverPID;			//A PID controller to smooth the ship's hovering

    [BoxGroup("Physics Settings")]
    public Transform shipBody;				//A reference to the ship's body, this is for cosmetics
    [BoxGroup("Physics Settings")]
    public float terminalVelocity = 100f;   //The max speed the ship can go
    [BoxGroup("Physics Settings")]
    public float fallGravity = 80f;         //The gravity applied to the ship while it is falling

    [Header("References")]

    //Camera tricks
    [Range(0, 1)] public float cameraShakeSpeedRatio;
    float cameraShakeMinSpeed;
    float cameraShakeRemainSpeed;
    public CameraController camController;

    //EngineStuff
    float driftAmount;
    public enum DriftingState { NoDrifting, IsDrifting, StoppingDrifting, OnIce }
    float driftTimer;


    Rigidbody vehicleRigidBody;                    //A reference to the ship's rigidbody
    PlayerInput input;                      //A reference to the player's input                 
    float drag;                             //The air resistance the ship recieves in the forward direction
    bool isOnGround;                        //A flag determining if the ship is currently on the ground

    public PostProcessVolume boostPPFX;

    [Header("Debug Stuff")]
    //Debug
    public bool debugOn;
    LineDrawer lineDrawer;


    float characterTopSpeed;
    //float characterAccelerationTime; // måske en brugbar ting på et tidspunkt.


    float boost;
    float boostScaler;
    Tween boostTween;
    bool playerIsBoosting;

    void Start()
    {
        //Get references to the Rigidbody and PlayerInput components
        vehicleRigidBody = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();

        //Calculate the ship's drag value
        //drag = driveForce / terminalVelocity;

        selectedCharacter = GameManager.instance.selectedCharacter;

        characterTopSpeed = selectedCharacter.speedCurve.keys[selectedCharacter.speedCurve.length - 1].value;

        //characterAccelerationTime = selectedCharacter.speedCurve.keys[selectedCharacter.speedCurve.length - 1].time; //Måske en brugbar ting på et tidspunkt

        //Set the speed when the camera should start shaking.
        cameraShakeMinSpeed = characterTopSpeed * cameraShakeSpeedRatio;
        cameraShakeRemainSpeed = characterTopSpeed - cameraShakeMinSpeed;

        //Dont drift at start;
        driftingState = DriftingState.NoDrifting;

        boostTween = DOTween.To(() => boost, x => boost = x, 0, .5f); //Just to make sure it's not null.

#if UNITY_EDITOR //Debug setup
        if (debugOn)
        {
            lineDrawer = new LineDrawer();
        }

#endif
    }

    void FixedUpdate()
    {
        //Calculate the current speed by using the dot product. This tells us
        //how much of the ship's velocity is in the "forward" direction 
        speed = Vector3.Dot(vehicleRigidBody.velocity, transform.forward);

        //Calculate the forces to be applied to the ship
        CalculatHover();
        CalculatePropulsion();


        if (input.playerIsBoosting && GameManager.instance.playerBoostIsAvailable && !playerIsBoosting && GameManager.instance.IsActiveGame() && GameManager.instance.currentHealth > 1)
        {
            PlayerBoost();
        }


        //Boost Setup
        if (boostScaler > 0.01f)
        {
            vehicleRigidBody.AddForce(transform.forward * boost * boostScaler, ForceMode.Impulse);
        }
    }

    private void Update()
    {

        //Sending speed value out to get the cam to shake
        if (speed > cameraShakeMinSpeed)
        {
            camController.CameraEditsSpeed((speed - cameraShakeMinSpeed) / cameraShakeRemainSpeed);
        }

        //Boost PPFX Setup
        if (boostScaler > 0.01f)
        {
            boostPPFX.weight = boostScaler;
        }
        else
        {
            boostPPFX.weight = 0f;
        }
    }

    void CalculatHover()
    {
        //This variable will hold the "normal" of the ground. Think of it as a line
        //the points "up" from the surface of the ground
        Vector3 groundNormal;

        //Calculate a ray that points straight down from the ship
        Ray ray = new Ray(transform.position, -transform.up);

        //Declare a variable that will hold the result of a raycast
        RaycastHit hitInfo;

        //Determine if the ship is on the ground by Raycasting down and seeing if it hits 
        //any collider on the whatIsGround layer
        isOnGround = Physics.Raycast(ray, out hitInfo, checkForGroundDist, whatIsGround);

        //If the ship is on the ground...
        if (isOnGround)
        {
            //Wait for introsequence to switch hover on

            //...determine how high off the ground it is...
            float height = hitInfo.distance;
            //...save the normal of the ground...
            groundNormal = hitInfo.normal.normalized;
            //...use the PID controller to determine the amount of hover force needed...
            float forcePercent = hoverPID.Seek(selectedCharacter.hoverHeight, height);

            //...calulcate the total amount of hover force based on normal (or "up") of the ground...
            Vector3 force = groundNormal * selectedCharacter.hoverForce * forcePercent;
            //...calculate the force and direction of gravity to adhere the ship to the 
            //track (which is not always straight down in the world)...
            //Vector3 gravity = -groundNormal * hoverGravity * height;

            //...and finally apply the hover and gravity forces
            vehicleRigidBody.AddForce(force, ForceMode.Acceleration);
            //vehicleRigidBody.AddForce(gravity, ForceMode.Acceleration);
        }
        //...Otherwise...
        else
        {
            //...use Up to represent the "ground normal". This will cause our ship to
            //self-right itself in a case where it flips over
            groundNormal = Vector3.up;

            //Calculate and apply the stronger falling gravity straight down on the ship
            Vector3 gravity = -groundNormal * fallGravity;
            vehicleRigidBody.AddForce(gravity, ForceMode.Acceleration);
        }

        //Calculate the amount of pitch and roll the ship needs to match its orientation
        //with that of the ground. This is done by creating a projection and then calculating
        //the rotation needed to face that projection
        Vector3 projection = Vector3.ProjectOnPlane(transform.forward, groundNormal);
        Quaternion rotation = Quaternion.LookRotation(projection, groundNormal);

        //Move the ship over time to match the desired rotation to match the ground. This is 
        //done smoothly (using Lerp) to make it feel more realistic
        vehicleRigidBody.MoveRotation(Quaternion.Lerp(vehicleRigidBody.rotation, rotation, Time.deltaTime * 10f));

    }

    void CalculatePropulsion()
    {
        //Calculate the yaw torque based on the rudder and current angular velocity - Den der angular velocity laver noget fucked når man er på hovedet
        float rotationTorque = input.rudder * selectedCharacter.turnForceTimer /*- rigidBody.angularVelocity.y*/;
        //Apply the torque to the ship's Y axis
        vehicleRigidBody.AddRelativeTorque(0f, rotationTorque, 0f, ForceMode.VelocityChange);

        //add force to the side when turning


        #region Drifting setup

        //Calculate the current sideways speed by using the dot product. This tells us
        //how much of the ship's velocity is in the "right" or "left" direction
        float sidewaysSpeed = Vector3.Dot(vehicleRigidBody.velocity, transform.right);

        //Calculate the desired amount of friction to apply to the side of the vehicle. This
        //is what keeps the ship from drifting into the walls during turns. If you want to add
        //drifting to the game, divide Time.fixedDeltaTime by some amount
        Vector3 sideFriction;

        switch (driftingState)
        {
            case DriftingState.NoDrifting:

                driftAmount = 0;
                driftTimer = 1;

                // if speed is enought to start drifting, drift.
                if (speed > selectedCharacter.speedToStartDrifting)
                {
                    driftingState = DriftingState.IsDrifting;
                }
                break;

            case DriftingState.IsDrifting:

                driftAmount = selectedCharacter.driftAmount;
                driftTimer = 1;

                // If speed is under amount to drift, stop
                if (speed < selectedCharacter.speedToStartDrifting)
                {
                    driftingState = DriftingState.StoppingDrifting;
                }

                break;

            case DriftingState.StoppingDrifting:

                driftTimer = Mathf.Clamp01(driftTimer - Time.fixedDeltaTime * .5f);

                driftAmount = selectedCharacter.driftAmount * driftTimer;

                //If driftTimer ends stop drifting
                if (driftTimer <= 0.1f)
                {
                    driftingState = DriftingState.NoDrifting;
                }

                // if speed gets above drifting needed, start drifting again.
                if (speed > selectedCharacter.speedToStartDrifting)
                {
                    driftingState = DriftingState.IsDrifting;
                }

                break;

            case DriftingState.OnIce:

                break;

            default:
                break;
        }

        if (driftAmount > 0)
        {
            sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime / driftAmount);
        }
        else
        {
            sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime);
        }

        //Finally, apply the sideways friction
        vehicleRigidBody.AddForce(sideFriction, ForceMode.Acceleration);

        #endregion

        //Make the ship bank.
        CalculateBanking(sidewaysSpeed);

        //If not propelling the ship, slow the ships velocity
        if (input.thruster <= 0f)
            vehicleRigidBody.velocity *= slowingVelFactor;

        //Braking or driving requires being on the ground, so if the ship
        //isn't on the ground, exit this method
        if (!isOnGround)
            return;

        //If the ship is braking, apply the braking velocty reduction
        if (input.isBraking)
            vehicleRigidBody.velocity *= brakingVelFactor;

        #region Vehicle Populsion calculations

        //Calculate and apply the amount of propulsion force by multiplying the drive force
        //by the amount of applied thruster and subtracting the drag amount
        //float propulsion = driveForce * input.thruster - drag * Mathf.Clamp(speed, 0f, terminalVelocity); //Den game version

        float currentVelocity = vehicleRigidBody.velocity.magnitude;

        float velocityCurveTime = GetCurveTimeForValue(selectedCharacter.speedCurve, currentVelocity, Mathf.RoundToInt(selectedCharacter.speedCurve[selectedCharacter.speedCurve.length - 1].value));

        float nextVelocity = selectedCharacter.speedCurve.Evaluate(velocityCurveTime + Time.fixedDeltaTime);

        float deltaVelocity = nextVelocity - currentVelocity;

        //Add propulsion
        vehicleRigidBody.AddForce(transform.forward * deltaVelocity * input.thruster * (1-boostScaler), ForceMode.VelocityChange);

        #endregion
    }

    void CalculateBanking(float angleAmount)
    {

        //Calculate the angle we want the ship's body to bank into a turn based on the current rudder.
        //It is worth noting that these next few steps are completetly optional and are cosmetic.
        //It just feels so darn cool

        float angle = Mathf.Clamp(angleAmount * angleOfRoll.x, -angleOfRoll.y, angleOfRoll.y);

        //Calculate the rotation needed for this new angle
        Quaternion bodyRotation = transform.rotation * Quaternion.Euler(0, 0f, angle); //SKAL LAVES OM HVIS MODELLEN IKKE LIGGER NED!
                                                                                       //Finally, apply this angle to the ship's body
        shipBody.rotation = Quaternion.Lerp(shipBody.rotation, bodyRotation, Time.deltaTime * 10f);

    }

    public float GetCurveTimeForValue(AnimationCurve curveToCheck, float value, int accuracy)
    {

        float startTime = curveToCheck.keys[0].time;
        float endTime = curveToCheck.keys[curveToCheck.length - 1].time;
        float nearestTime = startTime;
        float step = endTime - startTime;

        for (int i = 0; i < accuracy; i++)
        {

            float valueAtNearestTime = curveToCheck.Evaluate(nearestTime);
            float distanceToValueAtNearestTime = Mathf.Abs(value - valueAtNearestTime);

            float timeToCompare = nearestTime + step;
            float valueAtTimeToCompare = curveToCheck.Evaluate(timeToCompare);
            float distanceToValueAtTimeToCompare = Mathf.Abs(value - valueAtTimeToCompare);

            if (distanceToValueAtTimeToCompare < distanceToValueAtNearestTime)
            {
                nearestTime = timeToCompare;
                valueAtNearestTime = valueAtTimeToCompare;
            }
            step = Mathf.Abs(step * 0.5f) * Mathf.Sign(value - valueAtNearestTime);
        }

        return Mathf.Clamp(nearestTime, 0, endTime);
    }

    void OnCollisionStay(Collision collision)
    {
        //If the ship has collided with an object on the Wall layer...
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            //...calculate how much upward impulse is generated and then push the vehicle down by that amount 
            //to keep it stuck on the track (instead up popping up over the wall)
            Vector3 upwardForceFromCollision = Vector3.Dot(collision.impulse, transform.up) * transform.up;
            vehicleRigidBody.AddForce(-upwardForceFromCollision, ForceMode.Impulse);

            vehicleRigidBody.AddRelativeForce(input.rudder * selectedCharacter.turnForceTimer * 20, 0, 0, ForceMode.VelocityChange);

            float collisionForce = collision.relativeVelocity.sqrMagnitude*0.00005f;

            if (collisionForce>GameManager.instance.selectedCharacter.minMaxDamageTaken.x)
            {
                GameManager.instance.DamagePlayer(Mathf.Clamp(collisionForce,GameManager.instance.selectedCharacter.minMaxDamageTaken.x,GameManager.instance.selectedCharacter.minMaxDamageTaken.y));
            }


            if (debugOn)
            {
                //Make the player bounce off the wall
                //Calculate the reflection angle
                //Vector3 pushAwayFromWall = Vector3.Reflect(transform.forward, collision.contacts[0].normal);

                Vector3 wallHit = collision.contacts[0].point;
                Vector3 endPoint = wallHit + (collision.impulse.normalized * collision.impulse.magnitude);

                //Debug to see it.
                lineDrawer.DrawLineInGameView(wallHit, endPoint, Color.cyan);
            }
        }
    }


    public float GetSpeedPercentage()
    {
        //Returns the total percentage of speed the ship is traveling
        return vehicleRigidBody.velocity.magnitude / terminalVelocity;
    }


    public void PlayerBoost()
    {
        playerIsBoosting = true;
        boost = selectedCharacter.playerBoostAmount;

        if (boostTween != null && boostTween.IsPlaying())
            boostTween.Pause();

        boostTween = DOTween.To(() => boostScaler, x => boostScaler = x, 1, selectedCharacter.boostUpDownTime.x)
                            .OnUpdate(PlayerDamageBoost)
                            .OnComplete(() => PlayerBoostFinished(selectedCharacter.boostUpDownTime.y));

    }

    public void PlayerBoostFinished(float boostDownTime)
    {

        playerIsBoosting = false;

        boostTween = DOTween.To(() => boostScaler, x => boostScaler = x, 0, boostDownTime)
                            .OnUpdate(PlayerDamageBoost);
    }

    void PlayerDamageBoost()
    {
        if (GameManager.instance.currentHealth > 1)
        {
            GameManager.instance.DamagePlayer(selectedCharacter.boostDamagePerUpdate * boostScaler);
        } else
        {
            print("Player is too fragile to boost");
        }
    }


    public void VehicleBoostPad(float maxBoost, float boostUpTime, float boostDownTime)
    {
        boost = maxBoost;
        playerIsBoosting = true;

        if (boostTween != null && boostTween.IsPlaying())
            boostTween.Pause();

        boostTween = DOTween.To(() => boostScaler, x => boostScaler = x, 1, boostUpTime)
               .OnComplete(() => VehicleBoostFinished(boostDownTime));
    }

    public void VehicleBoostFinished(float boostDownTime)
    {

        playerIsBoosting = false;

        boostTween = DOTween.To(() => boostScaler, x => boostScaler = x, 0, boostDownTime)
                            .OnUpdate(()=>print("Downtime"));
    }

}
