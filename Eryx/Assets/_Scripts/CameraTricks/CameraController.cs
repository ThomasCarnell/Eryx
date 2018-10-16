using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

public class CameraController : MonoBehaviour {

    public bool useShakeWhenGoingFast;
    [ShowIf("useShakeWhenGoingFast")] public float shakeAmountMultiplier = 1f;

    public bool changeAspectratioWhenGoingFast;
    [ShowIf("changeAspectratioWhenGoingFast")]public Vector2 aspectRatioMinMax;

    CinemachineVirtualCamera playerCam;
    CinemachineBasicMultiChannelPerlin playerCamNoise;

    // Use this for initialization
    void Start () {
        playerCam = GetComponent<CinemachineVirtualCamera>();
        playerCamNoise = playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
	}

    public void CameraEditsSpeed(float speedRatio){
        if (useShakeWhenGoingFast)
        {
            playerCamNoise.m_AmplitudeGain = speedRatio * shakeAmountMultiplier;
        }

        if (changeAspectratioWhenGoingFast)
        {
            playerCam.m_Lens.FieldOfView = Mathf.Lerp(aspectRatioMinMax.x, aspectRatioMinMax.y, speedRatio);
        }
    }

}
