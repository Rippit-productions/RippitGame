using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class AnyKeyButton : MonoBehaviour
{
    private Button _ButtonComponent;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
        _ButtonComponent = GetComponent<Button>();

        InputSystem.onAnyButtonPress.Call(
            (InputControl) => {
                if (_ButtonComponent == null || EventSystem.current.currentSelectedGameObject != this.gameObject) return;
                _ButtonComponent.onClick.Invoke();
            }
            );

        _ButtonComponent.navigation = Navigation.defaultNavigation;
    }
}
