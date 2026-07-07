using UnityEngine;
using GameAudio;

namespace GameLevel
{
    public class Level : MonoBehaviour
    {
        public static Level GetInstance() => FindFirstObjectByType<Level>(FindObjectsInactive.Exclude);

        [SerializeField] private FMODUnity.EventReference _Song;
        [SerializeField] private FMODUnity.EventReference _AmbienceTrack;

        private GameAudio.AudioEvent _LevelMusic;
        public GameAudio.AudioEvent LevelMusic => _LevelMusic;

        // Start is called before the first frame update
        void Start()
        {
            _LevelMusic = GameAudio.AudioEvent.Instansiate(_Song, GameAudio.AudioEventType.Music);
            _LevelMusic.Play();
            _LevelMusic.SetVolume(GameAudio.AudioSettings.Instance.GetMusicVolume());
        }
    }
}
