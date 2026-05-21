using CharacterSelect.Controller;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSelectScene : MonoBehaviour
{
    [SerializeField] private EventReference _MusicTrack;

    public GameObject PlayerControllerPrefab;
    public CanvasGroup JoinPrompt;

    [Header("UI Setup")]
    public HorizontalLayoutGroup DisplayList;
    public GameObject PlayerUIPrefab;

    private Dictionary<string, InputDevice> _DeviceQueue = new Dictionary<string, InputDevice>();

    // Start is called before the first frame update
    void Start()
    {

        foreach (var gamepad in Gamepad.all)
        {
            _DeviceQueue.Add(gamepad.path,gamepad);
        }
        if (Keyboard.current != null)
        {
            _DeviceQueue.Add(Keyboard.current.path,Keyboard.current);
        }


        // Add device to queue
        InputSystem.onDeviceChange += (device, change) =>
        {
            if (change != InputDeviceChange.Added) return;
            else
            {
                if (device is Gamepad)
                {
                    _DeviceQueue.Add(device.path, device);
                }
            }
        };

        AudioManager.Instance.PlayAudioInstance(_MusicTrack, AudioManager.AudioType.Music);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var device in _DeviceQueue)
        {
            if (device.Value is Keyboard)
            {
                if (((Keyboard)device.Value).enterKey.wasPressedThisFrame)
                {
                    AddPlayer(device.Value);
                    break;
                }
            }
            else if (device.Value is Gamepad)
            {
                if (((Gamepad)device.Value).aButton.wasPressedThisFrame)
                {
                    AddPlayer(device.Value);
                    break;
                }
            }
        }


        if (_DeviceQueue.Count > 0)
        {
            JoinPrompt.alpha = 1;
        }
        else
        {
            JoinPrompt.alpha = 0;
        }

    }

    private void AddPlayer(InputDevice device)
    {
        var ControllerGameObj = PlayerInput.Instantiate(PlayerControllerPrefab, -1, null, -1, device).gameObject;
        CharacterSelectController CharSelectControllerModule = ControllerGameObj.GetComponent<CharacterSelectController>();

        var UIObject = GameObject.Instantiate(PlayerUIPrefab);
        UIObject.transform.SetParent(DisplayList.gameObject.transform,false);
        UIObject.transform.SetSiblingIndex(CharSelectControllerModule.PlayerIndex);

        CharacterSelectController.OnControllerDestroy += (CharacterSelectController controller) =>
        {
            if (!_DeviceQueue.ContainsKey(controller.InputDevice.path))
            {
                _DeviceQueue.Add(controller.InputDevice.path, controller.InputDevice);
            }
            GameObject.Destroy(UIObject);
        };

        _DeviceQueue.Remove(device.path);
    }

#if UNITY_EDITOR
    private int GuiID = Guid.NewGuid().GetHashCode();
    private Rect _GuiRect = new Rect(20, 20, 300, 50);
    void OnGUI()
    {
        _GuiRect = GUILayout.Window(GuiID, _GuiRect, _DrawGUIWindow, $"Character Select Scene");
    }

    void _DrawGUIWindow(int WindowID)
    {
        GUILayout.Label($"Queued Devices = {_DeviceQueue.Count}");
        GUI.DragWindow(new Rect(0, 0, float.MaxValue, float.MaxValue));
    }
#endif
}
