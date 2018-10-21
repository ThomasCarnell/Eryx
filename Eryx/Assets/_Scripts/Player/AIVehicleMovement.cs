//This script handles all of the physics behaviors for the player's ship. The primary functions
//are handling the hovering and thrust calculations. 

using UnityEngine;
using UnityEditor;
using NaughtyAttributes;


public class AIVehicleMovement : MonoBehaviour
{
    public float speed;						//The current forward speed of the ship

    [BoxGroup("Character Profile")]
    [Expandable]
    public CharacterProfile selectedCharacter; //Skal nok være en gamecontroller der håndtere denne.

    [BoxGroup("AI Settings")]
    public float rubberbandFactor = .99f;
    [BoxGroup("AI Settings")]
    public float brakingVelFactor = .95f;   //The percentage of velocty the ship maintains when braking
    [Tooltip("x = amount, y = min and max angle")]
    [BoxGroup("AI Settings")]
    public Vector2 angleOfRoll;			//The angle that the ship "banks" into a turn
    [BoxGroup("AI Settings")]
    public float centerLineSeekForce;
    [BoxGroup("AI Settings")]
    public float raycastRadius;
    [BoxGroup("AI Settings")]
    public float raycastDistance;
    [BoxGroup("AI Settings")]
    public LayerMask whatIsWall;

    [BoxGroup("Hover Settings")]
    public float maxGroundDist = 5f;        //The distance the ship can be above the ground before it is "falling"
    [BoxGroup("Hover Settings")]
    public LayerMask whatIsGround;			//A layer mask to determine what layer the ground is on
    public PIDController hoverPID;			//A PID controller to smooth the ship's hovering

    [BoxGroup("Physics Settings")]
    public Transform shipBody;				//A reference to the ship's body, this is for cosmetics
    [BoxGroup("Physics Settings")]
    public float terminalVelocity = 100f;   //The max speed the ship can go
    [BoxGroup("Physics Settings")]
    public float hoverGravity = 20f;        //The gravity applied to the ship while it is on the ground
    [BoxGroup("Physics Settings")]
    public float fallGravity = 80f;         //The gravity applied to the ship while it is falling

    [Header("References")]

    Rigidbody vehicleRigidBody;                    //A reference to the ship's rigidbody
    bool isOnGround;                        //A flag determining if the ship is currently on the ground


    [Header("Debug Stuff")]
    //Debug
    public bool debugOn;
    LineDrawer raycastDrawerRight;
    LineDrawer raycastDrawerLeft;


    //float characterTopSpeed;
    //float characterAccelerationTime;

    CenterLineRailHelper centerLineHelper;
    float seekForceTimer;

