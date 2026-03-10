using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CinemachineVirtualCamera))]
[RequireComponent(typeof(CinemachineBrain))]
public class PlayerCamera : MonoBehaviour
{
    private float _CurrentDistanceValue = 0.0f;
    [SerializeField] private float zOffsetDistance = 30.0f;
    [SerializeField] private float MinZOffset = 20.0f;

    private Camera _CameraComponent;
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    private CinemachineFramingTransposer _cinemachineTransposer;
    private Skater linkedCharacter;

    private static List<CameraLink> Connections = new List<CameraLink>();

    private struct CameraLink
    {
        public PlayerCamera Camera;
        public Skater Player;
    }

    public static PlayerCamera CreateCamera()
    {
        var newGameObject = new GameObject();
        var component = newGameObject.AddComponent<PlayerCamera>();

        return component;
    }
    
    public PlayerCamera GetLinkedCamera(Skater Player)
    {
        foreach (var playerCharacter in Skater.All)
        {
            foreach (var link in Connections)
            {
                if (link.Player == playerCharacter)
                {
                    this.linkedCharacter = playerCharacter;
                    return link.Camera;
                }
            }
        }
        return null;
    }
    
    private void _Init()
    {
        _CameraComponent = GetComponent<Camera>();
        _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        _cinemachineTransposer = _cinemachineVirtualCamera.AddCinemachineComponent<CinemachineFramingTransposer>();
        
        foreach (var player in Skater.All)
        {
            if (!GetLinkedCamera(player))
            {
                var newConnection = new CameraLink
                {
                    Camera = this,
                    Player = player
                };
                this.linkedCharacter = player;
                Connections.Add(newConnection);
                _cinemachineVirtualCamera.Follow = player.transform;

                var inputComponent = this.linkedCharacter.GetComponent<PlayerInput>();
                inputComponent.camera = this._CameraComponent;

                this.gameObject.name = $"{this.linkedCharacter.name}: Camera";
            }
        }
    }
    
    void Awake()
    {
        _Init();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_cinemachineVirtualCamera == null) return;
        _UpdatePosition();
    }

    private void _UpdatePosition()
    {
        if (_cinemachineVirtualCamera == null || linkedCharacter == null) return;
        float newDistance = MinZOffset +  ( zOffsetDistance * linkedCharacter.GetNormalisedSpeed() );
        _cinemachineTransposer.m_CameraDistance = newDistance;
    }
}
