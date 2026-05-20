using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Skater))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody2D))]
public class SkaterAirTrickController : MonoBehaviour
{
    private const float DirectionDeadzone = 0.1f;

    // Runtime UI uses this registry instead of searching the scene every frame.
    private static readonly List<SkaterAirTrickController> _activeControllers = new List<SkaterAirTrickController>();
    public static IReadOnlyList<SkaterAirTrickController> ActiveControllers => _activeControllers;

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
    [SerializeField, Range(0.2f, 2.0f)] private float _StepTimeLimit = 0.75f;
    [SerializeField, Range(0.1f, 1.0f)] private float _ResultDisplayTime = 0.45f;
    [SerializeField, Range(0.2f, 1.0f)] private float _InputThreshold = 0.65f;

    [Header("Boost")]
    [SerializeField, Range(1.0f, 3.0f)] private float _BoostMultiplier = 1.3f;
    [SerializeField, Range(0.1f, 3.0f)] private float _BoostDuration = 0.85f;
    [SerializeField, Range(0.0f, 80.0f)] private float _BoostFallForce = 28.0f;
    [SerializeField, Range(0.0f, 100.0f)] private float _MinimumBoostSpeed = 18.0f;
    [SerializeField, Range(1.0f, 120.0f)] private float _MaximumBoostSpeed = 65.0f;

    private Skater _skater;
    private PlayerController _playerController;
    private Rigidbody2D _rigidbody;

    private TrickCombo _currentCombo;
    private TrickSessionState _state = TrickSessionState.Idle;
    private Vector2 _lastMoveInput;
    private float _timeRemaining;
    private float _stepTimeRemaining;
    private float _resultTimer;
    private float _boostTimer;
    private float _boostStartSpeed;
    private float _boostEndSpeed;
    private float _boostDirection = 1.0f;
    private int _comboIndex;
    private int _currentStepIndex;
    private bool _wasAirborne;

    public TrickCombo CurrentCombo => _currentCombo;
    public TrickSessionState State => _state;
    public int CurrentStepIndex => _currentStepIndex;
    public float TimeRemaining => _timeRemaining;
    public float TimeLimit => _ComboTimeLimit;
    public float StepTimeRemaining => _stepTimeRemaining;
    public float StepProgress => _StepTimeLimit <= 0.0f ? 0.0f : Mathf.Clamp01(_stepTimeRemaining / _StepTimeLimit);
    public float CurrentSpeed => _rigidbody == null ? 0.0f : _rigidbody.velocity.magnitude;
    public float BoostTimeRemaining => _boostTimer;
    public float BoostProgress => _BoostDuration <= 0.0f ? 0.0f : Mathf.Clamp01(_boostTimer / _BoostDuration);
    public float BoostStartSpeed => _boostStartSpeed;
    public float BoostEndSpeed => _boostEndSpeed;
    public bool IsBoosting => _boostTimer > 0.0f;
    public bool IsAirborne => _skater != null && _skater.State == Skater.SkaterState.Jumping;
    public bool HasVisiblePrompt => _state == TrickSessionState.Active || _resultTimer > 0.0f || IsBoosting;

    private void Awake()
    {
        _skater = GetComponent<Skater>();
        _playerController = GetComponent<PlayerController>();
        _rigidbody = GetComponent<Rigidbody2D>();

        // Designers can override these on the prefab; defaults keep play mode usable.
        if (_Combos == null || _Combos.Length == 0)
        {
            _Combos = CreateDefaultCombos();
        }
    }

    private void OnEnable()
    {
        if (!_activeControllers.Contains(this))
        {
            _activeControllers.Add(this);
        }
    }

    private void OnDisable()
    {
        _activeControllers.Remove(this);
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

    private void FixedUpdate()
    {
        TickBoost();
    }

    private void BeginAirTrick()
    {
        _currentCombo = GetNextCombo();
        if (_currentCombo == null || !_currentCombo.IsValid)
        {
            ResetComboState(TrickSessionState.Idle);
            return;
        }

        ResetComboState(TrickSessionState.Active);
        _timeRemaining = _ComboTimeLimit;
        _stepTimeRemaining = _StepTimeLimit;
    }

    private void CancelAirTrick()
    {
        ResetComboState(TrickSessionState.Idle);
    }

    private void TickActiveCombo()
    {
        _timeRemaining -= Time.deltaTime;
        _stepTimeRemaining -= Time.deltaTime;
        if (_timeRemaining <= 0.0f || _stepTimeRemaining <= 0.0f)
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
            return;
        }

        _stepTimeRemaining = _StepTimeLimit;
    }

