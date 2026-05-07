using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;


namespace AnimationStateReference
{
    [Serializable]
    public struct AnimationStatePath{
        public string LayerName;
        public string StateName;

        public string Get()
        {
            return $"{LayerName}.{StateName}";
        }
    }

    [Serializable]
    public struct AnimatorStateReference
    {
        public AnimatorController Controller;
        public AnimationStatePath path;

        public string GetStatePath() => path.Get();

        public bool IsValid()
        {
            string LayerName = path.LayerName;
            string StateName = path.StateName;
            if (Controller == null) return false;
            else
            {
                var matchedLayer = Controller.layers.Where(layer =>
                {
                    return layer.name == LayerName;
                }).FirstOrDefault();

                if (matchedLayer != null)
                {
                    var states = matchedLayer.stateMachine.states;
                    bool stateMatch = states.Where(s =>
                    {
                        return s.state.name == StateName;
                    }).Any();

                    if (stateMatch) return true;
                }
            }
            return true;
        }
    }
}
