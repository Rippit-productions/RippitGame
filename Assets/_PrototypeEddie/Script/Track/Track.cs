using UnityEditor;
using UnityEngine;

public class Track : MonoBehaviour
{
    public bool IsClosed; // Track Looping
    public TrackCheckPoint[] CheckPoints = {new TrackCheckPoint()};

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
        }
    }
#endif
}
