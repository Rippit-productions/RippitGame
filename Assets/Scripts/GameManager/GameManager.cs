using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using FMODUnity;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


// ALL SUBJECT TO A CODE REVIEW:
// Stephen McGuinness

/// <summary>
/// Manages the overall state of the game including pausing, volume controls, and graphic quality.
/// </summary>
/// 
public class GameManager 
{
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }

            return _instance;
        }
    }
    /// <value>Singleton instance of the game manager</value>
    private static GameManager _instance;

    public bool IsPaused => _paused;
    private bool _paused;
    /// <value>Indicates whether the game is paused</value>


    public CanvasSwitcher canvasSwitcher;
    public Toggle[] qualitySettings;
    public TMP_Dropdown resolutionDropdown;
    private List<Resolution> filteredResolutions = new List<Resolution>();

    public Action<bool> OnPause = new Action<bool>((bool IsPaused) => { });

    private static Coroutine _LoadingSceneCoroutine;

    public void TogglePause(bool Pause)
    {
        Time.timeScale = 1.0f;
        if (Pause)
        {
            Time.timeScale = 0.0f;
            OnPause(Pause);
        }
    }

    /// <summary>
    /// Sets the quality of the graphics.
    /// </summary>
    /// <param name="qualityIndex">Index representing the desired quality level</param>
    public void SetQuality(int qualityIndex) {
        for (int i = 0; i < qualitySettings.Length; i++) 
        { 
            if (i != qualityIndex) qualitySettings[i].isOn = false; 
        } 
        QualitySettings.SetQualityLevel(qualityIndex, true); 
    }

    /// <summary>
    /// Sets the resolution of the game.
    /// </summary>
    /// <param name="resolutionIndex">Index of the selected resolution</param>
    public void SetResolution(int resolutionIndex) => Screen.SetResolution(filteredResolutions[resolutionIndex].width, filteredResolutions[resolutionIndex].height, Screen.fullScreen);
}
