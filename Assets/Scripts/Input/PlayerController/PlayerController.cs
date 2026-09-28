using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(InputSystemUIInputModule))]
[RequireComponent(typeof(MultiplayerEventSystem))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController[] All  {
        get
        {
            var all = FindObjectsByType<PlayerController>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            return all.OrderBy(controller => controller.PlayerIndex).ToArray();
        }
    }

    public static PlayerController Instantiate(GameObject Prefab, int playerIndex = -1, params InputDevice[] InputDevices)
    {
        var newObject = PlayerInput.Instantiate(Prefab, playerIndex, null, -1, InputDevices).gameObject;
        var existingComponent = newObject.GetComponent<PlayerController>();
        if (existingComponent) return existingComponent;
        else
        {
            return newObject.AddComponent<PlayerController>();
        }
    }

    public int PlayerIndex => _PlayerInputComponent.playerIndex;

    public PlayerInput InputComponent => _PlayerInputComponent;
    private PlayerInput _PlayerInputComponent;

    private float _NoMoveInputTime = 0.0f;


    [Header("Action Maps")]
    [SerializeField] private string SkateActionMapName;
    [SerializeField] private string UIActionMapName;

    [Header("Skater Actions")]
    [SerializeField] private InputActionReference _MoveAction;
    [SerializeField] private InputActionReference _JumpAction;
    [SerializeField] private InputActionReference _TrickAction;
    [SerializeField] private InputActionReference _GrappleAction;

    public Vector2 Move => _PlayerInputComponent.actions.FindAction(_MoveAction.name).ReadValue<Vector2>();
    public float NoMoveInput => _NoMoveInputTime;
    public InputAction Jump => _PlayerInputComponent.actions.FindAction(_JumpAction.name);
    public InputAction Grapple => _PlayerInputComponent.actions.FindAction(_GrappleAction.name);

    [Header("UI")]
    public InputSystemUIInputModule UIInputModule => _UIInputModule;
    private InputSystemUIInputModule _UIInputModule;
    private MultiplayerEventSystem _EventSystem;


    private void Awake()
    {
        _PlayerInputComponent = GetComponent<PlayerInput>();
        _PlayerInputComponent.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

        _UIInputModule = GetComponent<InputSystemUIInputModule>();
        _EventSystem = GetComponent<MultiplayerEventSystem>();
    }
    
    private void Update()
    {
        UpdateNoInputTime();
    }

    private void UpdateNoInputTime()
    {
        if (Move.magnitude <= 0.1f)
        {
            _NoMoveInputTime += Time.deltaTime;
        }
        else
        {
            _NoMoveInputTime = 0.0f;
        }
    }
    
    public void SetUISelectedGameObject(GameObject Target)
    {
        _EventSystem.SetSelectedGameObject(Target);
    }

    public void SetUIPlayerRoot(GameObject Target)
    {
        _EventSystem.playerRoot = Target;
    }

}
