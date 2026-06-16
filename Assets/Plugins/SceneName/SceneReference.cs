using Codice.Utils;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SceneReference
{
    public string AssetGUID;
    public string name;

    public override string ToString() => name;

    

#if UNITY_EDITOR
    public bool IsValid()
    {
        return AssetDatabase.GUIDToAssetPath(AssetGUID) != null;
    }
#endif

}
