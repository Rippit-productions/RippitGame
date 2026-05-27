using CharacterSelect;
using RippitGameManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSpawn
{
    [Serializable]
    public struct SpawnPrefab {
        public CharacterSelect.Character CharacterName;
        public GameObject Prefab; 
    }
    public class PlayerSpawn : MonoBehaviour
    {
        public bool SpawnOnStart = false;

        [SerializeField] private SpawnPrefab[] PrefabOptions;
        // Use this for initialization
        void Start()
        {
            if (SpawnOnStart)
            {
                SpawnPlayers();
            }
        }

        public GameObject GetPrefabForCharacter (CharacterSelect.Character character)
        {
            return PrefabOptions.Where( p => p.CharacterName == character ).FirstOrDefault().Prefab;
        }

        public void SpawnPlayers()
        {
            if (GameManager.Instance.CharacterSelection.Count == 0)
            {
                var newPlayerObj = GameObject.Instantiate(PrefabOptions[0].Prefab);
                newPlayerObj.transform.position = transform.position;
            }
            else
            {
                foreach (KeyValuePair<int,PlayerCharacterSelection> selection in GameManager.Instance.CharacterSelection)
                {
                    var toSpawn = GetPrefabForCharacter(selection.Value.Character);
                    var inputDevice = selection.Value.InputDevice;
                    var playerIndex = selection.Key;
                    var newPlayerObj = PlayerInput.Instantiate(toSpawn, playerIndex, null, -1, inputDevice).gameObject;
                    newPlayerObj.transform.position = transform.position;
                }
            }

        }

    }
}