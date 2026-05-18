using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


namespace CharacterSelect
{
    public class CharacterSelectBanner : MonoBehaviour
    {
        private InputSystemUIInputModule _UIInputModule;
        private PlayerInput _PlayerInput;

        public int SelectIndex => _SelectIndex;
        private int _SelectIndex = -1;
        [SerializeField] private CharacterPrefab[] Options;


        public UnityEvent OnSelectionChange = new UnityEvent();
        // Start is called before the first frame update

        public void SetController(PlayerInput InputComponent)
        {
            _PlayerInput = InputComponent;
            _UIInputModule = InputComponent.uiInputModule;

            _UIInputModule.move.action.performed += OnControllerMove;
        }

        private void OnControllerMove(InputAction.CallbackContext Context)
        {
            if (Context.phase == InputActionPhase.Started)
            {
                var moveValue = Context.ReadValue<Vector2>();
                _SelectIndex += (int)moveValue.x;

                if (_SelectIndex >= Options.Length) 
                {
                    _SelectIndex = 0;
                }
                else if (_SelectIndex < 0)
                {
                    _SelectIndex = Options.Length - 1;
                }
            }
        }
    }
}
