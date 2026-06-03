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
using GameAudio;

public class CharacterSelectScene : MonoBehaviour
{
    public static CharacterSelectScene Instance => FindFirstObjectByType<CharacterSelectScene>();

    [SerializeField] private EventReference _MusicTrack;
    [SerializeField] private string MainMenuScene;
    [SerializeField] private string GreyboxSceneName;

    [Header("UI Setup")]
    [SerializeField] private GameObject UIControllerPrefab;

    [SerializeField] private HorizontalLayoutGroup DisplayList;
    [SerializeField] private GameObject PlayerUIPrefab;

    public bool PlayersReady 
    {
        get
        {
            if (_PlayerUI.Count == 0 && PlayerUIController.GetPlayerController(0))
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

        CharacterSelectUI.OnBeforeDestroy += OnCharacterUIDestroy;
        GameAudio.AudioEvent.Instansiate(_MusicTrack, GameAudio.AudioEventType.Music).Play();
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

        if (PlayersReady)
        {
            GameManager.Instance.LoadScene(GreyboxSceneName); 
        }
    }

    private void OnCharacterUIDestroy(CharacterSelectUI UI)
    {
        var device = GameManager.Instance.CharacterSelection[UI.PlayerIndex].InputDevice;
        GameManager.Instance.CharacterSelection.RemovePlayer(UI.PlayerIndex);
        _DeviceQueue.Add(device.path, device);
        _PlayerUI.Remove(UI.PlayerIndex);
        GameObject.Destroy(PlayerUIController.GetPlayerController(UI.PlayerIndex).gameObject);
    }

    private void AddPlayer(InputDevice device)
    {
        var newInputController = PlayerInput.Instantiate(UIControllerPrefab, -1, null, -1, device);
        newInputController.neverAutoSwitchControlSchemes = true;

        var newCharacterSelectUI = GameObject.Instantiate(PlayerUIPrefab).GetComponent<CharacterSelectUI>();
        newCharacterSelectUI.SetPlayerIndex(newInputController.playerIndex);
        newCharacterSelectUI.transform.SetParent(DisplayList.transform, false);

        int playerIndex = newInputController.playerIndex;
        _PlayerUI.Add(playerIndex, newCharacterSelectUI);

        newInputController.GetComponent<PlayerUIController>().SetSelectedGameObject(newCharacterSelectUI.gameObject);

        //Add Selection to GameManager
        GameManager.Instance.CharacterSelection.AddPlayer(newInputController.playerIndex, device);

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
