using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName ="Character Profile")]
public class CharacterProfile : ScriptableObject {

    [BoxGroup("Engine Setup")]
    public AnimationCurve speedCurve;
    [BoxGroup("Engine Setup")]
    public float hoverHeight = 1.5f;
    [BoxGroup("Engine Setup")]
    public float hoverForce = 300f;

    [BoxGroup("Control Setup")]
    public float speedToStartDrifting;
    [BoxGroup("Control Setup")]
    public float driftAmount;
    [BoxGroup("Control Setup")]
    [Range(0.01f, 1)] public float turnForceTimer = .5f;
}
