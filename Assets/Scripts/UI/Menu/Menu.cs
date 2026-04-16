using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public GameObject DefaultObj;
    private CanvasSwitcher[] _CanvasSwitchers => GetComponentsInChildren<CanvasSwitcher>();

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(DefaultObj);
    }

    public void SwitchTo(GameObject TargetObj)
    {
        if (!TargetObj.transform.IsChildOf(this.transform)) return;

        foreach (var switcher in _CanvasSwitchers)
        {
            switcher.SwitchToObject(TargetObj);
        }

        EventSystem.current.SetSelectedGameObject(TargetObj);
    }
}
