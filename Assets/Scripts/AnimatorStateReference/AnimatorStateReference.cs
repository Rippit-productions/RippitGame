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

        public override string ToString()
        {
            return $"{LayerName}.{StateName}";
        }

        public static implicit operator string(AnimationStatePath obj)
        {
            return obj.ToString();
        }
    }

    [System.Serializable]
    public class AnimatorStateReference
    {
        public AnimatorStateReference(AnimatorController Controller, AnimationStatePath path)
        {
            this._Controller = Controller;
            this._path = path; 
        }

        public AnimatorController AnimController => _Controller;
        [SerializeField]private AnimatorController _Controller;
        [SerializeField]private AnimationStatePath _path;

        public string GetStatePath() => _path;

        public bool IsValid()
        {
            if (_Controller == null)
            {
                return false;
            }
            else
            {
                var matchedLayer = _Controller.layers.Where(layer =>
                {
                    return layer.name == this._path.LayerName;
                }).FirstOrDefault();

                if (matchedLayer != null)
                {
                    var states = matchedLayer.stateMachine.states;
                    bool stateMatch = states.Where(s =>
                    {
                        return s.state.name == this._path.StateName;
                    }).Any();

                    // Layer and State name match. Reference is valid
                    if (stateMatch) return true;
                }
            }
            // Layer name or State name don't match. Reference is invalid
            return true;
        }

        public override string ToString() => _path;

        public static implicit operator string(AnimatorStateReference obj) => obj.ToString();
        public static implicit operator bool(AnimatorStateReference obj) => obj != null;
    }
}
