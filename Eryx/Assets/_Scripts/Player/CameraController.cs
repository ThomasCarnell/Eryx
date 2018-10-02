using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour {

    public float shakeAmountMultiplier = 1f;

    public Vector2 aspectRatioMinMax;

    CinemachineVirtualCamera playerCam;
    CinemachineBasicMultiChannelPerlin playerCamNoise;

    // Use this for initialization
    void Start () {
        playerCam = GetComponent<CinemachineVirtualCamera>();
        playerCamNoise = playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
	}

    public void CameraEditsSpeed(float speedRatio){
        playerCamNoise.m_AmplitudeGain = speedRatio * shakeAmountMultiplier;

        playerCam.m_Lens.FieldOfView = Mathf.Lerp(aspectRatioMinMax.x, aspectRatioMinMax.y, speedRatio);
    }

}
