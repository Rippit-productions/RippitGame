using RippitGameManager;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(PlayableDirector))]
[RequireComponent(typeof(FMODBankLoader))]
public class StartButton : MonoBehaviour
{
    SceneReference MainMenuScene;

    private Button _Button;

    [Header("Animation")]
    private PlayableDirector _PlayableDirector;
    private TimelineAsset IntroTimeLine;

    private FMODBankLoader _FMODBankLoader;
    // Use this for initialization
    void Awake()
    {
        _FMODBankLoader = GetComponent<FMODBankLoader>(); 
        _PlayableDirector = GetComponent<PlayableDirector>();
        _Button.onClick.AddListener(() => {
            _FMODBankLoader.LoadBanks();
            this._PlayableDirector.Play(IntroTimeLine);
        });
    }

    public void TimeLineWaitForFMODLoad() => StartCoroutine(_FMODWait());
    private IEnumerator _FMODWait()
    {
        _PlayableDirector.Pause();
        // Wait for FMOD Audio to load first
        if (FMODBankLoader.Loading)
        {
            yield return null;
        }
        _PlayableDirector.Play();
        yield break;
    }

    public void GotoMainMenu() => GameManager.Instance.LoadScene(MainMenuScene);
}
