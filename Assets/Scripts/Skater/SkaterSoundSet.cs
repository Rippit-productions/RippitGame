using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


[CreateAssetMenu(fileName = "SkaterSoundSet", menuName = "ScriptableObjects/SFX/SkaterSoundSet", order = 1)]
public class SkaterSoundSet : ScriptableObject
{
    public EventReference JumpSFX;
    public EventReference GrindOnSFX;
    public EventReference GrindOffSFX;
    public EventReference GrindSFX;

}
