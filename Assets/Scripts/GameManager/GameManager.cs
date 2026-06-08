using UnityEngine;
using System;
using UnityEditor;

public enum GameMode
{
    Race,
    Practise,
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
        private static GameManager _instance = null;

        public GameMode Mode = GameMode.Race;
        public const int MaxPlayerCount = 6;

        public CharacterSelect.Dictionary CharacterSelection = new CharacterSelect.Dictionary();
        public string SelectedSceneName = null;

        public bool IsPaused => _paused;
        private bool _paused = false;

        //Events
        public Action<bool> OnPause = new Action<bool>((bool IsPaused) => { });

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        public void QuitGame() => Application.Quit();

        public void TogglePause(bool Pause)
        {
            switch (_paused)
            {
                case false:
                    if (Pause == true)
                    {
                        Time.timeScale = 0.0f;
                        OnPause(Pause);
                    }
                    break;
                case true:
                    if (Pause == false)
                    {
                        Time.timeScale = 1.0f;
                        OnPause(Pause);
                    }
                    break;
            }
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(SceneLoader.LoadScene(sceneName));
        }

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


    }
} 
