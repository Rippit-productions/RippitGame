using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class AnyKeyButton : MonoBehaviour
{
    private Button _ButtonComponent;
    public InputActionReference CancelAction;
    private InputSystemUIInputModule _InputComponenet;

    // Start is called before the first frame update
    void Start()
    {
        _InputComponenet = GetComponent<InputSystemUIInputModule>();
    }

    private void Update()
    {
       
    }

    private void Awake()
    {
        _ButtonComponent = GetComponent<Button>();
        _ButtonComponent.navigation = Navigation.defaultNavigation;
    }
}
