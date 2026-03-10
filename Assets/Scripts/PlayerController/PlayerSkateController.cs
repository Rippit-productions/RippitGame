using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;



// PlayerController - to bind input to Actions : eddie
// Uses Unity Input System | https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/QuickStartGuide.html

namespace PlayerControllerInput
{
    public struct ControlStickButtonSet
    {
        public int Up;
        public int Down;
        public int Left;
        public int Right;
    }
    
    public class PlayerControllerButton
    {
        private string _name;
        public PlayerControllerButton(string name, Key KeyboardKey, GamepadButton gamepadButton)
        {
            _keyboardKey = KeyboardKey;
            _gamepadButton = gamepadButton;
        }

        private bool _state;
        private bool _pressed;
        private bool _released;

        // Public Getters
        public bool state => _state;
        public bool pressed => _pressed;
        public bool released => _released;

        //Keys linked to this Virtual Button
        private Key _keyboardKey;
        private GamepadButton _gamepadButton;

        public void ReadKeyboard()
        {
            var myKeyboard = Keyboard.current;

            _pressed = false;
            _released = false;
            _state = false;

            if (Keyboard.current[_keyboardKey].wasPressedThisFrame)
            {
                _pressed = true;

            }
            else if (myKeyboard[_keyboardKey].wasReleasedThisFrame)
            {
                _released = true;
            }
            _state = myKeyboard[_keyboardKey].isPressed;
        }
        // To be used in Key-rebinding (stretch goal)
        public void ChangeKey(Key KeyboardKey, GamepadButton gamepadButton)
        {
            _keyboardKey = KeyboardKey;
            _gamepadButton = gamepadButton;
        }
    }


}

public class PlayerSkateController : MonoBehaviour
{
    public PlayerControllerInput.PlayerControllerButton Jump;
    
    private Vector2 _moveVector = new Vector2();
    public Vector2 moveVector => _moveVector;
   
    private void Start()
    {
        Jump = new PlayerControllerInput.PlayerControllerButton("Jump", Key.Space, GamepadButton.A);
        PlayerControllerInput.ControlStickButtonSet VirtualStickButton;
        VirtualStickButton.Up = (int)Key.I;
        VirtualStickButton.Down = (int)Key.K;
        VirtualStickButton.Left = (int)Key.J;
        VirtualStickButton.Right = (int)Key.L;

        // Hacks to get the pause button to work
        GameObject pauseButton = GameObject.Find("PauseButton");
        if (pauseButton != null)
        {
            pauseButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(GameManager.instance.TogglePause);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ReadKeyboard();
    }
    
    // To check if Input Device is Active
    // E.g There is any key press or movement
    private bool IsDeviceActive(InputDevice Device)
    {
        Keyboard isKeyboard = Device as Keyboard;
        Gamepad isGamepad = Device as Gamepad;

        // Device is keyboard
        if (isKeyboard != null)
        {
            foreach (int key in Enum.GetValues(typeof(Key)))
            {
                if (isKeyboard[(Key)key].isPressed)
                {
                    return true;
                }
            }
        }
        //Device is gamepad
        else if (isGamepad != null)
        {
            // Check Buttons
            foreach (int button in Enum.GetValues(typeof(GamepadButton)))
            {
                if (isGamepad[(GamepadButton)button].isPressed)
                {
                    return true;
                }
            }
            
            // Check Stick Value
            if (isGamepad.leftStick.EvaluateMagnitude() > 0.0f ||
                isGamepad.rightStick.EvaluateMagnitude() > 0.0f
               )
            {
                return true;
            }
        }

        // The device is not active in anyway. Return false
        return false;
    }
    
    // Read Input front keyboard and trigger actions
    private void ReadKeyboard()
    {
        Jump.ReadKeyboard();
        if (Keyboard.current == null) return;
        Keyboard MyKeyboard = Keyboard.current;

        var leftKey = MyKeyboard.aKey.isPressed || MyKeyboard.leftArrowKey.isPressed;
        var rightKey = MyKeyboard.dKey.isPressed || MyKeyboard.rightArrowKey.isPressed;
        var upKey = MyKeyboard.wKey.isPressed || MyKeyboard.upArrowKey.isPressed;
        var downKey = MyKeyboard.sKey.isPressed || MyKeyboard.downArrowKey.isPressed;
        var espKey = MyKeyboard.escapeKey.isPressed;
        
        _moveVector = new Vector2(0, 0);
        if (leftKey)
        {
            _moveVector.x = -1;
        }
        else if (rightKey)
        {
            _moveVector.x = 1;
        }

        if (upKey)
        {
            _moveVector.y = -1;
        }
        else if (downKey)
        {
            _moveVector.y = 1;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
             GameManager.instance.TogglePause();
        }

    }
}
