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

    public void SwitchToSibling(int newIndex)
    {
        var ParentObj  = transform.parent;
        CanvasSwitcher ParentSwitcher = ParentObj.GetComponent<CanvasSwitcher>();

        if (ParentSwitcher == null) return;

        ParentSwitcher.SetActiveIndex(newIndex);
    }
}
