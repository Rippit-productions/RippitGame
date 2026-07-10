using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using GameAudio;
using RippitGameManager;

public class MainMenu : Menu
{
    private bool FirstClick = false;
    [SerializeField] private FMODUnity.EventReference _MusicTrack;
    private AudioEvent BGMusic;

    [SerializeField] private SceneReference _CharacterSelectScene;
    [SerializeField] private SceneReference _GreyboxScene;
    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        InputSystemUIInputModule InputModule = FindFirstObjectByType<InputSystemUIInputModule>();
        InputModule.leftClick.action.performed += ClickActionCallback;
        yield break;
    }

    private void ClickActionCallback(InputAction.CallbackContext callbackContext)
    {
        if (BGMusic != null) return;
        if (callbackContext.phase == InputActionPhase.Performed)
        {
            FirstClick = true;
            BGMusic = GameAudio.AudioEvent.Instansiate(_MusicTrack, GameAudio.AudioEventType.Music);
            BGMusic.Play();
        }
    }

    public void GotoCharacterSelect()
    {
        GameManager.Instance.LoadScene(_CharacterSelectScene);
    }

    public void GotoRaceMode()
    {
        GameManager.Instance.LoadScene(_GreyboxScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
