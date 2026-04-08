using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    private int _ActiveIndex;
    public int ActiveIndex
    {
        get { return _ActiveIndex; }
        set { SetActiveIndex(value); }
    }
    public int DefaultIndex = 0;

    public Action OnCanvasSwitch = () => { };
    private void Awake()
    {
        SetActiveIndex(DefaultIndex);
        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == _ActiveIndex);
        }
    }
    
    public void SetActiveIndex(int newIndex)
    {
        if (transform.childCount == 0) return;
        else if (newIndex < 0 || newIndex >= transform.childCount) return;
        else if (_ActiveIndex != newIndex)
        {
            OnCanvasSwitch.Invoke();
            _ActiveIndex = newIndex;
            Refresh();
        }
    }

    /// <summary>
    /// Set Canvas layer to show target Child Object.
    /// Object must be child of this Canvas Switcher
    /// </summary>
    /// <param name="targetObj">The Child Object to Show</param>
    public void SwitchToObject(GameObject targetObj)
    {
        if (targetObj == null) return;
        var targetTransform = targetObj.transform; 

        for (int i = 0; i < transform.childCount;i++)
        {
            var layerTransform = transform.GetChild(i).transform;
            if (layerTransform == targetTransform || targetTransform.IsChildOf(layerTransform))
            {
                SetActiveIndex(i);
                break;
            }
        }
    }
}
