using CharacterSelect.UI;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using RippitGameManager;
using System.Linq;
using CharacterSelect;
using UnityEngine.Events;






public class CharacterSelectScene : MonoBehaviour
{
    public struct Player
    {
        public CharacterSelectUI UI;
        public PlayerInput InputComponent;
        public InputDevice[] InputDevices;
    }

    private Dictionary<int, Player> Players = new Dictionary<int, Player>();
    public static CharacterSelectScene Instance => FindFirstObjectByType<CharacterSelectScene>();

    [Header("Sound")]
    [SerializeField] private EventReference _MusicTrack;
    [SerializeField] private StudioEventEmitter _CancelSFX;

    [Header("Scenes")]
    [SerializeField] private SceneReference MainMenuScene;
    [SerializeField] private SceneReference GreyboxSceneName;

    [Header("UI Setup")]
    [SerializeField] private GameObject UIControllerPrefab;

    private PlayerInput DefaultController;
    private PlayerInput UIControllers;

    [SerializeField] private HorizontalLayoutGroup DisplayList;
    [SerializeField] private GameObject PlayerUIPrefab;

    public bool IsPlayersReady 
    {
        get
        {
            if (_PlayerUI.Count == 0)
            {
                return false;
            }
            else
            {
                return _PlayerUI.Where(obj => obj.Value.Confirmed == false).Count() == 0;
            }
        }
    }
    private Dictionary<int, CharacterSelectUI> _PlayerUI = new Dictionary<int, CharacterSelectUI>();

    // Device Queue
    public InputDevice[] DeviceQueue => _DeviceQueue.Values.ToArray();
    private Dictionary<string, InputDevice> _DeviceQueue = new Dictionary<string, InputDevice>();

    [SerializeField] private GameObject _PlayerJoinPrompt;
    [SerializeField] private GameObject _StartRacePrompt;

    private void Start()
    {
        DefaultController = GameObject.Instantiate(UIControllerPrefab).GetComponent<PlayerInput>();
        DefaultController.gameObject.name = "Default UI Controller";

        foreach (var device in InputSystem.devices)
        {
            if (device is Keyboard || device is Gamepad)
            {
                _DeviceQueue.Add(device.path, device);
            }
        }

    }

    private void Update()
    {
        
        
    }

    private void CheckDeviceJoin()
    {
        foreach (var device in _DeviceQueue)
        {
            var keyboard = device.Value as Keyboard;
            var gamepad = device.Value as Gamepad;

            if (keyboard != null)
            {

            }

        }
    }

    private void AddPlayer()
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

        foreach (KeyValuePair<int,CharacterSelect.PlayerCharacterSelection> element in GameManager.Instance.CharacterSelection)
        {
            GUILayout.Label($"Selection : {element.Key} /{element.Value} "); 
        }
        GUILayout.Label($"Player Count = {_PlayerUI.Count}");
        GUILayout.Label($"Player Ready = {IsPlayersReady}");
        GUI.DragWindow(new Rect(0, 0, float.MaxValue, float.MaxValue));
    }
#endif
}
