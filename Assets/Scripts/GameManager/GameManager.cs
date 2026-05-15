using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;

public enum GameMode
{
    Race,
    Practise,
}

public class GameManager : MonoBehaviour
{
    public static GameMode Mode = GameMode.Race;
    public const int MaxPlayerCount = 6;
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
