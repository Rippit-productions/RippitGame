using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;


public class Track : MonoBehaviour
{
    public int Laps = 1;
    public bool IsClosed; // Track Looping
    public TrackCheckPoint[] CheckPoints = {new TrackCheckPoint(Vector3.zero)};

    public int CheckPointCount => CheckPoints.Length;

    public TrackCheckPoint this[int index]
    {
        get
        {
            return CheckPoints[index];
        }
    }

    public Vector3 GetCheckPointPosition(int CheckPointIndex)
    {
        return CheckPoints[CheckPointIndex].LocalPosition + transform.position;
    }

    public Vector3 GetRespawnPosition(int CheckPointIndex)
    {
        return GetCheckPointPosition(CheckPointIndex) + CheckPoints[CheckPointIndex].LocalRespawnPos;
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

    /// <summary>
    /// Get closest point on Track's spline.
    /// </summary>
    public (Vector3 Position,float NPosition) GetPointOnTrack(Vector3 Position)
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
            else if (i == this.CheckPointCount - 1 && !IsClosed) Gizmos.color = Color.red;

            var handleSize = HandleUtility.GetHandleSize(
                GetCheckPointPosition(i));

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(
                GetCheckPointPosition(i),
                (Vector3)CheckPoints[i].CollisionBoxSize 
                );

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                GetRespawnPosition(i),
                Vector3.one * handleSize * 0.1f
                );
        }
    }
#endif
}
