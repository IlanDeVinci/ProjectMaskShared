using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FlyingEnemyEntity : MonoBehaviour
{
    [SerializeField] private FlyingEnemyMovementSettings movementSettings;
    [SerializeField] private FlyingEnemyRaycasts raycasts;
    [SerializeField] private float detectionRange;

    private bool canMove = true;
    private bool isPlayerDetected = false;
    private float distanceToGround => raycasts.DistanceFromGround(posWithoutOscillationTransform);
    private float distanceToLeft => raycasts.DistanceFromLeft();
    private float distanceToRight => raycasts.DistanceFromRight();
    private Tween oscillationTween;
    [SerializeField] private TweenSettings settingsTween;
    private float oscillation = 0;
    private float maxOscillation = 2;
    private Vector2 posWithoutOscillation;
    private Transform posWithoutOscillationTransform;
    private Transform target;
    private bool canOscillate = false;
    private Vector2 direction;
    private RaycastHit2D raycastHit2D;
    [SerializeField] private Transform gun;
    [SerializeField] private LayerMask player;

    // Start is called before the first frame update
    void Start()
    {
        posWithoutOscillationTransform = transform;
        posWithoutOscillation = transform.position;
        target = GameObject.FindGameObjectWithTag("PlayerTrigger").transform;
        oscillationTween = Tween.Custom(startValue: oscillation, endValue: maxOscillation, settings: settingsTween, onValueChange: val => oscillation = val);

    }

    private void DoIdle()
    {
        Vector2 newPos = posWithoutOscillation;
        
        if(distanceToGround < movementSettings.minHeight || distanceToGround > movementSettings.maxHeight)
        {
            canOscillate = false;
            oscillationTween.isPaused = true;
            if (distanceToGround < movementSettings.minHeight)
            {
                newPos.y += movementSettings.moveSpeed * Time.deltaTime;
            }
            if (distanceToGround > movementSettings.maxHeight)
            {
                newPos.y -= movementSettings.moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            canOscillate = true;    
            oscillationTween.isPaused = false;
        }

        
        posWithoutOscillation = newPos;
        if(canOscillate)
        {
            newPos.y += oscillation;
        }
        transform.position = newPos;
        
    }
    private void LocatePlayer()
    {

        Vector2 targetPos = target.position;
        direction = targetPos - (Vector2)transform.position;
        direction.Normalize();
        raycastHit2D = Physics2D.Raycast(transform.position, direction, detectionRange, player);
        if (raycastHit2D.collider != null)
        {
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        LocatePlayer();
        posWithoutOscillationTransform.position = posWithoutOscillation;
        /*
        if (oscillationUp)
        {
            oscillation += Time.deltaTime;
            if (oscillation >= maxOscillation) 
            {
            oscillationUp = false;
            }
        }
        else
        {
            oscillation -= Time.deltaTime;
            if (oscillation <= -maxOscillation)
            {
                oscillationUp = true;
            }
        }
        */
        if (canMove)
        {
            if(!isPlayerDetected)
            {
                DoIdle();
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"NO OSCILL {posWithoutOscillation.y}");
        GUILayout.Label($"WITH OSCILL {transform.position.y}");
        GUILayout.Label($"CAN OSCILL {canOscillate}");

        GUILayout.EndHorizontal();

    }
}
