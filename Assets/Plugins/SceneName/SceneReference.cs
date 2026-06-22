using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SceneReference
{
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif
    public string name;

    public override string ToString() => name;
    public static implicit operator string(SceneReference obj) => obj.ToString();

    public static implicit operator bool(SceneReference obj)
    {
        if (obj == null) return false;
        else
        {
            return obj.name == null;
        }
    }

}
