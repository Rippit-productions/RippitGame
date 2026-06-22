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
        public static PlayerSpawn Instance => FindFirstObjectByType<PlayerSpawn>();

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

        public GameObject[] SpawnPlayers()
        {
            List<GameObject> spawnedObjects = new List<GameObject>();
            if (GameManager.Instance.CharacterSelection.Count == 0)
            {
                var newPlayerObj = GameObject.Instantiate(PrefabOptions[0].Prefab);
                newPlayerObj.transform.position = transform.position;

                var Skater = newPlayerObj.GetComponent<Skater>();
                var playerIndex = Skater.PlayerIndex;

                spawnedObjects.Add(newPlayerObj);
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
                    spawnedObjects.Add(newPlayerObj);
                }
            }
            return spawnedObjects.ToArray();
        }

    }
}