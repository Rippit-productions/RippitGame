using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEditor.Splines;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

[CustomEditor(typeof(GrindRail))]
public class GrindRailEditor : Editor
{
    private GrindRail _Component;

    private void OnEnable()
    {
        _Component = (GrindRail)target;
        EditorSplineUtility.AfterSplineWasModified += OnSplineChange;
    }

    private void OnDisable()
    {
        EditorSplineUtility.AfterSplineWasModified -= OnSplineChange;
    }


    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        root.Add(base.CreateInspectorGUI());

        return base.CreateInspectorGUI();
    }

    private SelectableKnot[] GetSelectedKnots()
    {
        var container = _Component.GetComponent<SplineContainer>();
        List<SplineInfo> info = new List<SplineInfo>();

        for (int i = 0; i < container.Splines.Count; i++)
        {
            info.Add(new SplineInfo(container, i));
        }

        List<SelectableKnot> Knots = new List<SelectableKnot>();
        SplineSelection.GetElements<SelectableKnot>(info.ToArray(), Knots);

        return Knots.ToArray();
    }

    private void OnSplineChange(Spline spline)
    {
        if (_Component == null) return;
        Refresh();
    }

    void Refresh()
    {


        // Set Spline Objects
        var splineCount = _Component.GetSplines().Length;
        for (int  i = 0; i < splineCount; i++)
        {
            var SplineObjName = $"Spline: {i}";

            if (i >= _Component.transform.childCount)
            {
                var newObj = new GameObject($"Spline:{i}");
                newObj.AddComponent<SpriteRenderer>();
                newObj.transform.SetParent(_Component.transform);
            }
        }

        // Delete Existing 
        for (int i = 0; i < _Component.transform.childCount; i++)
        {
            if (i >= splineCount)
            {
                GameObject.DestroyImmediate(_Component.transform.GetChild(i).gameObject); 
            }
        }


        // Set Knot Objects
        for (int i = 0; i < _Component.transform.childCount; i++)
        {
            var parentObject = _Component.transform.GetChild(i);
            var targetSpline = _Component.GetSplines().ElementAt(i);

            for (int knotI = 0; knotI < targetSpline.Count; knotI++)
            {
                var knot = targetSpline.ElementAt(knotI);
                var knotNPosition = SplineUtility.ConvertIndexUnit(targetSpline, knotI, PathIndexUnit.Knot, PathIndexUnit.Normalized);
                Vector3 knotTangent = SplineUtility.EvaluateTangent(targetSpline, knotNPosition);
                Vector3 knotUpVector = Vector3.Cross(knotTangent, Vector3.back);

                if (knotI >= parentObject.transform.childCount)
                {
                    var newChildObj = new GameObject($"Knot Sprite{knotI}");
                    newChildObj.transform.SetParent(parentObject.transform);
                    newChildObj.AddComponent<SpriteRenderer>();
                }

                var knotObject = parentObject.transform.GetChild(knotI);
                knotObject.transform.localPosition = knot.Position;


                knotObject.transform.rotation = Quaternion.LookRotation(
                    Vector3.forward,knotUpVector
                    );
            }
        }


    }

}
