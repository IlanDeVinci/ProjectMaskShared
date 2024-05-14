using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class HeroEntity : MonoBehaviour
{
    [Header("Physics")] [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")] [FormerlySerializedAs("_movementSettings")] [SerializeField]
    private HeroHorizontalMovementSettings _groundHorizontalMovementSettings;

    [SerializeField] private HeroHorizontalMovementSettings _airHorizontalMovementSettings;

    private float _horizontalSpeed = 0f;
    private float _moveDirX = 0f;

    [Header("Vertical Movements")] private float _verticalSpeed = 0f;

    [Header("Fall")] [SerializeField] private HeroFallSettings _fallSettings;

    [Header("Ground")] [SerializeField] private GroundDetector _groundDetector;
    public bool IsTouchingGround { get; private set; } = false;

    [Header("Ceiling")] [SerializeField] private CeilingDetector _ceilingDetector;
    public bool IsTouchingCeiling { get; private set; } = false;

    public bool IsTouchingCeilingCenter { get; private set; } = false;

    public bool isFixingCeiling = false;

    [Header("Wall")] [SerializeField] private WallDetector _wallDetector;
    public bool IsTouchingWallLeft { get; private set; } = false;

    public bool IsTouchingWallRight { get; private set; } = false;

    private bool IsTouchingWallCenterLeft = false;

    private bool IsTouchingWallCenterRight = false;

    public bool wasTouchingWall = false;

    [Header("Jump")] [SerializeField] private HeroJumpSettings[] _jumpSettings;
    public int _jumpIndex = 0;


    [SerializeField] private HeroFallSettings _jumpFallSettings;

    [SerializeField] private HeroWallJumpSettings _wallJumpSettings;

    enum JumpState
    {
        NotJumping,
        JumpImpulsion,
        WallJump,
        TremplinJump,
        Falling
    }

    private JumpState _jumpState = JumpState.NotJumping;
    private float _jumpTimer = 0f;
    public bool isJumpImpulsing => (_jumpState == JumpState.JumpImpulsion) || (_jumpState == JumpState.WallJump || _jumpState == JumpState.TremplinJump);
    public bool isJumpMinDurationReached => JumpMinDurationReached();

    public bool isJumping => _jumpState != JumpState.NotJumping;
    public bool hasJumpsLeft => _jumpIndex < _jumpSettings.Length;

    public bool canJump =>
        (hasJumpsLeft && ((_jumpState == JumpState.NotJumping) || (_jumpState == JumpState.Falling))) || isSliding;
    private float jumpMultiplier = 1;
    public bool isSliding => isSlidingLeft() || isSlidingRight() || _slideTimer < slidingCooldown;
    public float _slideTimer = 0f;
    private float _timeSinceSlideStart = 0f;
    private bool wasSliding = false;

    [SerializeField] private float slidingCooldown = 0.2f;


    [Header("Dash")] [SerializeField] private HeroDashSettings _dashSettings;

    private DashState _dashState = DashState.NotDashing;
    private float _dashTimer = 0f;
    private float _timeSinceDash = 0f;
    private float _dashOrient = 1f;
    private bool isAirDash = false;
    public bool canDash => _timeSinceDash >= _dashSettings.cooldown;
    public bool isDashing => _dashState != DashState.NotDashing;


    enum DashState
    {
        Dashing,
        NotDashing
    }

    [Header("Tremplin")] [SerializeField] private HeroJumpSettings tremplinSettings;
    private bool isTouchingTremplin => _groundDetector.DetectTremplin();


    [Header("Orientation")] [SerializeField]
    private Transform _orientVisualRoot;

    public float _orientX = 1f;

    [Header("Etat Player")]
    public bool isPlayerVisible = true;

    [Header("Debug")] [SerializeField] private bool _guiDebug = false;

    private CameraFollowable _cameraFollowable;
    public bool isHorizontalMoving => _moveDirX != 0f;

    private Vector2 velocityBeforePause;
    public bool isPaused = false;
    [SerializeField] private HealthManager healthManager;
    private Vector3 startPos;

    private void Awake()
    {
        startPos = transform.position;
        _cameraFollowable = GetComponent<CameraFollowable>();
        _cameraFollowable.FollowPositionX = _rigidbody.position.x;
        _cameraFollowable.FollowPositionY = _rigidbody.position.y;
        
    }

    public void AddSpeed(Vector2 speed)
    {
        _horizontalSpeed = speed.x;
        _verticalSpeed = speed.y;
    }
    private void _UpdateCameraFollowPosition()
    {
        _cameraFollowable.FollowDirection = _orientX;
        _cameraFollowable.FollowPositionX = _rigidbody.position.x;
        if ((IsTouchingGround && !isJumping) || isSliding || _jumpState == JumpState.TremplinJump)
        {
            _cameraFollowable.FollowPositionY = _rigidbody.position.y;
        }
    }

    private bool JumpMinDurationReached()
    {
        switch (_jumpState)
        {
            case JumpState.TremplinJump:
                return _jumpTimer >= tremplinSettings.jumpMinDuration;
            case JumpState.WallJump:
                return _jumpTimer >= _wallJumpSettings.wallJumpMinDuration;
            case JumpState.JumpImpulsion:
                return _jumpTimer >= _jumpSettings[_jumpIndex - 1].jumpMinDuration;

        }

        return true;
    }

    private bool isSlidingLeft()
    {
        return IsTouchingWallLeft && !IsTouchingGround && (_moveDirX == -1 || _jumpState == JumpState.WallJump);
    }

    private bool isSlidingRight()
    {
        return IsTouchingWallRight && !IsTouchingGround && (_moveDirX == 1 || _jumpState == JumpState.WallJump);
    }

    private void changeOrientIfSliding()
    {
        if (isSlidingLeft())
        {
            _orientX = 1f;
            if (!IsTouchingWallCenterLeft)
            {
                _slideTimer = slidingCooldown;
            }
        }

        if (isSlidingRight())
        {
            _orientX = -1f;
            if (!IsTouchingWallCenterRight)
            {
                _slideTimer = slidingCooldown;
            }
        }

        if (_jumpState != JumpState.WallJump) _ResetHorizontalSpeed();
    }

    private void ClampFallSpeedWhenSliding()
    {
        if ((IsTouchingWallRight && _moveDirX == 1) || (IsTouchingWallLeft && _moveDirX == -1))
        {
            _verticalSpeed = Mathf.Clamp(_verticalSpeed, -2f, _verticalSpeed);
        }
    }

    private void UpdateSlide()
    {
        changeOrientIfSliding();
        ClampFallSpeedWhenSliding();
        if ((!IsTouchingWallRight && _moveDirX == 1) || (!IsTouchingWallLeft && _moveDirX == -1))
        {
            if (_timeSinceSlideStart < 0.3f)
            {
                _horizontalSpeed = _airHorizontalMovementSettings.speedMax / 1.3f;
                _slideTimer = slidingCooldown;
            }
            else
            {
                _horizontalSpeed = _airHorizontalMovementSettings.speedMax / 3f;
            }
        }
    }

    public void DashPressed()
    {
        if (_dashSettings.isLongDash) _dashTimer = 0f;
    }


    public void DashStart()
    {
        _dashState = DashState.Dashing;
        _dashTimer = 0f;

        if (!_dashSettings.isLongDash)
        {
            if (canDash)
            {
                if (_moveDirX != 0)
                {
                    _dashOrient = _moveDirX;
                }
                else
                {
                    _dashOrient = _orientX;
                }

                _timeSinceDash = 0f;
                isAirDash = !IsTouchingGround;
            }
        }
    }


    private void ClampHorizontalSpeed()
    {
        if (isAirDash)
        {
            _horizontalSpeed = Mathf.Clamp(_horizontalSpeed, -_airHorizontalMovementSettings.speedMax,
                _airHorizontalMovementSettings.speedMax);
        }
        else
        {
            _horizontalSpeed = Mathf.Clamp(_horizontalSpeed, -_groundHorizontalMovementSettings.speedMax,
                _groundHorizontalMovementSettings.speedMax);
        }
    }

    private void _UpdateDash()
    {
        if (!_dashSettings.isLongDash)
        {
            _orientX = _dashOrient;
            if (isAirDash)
            {
                _jumpState = JumpState.NotJumping;
                if (_dashOrient == 1)
                {
                    if (!IsTouchingWallRight)
                    {
                        if (_dashTimer < _dashSettings.airDuration)
                        {
                            _horizontalSpeed = _dashSettings.airSpeed;
                        }
                        else
                        {
                            ClampHorizontalSpeed();
                            _dashState = DashState.NotDashing;
                        }
                    }
                    else
                    {
                        _dashTimer = _dashSettings.airDuration;
                        _ResetHorizontalSpeed();
                        _dashState = DashState.NotDashing;
                    }
                }
                else
                {
                    if (!IsTouchingWallLeft)
                    {
                        if (_dashTimer < _dashSettings.airDuration)
                        {
                            _horizontalSpeed = _dashSettings.airSpeed;
                        }
                        else
                        {
                            ClampHorizontalSpeed();
                            _dashState = DashState.NotDashing;
                        }
                    }
                    else
                    {
                        _dashTimer = _dashSettings.airDuration;
                        _ResetHorizontalSpeed();
                        _dashState = DashState.NotDashing;
                    }
                }
            }
            else
            {
                if (_dashOrient == 1)
                {
                    if (!IsTouchingWallRight)
                    {
                        if (_dashTimer < _dashSettings.groundDuration)
                        {
                            _horizontalSpeed = _dashSettings.groundSpeed;
                        }
                        else
                        {
                            ClampHorizontalSpeed();
                            _dashState = DashState.NotDashing;
                        }
                    }
                    else
                    {
                        _dashTimer = _dashSettings.groundDuration;
                        _ResetHorizontalSpeed();
                        _dashState = DashState.NotDashing;
                    }
                }
                else
                {
                    if (!IsTouchingWallLeft)
                    {
                        if (_dashTimer < _dashSettings.groundDuration)
                        {
                            _horizontalSpeed = _dashSettings.groundSpeed;
                        }
                        else
                        {
                            ClampHorizontalSpeed();
                            _dashState = DashState.NotDashing;
                        }
                    }
                    else
                    {
                        _dashTimer = _dashSettings.groundDuration;
                        _ResetHorizontalSpeed();
                        _dashState = DashState.NotDashing;
                    }
                }
            }
        }
        else
        {
            if (_dashTimer > _dashSettings.dashTransition)
            {
                _dashState = DashState.NotDashing;
            }
            else if (!_AreOrientAndMovementOpposite() && _moveDirX != 0 &&
                     _horizontalSpeed >
                     _dashSettings.longDashGroundSettings.speedMax / 1.1)
            {
                float percent = _dashTimer / _dashSettings.dashTransition;
                if (IsTouchingGround)
                {
                    _horizontalSpeed = Mathf.Lerp(_dashSettings.longDashGroundSettings.speedMax,
                        _groundHorizontalMovementSettings.speedMax,
                        percent);
                }
                else
                {
                    _horizontalSpeed = Mathf.Lerp(_dashSettings.longDashAirSettings.speedMax,
                        _airHorizontalMovementSettings.speedMax,
                        percent);
                }
            }
        }
    }

    private HeroHorizontalMovementSettings _GetCurrentHorizontalMovementSettings()
    {
        return IsTouchingGround ? _groundHorizontalMovementSettings : _airHorizontalMovementSettings;
    }

    private HeroHorizontalMovementSettings _GetCurrentHorizontalMovementSettingsDash()
    {
        return IsTouchingGround ? _dashSettings.longDashGroundSettings : _dashSettings.longDashAirSettings;
    }

    public void JumpStart()
    {
        if(!isTouchingTremplin)
        {
            if (!IsTouchingCeiling)
            {
                if (!isSliding)
                {
                    _jumpState = JumpState.JumpImpulsion;
                    _jumpTimer = 0f;
                    if (_jumpIndex < _jumpSettings.Length)
                    {
                        _jumpIndex += 1;
                    }
                }
                else
                {
                    _jumpState = JumpState.WallJump;
                    _jumpTimer = 0f;
                    _timeSinceDash = _dashSettings.cooldown / 2;
                }
            }
        }
        else
        {
            _jumpState = JumpState.TremplinJump;
            _jumpTimer = 0f;
        }


        _slideTimer = slidingCooldown;
    }

    public void StopJumpImpulsion()
    {
        _jumpState = JumpState.Falling;
    }

    private void _UpdateJumpStateImpulsion(int index)
    {
        _jumpTimer += Time.fixedDeltaTime;
        if (_jumpTimer < _jumpSettings[index - 1].jumpMaxDuration && !IsTouchingCeiling)
        {
            _verticalSpeed = _jumpSettings[index - 1].jumpSpeed * jumpMultiplier;
        }
        else
        {
            _jumpState = JumpState.Falling;
        }
    }

    private void _UpdateJumpStateTremplin()
    {
        _jumpTimer += Time.fixedDeltaTime;
        if (_jumpTimer < tremplinSettings.jumpMaxDuration && !IsTouchingCeiling)
        {
            _verticalSpeed = tremplinSettings.jumpSpeed;
        }
        else
        {
            _jumpState = JumpState.Falling;
        }
    }
    private void _UpdateJumpStateFalling()
    {
        if (!IsTouchingGround)
        {
            _ApplyFallGravity(_jumpFallSettings);
        }
        else
        {
        if (_verticalSpeed <= 0) _jumpState = JumpState.NotJumping;
        _ResetVerticalSpeed();

        }
    }

    private void _UpdateJumpStateWalljump()
    {
        _jumpTimer += Time.fixedDeltaTime;
        if (_jumpTimer < _wallJumpSettings.wallJumpMaxDuration && !IsTouchingCeiling)
        {
            _verticalSpeed = _wallJumpSettings.wallJumpVerticalSpeed;
            _horizontalSpeed = _wallJumpSettings.wallJumpHorizontalSpeed;
        }
        else
        {
            _jumpState = JumpState.Falling;
        }
    }

    private void _UpdateJump()
    {
        switch (_jumpState)
        {
            case JumpState.Falling:
                _UpdateJumpStateFalling();
                break;
            case JumpState.JumpImpulsion:
                _UpdateJumpStateImpulsion(_jumpIndex);
                break;
            case JumpState.WallJump:
                _UpdateJumpStateWalljump();
                break;
            case JumpState.TremplinJump:
                _UpdateJumpStateTremplin();
                break;
        }
    }

    
    private void _ApplyGroundDetection()
    {
        IsTouchingGround = _groundDetector.DetectGroundNearBy();
        if (IsTouchingGround && _jumpState != JumpState.JumpImpulsion)
        {
            _jumpIndex = 0;
        }

        if (IsTouchingGround)
        {
            _slideTimer = slidingCooldown;
        }
    }

    private IEnumerator fixCeiling()
    {
        float i = 1;
        isFixingCeiling = true;
        while (IsTouchingCeiling)
        {
            IsTouchingWallLeft = false;
            IsTouchingWallRight = false;
            _jumpState = JumpState.Falling;
            i *= 2;
            _verticalSpeed -= i;
            yield return new WaitForFixedUpdate();
        }

        _verticalSpeed = -_jumpFallSettings.fallSpeedMax / 6;
        isFixingCeiling = false;
    }

    private void _ApplyCeilingDetection()
    {
        IsTouchingCeiling = _ceilingDetector.DetectCeilingNearBy();
        IsTouchingCeilingCenter = _ceilingDetector.DetectCeilingCenter();

        if (_jumpState == JumpState.WallJump)
        {
            IsTouchingCeiling = IsTouchingCeilingCenter;
        }

        if (IsTouchingCeiling && !isFixingCeiling)
        {
            _verticalSpeed = -1;
            _jumpState = JumpState.Falling;
            StartCoroutine(fixCeiling());
        }
    }

    private void _ApplyWallDetection()
    {
        IsTouchingWallLeft = _wallDetector.DetectWallNearByLeft();
        IsTouchingWallRight = _wallDetector.DetectWallNearByRight();
        IsTouchingWallCenterLeft = _wallDetector.DetectWallCenterLeft();
        IsTouchingWallCenterRight = _wallDetector.DetectWallCenterRight();
        /*
        if (isSliding)
        {
            IsTouchingWallLeft = IsTouchingWallCenterLeft;
            IsTouchingWallRight = IsTouchingWallCenterRight;
        }
        */
    }

    private void _ResetVerticalSpeed()
    {
        if(_verticalSpeed < 0)  _verticalSpeed = 0f;
    }

    private void _ResetHorizontalSpeed()
    {
        _horizontalSpeed = 0f;
    }

    private void _Accelerate(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed += settings.acceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed > settings.speedMax)
        {
            _horizontalSpeed = settings.speedMax;
        }
    }

    private void _TurnBack(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed -= settings.turnBackFrictions * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
            _ChangeOrientFromHorizontalMovement();
        }
    }

    public void enterSlide()
    {
        if (!wasSliding)
        {
            wasSliding = true;
            _timeSinceSlideStart = 0f;
        }

        if (isSlidingLeft() || isSlidingRight())
        {
            if (_jumpState != JumpState.WallJump)
            {
                _slideTimer = 0;
            }
        }
    }

    private void _ResetSpeedOnWallCollision()
    {
        if (!IsTouchingCeiling)
        {
            if (!isJumping || _jumpState == JumpState.Falling)
            {
                if (IsTouchingWallLeft)
                {
                    if (_orientX != 1)
                    {
                        _horizontalSpeed = 0;
                    }
                }

                if (IsTouchingWallRight)
                {
                    if (_orientX != -1)
                    {
                        _horizontalSpeed = 0;
                    }
                }
            }
        }
      
    }

    private bool _AreOrientAndMovementOpposite()
    {
        return _moveDirX * _orientX < 0f;
    }

    private void _Decelerate(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed -= settings.deceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
        }
    }

    private void _UpdateHorizontalSpeed(HeroHorizontalMovementSettings settings)
    {
        if (_moveDirX != 0f)
        {
            _Accelerate(settings);
        }
        else
        {
            _Decelerate(settings);
        }
    }

    #region Functions Move Dir

    public float GetMoveDirX()
    {
        return _moveDirX;
    }

    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;
    }

    #endregion

    private void _ApplyHorizontalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _horizontalSpeed * _orientX;
        _rigidbody.velocity = velocity;
    }

    private void FixedUpdate()
    {

        if (!GlobalManager.isGamePaused && healthManager.currentHealth >= 0)
        {
            isPaused = false;
            _ApplyGroundDetection();
            _ApplyWallDetection();
            _UpdateCameraFollowPosition();
            HeroHorizontalMovementSettings horizontalMovementSettings = _GetCurrentHorizontalMovementSettings();
            HeroHorizontalMovementSettings horizontalMovementSettingsDash = _GetCurrentHorizontalMovementSettingsDash();
            _timeSinceDash += Time.fixedDeltaTime;
            _timeSinceSlideStart += Time.fixedDeltaTime;
            _slideTimer += Time.fixedDeltaTime;
            _dashTimer += Time.fixedDeltaTime;

            if (isDashing && !_dashSettings.isLongDash)
            {
                _UpdateDash();
            }
            else
            {
                if (_AreOrientAndMovementOpposite())
                {
                    if (isDashing && _dashSettings.isLongDash)
                    {
                        _TurnBack(horizontalMovementSettingsDash);
                    }
                    else
                    {
                        _TurnBack(horizontalMovementSettings);
                    }
                }
                else
                {
                    if (isDashing && _dashSettings.isLongDash)
                    {
                        _UpdateHorizontalSpeed(horizontalMovementSettingsDash);
                    }
                    else
                    {
                        _UpdateHorizontalSpeed(horizontalMovementSettings);
                    }

                    _ChangeOrientFromHorizontalMovement();
                }
            }


            if (isJumping)
            {
                _UpdateJump();
            }
            else
            {
                if (!IsTouchingGround && (!isDashing || _dashSettings.isLongDash))
                {
                    _ApplyFallGravity(_fallSettings);
                }
                else
                {
                    if (_jumpState != JumpState.JumpImpulsion && _jumpState != JumpState.WallJump)
                    {
                        _ResetVerticalSpeed();
                    }
                }
            }

            _ApplyCeilingDetection();

            if (isSliding)
            {
                enterSlide();
                UpdateSlide();
            }
            else
            {
                wasSliding = false;
            }

            if (isDashing && _dashSettings.isLongDash)
            {
                _UpdateDash();
            }

            _ApplyHorizontalSpeed();

            _ApplyVerticalSpeed();

            _ResetSpeedOnWallCollision();

        }
        else
        {
            if (!isPaused)
            {
                StartCoroutine(Pause());
            }
        }
        if (healthManager.currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void Reload()
    {
        healthManager.Reload();

        if (GlobalManager.playerCheckpointPosition != Vector2.zero)
        {
            transform.position = GlobalManager.playerCheckpointPosition;
        }
        else
        {
            transform.position = startPos;
        }
    }
    private IEnumerator Pause()
    {
        isPaused = true;
        velocityBeforePause = _rigidbody.velocity;
        _rigidbody.velocity = Vector2.zero;
        yield return new WaitUntil(() => GlobalManager.isGamePaused == false);
        _rigidbody.velocity = velocityBeforePause;
    }

    private void _ApplyFallGravity(HeroFallSettings settings)
    {
        _verticalSpeed -= settings.fallGravity * Time.fixedDeltaTime;
        if (_verticalSpeed < -settings.fallSpeedMax)
        {
            _verticalSpeed = -settings.fallSpeedMax;
        }
    }

    private void _ApplyVerticalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.y = _verticalSpeed;
        _rigidbody.velocity = velocity;
    }


    private void Update()
    {
        _UpdateOrientVisual();
        var upgrade = GlobalUpgrades.Instance.Upgrades.Find(x => x.upgradeType == GlobalUpgrades.UpgradeType.JumpHeight);
        jumpMultiplier = upgrade.upgradesList[upgrade.upgradeLevel].upgradeValue;
    }

    private void _UpdateOrientVisual()
    {
        Vector3 newScale = _orientVisualRoot.localScale;
        newScale.x = _orientX;
        _orientVisualRoot.localScale = newScale;
    }

    public void _ChangeOrientFromHorizontalMovement()
    {
        if (_moveDirX == 0) return;
        _orientX = Mathf.Sign(_moveDirX);
    }
    
    public void _ApplyOrientDirX(float dirX)
    {
        Debug.Log("Apply Orient");
        _orientX = dirX;
    }

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        GUILayout.Label($"Dash Orient = {_dashOrient}");

        if (IsTouchingGround)
        {
            GUILayout.Label($"On Ground");
        }
        else
        {
            GUILayout.Label($"In Air");
        }
        // GUILayout.Label($"jumpmin duration reached = {isJumpMinDurationReached}");
        GUILayout.Label($"jumpyimer = {_jumpTimer}");
        GUILayout.Label($"tremplinminduration = {tremplinSettings.jumpMinDuration}");

        GUILayout.Label($"dashtime = {_dashTimer}");

        GUILayout.Label($"isSliding = {isSliding}");
        GUILayout.Label($"Slide Timer = {_slideTimer}");


        GUILayout.Label($"Sliding left = {isSlidingLeft()}");
        GUILayout.Label($"Sliding right = {isSlidingRight()}");
        GUILayout.Label($"Touching right = {IsTouchingWallRight}");

        GUILayout.Label($"hasJumpsLeft = {hasJumpsLeft}");
        // GUILayout.Label($"canJump = {canJump}");
        //GUILayout.Label($"jumpIndex = {_jumpIndex}");
        //GUILayout.Label($"hasJumpsLeft = {hasJumpsLeft}");

        GUILayout.Label($"JumpState = {_jumpState}");
        GUILayout.Label($"DashState = {_dashState}");
        //GUILayout.Label($"TimeSinceDash = {_timeSinceDash}");
        GUILayout.Label($"AirDash = {isAirDash}");

        GUILayout.Label($"Vertical Speed = {_verticalSpeed}");
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");

        GUILayout.EndVertical();
    }
}