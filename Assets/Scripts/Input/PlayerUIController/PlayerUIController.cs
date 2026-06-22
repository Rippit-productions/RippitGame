using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(InputSystemUIInputModule))]
[RequireComponent(typeof(MultiplayerEventSystem))]
public class PlayerUIController : MonoBehaviour
{
    public static PlayerUIController[] All => 
        FindObjectsByType<PlayerUIController>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).OrderBy(controller => controller.PlayerIndex).ToArray();
    
    public static PlayerUIController GetController(int PlayerIndex)
    {
        return All.Where(component => component.PlayerIndex == PlayerIndex).FirstOrDefault();
    }

    public static PlayerUIController Instantiate(GameObject Prefab, int PlayerIndex = -1,params InputDevice[] Devices)
    {
        return PlayerInput.Instantiate(Prefab,PlayerIndex,null,PlayerIndex,Devices).GetComponentInChildren<PlayerUIController>();
    }
    
    public int PlayerIndex => _PlayerInput.playerIndex;

    private InputSystemUIInputModule _UIInputModule;
    private PlayerInput _PlayerInput;
    private MultiplayerEventSystem _EventSystem;
    
    void Awake()
    {
        _UIInputModule = GetComponent<InputSystemUIInputModule>();
        _PlayerInput = GetComponent<PlayerInput>();
        _PlayerInput.uiInputModule = _UIInputModule;
        _EventSystem = GetComponent<MultiplayerEventSystem>();
    }

    public void SetSelectedGameObject(GameObject TargetObject)
    {
        _EventSystem.SetSelectedGameObject(TargetObject);
    }

    public void SetPlayerRoot(GameObject Target)
    {
        _EventSystem.playerRoot = Target;
    }
}
