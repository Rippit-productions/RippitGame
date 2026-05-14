using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;



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

    public CanvasSwitcher canvasSwitcher;
    public Toggle[] qualitySettings;

    private void Start()
    {
        if (_instance == null)
        {
            _instance = GetComponent<GameManager>();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void TogglePause(bool Pause)
    {
        Time.timeScale = 1.0f;
        if (Pause)
        {
            Time.timeScale = 0.0f;
            OnPause(Pause);
        }
    }

    
}
