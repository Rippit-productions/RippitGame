using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class CanvasSwitcherPlayableAsset : PlayableAsset
{
    public int index;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CanvasSwitcherPlayableBehaviour>.Create(graph);
        CanvasSwitcherPlayableBehaviour behaviour = playable.GetBehaviour();

        behaviour.index = index;

        return playable;
    }
    
}
