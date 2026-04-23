using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CanvasSwitcherChild : MonoBehaviour
{
    public UnityEvent OnSwitcherEnable = new UnityEvent();
    public UnityEvent OnSwitcherDisable = new UnityEvent();

    private void OnEnable() => OnSwitcherEnable.Invoke();

    private void OnDisable() => OnSwitcherDisable.Invoke();
}
