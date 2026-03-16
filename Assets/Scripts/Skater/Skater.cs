using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;
using UnityEngine.InputSystem;


public class SkateTrick
{
    public SkateTrick() { 
    }
    public SkateTrick(string TrickName, float Score)
    {
        _Name = TrickName;
        _Score = Score;
    }
    string _Name;
    float _Score;
    public string Name => _Name;
    public float Score => _Score;
    public void SetScore(float Value)
    {
        _Score = Value;
    }
}

public struct GrindAction
{
    public bool InSplineDirection;
    public float GrindSpeed;
    public GrindRailPoint grindRailPoint; 
    public Spline PreviousSpline;
}


[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
public class Skater : MonoBehaviour
{
    public static Skater[] All => FindObjectsByType<Skater>(FindObjectsSortMode.InstanceID);

    public enum SkaterState
    {
        Grounded,
        Jumping,
        Grind,
        Trick,
        Grapple
    }

    public SkaterState State => _CharacterState;
    [field:SerializeField] protected SkaterState _CharacterState = SkaterState.Grounded;
    public bool IsGrounded => _CharacterState == SkaterState.Grounded;

    [Header("Components")]
    public SkateController playerController;
    protected Rigidbody2D _RigidBody;
    protected CircleCollider2D _CirlceCollider;

    public Animator _AnimatorComp;
    public SpriteRenderer _SpriteRenderer;

    //Animation
    public const string AnimState_Stand = "Stand";
    public const string AnimState_Skate = "Skate";
    public const string AnimState_Jump = "Jump";

    public const string AnimVar_SkateSpeed = "SkateSpeed";

    [Header("Physics")]
    [Range(0.0f,1.0f)]
    public float Friction = 0.1f;
    public float Gravity = 1.0f;

    [field:SerializeField] private float _FloorRayLength = 0.5f;
    private PhysicsMaterial2D _physicsMaterial;

    public Vector3 Upvector => _UpVector;
    private Vector3 _UpVector = Vector3.up;
    [SerializeField] private LayerMask GroundLayerMask;
    private Platform _CurrentPlatform;


    [Header ("Stats")]
    [Range(1.0f,100.0f)]
    public float Maxspeed;
    [Range(0.01f,100.0f)]
    public float MoveSpeed;
    [Range(0.01f, 2.0f)]
    public float BreakingSpeedScale;
    [Range(0.0f,20.0f)]
    public float JumpForce = 3.0f;

    private GrindAction _GrindAction = new GrindAction();


    [Header("Sounds")]
    [SerializeField] private SkaterSoundSet SoundSet;
    private AudioManager.Event JumpSFX;
    private AudioManager.Event GrindOnSFX;
    private AudioManager.Event GrindOffSFX;
    private AudioManager.Event GrindSFX;

    [Header("Camera")]
    public PlayerCamera.Setup CameraSetup;

    public void _InitRigidbody()
    {
        _RigidBody = GetComponentInChildren<Rigidbody2D>();
        _CirlceCollider = GetComponent<CircleCollider2D>();
        _physicsMaterial = new PhysicsMaterial2D("Skate Physics Material");
        _physicsMaterial.bounciness = 0.0f;
        _physicsMaterial.friction = Friction;
        _physicsMaterial.bounciness = 0.0f;
        _RigidBody.sharedMaterial = _physicsMaterial;
    }

    void Start()
    {
        _InitRigidbody();
        _SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _AnimatorComp = GetComponentInChildren<Animator>(); 
        playerController = GetComponent<SkateController>();

        PlayerCamera.CreateCamera(this);

        _InitSounds();
    }

    private void _InitSounds()
    {
        var Manager = AudioManager.Instance;
        JumpSFX = Manager.CreateAudioInstance(SoundSet.JumpSFX, AudioManager.AudioType.SFX);
        GrindOnSFX = Manager.CreateAudioInstance(SoundSet.GrindOnSFX, AudioManager.AudioType.SFX);
        GrindOffSFX = Manager.CreateAudioInstance(SoundSet.GrindOffSFX, AudioManager.AudioType.SFX);
        GrindSFX = Manager.CreateAudioInstance(SoundSet.GrindSFX, AudioManager.AudioType.SFX);
    }

    // Update is called once per frame
    void Update()
    {
        var lastPosition = this.transform.position;
        switch (_CharacterState)
        {
            case SkaterState.Grounded:
                {
                    bool solidGround = Vector2.Angle(_UpVector, Vector2.up) < 45.0f;
                    bool wallRunning = !solidGround && playerController.NoMoveInput < 0.2f && _RigidBody.velocity.magnitude >= 0.3f;
                    if (wallRunning || solidGround )
                    {
                        StickToFloor();
                    }


                    if (playerController.NoMoveInput < 0.2f || _RigidBody.velocity.magnitude < 0.5f && wallRunning)
                    {
                        StickToFloor();
                    }
                    Move(playerController.Move);
                    ApplyGravity(Vector2.down);
                    

                    var floorCast = RaycastFloor();
                    if (!floorCast.Success)
                    {
                        this._CharacterState = SkaterState.Jumping;
                    }
                    else
                    {
                        this._UpVector = floorCast.Result.normal;
                    }

                    if (playerController.Jump.WasPressedThisFrame())
                    {
                        Jump();
                        this._CharacterState = SkaterState.Jumping;

                        this.JumpSFX.Play();
                    }

                    //Animation 
                    if (this.GetNormalisedSpeed() > 0.1f && this.GetNormalisedSpeed() < 0.7f)
                    {
                        this._AnimatorComp.SetFloat(AnimVar_SkateSpeed, 1.0f);
                        this.PlayAnimationState(AnimState_Skate);
                    }
                    else
                    {
                        this.PlayAnimationState(AnimState_Stand);
                    }
                    break;
                }
            case SkaterState.Jumping:
                {
                    Move(playerController.Move);
                    ApplyGravity(Vector2.down);

                    // Add AirTime
                    _UpVector = Vector3.up;

                    if (playerController.Jump.WasReleasedThisFrame())
                    {
                        if (_RigidBody.velocity.y > 0.0f)
                        {
                            _RigidBody.velocity = new Vector2(
                                _RigidBody.velocity.x,
                                _RigidBody.velocity.y - JumpForce * 0.2f);
                        }
                    }

                    //Check / Return to Grounded State
                    var floorCast = RaycastFloor();
                    if (floorCast.Success  && _RigidBody.velocity.y < 0.0f) 
                    {
                        this._CharacterState = SkaterState.Grounded;
                        this._CurrentPlatform = floorCast.Result.collider.GetComponent<Platform>();
                        this._UpVector = floorCast.Result.normal;
                        this._GrindAction.PreviousSpline = null;
                    }

                    var collidingRailPoint = GetCollidingRail();
                    if (collidingRailPoint != null)
                    {
                        // Check if moving in Spline's Direction
                        var DotVector = Vector2.Dot(
                            _RigidBody.velocity,
                            collidingRailPoint.GetForwardVector()
                            );
                        if (DotVector > 0.0f)
                        {
                            this._GrindAction.InSplineDirection = true;
                        }
                        else
                        {
                            this._GrindAction.InSplineDirection = false;
                        }
                        this._GrindAction.GrindSpeed = _RigidBody.velocity.magnitude;
                        this._GrindAction.grindRailPoint = collidingRailPoint;
                        _RigidBody.velocity = Vector2.zero;
                        this._CharacterState = SkaterState.Grind;
                        this.GrindOnSFX.Play();
                        this.GrindSFX.Play();
                    }
                    break; 
                }
            case SkaterState.Grind:
                {
                    if (this._GrindAction.InSplineDirection)
                    {
                        _GrindAction.grindRailPoint += _GrindAction.GrindSpeed * Time.deltaTime;
                    }
                    else
                    {
                         _GrindAction.grindRailPoint -= _GrindAction.GrindSpeed * Time.deltaTime;
                    }

                    this._UpVector = _GrindAction.grindRailPoint.GetUpVector();
                    Vector2 newPosition = this._GrindAction.grindRailPoint.GetWorldPosition();
                    newPosition += (Vector2)_GrindAction.grindRailPoint.GetUpVector() * this.GetBounds().extents.y;
                    SetPosition(newPosition);

                    bool JumpPresssed = this.playerController.Jump.WasPressedThisFrame();
                    bool breakGrind = JumpPresssed || !this._GrindAction.grindRailPoint.OnSpline;

                    if (breakGrind)
                    {
                        this._CharacterState = SkaterState.Jumping;
                        Vector2 grindVector = _GrindAction.grindRailPoint.GetForwardVector();
                        if (!_GrindAction.InSplineDirection) grindVector *= -1.0f;
                        _RigidBody.velocity = grindVector * _GrindAction.GrindSpeed;

                        if (JumpPresssed)
                        {
                            Jump();
                        }
                        else
                        {
                            this._GrindAction.PreviousSpline = _GrindAction.grindRailPoint.RailSpline;
                        }
                        this.GrindSFX.Stop();
                        this.GrindOffSFX.Play();
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
        UpdateSpriteTransform();
    }
    public Bounds GetBounds()
    {
        return RigidBodyBounds.Get2DBodyBounds(_RigidBody);
    }
    protected Vector3 GetForwardMoveVector() => Vector3.Cross(_UpVector, Vector3.forward);
    public void Move(Vector2 Input)
    {
        Vector2 moveVector = Vector2.zero +
            ((Vector2)GetForwardMoveVector() * MoveSpeed * Input.x);

        bool FollowVelocity = Vector3.Dot(moveVector, _RigidBody.velocity) >= 0.0f;
        // Breaking velocity.
        // i.e moving in opposite of current velocity
        if (!FollowVelocity && this._CharacterState == SkaterState.Grounded)
        {
            moveVector *= BreakingSpeedScale;
        }
        //Weaken move speed when in air
        else if (this._CharacterState == SkaterState.Jumping)
        {
            moveVector *= 0.2f;
        }

        if (_RigidBody.velocity.magnitude < Maxspeed)
        {
            moveVector = Vector2.ClampMagnitude(moveVector, Maxspeed - _RigidBody.velocity.magnitude);
            _RigidBody.AddForce(moveVector * Time.deltaTime, ForceMode2D.Impulse);
        }
    }
    public void Jump()
    {
        float scaledJumpForce = JumpForce * 100.0f * Time.fixedDeltaTime;
        Vector2 jumpVector = Vector2.zero + (Vector2)_UpVector;
        _RigidBody.AddForce(jumpVector * scaledJumpForce, ForceMode2D.Impulse);

        this.PlayAnimationState(AnimState_Jump);

    }
    public void SetPosition(Vector2 NewPosition)
    {
        _RigidBody.position = NewPosition;
        transform.position = NewPosition; 
    }
    protected void ApplyGravity(Vector2 GravityVector)
    {
        _RigidBody.AddForce(GravityVector * this.Gravity * Time.deltaTime, ForceMode2D.Impulse);
        return;
    }

    public float GetNormalisedSpeed()
    {
        if (this._CharacterState == SkaterState.Grind) {
            return _GrindAction.GrindSpeed / Maxspeed;
        }
        return _RigidBody.velocity.magnitude / Maxspeed;
    }
    
    //Floor Raycasting
    protected (bool Success, RaycastHit2D Result) RaycastFloor()
    {
        if (!_RigidBody)
        {
            return (false, new RaycastHit2D());
        }

        // Cast Ray of max distance
        RaycastHit2D[] rayHits = new RaycastHit2D[10];
        rayHits = Physics2D.RaycastAll( _RigidBody.position, -_UpVector,GetBounds().extents.x + _FloorRayLength);
        Func<RaycastHit2D, bool> valdHit = (RaycastHit2D input) =>
        {
            bool validCollider = input.collider != null &&
            input.collider.attachedRigidbody != _RigidBody;
            bool BelowBody = Vector3.Dot(_UpVector, _RigidBody.position - input.point) > 0.5f;
            return validCollider && BelowBody;
        };

        rayHits = rayHits.Where(valdHit).
            OrderBy(hit => hit.distance).
            ToArray(); 
        
        if (rayHits.Length > 0)
        {
            return (
                true,
                rayHits[0]
                );
        }
        else
        {
            return (false, new RaycastHit2D());
        }
    }

    protected void StickToFloor()
    {
        if (!this._CurrentPlatform) return;
        var stickPoint = _CurrentPlatform.ClosestPoint(_RigidBody.position);
        var normal = Physics2D.Raycast(_RigidBody.position, (Vector2)stickPoint - _RigidBody.position, 1000.0f, GroundLayerMask).normal;
        if (Vector3.Angle(_UpVector, normal) < 45.0f)
        {
            transform.position = (Vector2)stickPoint + (normal * _CirlceCollider.radius);
            _UpVector = normal;
            _RigidBody.velocity = Vector3.ProjectOnPlane(_RigidBody.velocity, normal);
        }
    }

    private GrindRailPoint GetCollidingRail()
    {
        var GrindRailSearch = GrindRail.FindClosestRailToPoint(transform.position);
        if (!GrindRailSearch.Success) return null; // No rails found
        else
        {
            var resultRailPoint = GrindRailSearch.ResultRailPoint;
            Bounds selfCollisionBounds = new Bounds(
                GetBounds().center,
                GetBounds().size
                );
            selfCollisionBounds.Expand(1.5f); // Collision Padding. Be Generous

            bool newSpline = resultRailPoint.RailSpline != _GrindAction.PreviousSpline;
            bool railCollision = selfCollisionBounds.Contains(resultRailPoint.GetWorldPosition());

            // Rail can't be upside down of player
            if (
                _RigidBody.velocity.y < 0.0f
                && transform.position.y > resultRailPoint.GetWorldPosition().y 
                && railCollision && newSpline && resultRailPoint.OnSpline
                )
            {
                return resultRailPoint;
            }
        }
        return null;
    }

    public void PlayAnimationState(string StateName, bool Override = false)
    {
        if (_AnimatorComp.GetCurrentAnimatorStateInfo(0).IsName(StateName) ) return;
        _AnimatorComp.Play(StateName);
    }
    // Set Sprite Direction and Rotation Transform
    
    private void SetSpriteRotation(Vector2 upVector)
    {
        Quaternion ObjRotation = Quaternion.Euler(0, 0, transform.rotation.z);
        Quaternion SpriteRot = Quaternion.LookRotation(Vector3.forward, upVector);
        _SpriteRenderer.transform.localRotation = Quaternion.Inverse(ObjRotation) * SpriteRot; 
    }
    protected void UpdateSpriteTransform()
    {
        switch (_CharacterState)
        {
            case SkaterState.Jumping:
                {
                    Vector3 upVector = Vector3.Cross(_RigidBody.velocity, Vector3.forward);
                    if (_RigidBody.velocity.x >= 0.0f)
                    {
                        upVector *= -1.0f;
                    }
                    SetSpriteRotation(upVector);
                    _SpriteRenderer.flipX = Vector3.Dot(GetForwardMoveVector(), _RigidBody.velocity) < 0.0f;
                    break;
                }
            case SkaterState.Grind:
                {
                    SetSpriteRotation(_GrindAction.grindRailPoint.GetUpVector());
                    break;
                }
            default:
                {
                    SetSpriteRotation(_UpVector);
                    if (_RigidBody.velocity.magnitude > 0.0f)
                    {
                        _SpriteRenderer.flipX = Vector3.Dot(GetForwardMoveVector(), _RigidBody.velocity) < 0.0f;
                    }
                    break;
                }
        }
    }

#if UNITY_EDITOR
    // Draw Debug Gizmos
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            _RigidBody.position,
            (Vector3)_RigidBody.position + (Vector3)_RigidBody.velocity
            );

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            transform.position,
            transform.position + (GetForwardMoveVector() * GetBounds().size.magnitude)
            );

        //Up Vector 
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            transform.position,
            transform.position + (_UpVector * GetBounds().size.magnitude)
         );

        // Floor Ray
        Gizmos.color = Color.red;
        Vector3 floorRayVec = -_UpVector * (GetBounds().size.y / 2 + _FloorRayLength);
        Gizmos.DrawLine(
            _RigidBody.position,
            (Vector3)_RigidBody.position + floorRayVec
            );

        // Closest Grind Rail Point
        Gizmos.color = Color.yellow;
        var closestRailPoint = GrindRail.FindClosestRailToPoint(transform.position);
        if (closestRailPoint.Success)
        {
            Gizmos.DrawWireCube(closestRailPoint.ResultRailPoint.GetWorldPosition(), Vector3.one * 0.75f);
        }
    }

    protected bool _EditorSelected
    {
        get
        {
            if (Selection.activeGameObject == null) return false;
            else
            {
                var Selected = Selection.activeGameObject;
                return Selected == this.gameObject || Selected.transform.IsChildOf(this.transform);
            }
        }
    }

    private Rect _GuiRect = new Rect(20, 20, 300, 200);
    void OnGUI()
    {
        _GuiRect = GUILayout.Window(0, _GuiRect, _DrawGUIWindow, $"Skate Player - {this.gameObject.name}");
    }
    void _DrawGUIWindow(int WindowID)
    {
        GUILayout.Label($"Speed: {_RigidBody.velocity.magnitude.ToString().Truncate(3)} | {(GetNormalisedSpeed() * 100).ToString().Truncate(3)}%");
        GUILayout.Label($"Speed Vector: {_RigidBody.velocity}");

        GUILayout.Label($"State: {_CharacterState}");

        string PlatformName = this._CurrentPlatform ? this._CurrentPlatform.gameObject.name : "None";
        GUILayout.Label($"Current Platform: {PlatformName}");
        GUI.DragWindow(new Rect(0,0,float.MaxValue, float.MaxValue));
    }
#endif
}
