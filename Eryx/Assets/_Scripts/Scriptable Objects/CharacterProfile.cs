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
    public float speedToStartDrifting = 100f;
    [BoxGroup("Control Setup")]
    public float driftAmount = 5f;
    [BoxGroup("Control Setup")]
    [Range(0.01f, 1)] public float turnForceTimer = .5f;

    [BoxGroup("Ship Setup")]
    public float maxHealth = 100f;
    [BoxGroup("Ship Setup")]
    public Vector2 minMaxDamageTaken;
    [BoxGroup("Ship Setup")]
    public GameObject shipModel;
}
