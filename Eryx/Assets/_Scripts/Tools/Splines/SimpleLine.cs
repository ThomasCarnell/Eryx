using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SimpleLine : MonoBehaviour
{
    public GameObject trackPointsHolder;
    public bool loop;
    public enum SplineMode { Linear, ChaikinsAlgorithm }
    public SplineMode splineMode;


}
