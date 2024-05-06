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
    private float _OrientDirX = 1f; // Direction du mouvement
    private float _horizontalSpeed = 0f; // Vitesse de déplacement

    [Header("Sauts")]
    private float _verticalSpeed = 0f; // Vitesse de saut
    [Header("Attaques")]
    private float attackForce = 5f;
    [Header("Vie")]
    private float health = 5f;
    [Header("Patterns")]
    private float pattern = 5f;

    void Awake()
    {
        _ResetHorizontalSpeed();
        _ResetVerticalSpeed();
    }

    void FixedUpdate()
    {
        _ApplyHorizontalSpeed();
        _ApplyVerticalSpeed();
        
    }
    void Update()
    {
        _ApplyGroundDetection();
        _ApplyWallDetection();
        _ApplyOrientDirX();
        _EnnemyJump();
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
        if(IsEnemyTouchingWallLeft){
            _OrientDirX = 1f;
        }
        else if(IsEnemyTouchingWallRight){
            _OrientDirX = -1f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _heroEntity.isPlayerVisible)
        {
            float directionToPlayer = Mathf.Sign(other.transform.position.x - transform.position.x);
            _OrientDirX = directionToPlayer;
            IsPlayerDetected = true;
        }
        else
        {
            IsPlayerDetected = false;
        }
    }
    
    private void _ResetVerticalSpeed()
    {
        _verticalSpeed = _movementsSettings._jumpSpeed;
    }

    private void _ResetHorizontalSpeed()
    {
        _horizontalSpeed = _movementsSettings._Speed;
    }
    private void _EnnemyJump(){

        if(IsPlayerDetected){
            Debug.Log("Player Detected");
            if(IsEnemyTouchingWallLeft || IsEnemyTouchingWallRight){
                Debug.Log("Jump");
                _verticalSpeed = _movementsSettings._jumpSpeed;
            }
        }
        else{
            _verticalSpeed = 0f;
        }
    }
}
