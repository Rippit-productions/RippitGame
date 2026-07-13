using System.Collections;
using UnityEngine;

namespace GameAudio
{
    public class AudioSettings : MonoBehaviour
    {
        public const string PrefName_MasterVolume = "MasterVolume";
        public const string PrefName_MusicVolume = "MasterVolume";
        public const string PrefName_SFXVolume = "MasterVolume";


        private const string FMODParam_MasterVolume = "Master Volume";
        private const string FMODParam_MusicVolume = "Music Volume";
        private const string FMODParam_SFXVolume = "SFX Volume";

        public float MasterVolume
        {
            get
            {
                float value = 0.0f;
                FMODUnity.RuntimeManager.StudioSystem.getParameterByName(FMODParam_MasterVolume,out value);
                return value;
            }
            set
            {
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName(FMODParam_MasterVolume,value);
            }
        }
        public float MusicVolume
        {
            get
            {
                float value = 0.0f;
                FMODUnity.RuntimeManager.StudioSystem.getParameterByName(FMODParam_MusicVolume, out value);
                return value;
            }
            set
            {
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName(FMODParam_MusicVolume, value);
            }
        }
        public float SFXVolume
        {
            get
            {
                float value = 0.0f;
                FMODUnity.RuntimeManager.StudioSystem.getParameterByName(FMODParam_SFXVolume, out value);
                return value;
            }
            set
            {
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName(FMODParam_SFXVolume, value);
            }
        }

        private static AudioSettings _Instance = null;
        
        public static AudioSettings Instance
        {
            get
            {
                if (_Instance == null)
                {
                    var newObj = new GameObject();
                    newObj.name = "AudioSettings";
                    GameObject.DontDestroyOnLoad(newObj);
                    _Instance = newObj.AddComponent<AudioSettings>();
                }
                return _Instance;
            }
        }

        private void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
            else if (_Instance != this)
            {
                GameObject.Destroy(this);
            }

            MasterVolume = PlayerPrefs.GetFloat(PrefName_MasterVolume, 0.2f);
            MusicVolume = PlayerPrefs.GetFloat(PrefName_MusicVolume, 1.0f);
            SFXVolume = PlayerPrefs.GetFloat(PrefName_SFXVolume, 1.0f);
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat(PrefName_MasterVolume, MasterVolume);
            PlayerPrefs.SetFloat(PrefName_MusicVolume, MusicVolume);
            PlayerPrefs.SetFloat(PrefName_SFXVolume, SFXVolume);
            PlayerPrefs.Save();
        }
    }
}