using RippitGameManager;
using UnityEngine;
using UnityEngine.UI;

public class RaceUI : MonoBehaviour
{
    [SerializeField] private RaceGameMode GameMode;
    public TMPro.TMP_Text timerText;

    [SerializeField] private Button ExitButton;
    [SerializeField] private SceneReference MainMenuScene;

    [Header("Layers")]
    [SerializeField] private CanvasSwitcher _CanvasSwitcher;
    [SerializeField] private GameObject RaceIntroLayer;
    [SerializeField] private GameObject RaceFinishLayer;


    private void Awake()
    {
        this.GameMode = FindFirstObjectByType<RaceGameMode>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GameMode == null) return;
        timerText.text = GameMode.GetTimeString();
    }

    public void TakeControllerFocus(GameObject TargetObject)
    {
        if (!TargetObject.transform.IsChildOf(this.transform)) return;

        var playerController = PlayerController.All[0];
        playerController.SetUISelectedGameObject(TargetObject);
    }

    public void GotoRaceIntro()
    {
        this._CanvasSwitcher.SwitchToObject(RaceIntroLayer);
    }

    public void GotoRaceFinish()
    {
        this._CanvasSwitcher.SwitchToObject(RaceFinishLayer);
        TakeControllerFocus(ExitButton.gameObject);
    }

    public void GotoMainMenu()
    {
        GameManager.Instance.LoadScene(MainMenuScene);
    }
}
