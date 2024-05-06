using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using System;


public class EnemyEntity : MonoBehaviour
{
    [Header("Composants")]
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Detection")]
    [SerializeField] private EnemyGroundDetector _eGroundDetector;
    [SerializeField] private EnemyRaycast _eRaycast;
    [SerializeField] private Collider2D _collider;

    [SerializeField] private HeroEntity _heroEntity;

    public bool IsEnemyTouchingGround { get; private set; } = false;
    public bool IsEnemyTouchingWallRight { get; private set; } = false;
    public bool IsEnemyTouchingWallLeft { get; private set; } = false;

    public bool IsPlayerDetected { get; private set; } = false;

    [Header("Mouvements")]
    [SerializeField] private EnnemyMovementsSettings _movementsSettings;
    [SerializeField] private EnemyFallSettings _fallSettings;
    private float _OrientDirX = 1f; // Direction du mouvement
    private float _directionToPlayer = 0f; // Direction du joueur
    private float _horizontalSpeed = 0f; // Vitesse de déplacement

    [Header("Sauts")]
    private float _verticalSpeed = 0f; // Vitesse de saut
    // [Header("Attaques")]
    // private float attackForce = 5f;
    // [Header("Vie")]
    // private float health = 5f;
    // [Header("Patterns")]
    // private float pattern = 5f;

    void Awake()
    {
        _horizontalSpeed = _movementsSettings._Speed;
        _verticalSpeed = 0f;
    }

    void FixedUpdate()
    {
        _ApplyGroundDetection();
        _ApplyWallDetection();
        if (!IsEnemyTouchingGround)
        {
            _ApplyFallGravity(_fallSettings);
        }
        else
        {
            _ResetVerticalSpeed();
        }
        _ApplyHorizontalSpeed();
        _ApplyVerticalSpeed();

    }
    void Update()
    {
        _ApplyOrientDirX();
    }

    
    private void _ApplyHorizontalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _horizontalSpeed * _OrientDirX;
        _rigidbody.velocity = velocity;
    }

    private void _ApplyVerticalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.y = _verticalSpeed;
        _rigidbody.velocity = velocity;
    }

    private void _ApplyGroundDetection()
    {
        IsEnemyTouchingGround = _eGroundDetector.DetectGroundNearBy();
    }

    private void _ApplyWallDetection()
    {
        IsEnemyTouchingWallLeft = _eRaycast.DetectWallNearByLeft();
        IsEnemyTouchingWallRight = _eRaycast.DetectWallNearByRight();
    }

    private void _ApplyOrientDirX(){
        if (IsPlayerDetected)
        {
            _OrientDirX = Mathf.Sign(_heroEntity.transform.position.x + 0.01f - transform.position.x);
        }
        else
        {
            if (IsEnemyTouchingWallLeft)
            {
                _OrientDirX = 1f;
            }
            else if (IsEnemyTouchingWallRight)
            {
                _OrientDirX = -1f;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _heroEntity.isPlayerVisible)
        {
            IsPlayerDetected = true;
            _EnnemyJump();
        }
        else
        {
            IsPlayerDetected = false;
        }
    }
    
    private void _ResetVerticalSpeed()
    {
        _verticalSpeed = 0f;
    }

    private void _ResetHorizontalSpeed()
    {
        _horizontalSpeed = 0f;
    }
    private void _EnnemyJump(){

        if(IsPlayerDetected && IsEnemyTouchingGround && (IsEnemyTouchingWallLeft || IsEnemyTouchingWallRight))
        {
            Debug.Log("Jump");
            Debug.Log(_verticalSpeed);
            _verticalSpeed = _movementsSettings._jumpSpeed;
        }
        else{
            _verticalSpeed = 0f;
        }
    }

    private void _ApplyFallGravity(EnemyFallSettings settings)
    {
        _verticalSpeed -= settings.fallGravity * Time.fixedDeltaTime;
        if (_verticalSpeed < -settings.fallSpeedMax)
        {
            _verticalSpeed = -settings.fallSpeedMax;
        }
    }
}
