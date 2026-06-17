using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaceUI : MonoBehaviour
{
    public RaceGameMode GameMode;
    public TMPro.TMP_Text timerText;

    [SerializeField]private GameObject ExitUIObject;

    public UnityEvent OnRaceFinish = new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {
        GameMode = FindAnyObjectByType<RaceGameMode>(FindObjectsInactive.Include);
        GameMode.OnRaceFinish.AddListener( () => { this.OnRaceFinish.Invoke(); } );
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
}
