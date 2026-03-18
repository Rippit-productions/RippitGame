using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;
using System.Linq;
using System;

/*
 * Grind Rail
 * Grind rail for Skate Character
 * Uses Unity Spline Component / Package
 */

public struct GrailPoint
{
    public Vector3 Position;
    public Vector3 Tangent;
    public float NormalisedPosition;
}

public class GrindRailPoint
{
    public GrindRailPoint() {}

    public GrindRailPoint(GrindRail grindRail, Spline railSpline, float splineNPosition)
    {
        GrindRail = grindRail;
        RailSpline = railSpline;
        SplineNPosition = splineNPosition;
    }

    // Is Spline Point is valid
    public bool OnSpline
    {
        get
        {
            if(SplineNPosition < 0.0f || SplineNPosition > 1.0f)
            {
                return false;
            }
                return true; 
            
        }
    }

    public Vector3 GetWorldPosition()
    {
        Vector3 locaPos = RailSpline.EvaluatePosition(SplineNPosition);
        return locaPos + GrindRail.transform.position; // Convert to world space and return
    }

    public Vector3 GetForwardVector()
    {
        Vector3 fVector = RailSpline.EvaluateTangent(SplineNPosition);
        fVector = fVector.normalized;

        // Second Case. Linear spline
        // For lnear splines, tangent could be zero. In this case take splineKnot's forward vector
        if (fVector.magnitude <= 0.0f)
        {
            var knotIndex = (int)RailSpline.ConvertIndexUnit(
                SplineNPosition, PathIndexUnit.Normalized,
                PathIndexUnit.Knot);

            var splineKnot = RailSpline[knotIndex];
            fVector = (Quaternion)splineKnot.Rotation * Vector3.forward;
        }
        return fVector;
    }

    public Vector3 GetUpVector()
    {
        Vector3 tangent = ((Vector3)RailSpline.EvaluateTangent(
            SplineNPosition)).normalized;

        Vector3 splineNVector = Vector3.Cross(tangent, Vector3.back);

        return splineNVector;
    }

    public static GrindRailPoint operator + (GrindRailPoint point, float value)
    {
        point.SplineNPosition += value / point.RailSpline.GetLength();
        return point;
    }

    public static GrindRailPoint operator -(GrindRailPoint point, float value)
    {
        point.SplineNPosition -= value / point.RailSpline.GetLength();
        return point;
    }

    public GrindRail GrindRail;
    public Spline RailSpline;
    public float SplineNPosition;

    public static implicit operator bool(GrindRailPoint rialPoint)
    {
        if (rialPoint == null)
        {
            return false;
        }
        return true;
    }
}

[ExecuteAlways]
[RequireComponent(typeof(SplineContainer))]
public class GrindRail : MonoBehaviour
{
    public SplineContainer _SplineComponent;
    public Sprite PointSprite;
    public Color Colour = Color.black;
    public Material Material;
    public float Width = 0.2f;
    public int Resolution = 128;
    public Spline[] Splines => GetSplines();
    private void Awake()
    {
        _SplineComponent = GetComponent<SplineContainer>();
    }



    public Spline[] GetSplines() => _SplineComponent.Splines.ToArray();

    public static GrindRailPoint[] GetAllGrindRails()
    {
        List<GrindRailPoint> RailList = new List<GrindRailPoint>();
        foreach (var component in FindObjectsByType<GrindRail>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            foreach (Spline spline in component.GetSplines())
            {
                RailList.Add(
                    new GrindRailPoint(component, spline, 0.0f)
                    );
            }
        }
        return RailList.ToArray();
    }

    public static (bool Success, GrindRailPoint ResultRailPoint) FindClosestRailToPoint(Vector3 TargetWorldPosition)
    {
        var All = GetAllGrindRails();
        if (All.Length == 0)
        {
            return (false, new GrindRailPoint());
        }
        else
        {
            // Sorting Lamba Func
            // Get distance to spline
            Func< GrindRailPoint , float> SortFunc =
            (
                GrindRailPoint railPoint 
            ) =>
            {
                var localPos = TargetWorldPosition - railPoint.GrindRail.transform.position;
                float3 SamplePoint;
                float SampleNPoint;
                SplineUtility.GetNearestPoint(
                    railPoint.RailSpline,
                    localPos,
                    out SamplePoint,
                    out SampleNPoint,
                    16
                    );
                Vector3 WorldSamplePoint = (Vector3)SamplePoint + railPoint.GrindRail.transform.position;
                railPoint.SplineNPosition = SampleNPoint;
                return (WorldSamplePoint - TargetWorldPosition).magnitude;
            };

            All = All.OrderBy(
                SortFunc
                ).ToArray();

            return (true, All.First());
        }
    }
    
    public static Vector3 GetSplineForwardVector(Spline spline, float normalisedPosition)
    {
        Vector3 fVector = spline.EvaluateTangent(normalisedPosition);
        fVector = fVector.normalized;

        // Second Case. Linear spline
        // For lnear splines, tangent could be zero. In this case take splineKnot's forward vector
        if (fVector.magnitude <= 0.0f)
        {
            var knotIndex = (int)spline.ConvertIndexUnit(
                normalisedPosition, PathIndexUnit.Normalized,
                PathIndexUnit.Knot);

            var splineKnot = spline[knotIndex];
            fVector =  (Quaternion)splineKnot.Rotation * Vector3.forward;
        }
        return fVector;
    }


}
