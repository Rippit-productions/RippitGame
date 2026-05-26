using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
public class Leaderboard : MonoBehaviour
{
    [SerializeField] private GameObject PinPrefab;
    // Start is called before the first frame update
    void Awake()
    {
        RaceGameMode.Instance.OnRaceFinish.AddListener(Init);
    }

    private void Init()
    {
        foreach (var skater in Skater.All)
        {
            var newGameObj = GameObject.Instantiate(PinPrefab);
            newGameObj.transform.SetParent(transform,false);
        }
        
    }
    
}
