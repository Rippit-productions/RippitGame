using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

namespace CharacterSelect
{
    public enum CharacterOption
    {
        Terry,
        Snake,
        Spider
    }

    public struct PlayerSelection
    {
        CharacterOption Character;
        int ColourIndex;
        string DevicePath;
    }
    

    public class CharacterSelect : MonoBehaviour
    {
        [SerializeField] private EventReference _MusicTrack;
        [SerializeField] private GameObject CharacterSelectControllerPrefab;

        private List<InputDevice> _DeviceQueue =  new List<InputDevice>();
        private List<(PlayerSelection Select,bool Confirmed)> _Selections = new List<(PlayerSelection Select, bool Confirmed)>();


        private void Start()
        {
            _DeviceQueue.AddRange(Gamepad.all);
            if (Keyboard.current != null) {
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
        }


        private void Update()
        {
           foreach (var device in _DeviceQueue)
            {
                bool AddPlayer = false;
                if (device is Gamepad)
                {
                    if (((Gamepad)device).aButton.wasPressedThisFrame)
                        AddPlayer = true; 
                }
                else if (device is Keyboard) 
                {
                    if (((Keyboard)device).enterKey.wasPressedThisFrame)
                        AddPlayer = true;
                }

                if (AddPlayer)
                {
                    var newInputObj = PlayerInput.Instantiate(CharacterSelectControllerPrefab, -1, null, -1, device);
                }
            }
        }
    }
}
