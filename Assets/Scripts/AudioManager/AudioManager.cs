using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Linq;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.SceneManagement;



public struct AudioSettings
{
    public float MasterVolume;
    public float MusicVolume;
    public float SFXVolume;

    public float GetMusicVolume() => MusicVolume * MasterVolume;
    public float GetSFXVolume() => SFXVolume * MasterVolume;
}


public class AudioManager 
{
    public class Event
    {
        public Event(EventReference fmodEventRef, AudioType AudioType)
        {
            _FmodInstance = RuntimeManager.CreateInstance(fmodEventRef);
            _AudioType = AudioType;
            SceneManager.activeSceneChanged += (oldScene, newScene) => { this.Release(); };
        }

        private void Release()
        {
            _FmodInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _FmodInstance.release();
        }

        private EventInstance _FmodInstance;

        public AudioType Type => _AudioType;
        private AudioType _AudioType;

        public void SetParam(string ParamName, float Value)
        {
            _FmodInstance.setParameterByName(ParamName, Value,true);
        }

        public void Play()
        {
            if (_FmodInstance.isValid())
            {
                _FmodInstance.start();
            }
        }

        public void Stop()
        {
            if (_FmodInstance.isValid())
            {
                _FmodInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
        }
        public void SetVolume(float volume) => _FmodInstance.setVolume(volume);
        public float GetVolume()
        {
            float value;
            _FmodInstance.getVolume(out value);
            return value;
        }
    }

    public enum AudioType
    {
        Music,
        SFX,
        Voice,
        Other
    }
    
    private static AudioManager _Instance;
    public static AudioManager Instance
    {
        get
        {
            if (_Instance == null) _Instance = new AudioManager();
            return _Instance;
        }
    }

    ~AudioManager()
    {
        CleanUp();
    }

    public AudioSettings Settings = new AudioSettings();

    private List<Event> _Events = new List<Event>();

    public void PlayOneShot(EventReference Sound, Vector3 WorldPosition)
    {
        FMODUnity.RuntimeManager.PlayOneShot(Sound, WorldPosition);
    }

    /// <summary>
    /// Create Audio Instance and Play immediate.
    /// Instance will be removed by Garbage Collector
    /// </summary>
    public AudioManager.Event PlayAudioInstance(EventReference audioEventReference,AudioType type)
    {
        if (audioEventReference.IsNull)
        {
            Debug.LogWarning("No FMOD Audio reference is NULL");
            return null;
        }
        var newEvent = new AudioManager.Event(audioEventReference, type);
        newEvent.Play();
        return newEvent;
    }

    public AudioManager.Event CreateAudioInstance(EventReference audioEventReference,AudioType type)
    {
        var newEvent = new AudioManager.Event(audioEventReference, type);
        _Events.Append(newEvent);
        return newEvent;
    }

    public void RefreshAllAudioVolume()
    {
        foreach (var musicInstance in GetAllAudioOfType(AudioType.Music))
        {
            musicInstance.SetVolume(this.Settings.GetMusicVolume());
        }
        foreach (var SFXInstance in GetAllAudioOfType(AudioType.SFX))
        {
            SFXInstance.SetVolume(this.Settings.GetSFXVolume());
        }
    }

    public Event[] GetAllAudioOfType(AudioType type)
    {
        return this._Events.Where(e =>
        {
            return e.Type == type;
        }).ToArray();
    }

    public void CleanUp() => _Events.Clear();
}
