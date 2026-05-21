using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;

namespace CharacterSelect
{
    public enum Character
    {
        Terry,
        K4RMA,
        Ax,
        Manny
    }

    [Serializable]
    public struct PlayerCharacterSelection
    {
        public InputDevice InputDevice;
        public Character Character;
    }
    public class Dictionary
    {
        private Dictionary<int, PlayerCharacterSelection> _Selection = new Dictionary<int, PlayerCharacterSelection>();

        public bool AddPlayer(int PlayerIndex, InputDevice inputDevice)
        {
            var newData = new PlayerCharacterSelection()
            {
                Character = Character.Terry,
                InputDevice = inputDevice
            };
            _Selection.Add(PlayerIndex, newData);
            return true;
        }

        public void RemovePlayer(int PlayerIndex)
        {
            _Selection.Remove(PlayerIndex);
        }

        public PlayerCharacterSelection this[int PlayerIndex]
        {
            get
            {
                return _Selection[PlayerIndex];
            }
            set
            {
                _Selection[PlayerIndex] = value;
            }
        }

        public bool HasPlayer(int playerIndex) => _Selection.ContainsKey(playerIndex);
    }

}
