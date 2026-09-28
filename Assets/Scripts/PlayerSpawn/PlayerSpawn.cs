using CharacterSelect;
using RippitGameManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

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

        [SerializeField] private GameObject PlayerControllerPrefab;
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
            var selection = PrefabOptions.Where(p => p.CharacterName == character).FirstOrDefault().Prefab;
            if (selection == null) return PrefabOptions[0].Prefab;
            return selection;
        }

        public GameObject[] SpawnPlayers()
        {
            List<GameObject> spawnedObjects = new List<GameObject>();

            // Default: No character selection done. Spawn 1 player.
            if (GameManager.Instance.CharacterSelection.Count == 0)
            {
                var controller = GameObject.Instantiate(PlayerControllerPrefab).GetComponent<PlayerController>();

                var newPlayerObj = GameObject.Instantiate(PrefabOptions[0].Prefab);
                newPlayerObj.transform.position = transform.position;

                var Skater = newPlayerObj.GetComponent<Skater>();
                Skater.BindToController(controller);

                spawnedObjects.Add(newPlayerObj);
            }
            else
            {
                foreach (KeyValuePair<int,PlayerCharacterSelection> selection in GameManager.Instance.CharacterSelection)
                {
                    var PlayerPrefab = GetPrefabForCharacter(selection.Value.Character);
                    var InputDevices = selection.Value.InputDevices;
                    var playerIndex = selection.Key;

                    var playerController = PlayerController.Instantiate(PlayerControllerPrefab, playerIndex, InputDevices);
                    var newSkater = GameObject.Instantiate(PlayerPrefab).GetComponent<Skater>();
                    newSkater.BindToController(playerController);

                    newSkater.gameObject.transform.position = transform.position;
                    spawnedObjects.Add(newSkater.gameObject);
                }
            }
            return spawnedObjects.ToArray();
        }

    }
}