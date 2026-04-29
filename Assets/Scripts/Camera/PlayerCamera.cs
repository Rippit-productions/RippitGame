using Unity.Cinemachine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CinemachineCamera))]
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
    private CinemachineCamera _cinemachineVirtualCamera;
    private CinemachinePositionComposer _cinemachineTransposer;
    private CinemachineBrain _CinemachineBrain;

    protected Skater linkedCharacter;



    public static PlayerCamera[] All => FindObjectsByType<PlayerCamera>(FindObjectsInactive.Exclude,FindObjectsSortMode.InstanceID);

    public static PlayerCamera CreateCamera()
    {
        var newGameObject = new GameObject();
        newGameObject.name = "Player Camera";
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

    public void DisconnectFromPlayer()
    {
        this.linkedCharacter = null;
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
        _cinemachineVirtualCamera = GetComponent<CinemachineCamera>();
        _cinemachineTransposer = _cinemachineVirtualCamera.AddComponent<CinemachinePositionComposer>();

        _CinemachineBrain = new GameObject().AddComponent<CinemachineBrain>();
        var confine = this.AddComponent<CinemachineConfiner2D>();
        confine.BoundingShape2D = CameraBounds.Instance.Collider;
        UpdateViewPorts();

        Skater.OnSkateDestroy += this.OnPlayerDestroy;
    }

    private void OnPlayerDestroy(Skater skater)
    {
        if (skater == this.linkedCharacter)
        {
            this.DisconnectFromPlayer();
            GameObject.Destroy(_CinemachineBrain.gameObject);
            GameObject.Destroy(this.gameObject);
        }
    }
    
    void Awake()
    {
        _Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (_cinemachineVirtualCamera == null) return;
        else if (this.linkedCharacter == null) return;
        this.gameObject.name = $"Player Camera: {this.linkedCharacter.name}";
        _CinemachineBrain.gameObject.name = $"Player Camera Brain {this.linkedCharacter.name}";

        _cinemachineVirtualCamera.Follow = this.linkedCharacter.transform;
        this.CameraSetup = this.linkedCharacter.CameraSetup;
        this._cinemachineTransposer.Lookahead.Enabled = true;
        this._cinemachineTransposer.Lookahead.Smoothing = this.CameraSetup.LookAheadSmoothing;
        this._cinemachineTransposer.Lookahead.Time = this.CameraSetup.LookAheadTime;
        this._cinemachineTransposer.Lookahead.IgnoreY = true;

        int index = 1 << linkedCharacter.SkaterIndex + 1;
        this._cinemachineVirtualCamera.OutputChannel = (OutputChannels)index;

        _CinemachineBrain.ChannelMask = (OutputChannels) index;
        _CinemachineBrain.UpdateMethod = CinemachineBrain.UpdateMethods.SmartUpdate;
        _UpdatePosition();
    }

    private void _UpdatePosition()
    {
        if (_cinemachineVirtualCamera == null || linkedCharacter == null) return;
        float offset = this.CameraSetup.DistanceCurve.Evaluate(linkedCharacter.GetNormalisedSpeed()) * this.CameraSetup.zOffset;
        float newDistance = CameraSetup.minZDistance +  offset;

        // Only zoom in camera when on flat ground
        // Should reduce sharp changes in camera zoom
        if (
            newDistance < _cinemachineTransposer.CameraDistance && 
            this.linkedCharacter.State != Skater.SkaterState.Grounded
            && Vector3.Dot(this.linkedCharacter.Upvector,Vector3.up) < 0.95f
            ) return;

        _cinemachineTransposer.CameraDistance = Mathf.Lerp(
            _cinemachineTransposer.CameraDistance,
            newDistance,
            Mathf.Abs(_cinemachineTransposer.CameraDistance - newDistance)
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

        for (int i = 0 ; i < count; i++)
        {
            int col = i % cols;
            int row = i / cols;

            // Unity's viewport origin is bottom-left
            float x = (col * width);
            float y = 1.0f - ((row + 1) * height);

            cameras[i].CameraComponent.rect = new Rect(x, y, width, height);
        }
    }

    private void OnDestroy() => UpdateViewPorts();
}
