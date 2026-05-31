using System;
using UnityEngine;

[Serializable]
public class TrickCombo
{
    public string Name;
    public TrickDirection[] Directions;

    public TrickCombo(string name, params TrickDirection[] directions)
    {
        Name = name;
        Directions = directions;
    }

    public bool IsValid => Directions != null && Directions.Length > 0;

    public int StepCount => Directions == null ? 0 : Directions.Length;

    public TrickDirection GetStep(int index)
    {
        return Directions[Mathf.Clamp(index, 0, StepCount - 1)];
    }
}
