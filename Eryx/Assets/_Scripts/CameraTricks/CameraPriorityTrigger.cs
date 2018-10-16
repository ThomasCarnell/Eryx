using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraPriorityTrigger : MonoBehaviour {

    public CinemachineVirtualCamera cinemachineCamera;

    public int ontriggerEnterPriority;
    public int ontriggerExitPriority;

    // Use this for initialization
    void Start () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            cinemachineCamera.Priority = ontriggerEnterPriority;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            cinemachineCamera.Priority = ontriggerExitPriority;
    }

}
