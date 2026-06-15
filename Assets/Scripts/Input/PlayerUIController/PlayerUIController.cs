using RippitGameManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(InputSystemUIInputModule))]
[RequireComponent(typeof(MultiplayerEventSystem))]
public class PlayerUIController : MonoBehaviour
{
    public static PlayerUIController[] All => FindObjectsByType<PlayerUIController>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
    
    public static PlayerUIController GetControllerByPlayerIndex(int PlayerIndex)
    {
        return All.Where(component => component.PlayerIndex == PlayerIndex).First();
    }

    public static PlayerUIController Instantiate(GameObject Prefab, int PlayerIndex = -1)
    {
        return PlayerInput.Instantiate(Prefab, PlayerIndex).GetComponentInChildren<PlayerUIController>();
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

    private void Update()
    {
        this.gameObject.name = $"Player UI Controller {PlayerIndex}";
    }

    private void OnDestroy()
    {
    }

    public void SetSelectedGameObject(GameObject TargetObject)
    {
        _EventSystem.SetSelectedGameObject(TargetObject);
    }


#if UNITY_EDITOR
    private int GuiID = Guid.NewGuid().GetHashCode();
    private Rect _GuiRect = new Rect(20, 20, 300, 50);
    void OnGUI()
    {
        _GuiRect = GUILayout.Window(GuiID, _GuiRect, _DrawGUIWindow, $"UI Controller {PlayerIndex}");
    }

    void _DrawGUIWindow(int WindowID)
    {
        GUILayout.Label($"Device: {_PlayerInput.devices.First().name}");
        GUILayout.Label($"Selected Obj: {_EventSystem.currentSelectedGameObject.name}");
        GUI.DragWindow(new Rect(0, 0, float.MaxValue, float.MaxValue));
    }
#endif

}
