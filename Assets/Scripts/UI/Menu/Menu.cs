using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;


public class Menu : MonoBehaviour
{
    [SerializeField]protected bool TakeFocusOnAwake = true;
    public GameObject DefaultObj;
    // GameObject to go back to when back button is pressed.
    private GameObject BackObject;

    private CanvasSwitcher[] _CanvasSwitchers => GetComponentsInChildren<CanvasSwitcher>();
    protected void Start()
    {
        EventSystem.current.SetSelectedGameObject(DefaultObj);
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
