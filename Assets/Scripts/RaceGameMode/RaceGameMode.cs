using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Track))]
public class RaceGameMode : MonoBehaviour
{
    private float _timer;
    public float Timer => _timer;

    private Track _track;
    public struct PlayerInfo
    {
        public Skater Component;
        public int Lap;
        public int CheckPoint;
        public bool Finished;
        public float FinalTime;
    }

    List<PlayerInfo> _Players = new List<PlayerInfo>();

    private void Awake()
    {
        // Add Player to tracker
        Skater.OnSkaterSpawn += (newPlayer) =>
        {
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
        _timer += Time.deltaTime; 

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

                if (info.Lap == _track.Laps)
                {
                    info.Finished = true;
                    info.FinalTime = _timer;
                }
                _Players[i] = info;
            }
        }
    }

    public string GetTimeString()
    {
        var timespan = System.TimeSpan.FromSeconds(_timer);
        var timerString = timespan.Duration().ToString(@"mm\:ss\.ff");
        return timerString;
    }

    public override string ToString() => GetTimeString();

    public PlayerInfo[] GetLeaderboard()
    {

        var finishedPlayers = this._Players.Where(p => p.Finished == true).OrderBy(p => p.FinalTime);

        var unfinishedPlayers = this._Players.Where(p => p.Finished == false).OrderBy(playerInfo =>
        {
            float PlayerValue = - (-playerInfo.Lap * _track.CheckPoints.Length) - playerInfo.CheckPoint;
            var nextCheckPointPosition = _track.GetCheckPointPosition(playerInfo.CheckPoint);
            PlayerValue += 1 / (playerInfo.Component.transform.position - nextCheckPointPosition).magnitude;
            return PlayerValue;
        }).ToArray();


        var leaderboard = new List<PlayerInfo>();
        leaderboard.AddRange(finishedPlayers);
        leaderboard.AddRange(unfinishedPlayers);

        return leaderboard.ToArray();

    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (var player in _Players) 
        {
            var playerPos = player.Component.transform.position;
            var targetCheckPoint = _track.GetCheckPointPosition(player.CheckPoint);

            var DirectionVector = (targetCheckPoint - playerPos).normalized;

            Gizmos.color = Color.green;
            Vector3 LineStart = playerPos + (Vector3)Vector2.up * 2.0f;
            Gizmos.DrawLine(LineStart, LineStart + DirectionVector );
        }
    }

    private int GuiID = Guid.NewGuid().GetHashCode();
    private Rect _GuiRect = new Rect(20, 20, 300, 50);

    private void OnGUI()
    {
        _GuiRect = GUILayout.Window(GuiID, _GuiRect, _DrawGUIWindow, $"Race GameMode  {this.gameObject.name}");
    }

    private void _DrawGUIWindow(int WindowID)
    {
        GUILayout.Label($"Time - {GetTimeString()}");

        var leaderboard = GetLeaderboard();
        for (int i = 0; i < leaderboard.Length; i++) {
            var playerName = leaderboard[i].Component.gameObject.name;
            var lapNum = leaderboard[i].Lap;
            var CheckPointNum = leaderboard[i].CheckPoint;
            GUILayout.Label($"{i + 1}. {playerName} | Lap {lapNum} | CheckPoint {CheckPointNum}");
        }

        GUI.DragWindow(new Rect(0, 0, float.MaxValue, float.MaxValue));
    }
#endif
}
