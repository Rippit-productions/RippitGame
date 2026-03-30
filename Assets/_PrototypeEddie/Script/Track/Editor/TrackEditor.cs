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
    }

    private void OnDisable()
    {
        selectIndex = -1;
    }

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        root.Add(_InspectorGUI.CloneTree());

        Button MoveToolButton = (Button)root.Q("MoveButton");
        Button ColliderToolButton = (Button)root.Q("ColliderButon");
        Button AddToolButton = (Button)root.Q("AddButton");

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
            this.CurrentTool = TrackTool.Add;
            this.selectIndex = -1;
            UnityEditor.Tools.current = Tool.Custom;
            SceneView.RepaintAll();
        };

        return root;
    }


    private void OnSceneGUI()
    {
        if (Tools.current != Tool.Custom) return;

        var currentEvent = Event.current;
        var MouseRay  = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector3 MouseWorldPos = MouseRay.GetPoint(-MouseRay.origin.z);

        DrawPathSpline();

        for (int i = 0; i < _Component.CheckPoints.Length; i++)
        {
            var handlePos = _Component.GetPointWorldPosition(i);
            Vector3 PointSize = _Component.CheckPoints[i].CollisionBoxSize;

            Gizmos.color = Color.black;
            string LabelName = $"CheckPoint:{i}";

            // Select Button Mode
            if (i != selectIndex && this.CurrentTool != TrackTool.Add)
            {
                Handles.color = Color.blue;
                var pressed = Handles.Button(
                    _Component.GetPointWorldPosition(i),
                    Quaternion.identity,
                    HandleUtility.GetHandleSize(handlePos) * 0.15f,
                    1.2f,
                    Handles.SphereHandleCap
                    );
                if (pressed)
                {
                    UnityEditor.Tools.current = Tool.Custom;
                    this.CurrentTool = TrackTool.Move;
                    selectIndex = i;
                }
            }
        }

        

        switch (this.CurrentTool)
        {
            case TrackTool.Move:
                if (selectIndex < 0) break;
                var currentlocalPos = _Component.CheckPoints[selectIndex].LocalPosition;
                Handles.color = Color.white;

                Vector3 WorldPosition = currentlocalPos + _Component.transform.position;
                Quaternion someQuat = Quaternion.identity;
                Handles.TransformHandle(ref WorldPosition, ref someQuat);

                _Component.CheckPoints[selectIndex].LocalPosition = WorldPosition - _Component.transform.position;
                break;

            case TrackTool.Collider:
                break;
            case TrackTool.Add:
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                Handles.color = Color.blue;
                Handles.DrawWireDisc(MouseWorldPos, Vector3.forward, HandleUtility.GetHandleSize(MouseWorldPos) * 0.05f);

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

        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Delete)
        {
            if (selectIndex > 0)
            {
                List<TrackCheckPoint> allPoints = new List<TrackCheckPoint>(_Component.CheckPoints);
                allPoints.RemoveAt(selectIndex);
                _Component.CheckPoints = allPoints.ToArray();
                GUIUtility.hotControl = 0;
                currentEvent.Use();
            }

            if (selectIndex > _Component.CheckPoints.Length - 1)
            {
                selectIndex = _Component.CheckPoints.Length - 1;
            }
        }

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
