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
    public string sceneName;

    public override string ToString() => sceneName;
    public static implicit operator string(SceneReference obj) => obj.ToString();

    public static implicit operator bool(SceneReference obj)
    {
        if (obj == null) return false;
        else
        {
            return obj.sceneName == null;
        }
    }

}
