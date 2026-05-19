using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;
using System.Collections.Generic;
using CharacterSelect;
using NUnit.Framework.Constraints;
using UnityEngine.InputSystem;

public enum GameMode
{
    Race,
    Practise,
}

namespace CharacterSelect
{
    [Serializable]
    public struct PlayerCharacterSelection
    {
        public InputDevice InputDevice;
        public GameObject CharacterPrefab;

        public override string ToString()
        {
            string objname = CharacterPrefab? CharacterPrefab.name: "None";
            return $"{InputDevice.path},{objname}";
        }
    }

    public class Dictionary
    {
        public Dictionary<int, PlayerCharacterSelection> _Selection = new Dictionary<int, PlayerCharacterSelection>();

        public bool AddPlayer(int PlayerIndex, InputDevice inputDevice)
        {
            if (_Selection.ContainsKey(PlayerIndex)) return false;
            var newData = new PlayerCharacterSelection()
            {
                CharacterPrefab = null,
                InputDevice = inputDevice
            };
            _Selection.Add(PlayerIndex, new PlayerCharacterSelection());

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
        }
        public bool HasPlayer(int playerIndex) => _Selection.ContainsKey(playerIndex);
    }

}

namespace RippitGameManager
{

    public class GameManager : MonoBehaviour
    {
        //Singleton
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("GameManager").AddComponent<GameManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
        private static GameManager _instance;


        public GameMode Mode = GameMode.Race;
        public const int MaxPlayerCount = 6;

        public CharacterSelect.Dictionary CharacterSelection = new CharacterSelect.Dictionary();

        //Events
        public Action<bool> OnPause = new Action<bool>((bool IsPaused) => { });

        public bool IsPaused => _paused;
        private bool _paused;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            if (_instance == null)
            {
                _instance = GetComponent<GameManager>();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void QuitGame() => Application.Quit();

#if UNITY_EDITOR
        [MenuItem("GameObject/GameManager", false, 10)]
        public static void _EditorCreateGameManager()
        {
            if (FindFirstObjectByType<GameManager>())
            {
                return;
            }
            var newObj = new GameObject("GameManager");
            newObj.AddComponent<GameManager>();
        }
#endif

        public void TogglePause(bool Pause)
        {
            Time.timeScale = 1.0f;
            if (Pause)
            {
                Time.timeScale = 0.0f;
                OnPause(Pause);
            }
        }


        public void LoadScene(SceneAsset Scene)
        {
            StartCoroutine(SceneLoader.LoadScene(Scene.name));
        }
    }
} 
