using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;


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
        public AnimatorStateReference(RuntimeAnimatorController Controller, AnimationStatePath path)
        {
            this._Controller = Controller;
            this._path = path; 
        }

        public RuntimeAnimatorController AnimController => _Controller;
        [SerializeField]private RuntimeAnimatorController _Controller;
        [SerializeField]private AnimationStatePath _path;

        public string GetStatePath() => _path;

        public override string ToString() => _path;

        public static implicit operator string(AnimatorStateReference obj) => obj.ToString();
        public static implicit operator bool(AnimatorStateReference obj) => obj != null;
    }
}
