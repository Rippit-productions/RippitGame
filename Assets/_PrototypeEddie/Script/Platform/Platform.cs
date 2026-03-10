using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(CompositeCollider2D))]
public class Platform : MonoBehaviour
{
    private CompositeCollider2D _CompositeCollider = null;
    private List<Spline> Splines = new List<Spline>();
    private Vector2[] _LocalPathPoints = null;
    private void _InitSplines()
    {
        for (int pathIndex = 0; pathIndex < _CompositeCollider.pathCount; pathIndex++)
        {
            this.Splines.Add(new Spline());
            var pointCount = _CompositeCollider.GetPathPointCount(pathIndex);
            _LocalPathPoints = new Vector2[pointCount];
            _CompositeCollider.GetPath(pathIndex, _LocalPathPoints);
            for (int pointIndex = 0; pointIndex < pointCount; pointIndex++)
            {
                var pos = (Vector3)_LocalPathPoints[pointIndex];
                BezierKnot point = new BezierKnot(pos);
                this.Splines[pathIndex].Add(point, TangentMode.Linear);
            }
        }
    }

    void Start()
    {
        _CompositeCollider = GetComponent<CompositeCollider2D>();
        _InitSplines();
    }

    public Vector2 ClosestPoint(Vector3 worldPosition)
    {
        return _CompositeCollider.attachedRigidbody.ClosestPoint(worldPosition);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
    }
#endif
}
