using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;


public struct AnimatorStateName
{
    public string LayerName;
    public string StateName;

    public string GetFullStateName()
    {
        return $"{LayerName}.{StateName}";
    }
}

[CreateAssetMenu(fileName = "AnimatorStateReference", menuName = "ScriptableObjects/Animator State Reference", order = 1)]
public class AnimatorStateReference : ScriptableObject
{
    public AnimatorController Controller;
    public AnimatorStateName StateName;

    public string GetStateName()
    {
        return StateName.GetFullStateName();
    }
}
