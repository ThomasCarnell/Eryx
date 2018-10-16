using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName ="Character Profile")]
public class CharacterProfile : ScriptableObject {

    [BoxGroup("Engine Setup")]
    public AnimationCurve speedCurve;
    [BoxGroup("Engine Setup")]
    public float topSpeed;
    [BoxGroup("Engine Setup")]
    public float accelerationTime;

    [Button("Update Acceleration Time & Top Speed")]
    void UpdateSpeedCurve(){
        accelerationTime= speedCurve.keys[speedCurve.length-1].time;
        topSpeed = speedCurve.keys[speedCurve.length-1].value;
    }


    [BoxGroup("Control Setup")]
    public float speedToStartDrifting;
    [BoxGroup("Control Setup")]
    public float driftAmount;
    [BoxGroup("Control Setup")]
    [Range(0.01f, 1)] public float turnForceTimer = .5f;
}
