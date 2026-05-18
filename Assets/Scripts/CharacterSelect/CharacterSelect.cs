using FMODUnity;
using System;
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
        [Header("Controller")]
        [SerializeField] private GameObject CharacterSelectControllerPrefab;
        [Header("Variables")]
        [SerializeField] private GameObject DefaultSelection;

        private List<InputDevice> _DeviceQueue = new List<InputDevice>();
        private List<(PlayerSelection Select, bool Confirmed)> _Selections = new List<(PlayerSelection Select, bool Confirmed)>();


        private void Start()
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

            AudioManager.Instance.PlayAudioInstance(_MusicTrack,AudioManager.AudioType.Music);
        }


        private void Update()
        {
            foreach (var device in _DeviceQueue)
            {
                if (device is Gamepad)
                {
                    if (((Gamepad)device).aButton.wasPressedThisFrame)
                        AddPlayer(device);
                }
                else if (device is Keyboard)
                {
                    if (((Keyboard)device).enterKey.wasPressedThisFrame)
                        AddPlayer(device);
                }


            }
        }

        private void AddPlayer(InputDevice Device)
        {
            var newInputObj = PlayerInput.Instantiate(CharacterSelectControllerPrefab, -1, null, -1, Device);
            _DeviceQueue.Remove(Device);
        }




#if UNITY_EDITOR
        private int GuiID = Guid.NewGuid().GetHashCode();
        private Rect _GuiRect = new Rect(20, 20, 300, 200);
        void OnGUI()
        {
            _GuiRect = GUILayout.Window(GuiID, _GuiRect, _DrawGUIWindow, $"Character Select");
        }
        void _DrawGUIWindow(int WindowID)
        {
            GUILayout.Label($"Queued Devices: {_DeviceQueue.Count}");
            GUI.DragWindow(new Rect(0, 0, float.MaxValue, float.MaxValue));
        }
#endif
    }

}
