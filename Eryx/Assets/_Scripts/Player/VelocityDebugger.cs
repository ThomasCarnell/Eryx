#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityDebugger : MonoBehaviour
{

    public bool showVehicleVelocity;

    Rigidbody vehicleRigid;

    Vector3 vehicleVelocity;

    LineDrawer lineDrawer;
    // Use this for initialization
    void Start()
    {
        if (showVehicleVelocity)
        {
            lineDrawer = new LineDrawer();
            vehicleRigid = GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        if (showVehicleVelocity)
        {
            vehicleVelocity = vehicleRigid.position + vehicleRigid.velocity.normalized * 10;
            lineDrawer.DrawLineInGameView(vehicleRigid.worldCenterOfMass, vehicleVelocity, Color.red);
        }
    }

}
#endif