    private void CompleteCombo()
    {
        ResetComboState(TrickSessionState.Completed, false);
        _resultTimer = _ResultDisplayTime;
        StartBoost();
    }

    private void FailCombo()
    {
        ResetComboState(TrickSessionState.Failed, false);
        _resultTimer = _ResultDisplayTime;
    }

    private void ResetComboState(TrickSessionState state, bool resetStepIndex = true)
    {
        _state = state;
        if (resetStepIndex)
        {
            _currentStepIndex = 0;
        }
        _timeRemaining = 0.0f;
        _stepTimeRemaining = 0.0f;
        _resultTimer = 0.0f;
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

    private void StartBoost()
    {
        if (_rigidbody == null)
        {
            return;
        }

        Vector2 currentVelocity = _rigidbody.velocity;
        _boostDirection = GetBoostDirection(currentVelocity);

        // Reward forward travel only. Boosting full velocity made jumps floaty and
        // could waste the reward before the skater touched the ground.
        float currentSpeed = Mathf.Abs(currentVelocity.x);
        float boostedSpeed = Mathf.Max(currentSpeed * _BoostMultiplier, _MinimumBoostSpeed);
        boostedSpeed = Mathf.Min(boostedSpeed, _MaximumBoostSpeed);
        boostedSpeed = Mathf.Max(boostedSpeed, currentSpeed);

        _boostTimer = _BoostDuration;
        _boostStartSpeed = boostedSpeed;
        _boostEndSpeed = currentSpeed;
        _rigidbody.velocity = new Vector2(_boostDirection * boostedSpeed, currentVelocity.y);
    }

    private void TickBoost()
    {
        if (_boostTimer <= 0.0f || _rigidbody == null)
        {
            return;
        }

        if (_rigidbody.bodyType != RigidbodyType2D.Dynamic)
        {
            return;
        }

        Vector2 currentVelocity = _rigidbody.velocity;
        if (IsAirborne)
        {
            _rigidbody.AddForce(Vector2.down * _BoostFallForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }
        else
        {
            // The boost duration is ground time. Airborne tricks get a landing payoff
            // instead of spending the timer before wheels touch the track.
            _boostTimer -= Time.fixedDeltaTime;
            if (_boostTimer <= 0.0f)
            {
                return;
            }
        }

        // Boosts should carry forward momentum; vertical speed is left to gravity/fall assist.
        float boostProgress = 1.0f - Mathf.Clamp01(_boostTimer / _BoostDuration);
        float targetSpeed = Mathf.Lerp(_boostStartSpeed, _boostEndSpeed, boostProgress);
        float forwardSpeed = currentVelocity.x * _boostDirection;
        if (forwardSpeed < targetSpeed)
        {
            _rigidbody.velocity = new Vector2(_boostDirection * targetSpeed, currentVelocity.y);
        }
    }

    private float GetBoostDirection(Vector2 currentVelocity)
    {
        if (Mathf.Abs(currentVelocity.x) > DirectionDeadzone)
        {
            return Mathf.Sign(currentVelocity.x);
        }

        if (Mathf.Abs(_playerController.Move.x) > DirectionDeadzone)
        {
            return Mathf.Sign(_playerController.Move.x);
        }

        return _boostDirection;
    }

    private TrickDirection? ReadPressedDirection()
    {
        // Arrow keys are not bound to the Player Move action in this input asset,
        // so read them directly before falling back to movement input.
        TrickDirection? keyboardDirection = ReadKeyboardDirection();
        if (keyboardDirection.HasValue)
        {
            return keyboardDirection.Value;
        }

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

    private TrickDirection? ReadKeyboardDirection()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return null;
        }

        if (keyboard.upArrowKey.wasPressedThisFrame)
        {
            return TrickDirection.Up;
        }

        if (keyboard.downArrowKey.wasPressedThisFrame)
        {
            return TrickDirection.Down;
        }

        if (keyboard.leftArrowKey.wasPressedThisFrame)
        {
            return TrickDirection.Left;
        }

        if (keyboard.rightArrowKey.wasPressedThisFrame)
        {
            return TrickDirection.Right;
        }

        return null;
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
