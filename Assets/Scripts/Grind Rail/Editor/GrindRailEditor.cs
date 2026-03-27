using NAudio.CoreAudioApi;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Splines;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

[CustomEditor(typeof(GrindRail))]
public class GrindRailEditor : Editor
{
    public VisualTreeAsset InspectorGUI;

    private GrindRail _Component;
    private static bool _AutoRefresh = true;

    public static string EDITORPREF_AutoRefresh = "GrindRail_AutoRefresh";

    private void OnEnable()
    {
        _Component = (GrindRail)target;
        EditorSplineUtility.AfterSplineWasModified += OnSplineChange;
    }

    [MenuItem("GameObject/Rails/Add New")]
    public static void CreateNewGrindRail()
    {
        var newObj = new GameObject("Grind Rail");
        newObj.AddComponent<GrindRail>();
        if (Selection.activeGameObject != null)
        {
            newObj.transform.SetParent(Selection.activeGameObject.transform);
        }

        Selection.activeGameObject = newObj;
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        root.Add(InspectorGUI.CloneTree());

        FloatField WidthField = (FloatField)root.Q("WidthField");
        WidthField.value = _Component.Width;
        WidthField.RegisterValueChangedCallback((eventInfo) =>
        {
            _Component.Width = (float)eventInfo.newValue;
            float minWidth = 0.05f;
            if (_Component.Width <= minWidth) _Component.Width = minWidth;
            Refresh();
        });

        IntegerField ResolutionField = (IntegerField)root.Q("ResolutionField");
        ResolutionField.value = _Component.Resolution;
        ResolutionField.RegisterValueChangedCallback((eventInfo) =>
        {
            _Component.Resolution = eventInfo.newValue;
            int minResolution = 2;
            if (_Component.Resolution <= minResolution) _Component.Resolution = minResolution;
            Refresh();
        });

        ColorField ColourField = (ColorField)root.Q("ColourField");
        ColourField.value = _Component.Colour;
        ColourField.RegisterValueChangedCallback((eventInfo) =>
        {
            _Component.Colour = (Color)eventInfo.newValue;
            Refresh();
        });

        ObjectField SpriteField = (ObjectField)root.Q("SpriteField");
        SpriteField.value = _Component.PointSprite;
        SpriteField.RegisterValueChangedCallback((eventInfo) =>
        {
            if (_Component)
            {
                _Component.PointSprite = (Sprite)eventInfo.newValue;
                Refresh();
            }
        });

        ObjectField MaterialField = (ObjectField)root.Q("MaterialField");
        MaterialField.value = _Component.Material;
        MaterialField.RegisterValueChangedCallback((eventInfo) =>
        {
            _Component.Material = (Material)eventInfo.newValue;
            Refresh();
        });


        LayerMaskField layerMaskField = (LayerMaskField)root.Q("GroundLayerMask");
        layerMaskField.RegisterValueChangedCallback((eventInfo) =>
        {
            Refresh();
        });

        Toggle AutoRefreshToggle = (Toggle)root.Q("AutoRefresh");
        AutoRefreshToggle.value = EditorPrefs.GetBool(EDITORPREF_AutoRefresh,true);
        AutoRefreshToggle.RegisterValueChangedCallback((eventInfo) =>
        {
            _AutoRefresh = eventInfo.newValue;
            EditorPrefs.SetBool(EDITORPREF_AutoRefresh, eventInfo.newValue);
        });

        Button GenerateButton = (Button)root.Q("GenerateButton");
        GenerateButton.clicked += () =>
        {
            Refresh();
        };

        return root;
    }

    private void OnSplineChange(Spline spline)
    {
        if (_Component == null) return;
        else if (_AutoRefresh) Refresh();
    }

    private void OnSceneGUI()
    {
        if (_Component.PointSprite == null) return;
        foreach(var spline in _Component.GetSplines())
        {

            for (int knotIndex = 0; knotIndex < spline.Count; knotIndex++)
            {
                var knot = spline[knotIndex];
                var normalPosition = SplineUtility.ConvertIndexUnit(
                    spline, knotIndex, 
                    PathIndexUnit.Knot, PathIndexUnit.Normalized
                    );

                Vector3 knotTangent = SplineUtility.EvaluateTangent(spline, normalPosition);

                Vector3 knotUpVector = Vector3.Cross(knotTangent, Vector3.back).normalized;

                Handles.color = Color.red;
                Vector3 LineFrom = (Vector3)knot.Position + _Component.transform.position;
                Handles.DrawLine(LineFrom , LineFrom - (knotUpVector * _Component.PointSprite.bounds.extents.y * 1.25f));
            }
        }
    }

    private void Refresh()
    {
        /*
         * Wish I could do this by foreach loop
         * But it always leave left over objects.... :(
         */
        while (_Component.transform.childCount > 0)
        {
            GameObject.DestroyImmediate(_Component.transform.GetChild(0).gameObject);
        }

        foreach (var spline in _Component.GetSplines())
        {
            var newLineRenderObject = new GameObject("GrindRail LineRenderer");
            newLineRenderObject.transform.SetParent(_Component.transform);
            newLineRenderObject.transform.localPosition = Vector3.back * 0.1f;
            
            var LineRenderComponent = newLineRenderObject.AddComponent<LineRenderer>();
            LineRenderComponent.startWidth = _Component.Width;
            LineRenderComponent.endWidth = _Component.Width;
            LineRenderComponent.material = _Component.Material;
            LineRenderComponent.startColor = _Component.Colour;
            LineRenderComponent.endColor = _Component.Colour;


            int segments = _Component.Resolution;
            List<Vector3> Points = new List<Vector3>();
            for (int i = 0; i <= segments; i++)
            {
                Vector3 pointPosition = SplineUtility.EvaluatePosition(spline, (float)i/segments);
                Points.Add(pointPosition);
            }
            LineRenderComponent.positionCount = Points.Count;
            LineRenderComponent.SetPositions(Points.ToArray());

            for (int knotIndex = 0 ;knotIndex < spline.Count; knotIndex++)
            {
                var knot = spline[knotIndex];
                float knotNPosition = SplineUtility.ConvertIndexUnit(spline, knotIndex, PathIndexUnit.Knot, PathIndexUnit.Normalized);
                Vector3 knotTangent = SplineUtility.EvaluateTangent(spline, knotNPosition);
                Vector3 knotUpVector = Vector3.Cross(knotTangent , Vector3.back).normalized;
                Vector3 knotWorldPosition = _Component.transform.position + (Vector3)knot.Position;

                var newSpriteObj = new GameObject("GrindRail Sprite Pole");
                
                var spriteRenderer = newSpriteObj.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = _Component.PointSprite;
                spriteRenderer.sortingOrder = -10;
                newSpriteObj.transform.SetParent(_Component.transform);
                newSpriteObj.transform.localPosition = knot.Position;
                newSpriteObj.transform.rotation = Quaternion.LookRotation( Vector3.forward, knotUpVector);
                ContactFilter2D filter = new ContactFilter2D();
                filter.useLayerMask = false;
                filter.useTriggers = false;
                filter.useDepth = false;

                RaycastHit2D[] hits = new RaycastHit2D[1]; 
                var raycast = Physics2D.Raycast(knotWorldPosition, -knotUpVector, filter,hits, _Component.PointSprite.bounds.size.y * 1.2f);
                spriteRenderer.color = Color.clear;
                if (raycast != 0)
                {
                    spriteRenderer.color = Color.white;
                }

            }
        }
    }

}
