using UnityEngine;
using Audio;

public class Level : MonoBehaviour
{
    public static Level GetInstance() => FindFirstObjectByType<Level>(FindObjectsInactive.Exclude);

    [SerializeField]private FMODUnity.EventReference _Song;
    [SerializeField]private FMODUnity.EventReference _AmbienceTrack;

    private Audio.AudioEvent _LevelMusic;
    public Audio.AudioEvent LevelMusic => _LevelMusic;

    // Start is called before the first frame update
    void Start()
    {
        _LevelMusic = Audio.AudioEvent.Instansiate(_Song, Audio.AudioEventType.Music);
        _LevelMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
