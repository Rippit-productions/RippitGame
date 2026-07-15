using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CanvasSwitcherPlayableBehaviour : PlayableBehaviour
{
    
    public int index = 0;


    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        CanvasSwitcher switcher = playerData as CanvasSwitcher;
        if (switcher == null) return;

        switcher.ActiveIndex = index;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
