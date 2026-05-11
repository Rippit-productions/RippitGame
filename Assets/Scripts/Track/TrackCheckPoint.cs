using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TrackCheckPoint 
{

    public Vector3 LocalPosition;
    public Vector2 CollisionBoxSize;
    public Vector3 LocalRespawnPos;

    public TrackCheckPoint(Vector3 LocalPos)
    {
        LocalPosition = LocalPos;
        this.CollisionBoxSize = Vector3.one * 10.0f;
        this.LocalRespawnPos = Vector2.zero;
    }
}
