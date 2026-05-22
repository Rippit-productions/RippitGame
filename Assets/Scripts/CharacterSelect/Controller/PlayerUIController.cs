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
    
    public static PlayerUIController GetPlayerController(int PlayerIndex)
    {
        return All.Where(component => component._PlayerIndex == PlayerIndex).First();
    }
    
    public int PlayerIndex => _PlayerIndex;
    private int _PlayerIndex = -1;

    public InputDevice InputDevice => _InputDevice;
    public InputDevice _InputDevice = null;

    private InputSystemUIInputModule _UIInputModule;
    private PlayerInput _PlayerInput;
    private MultiplayerEventSystem _EventSystem;
    
    void Awake()
    {
        _UIInputModule = GetComponent<InputSystemUIInputModule>();
        _PlayerInput = GetComponent<PlayerInput>();
        _EventSystem = GetComponent<MultiplayerEventSystem>();
        _PlayerIndex = _PlayerInput.playerIndex;
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
        //_GuiRect = GUILayout.Window(GuiID, _GuiRect, _DrawGUIWindow, $"UI Controller{}");
    }

    void _DrawGUIWindow(int WindowID)
    {
        GUI.DragWindow(new Rect(0, 0, float.MaxValue, float.MaxValue));
    }
#endif

}
