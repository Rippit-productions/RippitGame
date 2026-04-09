using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level GetInstance() => FindFirstObjectByType<Level>(FindObjectsInactive.Exclude);

    [SerializeField]private FMODUnity.EventReference _Song;
    [SerializeField] private FMODUnity.EventReference _AmbienceTrack;

    private AudioManager.Event _LevelMusic;
    public AudioManager.Event LevelMusic => _LevelMusic;

    // Start is called before the first frame update
    void Start()
    {
        _LevelMusic = AudioManager.Instance.PlayAudioInstance(_Song,AudioManager.AudioType.Music);
        AudioManager.Instance.PlayAudioInstance(_AmbienceTrack, AudioManager.AudioType.SFX);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
