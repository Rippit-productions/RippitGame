using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput PlayerInputComponent => _PlayerInputComponent;
    private PlayerInput _PlayerInputComponent;

    private float _NoMoveInputTime = 0.0f;
    [SerializeField] private InputActionReference _MoveAction;
    [SerializeField] private InputActionReference _JumpAction;
    [SerializeField] private InputActionReference _GrappleAction;

    public Vector2 Move => _PlayerInputComponent.actions.FindAction(_MoveAction.name).ReadValue<Vector2>();
    public float NoMoveInput => _NoMoveInputTime;
    public InputAction Jump => _PlayerInputComponent.actions.FindAction(_JumpAction.name);

    public InputAction Grapple => _PlayerInputComponent.actions.FindAction(_GrappleAction.name);


    void _Init() {
        _PlayerInputComponent = GetComponent<PlayerInput>();
        _PlayerInputComponent.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
        _PlayerInputComponent.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
    }

    private void Update()
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

    void Awake()
    {
        _Init();
    }
}
