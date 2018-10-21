using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SegmentTriggerSetup : MonoBehaviour {

    [BoxGroup("Trigger Setup")]
    public int triggerSegmentDistance;

    [BoxGroup("Trigger Setup")]
    [MinValue(0)]
    [ValidateInput("IsSmallerThanDistance", "Segment Offset must be smaller than Segment Distance")]
    public int triggerSegmentOffset;

    [BoxGroup("Trigger Setup")]
    public GameObject triggerGate;
    [BoxGroup("Trigger Setup")]
    public Vector3 triggerSize;

    [BoxGroup("Waypoint Setup")]
    public Cinemachine.CinemachineSmoothPath smoothPath;
    Cinemachine.CinemachineSmoothPath.Waypoint[] waypoints;

    [BoxGroup("Waypoint Setup")]
    public int waypointSegmentDistance;

    [BoxGroup("Waypoint Setup")]
    [MinValue(0)]
    [ValidateInput("IsSmallerThanDistanceWaypoint", "Segment Offset must be smaller than Segment Distance")]
    public int waypointSegmentOffset;

    [BoxGroup("Waypoint Setup")]
    public List<Vector3> waypointsList;

    [Button("Setup Triggers")]
    public void SetupCenterLineTriggers(){

        //Delete all available triggers
        BoxCollider[] allTriggers = GetComponentsInChildren<BoxCollider>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).childCount>=0)
            {
                for (int o = 0; o < transform.GetChild(i).childCount; o++)
                {
                    DestroyImmediate(transform.GetChild(i).GetChild(o).gameObject);
                }
            }
        }

        if (allTriggers != null)
        {
            foreach (var trigger in allTriggers)
            {
                DestroyImmediate(trigger);
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            //TriggerSetup
            if (i % triggerSegmentDistance == triggerSegmentOffset)
            {
                Transform currentChild = transform.GetChild(i);

                GameObject currentTriggerGate = Instantiate(triggerGate,currentChild);
                BoxCollider currentTrigger = currentTriggerGate.GetComponent<BoxCollider>();

                currentTriggerGate.transform.localScale = triggerSize;
                currentTriggerGate.transform.localEulerAngles = new Vector3(0,0,-currentChild.localEulerAngles.z);
                currentTriggerGate.transform.localPosition = Vector3.zero;

                currentTrigger.isTrigger = true;

            }
        }

    }

    [Button("Delete All Triggers")]
    public void DeleteAllTriggers(){

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).childCount >= 0)
            {
                for (int o = 0; o < transform.GetChild(i).childCount; o++)
                {
                    DestroyImmediate(transform.GetChild(i).GetChild(o).gameObject);
                }
            }
        }

        waypoints = new Cinemachine.CinemachineSmoothPath.Waypoint[0];
        smoothPath.m_Waypoints = waypoints;
    }


    [Button("Setup Waypoints")]
    public void SetupWaypoints(){

        waypointsList.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {


            //TriggerSetup
            if (i % waypointSegmentDistance == waypointSegmentOffset)
            {
                Transform currentChild = transform.GetChild(i);
                waypointsList.Add(currentChild.position);
            }
        }

        waypoints = new Cinemachine.CinemachineSmoothPath.Waypoint[waypointsList.Count];

        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i].position = waypointsList[i];
        }

        smoothPath.m_Waypoints = waypoints;

    }

    [Button("Delete All Waypoints")]
    public void DeleteAllWaypoints()
    {
        waypointsList.Clear();
        waypoints = new Cinemachine.CinemachineSmoothPath.Waypoint[0];
        smoothPath.m_Waypoints = waypoints;
    }

    private bool IsSmallerThanDistance(int integer){
        return integer < triggerSegmentDistance;
    }

    private bool IsSmallerThanDistanceWaypoint(int integer)
    {
        return integer < waypointSegmentDistance;
    }


}
