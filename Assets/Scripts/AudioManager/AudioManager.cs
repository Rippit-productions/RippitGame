using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager 
{
    private static AudioManager _Instance;
    public static AudioManager Instance
    {
        get
        {
            if (_Instance == null) _Instance = new AudioManager();
            return _Instance;
        }
    }

    private List<EventInstance> _Events = new List<EventInstance>();

    public void PlayOneShot(EventReference Sound, Vector3 WorldPosition)
    {
        FMODUnity.RuntimeManager.PlayOneShot(Sound, WorldPosition);
    }

    /// <summary>
    /// Create Audio Instance and Play immediate.
    /// Instance will be removed by Garbage Collector
    /// </summary>
    public EventInstance PlayAudioInstance(EventReference audioEventReference)
    {
        if (audioEventReference.IsNull)
        {
            Debug.LogWarning("No FMOD Audio reference is NULL");
            return new EventInstance();
        }
        EventInstance eventI = this.CreateAudioInstance(audioEventReference);
        _Events.Add(eventI);
        eventI.start();
        return eventI;
    }

    public EventInstance CreateAudioInstance(EventReference audioEventReference)
    {
        EventInstance eventI = RuntimeManager.CreateInstance(audioEventReference);
        return eventI;
    }

    private void CleanUp()
    {
        foreach (EventInstance eventI in _Events) {
            eventI.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventI.release();
            _Events.Remove(eventI);
        }
    }

    private void OnDestroy() => CleanUp();
}
