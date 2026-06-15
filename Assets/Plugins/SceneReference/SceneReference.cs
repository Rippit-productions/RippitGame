using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SceneReference
{
#if UNITY_EDITOR
    public SceneAsset Asset;
#endif
    public string AssetGUID;
    public string SceneName;

    public override string ToString()
    {
        return SceneName;
    }
}
