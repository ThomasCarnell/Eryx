using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterLineRailHelper : MonoBehaviour
{

    public bool showDebug;

    public Transform whoToHelp;
    Rigidbody helpingRigid;

    void Start()
    {
        whoToHelp = GameObject.FindWithTag("Player").transform;
        helpingRigid = whoToHelp.gameObject.GetComponent<Rigidbody>();
    }

    public void SeekCenterLine(float force){

        Vector3 heading = transform.position - whoToHelp.position;
        
        Quaternion lookRotation = Quaternion.LookRotation(heading, whoToHelp.up);
        Quaternion deltaRotation = Quaternion.Slerp(helpingRigid.rotation, lookRotation, force * Time.fixedDeltaTime);
        
        helpingRigid.MoveRotation(deltaRotation);

    }
}
