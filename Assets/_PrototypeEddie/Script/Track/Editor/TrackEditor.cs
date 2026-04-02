using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Track))]
public class TrackEditor : Editor
{
    public enum TrackTool
    {
        Move,
        Collider,
        Add
    }

    public VisualTreeAsset _InspectorGUI;

    private Track _Component;
    public int selectIndex = 0;

    public TrackTool CurrentTool;
    private void OnEnable()
    {
        _Component = (Track)target;
        UnityEditor.Tools.current = Tool.Custom;
    }

    private void OnDisable()
    {
        selectIndex = -1;
    }

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        root.Add(_InspectorGUI.CloneTree());


        SliderInt LapSlider = (SliderInt)root.Q("LapsSlider");

        Button MoveToolButton = (Button)root.Q("MoveButton");
        Button ColliderToolButton = (Button)root.Q("ColliderButon");
        Button AddToolButton = (Button)root.Q("AddButton");


        LapSlider.RegisterValueChangedCallback(e =>
        {
            if (e.newValue >= 1)
            {
                _Component.Laps = e.newValue;
            }
        });


        MoveToolButton.clicked += () =>
        {
            this.CurrentTool = TrackTool.Move;
            UnityEditor.Tools.current = Tool.Custom;
            SceneView.RepaintAll();
        };

        ColliderToolButton.clicked += () =>
        {
            this.CurrentTool = TrackTool.Collider;
            UnityEditor.Tools.current = Tool.Custom;
            SceneView.RepaintAll();
        };

        AddToolButton.clicked += () =>
        {
            this.selectIndex = -1;
            this.CurrentTool = TrackTool.Add;
            UnityEditor.Tools.current = Tool.Custom;
            SceneView.RepaintAll();
        };
        return root;
    }


    private void OnSceneGUI()
    {
        var currentEvent = Event.current;
        var MouseRay  = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector3 MouseWorldPos = MouseRay.GetPoint(-MouseRay.origin.z);

        DrawPathSpline();

        _HandleSelection();

        switch (this.CurrentTool)
        {
            case TrackTool.Move:
                if (selectIndex < 0) break;
                var currentlocalPos = _Component.CheckPoints[selectIndex].LocalPosition;
                Handles.color = Color.white;

                Vector3 WorldPosition = currentlocalPos + _Component.transform.position;
                Quaternion rotation = Quaternion.identity; // Only to fill below method. 
                Handles.TransformHandle(ref WorldPosition, ref rotation);

                _Component.CheckPoints[selectIndex].LocalPosition = WorldPosition - _Component.transform.position;
                break;

            case TrackTool.Collider:
                if (selectIndex < 0) break;
                var selection = _Component.CheckPoints[selectIndex];
                var worldPos = _Component.GetCheckPointPosition(selectIndex);

                float buttonSize = 0.15f;
                Handles.color = Color.green;
                var xHandle = Handles.FreeMoveHandle(
                    _Component.GetCheckPointPosition(selectIndex) +  Vector3.right * selection.CollisionBoxSize.x * 0.5f,
                    HandleUtility.GetHandleSize(worldPos) * buttonSize,
                    Vector3.one * 0.0f,
                    Handles.CubeHandleCap
                    );

                var yHandle = Handles.FreeMoveHandle(
                    _Component.GetCheckPointPosition(selectIndex) + Vector3.up * selection.CollisionBoxSize.y * 0.5f,
                    HandleUtility.GetHandleSize(worldPos) * buttonSize,
                    Vector3.one * 0.0f,
                    Handles.CubeHandleCap
                    );

                var newXValue = (xHandle.x - worldPos.x) * 2.0f;
                if (newXValue > 0.0f)
                {
                    _Component.CheckPoints[selectIndex].CollisionBoxSize.x = newXValue;
                }

                var newYValue = (yHandle.y - worldPos.y) * 2.0f;
                if (newYValue > 0.0f)
                {
                    _Component.CheckPoints[selectIndex].CollisionBoxSize.y = newYValue;
                }

                break;
            case TrackTool.Add:
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                var handleScale = HandleUtility.GetHandleSize(MouseWorldPos);
                Handles.color = Color.blue;
                Handles.DrawWireDisc(MouseWorldPos, Vector3.forward, handleScale * 0.2f);

                Handles.color = Color.white;
                Handles.DrawWireDisc(MouseWorldPos, Vector3.forward, handleScale * 0.05f);

                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                {
                    List<TrackCheckPoint> allPoints = new List<TrackCheckPoint>(_Component.CheckPoints);

                    var newPoint = new TrackCheckPoint();
                    newPoint.LocalPosition = MouseWorldPos - _Component.transform.position;
                    allPoints.Add(newPoint);
                    _Component.CheckPoints = allPoints.ToArray();
                }

                SceneView.RepaintAll();
                break;
            default:
                break;
        }

        _HandleEscape();
        _HandleDelete();
    }

    private void _HandleSelection()
    {
        for (int i = 0; i < _Component.CheckPoints.Length; i++)
        {
            var handlePos = _Component.GetCheckPointPosition(i);
            Vector3 PointSize = _Component.CheckPoints[i].CollisionBoxSize;

            Gizmos.color = Color.black;
            string LabelName = $"CheckPoint:{i}";

            // Select Button Mode
            if (i != selectIndex && this.CurrentTool != TrackTool.Add)
            {
                Handles.color = Color.blue;
                var pressed = Handles.Button(
                    _Component.GetCheckPointPosition(i),
                    Quaternion.identity,
                    HandleUtility.GetHandleSize(handlePos) * 0.15f,
                    1.2f,
                    Handles.SphereHandleCap
                    );
                if (pressed)
                {
                    this.CurrentTool = TrackTool.Move;
                    selectIndex = i;
                }
            }
        }
    }
    
    private void _HandleDelete()
    {
        var currentEvent = Event.current;
        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Delete)
        {
            // Check if element is selected
            if (selectIndex > 0)
            {
                List<TrackCheckPoint> allPoints = new List<TrackCheckPoint>(_Component.CheckPoints);
                allPoints.RemoveAt(selectIndex);
                _Component.CheckPoints = allPoints.ToArray();
                GUIUtility.hotControl = 0;
                
            }

            // Keep selection within Array.
            if (selectIndex > _Component.CheckPoints.Length - 1)
            {
                selectIndex = _Component.CheckPoints.Length - 1;
            }

            currentEvent.Use();
        }
    }

    private void _HandleEscape()
    {
        var currentEvent = Event.current;
        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Escape)
        {
            this.selectIndex = -1;
        }
        currentEvent.Use();
    }

    private void DrawPathSpline()
    {
        List<Vector3> DrawPoints = new List<Vector3>();
        foreach(var checkPoint in _Component.CheckPoints)
        {
            DrawPoints.Add(
                checkPoint.LocalPosition + _Component.transform.position
                );
        }
        Handles.color = Color.white;
        Handles.DrawPolyLine(DrawPoints.ToArray());
    }
}
