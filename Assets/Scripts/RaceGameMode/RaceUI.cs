using RippitGameManager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;


[RequireComponent(typeof(PlayableDirector))]
public class RaceUI : MonoBehaviour
{
    public RaceGameMode GameMode;
    public TMPro.TMP_Text timerText;

    [SerializeField] private Button ExitButton;
    [SerializeField] private SceneReference MainMenuScene;


    [Header("Cutscenes")]
    [SerializeField] private PlayableDirector _playableDirector;
    [SerializeField] private PlayableAsset IntroCutscene;
    [SerializeField] private PlayableAsset FinishCutscene;

    [Header("Layers")]
    [SerializeField] private CanvasSwitcher _CanvasSwitcher;
    [SerializeField] private GameObject RaceFinishLayer;

    [Header("Events")]
    [SerializeField] private UnityEvent OnStartRace = new UnityEvent();
    [SerializeField] private UnityEvent OnExitRace = new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {
        GameMode = FindAnyObjectByType<RaceGameMode>(FindObjectsInactive.Include);
    }

    // Update is called once per frame
    void Update()
    {
        timerText.text = GameMode.GetTimeString();
    }

    public void TakeControllerFocus(GameObject TargetObject)
    {
        if (!TargetObject.transform.IsChildOf(this.transform)) return;

        var playerController = PlayerUIController.All[0];
        playerController.SetPlayerRoot(this.gameObject);
        playerController.SetSelectedGameObject(TargetObject);
    }

    public void GotoRaceFinish()
    {
        _playableDirector.Play(FinishCutscene);
    }

    public void GotoMainMenu()
    {
        GameManager.Instance.LoadScene(MainMenuScene);
    }
}
