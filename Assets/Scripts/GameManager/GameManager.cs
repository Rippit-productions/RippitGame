using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// ALL SUBJECT TO A CODE REVIEW:
// Stephen McGuinness

/// <summary>
/// Manages the overall state of the game including pausing, volume controls, and graphic quality.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <value>Singleton instance of the game manager</value>
    public static GameManager instance;

    /// <value>Indicates whether the game is paused</value>
    public bool isPaused;

    public AudioMixer audioMixer;
    public CanvasSwitcher canvasSwitcher;
    public Button resumeButton;
    public Toggle[] qualitySettings;
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public AudioSource audioSource;
    //public Button pauseButton; WON'T WORK HERE?
    private List<Resolution> filteredResolutions = new List<Resolution>();
    private int currentResolutionIndex = 0;

    /// <summary>
    /// Initialization for the game manager, sets up UI events and options.
    /// </summary>
    private void Awake()
    {
        instance ??= this; if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        resumeButton.onClick.AddListener(Resume);
        volumeSlider.onValueChanged.AddListener(SetLevel);

        for (int i = 0; i < qualitySettings.Length; i++)
        {
            int toggleIndex = i;
            qualitySettings[i].onValueChanged.AddListener(delegate { SetQuality(toggleIndex); });
            qualitySettings[i].isOn = i == QualitySettings.GetQualityLevel();
        }

        Resolution[] resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        float currentRefreshRate = Screen.currentResolution.refreshRate;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
                string option = $"{resolutions[i].width} x {resolutions[i].height}";
                options.Add(option);
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                    currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

//        audioMixer.GetFloat("MusicVol", out float ourVolume);
//        volumeSlider.value = Mathf.Pow(10, ourVolume / 20);

      
    }

/// <summary>
/// Toggles the pause state.
/// </summary>
public void TogglePause()
{
    Debug.Log("TogglePause: Entry");
    isPaused = !isPaused;
    Time.timeScale = isPaused ? 0 : 1;
    int newIndex = isPaused ? 0 : -1;
    canvasSwitcher.SetActiveIndex(newIndex);
    Debug.Log("TogglePause: Exit");

    // if (newIndex >= 0) 
    // {
    //     Canvas canvas = canvasSwitcher.transform.GetChild(newIndex).GetComponent<Canvas>();
    //     if (canvas != null)
    //     {
    //         canvas.sortingOrder = 10; 
    //     }
    // }

    // Sorting layer 2 fixes this. I'll leave this here for now in case we need it later.

}


    /// <summary>
    /// Resumes the game by toggling the pause state.
    /// </summary>
    public void Resume() => TogglePause();

    /// <summary>
    /// Sets the volume of the music using a slider value.
    /// </summary>
    /// <param name="sliderValue">The value of the volume slider</param>
    public void SetLevel(float sliderValue) => audioSource.volume = sliderValue;

    /// <summary>
    /// Sets the quality of the graphics.
    /// </summary>
    /// <param name="qualityIndex">Index representing the desired quality level</param>
    public void SetQuality(int qualityIndex) { for (int i = 0; i < qualitySettings.Length; i++) { if (i != qualityIndex) qualitySettings[i].isOn = false; } QualitySettings.SetQualityLevel(qualityIndex, true); }

    /// <summary>
    /// Sets the resolution of the game.
    /// </summary>
    /// <param name="resolutionIndex">Index of the selected resolution</param>
    public void SetResolution(int resolutionIndex) => Screen.SetResolution(filteredResolutions[resolutionIndex].width, filteredResolutions[resolutionIndex].height, Screen.fullScreen);
}
