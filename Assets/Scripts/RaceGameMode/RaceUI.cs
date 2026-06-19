using RippitGameManager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RaceUI : MonoBehaviour
{
    public RaceGameMode GameMode;
    public TMPro.TMP_Text timerText;

    [SerializeField] private Button ExitButton;
    [SerializeField] private SceneReference MainMenuScene;

    public UnityEvent OnRaceFinish = new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {
        GameMode = FindAnyObjectByType<RaceGameMode>(FindObjectsInactive.Include);
        GameMode.OnRaceFinish.AddListener(() => {
            this.OnRaceFinish.Invoke();
            var uicontroller = PlayerUIController.All[0];
            uicontroller.SetPlayerRoot(this.gameObject);
            uicontroller.SetSelectedGameObject(ExitButton.gameObject);
        });
    }

    // Update is called once per frame
    void Update()
    {
        timerText.text = GameMode.GetTimeString();
    }

    public void TakeUIFocus(GameObject Target)
    {
        var controller = PlayerUIController.GetController(0);
        if (controller)
        {
            controller.SetSelectedGameObject(Target);
        }
    }


    public void GotoMainMenu()
    {
        GameManager.Instance.LoadScene(MainMenuScene);
    }
}
