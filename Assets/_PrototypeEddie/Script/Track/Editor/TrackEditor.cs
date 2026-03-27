using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


[CustomEditor(typeof(Track))]
public class TrackEditor : Editor
{

    public enum TrackTool
    {
        None,
        Transform,
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
        return root;
    }
    private void OnSceneGUI()
    {
        var inputEvent = Event.current;
        if (inputEvent.type == EventType.MouseDown && inputEvent.button == 0)
        {
            Debug.Log("Hello");
        }


        if (UnityEditor.Tools.current != Tool.None )
        {
            CurrentTool = TrackTool.None;
            selectIndex = -1;
        }

        var Ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        var something = HandleUtility.RaySnap(Ray);
        if (something != null) 
        {
            Debug.Log("Oi");
            Handles.color = Color.red;
            Handles.DrawWireCube(((RaycastHit)something).point, Vector3.one * 5.0f);
        }
        

        for (int i = 0; i < _Component.CheckPoints.Length; i++)
        {
            var handlePos = _Component.GetPointWorldPosition(i);
            Vector3 PointSize = _Component.CheckPoints[i].CollisionBoxSize;

            Gizmos.color = Color.black;
            string LabelName = $"CheckPoint:{i}";

            // Select Button Mode
            if (i != selectIndex)
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
                        UnityEditor.Tools.current = Tool.None;
                        selectIndex = i;
                    }
            }
            // Drag and Scale Mode
            else
            {
                Handles.color = Color.white;

                _Component.CheckPoints[i].LocalPosition = Handles.FreeMoveHandle(
                    handlePos,
                    HandleUtility.GetHandleSize(handlePos) * 0.5f,
                    Vector3.one * 2.0f,
                    Handles.RectangleHandleCap
                    ) - _Component.transform.position;


                _Component.CheckPoints[i].CollisionBoxSize = Handles.ScaleHandle(
                    PointSize,
                    handlePos,
                    Quaternion.identity
                    );
            }
            
        }
        
    }

}
