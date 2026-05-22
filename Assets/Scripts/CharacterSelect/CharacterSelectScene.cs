using CharacterSelect.UI;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using RippitGameManager;
using System.Linq;

public class CharacterSelectScene : MonoBehaviour
{
    [SerializeField] private EventReference _MusicTrack;

    public GameObject PlayerControllerPrefab;
    public CanvasGroup JoinPrompt;

    [Header("UI Setup")]
    public HorizontalLayoutGroup DisplayList;
    public GameObject PlayerUIPrefab;
    private Dictionary<PlayerUIController, GameObject> _PlayerUI = new Dictionary<PlayerUIController, GameObject>();
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
        var newInputController = PlayerInput.Instantiate(PlayerControllerPrefab, -1, null, -1, device);
        newInputController.neverAutoSwitchControlSchemes = true;

        var newCharacterSelectUI = GameObject.Instantiate(PlayerUIPrefab).GetComponent<CharacterSelectUI>();
        newCharacterSelectUI.SetPlayerIndex(newInputController.playerIndex);
        newCharacterSelectUI.transform.SetParent(DisplayList.transform, false);


        int playerIndex = newInputController.playerIndex;
        InputDevice PlayerDevice = device;
        GameObject InputControllerObj = newInputController.gameObject;
        newCharacterSelectUI.OnBeforeDestroy += UI =>
        {
            GameManager.Instance.CharacterSelection.RemovePlayer(playerIndex);
            _DeviceQueue.Add(PlayerDevice.path,PlayerDevice);
            GameObject.Destroy(InputControllerObj);
        };

        newInputController.GetComponent<PlayerUIController>().SetSelectedGameObject(newCharacterSelectUI.gameObject);

        //Add Selection to GameManager
        GameManager.Instance.CharacterSelection.AddPlayer(newInputController.playerIndex, device);

        _DeviceQueue.Remove(device.path);
    }


    private void RemovePlayer(int playerIndex)
    {

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
