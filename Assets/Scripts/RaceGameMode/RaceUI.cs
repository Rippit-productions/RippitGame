using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceUI : MonoBehaviour
{
    public RaceGameMode GameMode;
    public TMPro.TMP_Text timerText;
    // Start is called before the first frame update
    void Start()
    {
        GameMode = FindAnyObjectByType<RaceGameMode>(FindObjectsInactive.Include);
    }

    // Update is called once per frame
    void Update()
    {
        timerText.text = GameMode.ToString();
    }
}
