using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class PlatformGravity : MonoBehaviour
{
    public enum GravityType
    {
        Direction,
        ToPoint
    }

    public GravityType Type;
    [field:SerializeField]protected Vector2 _GravityVector;
    [field:SerializeField]protected Vector2 _GravityPoint;

    public Vector3  GravityVector => _GravityVector;
    public Vector3 GravityPoint
    {
        get
        {
            return transform.rotation * _GravityPoint;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlatformGravity))]
    public class PlaformGravityEditor : Editor
    {
        private void OnSceneGUI()
        {
            PlatformGravity Component = Selection.activeGameObject.GetComponent<PlatformGravity>();
            
            switch (Component.Type)
            {
                case GravityType.ToPoint:
                    Handles.color = Color.blue;
                    Handles.DrawWireCube(
                        Component.transform.position + 
                        (Vector3)Component.GravityPoint,
                        Vector3.one * 0.1f * DistanceToSelection()
                        );
                    break;
                case GravityType.Direction:
                    Handles.color = Color.blue;

                    Handles.DrawDottedLine(
                        Component.transform.position,
                        Component.transform.position + (Vector3)Component._GravityVector * DistanceToSelection(),
                        0.5f * DistanceToSelection()
                        );
                    break;
                default:
                    break;
            }

        }

        private float DistanceToSelection()
        {
            if (SceneView.currentDrawingSceneView && Selection.activeGameObject)
            {
                Camera myCam = SceneView.currentDrawingSceneView.camera;

                return (myCam.transform.position - Selection.activeGameObject.transform.position).magnitude;
            }
            return 0.0f;
        }
    }


    
#endif
}


