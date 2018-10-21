using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Course Setup")]
public class CourseSetup : ScriptableObject {

    public int lapsNeeded;
    public float timeToBeat;

}
