using RippitGameManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


namespace CharacterSelect.Controller
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(InputSystemUIInputModule))]
    public class CharacterSelectController : MonoBehaviour
    {
        public int PlayerIndex => _PlayerIndex;
        private int _PlayerIndex = -1;

        public InputDevice InputDevice => _InputDevice;
        public InputDevice _InputDevice = null;

        public Character SelectedCharacrter => (Character)_SelectIndex;
        private int _SelectIndex = 0;
        public bool Confirmed => _Confirmed;
        private bool _Confirmed = false;

        private InputSystemUIInputModule _UIInputModule;
        private PlayerInput _PlayerInput;


        public static Action<CharacterSelectController> OnSpawn = Controller => {};
        public static Action<CharacterSelectController> OnControllerDestroy = Controller => {};

        public Action<Character> OnSelectionChange = character => { };
        void Start()
        {
            _UIInputModule = GetComponent<InputSystemUIInputModule>();
            _UIInputModule.move.action.performed += _OnUIControllerMove;
            _UIInputModule.cancel.action.performed += _OnUIControllerCancel;

            _PlayerInput = GetComponent<PlayerInput>();
            _PlayerIndex = _PlayerInput.playerIndex;

            this._InputDevice = _PlayerInput.devices.First();

            if (!GameManager.Instance.CharacterSelection.HasPlayer(_PlayerIndex))
            {
                GameManager.Instance.CharacterSelection.AddPlayer(this._PlayerIndex, _PlayerInput.devices.First());
            }

            OnSpawn.Invoke(this);
        }

        private void OnDestroy()
        {
            if (!_Confirmed)
            {
                GameManager.Instance.CharacterSelection.RemovePlayer(this._PlayerIndex);
            }

            OnControllerDestroy.Invoke(this);
        }

        private void _OnUIControllerMove(InputAction.CallbackContext Context)
        {
            var value = Context.ReadValue<Vector2>();
            _SelectIndex += (int)value.x;

            var maxValue = Enum.GetNames(typeof(Character)).Length;

            if (_SelectIndex >= maxValue)
            {
                _SelectIndex = 0;
            }
            else if (_SelectIndex < 0) _SelectIndex = maxValue -1;

            var manager = GameManager.Instance;
            var SelectionData = manager.CharacterSelection[this._PlayerIndex];
            SelectionData.Character = (Character)_SelectIndex;

            GameManager.Instance.CharacterSelection[this._PlayerIndex] = SelectionData;
            OnSelectionChange.Invoke((Character)_SelectIndex);
        }

        private void _OnUIControllerCancel(InputAction.CallbackContext Context)
        {
            if (!this.Confirmed)
            {
                GameManager.Instance.CharacterSelection.RemovePlayer(this._PlayerIndex);
                _UIInputModule.cancel.action.performed -= _OnUIControllerCancel;
                Destroy(this.gameObject);
            }
        }

#if UNITY_EDITOR
        private int GuiID = Guid.NewGuid().GetHashCode();
        private Rect _GuiRect = new Rect(20, 20, 300, 50);
        void OnGUI()
        {
            _GuiRect = GUILayout.Window(GuiID, _GuiRect, _DrawGUIWindow, $"Character Select Controller - {this._PlayerIndex}");
        }

        void _DrawGUIWindow(int WindowID)
        {
            GUILayout.Label($"Device = {_PlayerInput.devices.First().name}");
            GUILayout.Label($"Selection = {(Character)_SelectIndex}");

            GUI.DragWindow(new Rect(0, 0, float.MaxValue, float.MaxValue));
#endif
        }
    }
}