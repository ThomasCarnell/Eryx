using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleLine))]
public class SimpleLineInspector : Editor
{
    Vector3 p0;
    Vector3 p1;

    private void OnSceneGUI()
    {
        SimpleLine line = target as SimpleLine;
        Handles.color = Color.green;
        switch (line.splineMode)
        {
            case SimpleLine.SplineMode.ChaikinsAlgorithm:
                for (int i = 1; i < line.trackPointsHolder.transform.childCount; i++)
                {
                    if (i == 1)
                    {
                        //First Point
                        Vector3 P0 = line.trackPointsHolder.transform.GetChild(i - 1).transform.position;
                        Vector3 P1 = line.trackPointsHolder.transform.GetChild(i).transform.position;
                        Vector3 P2 = line.trackPointsHolder.transform.GetChild(i + 1).transform.position;
                        Vector3 Q1 = (P1 * 0.75f) + (P2 * 0.25f);

                        Handles.DrawLine(P0, Q1);
                    }
                    else if (i == line.trackPointsHolder.transform.childCount - 1)
                    {
                        //Last point
                        Vector3 P0 = line.trackPointsHolder.transform.GetChild(i - 1).transform.position;
                        Vector3 P1 = line.trackPointsHolder.transform.GetChild(i).transform.position;
                        Vector3 Q0 = (P0 * 0.75f) + (P1 * 0.25f);

                        Handles.DrawLine(Q0, P1);
                    }
                    else
                    {
                        Vector3 P0 = line.trackPointsHolder.transform.GetChild(i - 1).transform.position;
                        Vector3 P1 = line.trackPointsHolder.transform.GetChild(i).transform.position;
                        Vector3 P2 = line.trackPointsHolder.transform.GetChild(i + 1).transform.position;

                        Vector3 Q0 = (P0 * 0.75f) + (P1 * 0.25f);
                        Vector3 R0 = (P0 * 0.25f) + (P1 * 0.75f);
                        Vector3 Q1 = (P1 * 0.75f) + (P2 * 0.25f);

                        Handles.DrawLine(Q0, R0);
                        Handles.DrawLine(R0, Q1);
                    }
                }
                break;

            case SimpleLine.SplineMode.Linear:
                for (int i = 1; i < line.trackPointsHolder.transform.childCount; i++)
                {
                    Vector3 currentPoint = line.trackPointsHolder.transform.GetChild(i - 1).transform.position;
                    Vector3 nextPoint = line.trackPointsHolder.transform.GetChild(i).transform.position;

                    Handles.DrawLine(currentPoint, nextPoint);
                }
                break;

            default:
                break;

        }

        if (line.loop)
        {
            Vector3 firstPoint = line.trackPointsHolder.transform.GetChild(0).transform.position;
            Vector3 lastPoint = line.trackPointsHolder.transform.GetChild(line.trackPointsHolder.transform.childCount - 1).transform.position;

            Handles.DrawLine(firstPoint, lastPoint);
        }
    }

}