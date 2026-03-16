using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level GetInstance() => FindFirstObjectByType<Level>(FindObjectsInactive.Exclude);

    [SerializeField]private FMODUnity.EventReference _Song;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayAudioInstance(_Song,AudioManager.AudioType.Music);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
