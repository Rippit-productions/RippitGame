using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;


public class Track : MonoBehaviour
{
    public int Laps = 1;
    public bool IsClosed; // Track Looping
    public TrackCheckPoint[] CheckPoints = {new TrackCheckPoint()};

    public int Count => CheckPoints.Length;

    public Vector3 GetCheckPointPosition(int CheckPointIndex)
    {
        return CheckPoints[CheckPointIndex].LocalPosition + transform.position;
    }

    public bool PointOverlapsCheckPoint(Vector3 Position,int CheckPointIndex)
    {
        Bounds CheckPointBounds = new Bounds(
            GetCheckPointPosition(CheckPointIndex),
            CheckPoints[CheckPointIndex].CollisionBoxSize
            );

        return CheckPointBounds.Contains(Position);
    }

    public Spline GetTrackSpline()
    {
        List<BezierKnot> Knots = new List<BezierKnot>();
        for (int i = 0; i < CheckPoints.Length; i++) 
        {
            var position = CheckPoints[i].LocalPosition;
            var newKnot = new BezierKnot((float3)position);
            Knots.Add(newKnot);
        }
        return new Spline(Knots.ToArray(), false);
    }

    public (Vector3 Position,float NPosition) GetPointOnSpline(Vector3 Position)
    {
        var localPosition = Position - transform.position;

        float3 resultPos;
        float resultNPos;
        SplineUtility.GetNearestPoint(
            GetTrackSpline(),
            Position,
            out resultPos,
            out resultNPos
            );

        return (resultPos, resultNPos);
    }

    

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        for (int i = 0; i < CheckPoints.Length; i++)
        {
            //CheckPoint
            Gizmos.color = Color.yellow;
            if (i == 0) Gizmos.color = Color.green;
            else if (i == this.Count - 1 && !IsClosed) Gizmos.color = Color.red;

            var handleSize = HandleUtility.GetHandleSize(
                GetCheckPointPosition(i));

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(
                GetCheckPointPosition(i),
                handleSize * 0.10f
                );

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(
                GetCheckPointPosition(i),
                (Vector3)CheckPoints[i].CollisionBoxSize 
                );
        }
    }
#endif
}
