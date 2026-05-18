using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace CharacterSelect
{
    public enum CharacterName
    {
        Terry,
        K4RMA,
        Ax,
        Manny
    }

    [Serializable]
    public struct CharacterPrefab
    {
        public CharacterName Name;
        public GameObject PrefabObject;
    }

    public class PlayerSelection
    {
        CharacterName Character;
        int ColourIndex;
        int PlayerIndex;
        string DevicePath;
        bool Confirmed;

        public PlayerSelection(int playerIndex,string InputDevicePath)
        {
            Character = CharacterName.Terry;
            ColourIndex = 0;
            this.PlayerIndex = playerIndex;
            DevicePath = "";
            Confirmed = false;
        }

        public override string ToString()
        {
            return $"Player Selection: Device {DevicePath} | {Character}";
        }
    }

    public enum PlayerChangeEvent
    {
        Joined,
        Left
    }

    public class CharacterSelect : MonoBehaviour
    {
        [SerializeField] private EventReference _MusicTrack;
        [Header("Controller")]
        [SerializeField] private GameObject CharacterSelectControllerPrefab;

        [Header("Player Banner Setup")]
        [SerializeField] private GameObject PlayerBannerList;
        [SerializeField] private GameObject PlayerBannerPrefab;
        [SerializeField] private GameObject PlayerJoinBanner;

        private List<InputDevice> _DeviceQueue = new List<InputDevice>();

        public static List<PlayerSelection> Selections => _Selections; 
        private static List<PlayerSelection> _Selections = new List<PlayerSelection>();

        //Events
        public UnityEvent<PlayerChangeEvent> OnPlayerChange = new UnityEvent<PlayerChangeEvent>();

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
                    {
                        AddPlayer(device);
                        break;
                    }
                }
                else if (device is Keyboard)
                {
                    if (((Keyboard)device).enterKey.wasPressedThisFrame)
                    {
                        AddPlayer(device);
                        break;
                    }
                }

            }
        }

        private void AddPlayer(InputDevice Device)
        {
            var newInputComp = PlayerInput.Instantiate(CharacterSelectControllerPrefab, -1, null, -1, Device);
            newInputComp.gameObject.name = $"Player Select Controller: {newInputComp.playerIndex}";

            var UIComponent = newInputComp.gameObject.GetComponent<InputSystemUIInputModule>();
            UIComponent.cancel.action.performed += inputState  =>
            {
                if (inputState.phase == InputActionPhase.Started)
                {
                    RemovePlayer(newInputComp.playerIndex);
                }
            };

            _DeviceQueue.Remove(Device);
            _Selections.Add(new PlayerSelection(newInputComp.playerIndex,Device.path));
            this.OnPlayerChange.Invoke(PlayerChangeEvent.Joined);
            
        }

        private void RemovePlayer(int index)
        {
            if (index < 0 || index >= _Selections.Count) return;

            _Selections.RemoveAt(index);
            OnPlayerChange.Invoke(PlayerChangeEvent.Left);
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
