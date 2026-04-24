using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    [SerializeField] private FMODUnity.EventReference _MusicTrack;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        AudioManager.Instance.PlayAudioInstance(_MusicTrack, AudioManager.AudioType.Music);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
