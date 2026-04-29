using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Track))]
public class RaceGameMode : MonoBehaviour
{
    public const int MaxPlayerCount = 4;
    public static RaceGameMode Instance => FindFirstObjectByType<RaceGameMode>();
    public enum RaceState
    {
        InProgress,
        Finished
    }

    private RaceState _State = RaceState.InProgress;
    public RaceState Stae => _State;

    public const string FMODParam_RaceEnd = "RaceEnd";


    [Header("Events")]
    public UnityEvent OnPlayerFinish = new UnityEvent();
    public UnityEvent OnRaceFinish = new UnityEvent();

    private float _timer;
    public float Timer => _timer;

    private Track _track;
    public struct PlayerInfo
    {
        public Skater SkaterComponent;
        public int Lap;
        public int TargetCheckPoint;
        public float Completion;
        public bool Finished;
        public float FinalTime;

        public string GetTimeString()
        {
            var timespan = System.TimeSpan.FromSeconds(FinalTime);
            var resultString = timespan.Duration().ToString(@"mm\:ss\.ff");

            return resultString; 
        }
    }

    List<PlayerInfo> _Players = new List<PlayerInfo>();

    private void Awake()
    {
        // Add Player to tracker
        Skater.OnSkaterSpawn += (Skater newPlayer) =>
        {
            _Players.Add(new PlayerInfo
            {
                SkaterComponent = newPlayer,
                Lap = 0,
                TargetCheckPoint = 0,
                Finished = false
            });
        };

        Skater.OnSkateDestroy += (Skater) =>
        {
            for (int i = 0; i < _Players.Count; i++)
            {
                if (_Players[i].SkaterComponent == Skater)
                {
                    _Players.RemoveAt(i);
                }
            }
        };


    }

    void Start()
    {
        _track = GetComponent<Track>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_State)
        {
            case RaceState.InProgress:
                _timer += Time.deltaTime;
                UpdateLeaderboard();

                if (IsRaceFinished())
                {
                    Level.GetInstance().LevelMusic.SetParam(FMODParam_RaceEnd, 1.0f);
                    this._State = RaceState.Finished;
                    this.OnRaceFinish.Invoke();
                }
                break;

            default:
                break;
        }
    }

    public bool IsRaceFinished()
    {
        return !_Players.Where(p => p.Finished == false).Any();
    }

    public string GetTimeString()
    {
        var timespan = System.TimeSpan.FromSeconds(_timer);
        var timerString = timespan.Duration().ToString(@"mm\:ss\.ff");
        return timerString;
    }

    public override string ToString() => GetTimeString();

    private void UpdateLeaderboard()
    {
        for (int i = 0; i < _Players.Count; i++)
        {
            var playerPosition = _Players[i].SkaterComponent.transform.position;
            var checkPointIndex = _Players[i].TargetCheckPoint;


            var info = _Players[i];
            if (_track.PointOverlapsCheckPoint(playerPosition, checkPointIndex))
            {
                info.TargetCheckPoint += 1;
                if (info.TargetCheckPoint >= _track.CheckPoints.Length)
                {
                    info.Lap += 1;
                    info.TargetCheckPoint = 0;
                }

                if (info.Lap == _track.Laps)
                {
                    info.Finished = true;
                    info.FinalTime = _timer;
                }
            }
            float ProgressValue = info.Lap;
            ProgressValue += _track.GetPointOnSpline(
                info.SkaterComponent.transform.position
                ).NPosition;
            info.Completion = ProgressValue / _track.Laps;
            _Players[i] = info;
        }

    }

    public PlayerInfo[] GetLeaderboard()
    {
        var finishedPlayers = this._Players.Where(p => p.Finished == true).OrderBy(p => p.FinalTime);
        var unfinishedPlayers = this._Players.Where(p => p.Finished == false)
            .OrderBy(playerInfo => playerInfo.Completion)
            .Reverse().ToArray();

        var leaderboard = new List<PlayerInfo>();
        leaderboard.AddRange(finishedPlayers);
        leaderboard.AddRange(unfinishedPlayers);
        return leaderboard.ToArray();
    }
    public PlayerInfo GetPlayerRaceInfo(Skater player)
    {
        var board = GetLeaderboard();
        return board.Where(info => info.SkaterComponent == player).FirstOrDefault();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (var player in _Players) 
        {
            var playerPos = player.SkaterComponent.transform.position;
            var targetCheckPoint = _track.GetCheckPointPosition(player.TargetCheckPoint);

            var DirectionVector = (targetCheckPoint - playerPos).normalized;

            Gizmos.color = Color.green;
            Vector3 LineStart = playerPos + (Vector3)Vector2.up * 2.0f;
            Gizmos.DrawLine(LineStart, LineStart + DirectionVector );
        }
    }

    private int GuiID = Guid.NewGuid().GetHashCode();
    private Rect _GuiRect = new Rect(20, 100, 300, 50);

    private void OnGUI()
    {
        _GuiRect = GUILayout.Window(GuiID, _GuiRect, _DrawGUIWindow, $"Race GameMode  {this.gameObject.name}");
    }

    private void _DrawGUIWindow(int WindowID)
    {
        GUILayout.Label($"Time - {GetTimeString()}");
        GUILayout.Label($"Track Length : Laps {_track.Laps} | CheckPoints {_track.CheckPoints.Length} ");

        var leaderboard = GetLeaderboard();
        for (int i = 0; i < leaderboard.Length; i++) {

            var playerName = leaderboard[i].SkaterComponent.gameObject.name;
            var lapNum = leaderboard[i].Lap;
            var CheckPointNum = leaderboard[i].TargetCheckPoint;

            string PlayerString = $"{i + 1}. {playerName} | Lap {lapNum} | CheckPoint {CheckPointNum}|  {leaderboard[i].Completion.ToString().Truncate(4)}%";

            if (leaderboard[i].Finished)
            {
                PlayerString = $"{i + 1}.{playerName} | {leaderboard[i].GetTimeString()} ";
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(PlayerString);
            if (GUILayout.Button("Clone"))
            {
                var toCopy = leaderboard[i].SkaterComponent.gameObject;
                var newPlayer = GameObject.Instantiate(toCopy.gameObject);
                newPlayer.transform.position = toCopy.transform.position;
            }
            if (GUILayout.Button("Delete"))
            {
                if (_Players.Count <= 1) return;
                var toDelete = leaderboard[i].SkaterComponent.gameObject;
                GameObject.Destroy(toDelete);
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Player"))
        {
            if (Skater.All.Length == 0) return;

            var ToCopy = Skater.All.First();
            var newPlayer = GameObject.Instantiate(ToCopy.gameObject);
            newPlayer.transform.position = ToCopy.transform.position + Vector3.up * 1.0f;
        }

        GUI.DragWindow(new Rect(0, 0, float.MaxValue, float.MaxValue));
    }
#endif
}
