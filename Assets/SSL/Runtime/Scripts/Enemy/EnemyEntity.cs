using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections.Generic;

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

    [SerializeField] private float _stopDistance = 3f;

    [Header("Sauts")]
    private float _verticalSpeed = 0f; // Vitesse de saut
    public bool isJumping = false;

    // [Header("Attaques")]
    // private float attackForce = 5f;
    // [Header("Vie")]
    // private float health = 5f;
    // [Header("Patterns")]
    // private float pattern = 5f;

    void Awake()
    {
        _horizontalSpeed = _movementsSettings._Speed;
    }

    void FixedUpdate()
    {
        _ApplyGroundDetection();
        _ApplyWallLeftDetection();
        _ApplyWallRightDetection();
        if (!_movementsSettings.IsEnemyDumb)
        {
            if (!IsEnemyTouchingGround)
            {
                _ApplyFallGravity(_fallSettings);
            }
            else if(!isJumping)
            {
                _ResetVerticalSpeed();
            }
            if (IsPlayerDetected)
            {
                _FollowPlayer();
            }
            else{
                _ApplyOrientDirX();
            }
        }
        else
        {
            _ApplyOrientDirX();
        }
        _ApplyHorizontalSpeed();
        _ApplyVerticalSpeed();

    }
    void Update()
    {
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

    private void _ApplyWallRightDetection()
    {
        IsEnemyTouchingWallRight = _eRaycast.DetectWallNearByRight();
    }

    private void _ApplyWallLeftDetection()
    {
        IsEnemyTouchingWallLeft = _eRaycast.DetectWallNearByLeft();
    }

    private void _ApplyOrientDirX(){
        if (IsEnemyTouchingWallLeft)
        {
            _OrientDirX = 1f;
        }
        else if (IsEnemyTouchingWallRight)
        {
            _OrientDirX = -1f;
        }
    
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _heroEntity.isPlayerVisible)
        {
            IsPlayerDetected = true;
        }
        // if (transform.position.y > _heroEntity.transform.position.y)
        // {
        //     _ResetVerticalSpeed();
        // }

    }
    
    private void _ResetVerticalSpeed()
    {
        _verticalSpeed = 0f;
    }

    private void _ResetHorizontalSpeed()
    {
        _horizontalSpeed = 0f;
    }
    

    private void _ApplyFallGravity(EnemyFallSettings settings)
    {
        _verticalSpeed -= settings.fallGravity * Time.fixedDeltaTime;
        if (_verticalSpeed < -settings.fallSpeedMax)
        {
            _verticalSpeed = -settings.fallSpeedMax;
        }
    }

    private IEnumerator Jump()
    {
        isJumping = true;
        for(float i = 0; i < _movementsSettings._jumpDuration; i+=Time.deltaTime)
        {
            _verticalSpeed = _movementsSettings._jumpSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        isJumping = false;
        yield return null;
    }
    private void _FollowPlayer()
    {
        _directionToPlayer = _heroEntity.transform.position.x - transform.position.x;
        if (_directionToPlayer > 0)
        {
            _OrientDirX = 1f;
        }
        if (_directionToPlayer < 0)
        {
            _OrientDirX = -1f;
        }
       
        if (IsEnemyTouchingGround && (IsEnemyTouchingWallLeft || IsEnemyTouchingWallRight))
        {
            StartCoroutine(Jump());
        }


        if (Mathf.Abs(_directionToPlayer) > _stopDistance)
        {
            _horizontalSpeed = _movementsSettings._Speed;
        }
        else
        {
            _ResetHorizontalSpeed();
        }
  
    }
    private void OnGUI(){
            GUILayout.Label($"IsEnemyTouchingGround: {IsEnemyTouchingGround}");
            GUILayout.Label($"IsEnemyTouchingWallRight: {IsEnemyTouchingWallRight}");
            GUILayout.Label($"IsEnemyTouchingWallLeft: {IsEnemyTouchingWallLeft}");
            GUILayout.Label($"Vertical Speed: {_verticalSpeed}");
            GUILayout.Label($"Horizontal Speed: {_horizontalSpeed}");
            GUILayout.Label($"DirectionToPlayer: {_directionToPlayer}");
        }
}
