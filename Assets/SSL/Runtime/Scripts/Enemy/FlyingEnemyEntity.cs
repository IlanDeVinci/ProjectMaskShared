using PrimeTween;
using System.Collections;
using UnityEngine;

public class FlyingEnemyEntity : MonoBehaviour
{
    [SerializeField] private FlyingEnemyMovementSettings movementSettings;
    [SerializeField] private FlyingEnemyRaycasts raycasts;
    [SerializeField] private FlyingEnemyLaser flyingLaser;
    [SerializeField] private EnemyHealthManager healthManager;
    [SerializeField] private float detectionRange;

    private bool canMove = true;
    private bool isPlayerDetected = false;
    private float distanceToGround => raycasts.DistanceFromGround(posWithoutOscillationTransform);
    private float distanceToCeiling => raycasts.DistanceFromCeiling(posWithoutOscillationTransform);

    private float distanceToLeft => raycasts.DistanceFromLeft();
    private float distanceToRight => raycasts.DistanceFromRight();
    private Tween oscillationTween;
    [SerializeField] private TweenSettings settingsTween;
    private float oscillation = 0;
    [SerializeField] private float maxOscillation = 2;
    private Vector2 posWithoutOscillation;
    [SerializeField] private Transform posWithoutOscillationTransform;
    private Transform target;
    [SerializeField] private bool canOscillate = true;
    private Vector2 direction;
    private RaycastHit2D raycastHit2D;
    private float shootTime = 0;
    private bool hasPutAwayGun = false;
    Tween putGunAwayx;
    Tween putGunAwayy;
    private Vector2 velocity;
    private bool isPatrolling = true;
    private bool isPatrollingRight = true;

    public bool isShooting = false;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private Transform gun;
    [SerializeField] private LayerMask player;
    [SerializeField] private float followOffsetX;
    [SerializeField] private float followOffsetY;
    // Start is called before the first frame update
    void Start()
    {
        posWithoutOscillation = transform.position;
        target = GameObject.FindGameObjectWithTag("PlayerTrigger").transform;
        oscillationTween = Tween.Custom(startValue: 0, endValue: maxOscillation, settings: settingsTween, onValueChange: val => oscillation = val);

    }

