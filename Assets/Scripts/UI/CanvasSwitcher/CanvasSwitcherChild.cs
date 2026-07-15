using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class CanvasSwitcherChild : MonoBehaviour
{
    public UnityEvent OnSwitcherEnable = new UnityEvent();
    public UnityEvent OnSwitcherDisable = new UnityEvent();

    private void OnEnable() => OnSwitcherEnable.Invoke();

    private void OnDisable() => OnSwitcherDisable.Invoke();

    public CanvasSwitcher ParentSwitcher => GetComponentInParent<CanvasSwitcher>();

    public void SwitchToSibling(int newIndex)
    {
        if (ParentSwitcher == null) return;
        ParentSwitcher.SetActiveIndex(newIndex);
    }

    public void GotoNextLayer()
    {
        if (ParentSwitcher == null) return;
        ParentSwitcher.ActiveIndex += 1;
    }

    public void GotoPreviousLayer()
    {
        if (ParentSwitcher == null) return;
        ParentSwitcher.ActiveIndex -= 1;
    }
    
}
