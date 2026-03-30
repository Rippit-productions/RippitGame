using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrackCheckPoint 
{
    public Vector3 LocalPosition = Vector3.zero;
    public Vector2 CollisionBoxSize = Vector3.one * 10.0f;
}
