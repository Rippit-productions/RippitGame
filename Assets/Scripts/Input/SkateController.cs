using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkateController : MonoBehaviour
{
    public PlayerInput PlayerInputComponent;

    private float _NoMoveInputTime = 0.0f;
    [SerializeField] private InputActionReference _MoveAction;
    [SerializeField] private InputActionReference _JumpAction;
    [SerializeField] private InputActionReference GrappleAction;

    public Vector2 Move => PlayerInputComponent.actions.FindAction(_MoveAction.name).ReadValue<Vector2>();
    public float NoMoveInput => _NoMoveInputTime;
    public InputAction Jump => PlayerInputComponent.actions.FindAction(_JumpAction.name);

    void _Init() {
        PlayerInputComponent = GetComponent<PlayerInput>();
        PlayerInputComponent.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
        PlayerInputComponent.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

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