    private void ResetOscillation()
    {
        if (oscillationTween.isAlive)
        {
            oscillationTween.Stop();
        }
        oscillation = 0;
        oscillationTween = Tween.Custom(startValue: 0, endValue: maxOscillation, settings: settingsTween, onValueChange: val => oscillation = val);
    }
    private void DoIdle()
    {

        float newPos = posWithoutOscillation.y;

        if (distanceToGround < movementSettings.minHeight || distanceToGround > movementSettings.maxHeight)
        {
            oscillationTween.isPaused = true;
            if (distanceToGround < movementSettings.minHeight)
            {
                newPos += movementSettings.moveSpeedy * Time.deltaTime;
            }
            if (distanceToGround > movementSettings.maxHeight)
            {
                newPos -= movementSettings.moveSpeedy * Time.deltaTime;
            }
        }
        else
        {

            oscillationTween.isPaused = false;
        }

        posWithoutOscillation = new Vector2(transform.position.x, newPos);
        //if (canOscillate)

        if(canOscillate)
        {
            newPos += oscillation;
        }

        transform.position = new Vector2(transform.position.x, newPos);
        if (isPatrollingRight)
        {
            transform.position = Vector2.SmoothDamp(transform.position, new Vector3(transform.position.x + 2, transform.position.y), ref velocity, 1, 500);
            if (distanceToRight < 2)
            {
                isPatrollingRight = false;
            }
        }
        else
        {
            {
                transform.position = Vector2.SmoothDamp(transform.position, new Vector3(transform.position.x - 2, transform.position.y), ref velocity, 1, 500);
                if (distanceToLeft < 2)
                {
                    isPatrollingRight = true;
                }
            }
        }
        
    }
    private void LocatePlayer()
    {

        Vector2 targetPos = target.position;
        direction = targetPos - (Vector2)posWithoutOscillation;
        direction.Normalize();
        raycastHit2D = Physics2D.Raycast(posWithoutOscillation, direction, detectionRange, player);
        if (raycastHit2D.collider != null)
        {
            if (raycastHit2D.collider.gameObject.CompareTag("PlayerTrigger"))
            {
                if (!isPlayerDetected)
                {
                    shootTime = Time.time + 1f/fireRate;
                    velocity = Vector2.zero;
                    posWithoutOscillation = transform.position;
                    isPlayerDetected = true;
                    oscillationTween.isPaused = false;

                }
            }
            else
            {
                if (isPlayerDetected)
                {
                    isPlayerDetected = false;
                    ResetOscillation();
                }
            }
        }
        else
        {
            if (isPlayerDetected)
            {
                ResetOscillation();
                isPlayerDetected = false;
            }
        }
        if (isPlayerDetected)
        {

            hasPutAwayGun = false;
            if (putGunAwayx.isAlive)
            {
                putGunAwayx.Stop();
            }
            if (putGunAwayy.isAlive)
            {
                putGunAwayy.Stop();
            }
            if(!isShooting) gun.transform.up = -direction;
            if (Time.time > shootTime)
            {
                shootTime = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        else
        {
            if (!isShooting)
            {
                flyingLaser.HideLaser();
            }
        }
    }

    private void Shoot()
    {
        isShooting = true;
        flyingLaser.ShootLaser(target.position);
    }

    private void FollowPlayer()
    {

        Vector2 posToFollowAt = target.position;
        if (!isShooting)
        {
            if (direction.x > 0)
            {
                posToFollowAt.y += followOffsetY + oscillation;
                posToFollowAt.x -= followOffsetX;
                if (distanceToLeft < 2.5f)
                {
                    posToFollowAt.x = transform.position.x + 10;
                }
                if (distanceToRight < 2.5f)
                {
                    posToFollowAt.x = transform.position.x - 10;
                }
                if(distanceToCeiling < 1.5f)
                {
                    posToFollowAt.y = transform.position.y - 10;
                }
                
                transform.position = Vector2.SmoothDamp(transform.position, posToFollowAt, ref velocity, 1, movementSettings.moveSpeedx * Time.deltaTime);
            }
            else
            {
                posToFollowAt.y += followOffsetY + oscillation;
                posToFollowAt.x += followOffsetX;
                if (distanceToLeft < 2.5f)
                {
                    posToFollowAt.x = transform.position.x + 10;
                }
                if (distanceToRight < 2.5f)
                {
                    posToFollowAt.x = transform.position.x - 10;
                }
                if (distanceToCeiling < 1.5f)
                {
                    posToFollowAt.y = transform.position.y - 10;
                }
                transform.position = Vector2.SmoothDamp(transform.position, posToFollowAt, ref velocity, 1, movementSettings.moveSpeedx * Time.deltaTime);
            }
            posWithoutOscillation = transform.position;

            flyingLaser.LaserPointer(target.position);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(healthManager.currentHealth <= 0)
        {
            Destroy(flyingLaser);
        }
        if (!GlobalManager.isGamePaused && healthManager.currentHealth >= 0)
        {
            LocatePlayer();
            if (!isPlayerDetected)
            {
                if(!isPatrolling)
                {
                    isPatrolling = true;
                    if(direction.x < 0)
                    {
                        isPatrollingRight = true;

                    }
                    else
                    {
                        isPatrollingRight = false;
                    }
                }
                DoIdle();
            }
            else
            {
                isPatrolling = false;
            }
            if (canMove)
            {
                if (isPlayerDetected)
                {
                    FollowPlayer();
                }
                else if (!hasPutAwayGun)
                {
                    ResetOscillation();

                    if (!isShooting)
                    {
                        StartCoroutine(PutAwayGun());
                        hasPutAwayGun = true;
                    }

                }
            }
            posWithoutOscillationTransform.position = posWithoutOscillation;

        }

    }

    private IEnumerator PutAwayGun()
    {
        float x = gun.transform.up.x;
        float y = gun.transform.up.y;
        putGunAwayx = Tween.Custom(startValue: gun.transform.up.x, endValue: 0, duration: 1, ease: Ease.OutSine, onValueChange: val => x = val);
        putGunAwayy = Tween.Custom(startValue: gun.transform.up.y, endValue: 1, duration: 1, ease: Ease.OutSine, onValueChange: val => y = val);
        while (putGunAwayx.isAlive && putGunAwayy.isAlive)
        {
            gun.transform.up = new Vector3(x, y, 0);
            yield return new WaitForEndOfFrame();
        }
    }

    /*
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"NO OSCILL {posWithoutOscillation.y}");
        GUILayout.Label($"WITH OSCILL {transform.position.y}");
        GUILayout.Label($"CAN OSCILL {canOscillate}");
        GUILayout.Label($"oscillation {oscillation}");

        GUILayout.EndHorizontal();

    }
    */

    private void OnDestroy()
    {
        oscillationTween.Stop();
    }
}
