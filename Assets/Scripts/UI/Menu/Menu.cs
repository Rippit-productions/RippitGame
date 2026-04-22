using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;


public class Menu : MonoBehaviour
{
    public GameObject DefaultObj;
    // GameObject to go back to when back button is pressed.
    private GameObject BackObject;

    private CanvasSwitcher[] _CanvasSwitchers => GetComponentsInChildren<CanvasSwitcher>();

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(DefaultObj);
    }

    private void Update()
    {
    }

    public void GoToBackObject()
    {
        if (BackObject == null) return;
        SwitchTo(BackObject);
        BackObject = null;
    }

    public void SwitchTo(GameObject TargetObj)
    {
        if (!TargetObj.transform.IsChildOf(this.transform)) return;
        EventSystem.current.SetSelectedGameObject(TargetObj);
        Debug.Log($"Selected - {EventSystem.current.currentSelectedGameObject}");
        foreach (var switcher in _CanvasSwitchers)
        {
            switcher.SwitchToObject(TargetObj);
        }

    }

    public void SetBackObject(GameObject TargetObj)
    {
        BackObject = TargetObj;
    }
}
