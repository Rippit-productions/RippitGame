using GameAudio;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Skater;


public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider _MasterVolSlider;
    [SerializeField] private Slider _MusicVolSlider;
    [SerializeField] private Slider _SFXVolSlider;

    [SerializeField] private Button _SaveButton;
    // Use this for initialization
    void Start()
    {
        _MasterVolSlider.value = GameAudio.AudioSettings.Instance.MasterVolume;
        _MasterVolSlider.onValueChanged.AddListener(newValue =>
        {
            GameAudio.AudioSettings.Instance.MasterVolume = newValue;

            foreach (var audio in AudioEvent.GetAudioOfType(AudioEventType.Music))
            {
                audio.SetVolume(GameAudio.AudioSettings.Instance.GetMusicVolume());
            }
        });


        _MusicVolSlider.value = GameAudio.AudioSettings.Instance.MusicVolume;
        _MusicVolSlider.onValueChanged.AddListener(newValue =>
        {
            GameAudio.AudioSettings.Instance.MusicVolume = newValue;

            foreach (var audio in AudioEvent.GetAudioOfType(AudioEventType.Music))
            {
                audio.SetVolume(GameAudio.AudioSettings.Instance.GetMusicVolume());
            }
        });

        _SFXVolSlider.value = GameAudio.AudioSettings.Instance.SFXVolume;
        _SFXVolSlider.onValueChanged.AddListener(newValue =>
        {
            GameAudio.AudioSettings.Instance.SFXVolume = newValue;
        });

        _SaveButton.onClick.AddListener(() => {
            GameAudio.AudioSettings.Instance.SaveSettings();
        }); 
    }


    private void RefreshAudioVolume()
    {

    }

#if UNITY_EDITOR
    private int GuiID = Guid.NewGuid().GetHashCode();
    private Rect _GuiRect = new Rect(20, 20, 300, 200);
    void OnGUI()
    {
        if (this.enabled)
        {
            _GuiRect = GUILayout.Window(GuiID, _GuiRect, _DrawGUIWindow, $"Settings Menu - {this.gameObject.name}");
        }
    }
    void _DrawGUIWindow(int WindowID)
    {
        GUILayout.Label($"Master Volume: {_MasterVolSlider.value}");
        GUILayout.Label($"Music: {_MusicVolSlider.value}");
        GUILayout.Label($"SFX Volume: {_SFXVolSlider.value}");
        GUI.DragWindow(new Rect(0, 0, float.MaxValue, float.MaxValue));
    }
#endif
}
