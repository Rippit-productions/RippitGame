using System.Collections;
using UnityEngine;

namespace Audio
{
    public class AudioSettings
    {
        public const string PrefName_MasterVolume = "MasterVolume";
        public const string PrefName_MusicVolume = "MasterVolume";
        public const string PrefName_SFXVolume = "MasterVolume";

        public float MasterVolume;
        public float MusicVolume;
        public float SFXVolume;

        public AudioSettings()
        {
            MasterVolume = PlayerPrefs.GetFloat(PrefName_MasterVolume, 1.0f);
            MusicVolume = PlayerPrefs.GetFloat(PrefName_MusicVolume, 1.0f);
            SFXVolume = PlayerPrefs.GetFloat(PrefName_SFXVolume, 1.0f);
        }

        public float GetMasterVolume() => MasterVolume;
        public float GetMusicVolume() => MusicVolume * MasterVolume;
        public float GetSFXVolume() => SFXVolume * MasterVolume;

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat(PrefName_MasterVolume, MasterVolume);
            PlayerPrefs.SetFloat(PrefName_MusicVolume, MusicVolume);
            PlayerPrefs.SetFloat(PrefName_SFXVolume, SFXVolume);
        }
    }
}