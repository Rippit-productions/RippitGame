using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace Audio
{
    public enum AudioEventType
    {
        Music,
        SFX,
        Voice
    }

    [RequireComponent(typeof(StudioEventEmitter))]
    public class AudioEvent : MonoBehaviour
    {
        public static AudioEvent[] All => FindObjectsByType<AudioEvent>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

        private StudioEventEmitter _FMODEmitter;

        public AudioEventType Type => _AudioType;
        private AudioEventType _AudioType;

        private void Awake()
        {
            _FMODEmitter = GetComponent<StudioEventEmitter>();
        }

        private void Update()
        {
            this.gameObject.name = $"Sound: {_FMODEmitter.EventInstance}";
        }

        public static AudioEvent Instansiate(EventReference fmodEventRef,AudioEventType AudioType)
        {
            var newGameObj = new GameObject();
            newGameObj.name = "Sound Event";
            var FMODemitter =  newGameObj.AddComponent<StudioEventEmitter>();
            var Component = newGameObj.AddComponent<AudioEvent>();
            Component._FMODEmitter = FMODemitter;

            FMODemitter.PlayEvent = EmitterGameEvent.None;
            FMODemitter.StopEvent = EmitterGameEvent.None;

            FMODemitter.EventReference = fmodEventRef;
            Component._AudioType = AudioType;
            return Component;
        }

        public void SetAudioParam(string ParamName, float Value)
        {
            _FMODEmitter.EventInstance.setParameterByName(ParamName, Value, true);
        }

        public void Play()
        {
            _FMODEmitter.Play();
        }
        public void Stop()
        {
            _FMODEmitter.Stop();
        }
        public void SetVolume(float volume)
        {
            _FMODEmitter.EventInstance.setVolume(volume);
        }
        public float GetVolume()
        {
            float value;
            _FMODEmitter.EventInstance.getVolume(out value);
            return value;
        }
    }
}
