using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(Track))]
public class RaceGameMode : MonoBehaviour
{
    public float timer;

    private Track _track;
    public struct PlayerInfo
    {
        public Skater Component;
        public int Lap;
        public int CheckPoint;
        public bool Finished;
    }

    List<PlayerInfo> _Players = new List<PlayerInfo>();

    private void Awake()
    {
        // Add Player to tracker
        Skater.OnSkaterSpawn += (newPlayer) =>
        {
            Debug.Log("RaceGameMode - Player Added");
            _Players.Add(new PlayerInfo
            {
                Component = newPlayer,
                Lap = 0,
                CheckPoint = 0,
                Finished = false
            });
        };
    }
    void Start()
    {
        _track = GetComponent<Track>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; 

        for (int i = 0; i < _Players.Count; i++)
        {
            var playerPosition = _Players[i].Component.transform.position;
            var checkPointIndex = _Players[i].CheckPoint;
            
            if (_track.PointOverlapsCheckPoint(playerPosition,checkPointIndex))
            {
                var info = _Players[i];
                info.CheckPoint += 1;
                if (info.CheckPoint >= _track.CheckPoints.Length) 
                {
                    info.Lap += 1;
                    info.CheckPoint = 0;
                }
                _Players[i] = info;
            }
        }
    }

    public PlayerInfo[] GetLeaderboard()
    {
        return this._Players.OrderBy(playerInfo =>
        {
            int lapScore = playerInfo.Lap * _track.CheckPoints.Length;
            return lapScore + playerInfo.CheckPoint;
        }).ToArray();
    }


#if UNITY_EDITOR
    private Rect _GuiRect = new Rect(20, 20, 300, 200);

    private void OnGUI()
    {
        _GuiRect = GUILayout.Window(0, _GuiRect, _DrawGUIWindow, $"Race GameMode - {this.gameObject.name}");
    }

    private void _DrawGUIWindow(int WindowID)
    {
        var timespan = System.TimeSpan.FromSeconds(timer);
        var timerString = timespan.Duration().ToString(@"mm\:ss\.fff");
        GUILayout.Label($"Time - {timerString}");
        GUI.DragWindow(new Rect(0, 0, float.MaxValue, float.MaxValue));

        var leaderboard = GetLeaderboard();
        for (int i = 0; i < leaderboard.Length; i++) {
            var playerName = leaderboard[i].Component.gameObject.name;
            var lapNum = leaderboard[i].Lap;
            var CheckPointNum = leaderboard[i].CheckPoint;
            GUILayout.Label($"{i + 1}. {playerName} | Lap {lapNum} | CheckPoint {CheckPointNum}");
        }
    }
#endif
}
