using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SceneName
{
    public string AssetGUID;
    public string name;

    public override string ToString()
    {
        return name;
    }

#if UNITY_EDITOR
    public bool IsValid()
    {
        return AssetDatabase.GUIDToAssetPath(AssetGUID) != null;
    }
#endif
}