    void Start()
    {
        //Get references to the Rigidbody and PlayerInput components
        vehicleRigidBody = GetComponent<Rigidbody>();

        //Calculate the ship's drag value
        //drag = driveForce / terminalVelocity;

        //characterTopSpeed = selectedCharacter.speedCurve.keys[selectedCharacter.speedCurve.length-1].value;
        //characterAccelerationTime = selectedCharacter.speedCurve.keys[selectedCharacter.speedCurve.length - 1].time;

        //CenterLine Helper
        centerLineHelper = FindObjectOfType<CenterLineRailHelper>();


#if UNITY_EDITOR //Debug setup
        if (debugOn)
        {
        raycastDrawerRight = new LineDrawer();
            raycastDrawerLeft = new LineDrawer();
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

        CheckForWalls();
    }

    void CheckForWalls()
    {

        RaycastHit hit;

        Ray wallCheckRayLeft = new Ray(vehicleRigidBody.position, transform.forward - transform.right);
        Ray wallCheckRayRight = new Ray(vehicleRigidBody.position, (transform.forward + transform.right));

        if (Physics.SphereCast(wallCheckRayLeft, raycastRadius, out hit, raycastDistance, whatIsWall))
        {
            float seekAmount = Mathf.Clamp01(raycastDistance - hit.distance);
            centerLineHelper.SeekCenterLine(seekAmount * centerLineSeekForce);

            if (debugOn){
                raycastDrawerRight.DrawLineInGameView(wallCheckRayRight.origin, vehicleRigidBody.position + (wallCheckRayRight.direction * raycastDistance), Color.green);
                raycastDrawerLeft.DrawLineInGameView(wallCheckRayLeft.origin, vehicleRigidBody.position + (wallCheckRayLeft.direction * raycastDistance), Color.red);
            }

        }
        else if (Physics.SphereCast(wallCheckRayRight, raycastRadius, out hit, raycastDistance, whatIsWall))
        {
            float seekAmount = Mathf.Clamp01(raycastDistance - hit.distance);
            centerLineHelper.SeekCenterLine(seekAmount * centerLineSeekForce);

            if (debugOn){
                raycastDrawerLeft.DrawLineInGameView(wallCheckRayLeft.origin, vehicleRigidBody.position + (wallCheckRayLeft.direction * raycastDistance), Color.green);
                raycastDrawerRight.DrawLineInGameView(wallCheckRayRight.origin, vehicleRigidBody.position + (wallCheckRayRight.direction * raycastDistance), Color.red);
            }


        }
        else if (debugOn)
        {
            raycastDrawerLeft.DrawLineInGameView(wallCheckRayLeft.origin, vehicleRigidBody.position + (wallCheckRayLeft.direction * raycastDistance), Color.green);
            raycastDrawerRight.DrawLineInGameView(wallCheckRayRight.origin, vehicleRigidBody.position + (wallCheckRayRight.direction * raycastDistance), Color.green);
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
        isOnGround = Physics.Raycast(ray, out hitInfo, maxGroundDist, whatIsGround);

        //If the ship is on the ground...
        if (isOnGround)
        {
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
            Vector3 gravity = -groundNormal * hoverGravity * height;

            //...and finally apply the hover and gravity forces
            vehicleRigidBody.AddForce(force, ForceMode.Acceleration);
            vehicleRigidBody.AddForce(gravity, ForceMode.Acceleration);
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

        #region Drifting setup

        //Calculate the current sideways speed by using the dot product. This tells us
        //how much of the ship's velocity is in the "right" or "left" direction
        float sidewaysSpeed = Vector3.Dot(vehicleRigidBody.velocity, transform.right);

        //Calculate the desired amount of friction to apply to the side of the vehicle. This
        //is what keeps the ship from drifting into the walls during turns. If you want to add
        //drifting to the game, divide Time.fixedDeltaTime by some amount
        Vector3 sideFriction;

        sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime);

        //Finally, apply the sideways friction
        vehicleRigidBody.AddForce(sideFriction, ForceMode.Acceleration);

        #endregion

        //Make the ship bank.
        CalculateBanking(sidewaysSpeed);

        //Braking or driving requires being on the ground, so if the ship
        //isn't on the ground, exit this method
        if (!isOnGround)
            return;

        #region Vehicle Populsion calculations

        //Calculate and apply the amount of propulsion force by multiplying the drive force
        //by the amount of applied thruster and subtracting the drag amount
        //float propulsion = driveForce * input.thruster - drag * Mathf.Clamp(speed, 0f, terminalVelocity); //Den game version

        float currentVelocity = vehicleRigidBody.velocity.magnitude;

        float velocityCurveTime = GetCurveTimeForValue(selectedCharacter.speedCurve, currentVelocity, Mathf.RoundToInt(selectedCharacter.speedCurve[selectedCharacter.speedCurve.length - 1].value));

        float nextVelocity = selectedCharacter.speedCurve.Evaluate(velocityCurveTime + Time.fixedDeltaTime);

        float deltaVelocity = nextVelocity - currentVelocity;

        //Always drive
        vehicleRigidBody.AddForce(transform.forward * deltaVelocity, ForceMode.VelocityChange);

#endregion
    }

    void CalculateBanking(float angleAmount){

        //Calculate the angle we want the ship's body to bank into a turn based on the current rudder.
        //It is worth noting that these next few steps are completetly optional and are cosmetic.
        //It just feels so darn cool

        float angle = Mathf.Clamp (angleAmount* angleOfRoll.x, -angleOfRoll.y, angleOfRoll.y);

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
        }
    }

    public float GetSpeedPercentage()
    {
        //Returns the total percentage of speed the ship is traveling
        return vehicleRigidBody.velocity.magnitude / terminalVelocity;
    }

//#if UNITY_EDITOR
//    void OnDrawGizmos()
//    {
//        Gizmos.color = Color.cyan;
//        Gizmos.DrawRay(wallHit, pushAwayFromWall);
//        Gizmos.DrawSphere(wallHit, 1f);
//        Gizmos.color = Color.red;
//        Gizmos.DrawLine(wallHit, endPoint);


//        print(endPoint);
//    }
//#endif
}
