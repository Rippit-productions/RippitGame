using UnityEditor;
using UnityEngine;

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
