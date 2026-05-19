using FMODUnity;
using RippitGameManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class CharacterSelectScene : MonoBehaviour
{
    [SerializeField] private EventReference _MusicTrack;

    public GameObject PlayerControllerPrefab;
    public CanvasGroup JoinPrompt;

    private List<InputDevice> _DeviceQueue = new List<InputDevice>();


    // Start is called before the first frame update
    void Start()
    {
        _DeviceQueue.AddRange(Gamepad.all);
        if (Keyboard.current != null)
        {
            _DeviceQueue.Add(Keyboard.current);
        }


        // Add device to queue
        InputSystem.onDeviceChange += (device, change) =>
        {
            if (change != InputDeviceChange.Added) return;
            else
            {
                if (device is Gamepad)
                {
                    _DeviceQueue.Add(device);
                }
            }
        };

        AudioManager.Instance.PlayAudioInstance(_MusicTrack, AudioManager.AudioType.Music);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (InputDevice device in _DeviceQueue)
        {
            if (device is Keyboard)
            {
                if (((Keyboard)device).enterKey.wasPressedThisFrame)
                {
                    AddPlayer(device);
                    break;
                }
            }
            else if (device is Gamepad)
            {
                if (((Gamepad)device).aButton.wasPressedThisFrame)
                {
                    AddPlayer(device);
                    break;
                }
            }
        }
    }

    private void AddPlayer(InputDevice device)
    {
        PlayerInput inputComponent = PlayerInput.Instantiate(PlayerControllerPrefab, -1, null, -1, device);
        GameManager.Instance.CharacterSelection.AddPlayer(inputComponent.playerIndex, device);
    }
}
