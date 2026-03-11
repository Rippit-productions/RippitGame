using Cinemachine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CinemachineVirtualCamera))]
[RequireComponent(typeof(CinemachineBrain))]
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float zOffsetDistance = 30.0f;
    [SerializeField] private float MinZOffset = 20.0f;

    private Camera _CameraComponent;
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    private CinemachineFramingTransposer _cinemachineTransposer;
    protected Skater linkedCharacter;

    public static PlayerCamera[] All => FindObjectsByType<PlayerCamera>(FindObjectsInactive.Exclude,FindObjectsSortMode.InstanceID);

    public static PlayerCamera CreateCamera()
    {
        var newGameObject = new GameObject();
        var component = newGameObject.AddComponent<PlayerCamera>();
        return component;
    }

    public static PlayerCamera CreateCamera(Skater Player)
    {
        var newCamera = PlayerCamera.CreateCamera();
        newCamera.ConnectToPlayer(Player);
        return newCamera;
    }

    public void ConnectToPlayer(Skater Player)
    {
        var existingCamera = GetLinkedCamera(Player);
        if (existingCamera)
        {
            existingCamera._cinemachineVirtualCamera.Follow = null;
            existingCamera.linkedCharacter = null;
        }

        this.linkedCharacter = Player;
        _cinemachineVirtualCamera.Follow = Player.transform;
    }
    
    public PlayerCamera GetLinkedCamera(Skater Player)
    {
        foreach (var playerCharacter in Skater.All)
        {
            foreach (var camera in PlayerCamera.All)
            {
                if (camera.linkedCharacter == playerCharacter)
                {
                    return camera;
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

        
    }
    
    void Awake()
    {
        _Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (_cinemachineVirtualCamera == null) return;
        _UpdatePosition();
    }

    private void _UpdatePosition()
    {
        if (_cinemachineVirtualCamera == null || linkedCharacter == null) return;
        float newDistance = MinZOffset +  ( zOffsetDistance * linkedCharacter.GetNormalisedSpeed() );
        _cinemachineTransposer.m_CameraDistance = Mathf.Lerp(_cinemachineTransposer.m_CameraDistance ,newDistance
            ,Mathf.Abs(_cinemachineTransposer.m_CameraDistance - newDistance));
        _cinemachineTransposer.m_LookaheadTime = 0.5f;

        this.gameObject.name = $"Player Camera: {this.linkedCharacter.name}";
    }
}
