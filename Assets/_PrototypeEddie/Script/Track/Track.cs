using UnityEditor;
using UnityEngine;

public class Track : MonoBehaviour
{
    public bool IsClosed; // Track Looping
    [SerializeField] public TrackCheckPoint[] CheckPoints = {new TrackCheckPoint()};

    public int Count => CheckPoints.Length;

    public Vector3 GetPointWorldPosition(int CheckPointIndex)
    {
        return CheckPoints[CheckPointIndex].LocalPosition + transform.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        for (int i = 0; i < CheckPoints.Length; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(
                GetPointWorldPosition(i),
                CheckPoints[i].CollisionBoxSize
                );

            int nextIndex = i + 1;
            if (nextIndex == CheckPoints.Length) nextIndex = 0;


            //CheckPoint
            Gizmos.color = Color.yellow;
            if (i == 0) Gizmos.color = Color.green;
            else if (i == this.Count - 1 && !IsClosed) Gizmos.color = Color.red;

            var handleSize = HandleUtility.GetHandleSize(
                GetPointWorldPosition(i));
            Gizmos.DrawWireSphere(
                GetPointWorldPosition(i),
                handleSize * 0.25f
                );

            //Collision Box
            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(
                GetPointWorldPosition(i),
                CheckPoints[i].CollisionBoxSize
                );

            Gizmos.color = Color.white;
            if (!IsClosed && nextIndex == 0) continue;
            Gizmos.DrawLine(
                GetPointWorldPosition(i),
                GetPointWorldPosition(nextIndex)
                );
        }
    }
#endif
}
