using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class LeaderboardPin : MonoBehaviour
{
    public TMPro.TMP_Text PositionText;
    public TMPro.TMP_Text TimeText;
    public Image CharacterBanner;

    // Start is called before the first frame update
    void Start()
    {
        var Board = RaceGameMode.Instance.GetLeaderboard();
        int childIndex = transform.GetSiblingIndex();
        var playerInfo = Board[childIndex];

        this.PositionText.text = $"{childIndex + 1}.";
        if (playerInfo.DNF)
        {
            this.PositionText.text = "-";
        }
        this.TimeText.text = playerInfo.GetTimeString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
