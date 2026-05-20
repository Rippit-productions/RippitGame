using UnityEngine;

[RequireComponent(typeof(Skater))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody2D))]
public class SkaterAirTrickController : MonoBehaviour
{
    public enum TrickSessionState
    {
        Idle,
        Active,
        Completed,
        Failed
    }

    [Header("Combos")]
    [SerializeField] private TrickCombo[] _Combos;
    [SerializeField, Range(0.2f, 3.0f)] private float _ComboTimeLimit = 1.25f;
    [SerializeField, Range(0.1f, 1.0f)] private float _ResultDisplayTime = 0.45f;
    [SerializeField, Range(0.2f, 1.0f)] private float _InputThreshold = 0.65f;

    private Skater _skater;
    private PlayerController _playerController;
    private Rigidbody2D _rigidbody;

    private TrickCombo _currentCombo;
    private TrickSessionState _state = TrickSessionState.Idle;
    private Vector2 _lastMoveInput;
    private float _timeRemaining;
    private float _resultTimer;
    private int _comboIndex;
    private int _currentStepIndex;
    private bool _wasAirborne;

    public TrickCombo CurrentCombo => _currentCombo;
    public TrickSessionState State => _state;
    public int CurrentStepIndex => _currentStepIndex;
    public float TimeRemaining => _timeRemaining;
    public float TimeLimit => _ComboTimeLimit;
    public bool IsAirborne => _skater != null && _skater.State == Skater.SkaterState.Jumping;
    public bool HasVisiblePrompt => _state == TrickSessionState.Active || _resultTimer > 0.0f;

    private void Awake()
    {
        _skater = GetComponent<Skater>();
        _playerController = GetComponent<PlayerController>();
        _rigidbody = GetComponent<Rigidbody2D>();

        if (_Combos == null || _Combos.Length == 0)
        {
            _Combos = CreateDefaultCombos();
        }
    }

    private void Update()
    {
        bool airborne = IsAirborne;
        if (airborne && !_wasAirborne)
        {
            BeginAirTrick();
        }
        else if (!airborne && _wasAirborne)
        {
            CancelAirTrick();
        }

        _wasAirborne = airborne;
        if (!airborne)
        {
            _lastMoveInput = _playerController.Move;
            return;
        }

        TickResultDisplay();

        if (_state == TrickSessionState.Active)
        {
            TickActiveCombo();
        }

        _lastMoveInput = _playerController.Move;
    }

    private void BeginAirTrick()
    {
        _currentCombo = GetNextCombo();
        if (_currentCombo == null || !_currentCombo.IsValid)
        {
            _state = TrickSessionState.Idle;
            return;
        }

        _state = TrickSessionState.Active;
        _currentStepIndex = 0;
        _timeRemaining = _ComboTimeLimit;
        _resultTimer = 0.0f;
    }

    private void CancelAirTrick()
    {
        _state = TrickSessionState.Idle;
        _currentStepIndex = 0;
        _timeRemaining = 0.0f;
        _resultTimer = 0.0f;
    }

    private void TickActiveCombo()
    {
        _timeRemaining -= Time.deltaTime;
        if (_timeRemaining <= 0.0f)
        {
            FailCombo();
            return;
        }

        TrickDirection? pressedDirection = ReadPressedDirection();
        if (!pressedDirection.HasValue)
        {
            return;
        }

        TrickDirection expectedDirection = _currentCombo.GetStep(_currentStepIndex);
        if (pressedDirection.Value != expectedDirection)
        {
            FailCombo();
            return;
        }

        _currentStepIndex += 1;
        if (_currentStepIndex >= _currentCombo.StepCount)
        {
            CompleteCombo();
        }
    }

    private void CompleteCombo()
    {
        _state = TrickSessionState.Completed;
        _timeRemaining = 0.0f;
        _resultTimer = _ResultDisplayTime;
    }

    private void FailCombo()
    {
        _state = TrickSessionState.Failed;
        _timeRemaining = 0.0f;
        _resultTimer = _ResultDisplayTime;
    }

    private void TickResultDisplay()
    {
        if (_resultTimer <= 0.0f)
        {
            return;
        }

        _resultTimer -= Time.deltaTime;
        if (_resultTimer <= 0.0f)
        {
            _state = TrickSessionState.Idle;
        }
    }

    private TrickDirection? ReadPressedDirection()
    {
        Vector2 moveInput = _playerController.Move;
        bool horizontalPress = Mathf.Abs(moveInput.x) >= _InputThreshold && Mathf.Abs(_lastMoveInput.x) < _InputThreshold;
        bool verticalPress = Mathf.Abs(moveInput.y) >= _InputThreshold && Mathf.Abs(_lastMoveInput.y) < _InputThreshold;

        if (!horizontalPress && !verticalPress)
        {
            return null;
        }

        if (horizontalPress && (!verticalPress || Mathf.Abs(moveInput.x) >= Mathf.Abs(moveInput.y)))
        {
            return moveInput.x > 0.0f ? TrickDirection.Right : TrickDirection.Left;
        }

        return moveInput.y > 0.0f ? TrickDirection.Up : TrickDirection.Down;
    }

    private TrickCombo GetNextCombo()
    {
        if (_Combos == null || _Combos.Length == 0)
        {
            return null;
        }

        TrickCombo combo = _Combos[_comboIndex % _Combos.Length];
        _comboIndex += 1;
        return combo;
    }

    private TrickCombo[] CreateDefaultCombos()
    {
        return new[]
        {
            new TrickCombo("Pop Shuv", TrickDirection.Up, TrickDirection.Right, TrickDirection.Down),
            new TrickCombo("Kickflip", TrickDirection.Left, TrickDirection.Right, TrickDirection.Up),
            new TrickCombo("Manual Twist", TrickDirection.Down, TrickDirection.Left, TrickDirection.Up),
            new TrickCombo("Rocket Air", TrickDirection.Up, TrickDirection.Down, TrickDirection.Right)
        };
    }
}
