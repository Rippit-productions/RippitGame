using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class MainMenu : Menu
{
    private bool FirstClick = false;
    [SerializeField] private FMODUnity.EventReference _MusicTrack;
    // Start is called before the first frame update
    private async void Start()
    {
        base.Start();
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        InputSystemUIInputModule InputModule = FindFirstObjectByType<InputSystemUIInputModule>();

        /* 
         * Google Chomr can't start Audio until the web page is clicked on. 
         * MainMenu shall wait until click to play its audio.
         * See https://docs.unity3d.com/2022.1/Documentation/Manual/webgl-audio.html
         * */
#if UNITY_WEBGL
        InputModule.leftClick.action.performed += ClickActionCallback;
        while (FMODBankLoader.Loading || !Application.isFocused || !FirstClick)
        {
            yield return null;
        }
#endif
        AudioManager.Instance.PlayAudioInstance(_MusicTrack, AudioManager.AudioType.Music);
    }


    private void ClickActionCallback(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Performed)
        {
            FirstClick = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
