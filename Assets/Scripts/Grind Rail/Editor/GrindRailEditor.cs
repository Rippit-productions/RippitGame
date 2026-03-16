using UnityEditor;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

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
        var _splineComponent = _Component.GetComponent<SplineContainer>();
        EditorSplineUtility.AfterSplineWasModified -= OnSplineChange;
    }

    private void OnSceneGUI()
    {
    }

    private void Awake()
    {
        
    }
    private void OnSplineChange(Spline spline)
    {
        Refresh();
    }

    void Refresh()
    {

        while (_Component.transform.childCount > 0)
        {
            foreach (Transform child in _Component.transform)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
        }

        foreach (var spline in _Component.GetSplines())
        {
            foreach (var knot in spline)
            {
                var newSpriteObj = new GameObject("Sprite Obj");
                var Spriterender = newSpriteObj.AddComponent<SpriteRenderer>();
                Spriterender.sortingOrder = -10;
                Spriterender.sprite = _Component.PointSprite;
                newSpriteObj.transform.SetParent(_Component.transform);
                newSpriteObj.transform.localPosition = (Vector3)knot.Position;
            }
        }
    }
}
