using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider _MasterVolSlider;
    [SerializeField] private Slider _MusicVolSlider;
    [SerializeField] private Slider _SFXVolSlider;

    [SerializeField] private Button _SaveButton;
    // Use this for initialization
    void Start()
    {
        _MasterVolSlider.onValueChanged.AddListener(newValue =>
        {
            GameAudio.AudioSettings.Instance.MasterVolume = newValue;
        });

        _MusicVolSlider.onValueChanged.AddListener(newValue =>
        {
            GameAudio.AudioSettings.Instance.MusicVolume = newValue;
        });

        _SFXVolSlider.onValueChanged.AddListener(newValue =>
        {
            GameAudio.AudioSettings.Instance.SFXVolume = newValue;
        });

        _SaveButton.onClick.AddListener(() => {
            GameAudio.AudioSettings.Instance.SaveSettings();
        }); 
    }
}
