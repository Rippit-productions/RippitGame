using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CinemachineVirtualCamera))]
[RequireComponent(typeof(CinemachineBrain))]
public class PlayerCamera : MonoBehaviour
{
    [Serializable]
    public struct Setup
    {
        public float minZDistance;
        public float zOffset;
        public float LookAheadTime;
        public float LookAheadSmoothing;
        public AnimationCurve DistanceCurve;
    }

    private PlayerCamera.Setup CameraSetup;

    private Camera _CameraComponent;
    public Camera CameraComponent => _CameraComponent;
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
        var inputCompo = linkedCharacter.GetComponent<PlayerInput>();
        inputCompo.camera = this._CameraComponent;
    }

    public PlayerCamera GetLinkedCamera(Skater Player)
    {
        foreach (var camera in PlayerCamera.All)
        {
            if (camera.linkedCharacter == Player)
            {
                return camera;
            }
        }
        return null;
    }
    
    private void _Init()
    {
        _CameraComponent = GetComponent<Camera>();
        _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        _cinemachineTransposer = _cinemachineVirtualCamera.AddCinemachineComponent<CinemachineFramingTransposer>();

        var confine = this.AddComponent<CinemachineConfiner2D>();
        confine.m_BoundingShape2D = CameraBounds.Instance.Collider;

        UpdateViewPorts();
    }
    
    void Awake()
    {
        _Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (_cinemachineVirtualCamera == null) return;
        this.gameObject.name = $"Player Camera: {this.linkedCharacter.name}";

        _cinemachineVirtualCamera.Follow = this.linkedCharacter.transform;
        this.CameraSetup = this.linkedCharacter.CameraSetup;
        this._cinemachineTransposer.m_LookaheadSmoothing = this.CameraSetup.LookAheadSmoothing;
        this._cinemachineTransposer.m_LookaheadTime = this.CameraSetup.LookAheadTime;
        this._cinemachineTransposer.m_LookaheadIgnoreY = true;
        _UpdatePosition();
    }

    private void _UpdatePosition()
    {
        if (_cinemachineVirtualCamera == null || linkedCharacter == null) return;
        float offset = this.CameraSetup.DistanceCurve.Evaluate(linkedCharacter.GetNormalisedSpeed()) * this.CameraSetup.zOffset;
        float newDistance = CameraSetup.minZDistance +  offset;

        if (newDistance < _cinemachineTransposer.m_CameraDistance && this.linkedCharacter.State != Skater.SkaterState.Grounded) return;
        _cinemachineTransposer.m_CameraDistance = Mathf.Lerp(
            _cinemachineTransposer.m_CameraDistance ,
            newDistance,
            Mathf.Abs(_cinemachineTransposer.m_CameraDistance - newDistance)
            );
    }


    public static void UpdateViewPorts()
    {
        PlayerCamera[] cameras = PlayerCamera.All;

        int count = cameras.Length;
        if (count == 0) return;

        // Determine grid size (rows x cols)
        int cols = Mathf.CeilToInt(Mathf.Sqrt(count));
        int rows = Mathf.CeilToInt((float)count / cols);

        float width = 1.0f / cols;
        float height = 1.0f / rows;

        for (int i = 0; i < count; i++)
        {
            int col = i % cols;
            int row = i / cols;

            // Unity's viewport origin is bottom-left
            float x = col * width;
            float y = 1f - ((row + 1) * height);

            cameras[i].CameraComponent.rect = new Rect(x, y, width, height);
        }
    }
}
