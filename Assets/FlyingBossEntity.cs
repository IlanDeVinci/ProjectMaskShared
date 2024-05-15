using PrimeTween;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FlyingBossEntity : MonoBehaviour
{
    [SerializeField] private FlyingEnemyMovementSettings movementSettings;
    [SerializeField] private FlyingEnemyRaycasts raycasts;
    [SerializeField] private FlyingEnemyLaser flyingLaser;
    [SerializeField] private EnemyHealthManager healthManager;
    [SerializeField] private float detectionRange;
    [SerializeField] private SpriteRenderer detectionImage;
    [SerializeField] private GameObject bullet;

    private bool canMove = true;
    private bool isPlayerDetected = true;
    private float distanceToGround => raycasts.DistanceFromGround(posWithoutOscillationTransform);
    private float distanceToCeiling => raycasts.DistanceFromCeiling(transform);

    private float distanceToLeft => raycasts.DistanceFromLeft();
    private float distanceToRight => raycasts.DistanceFromRight();
    private Tween oscillationTween;
    [SerializeField] private TweenSettings settingsTween;
    private float oscillation = 0;
    [SerializeField] private float maxOscillation = 1;
    private Vector2 posWithoutOscillation;
    [SerializeField] private Transform posWithoutOscillationTransform;
    private Transform target;
    [SerializeField] private bool canOscillate = true;
    private Vector2 direction;
    private RaycastHit2D raycastHit2D;
    private float lastAttackTime = 0;
    private Vector2 velocity;
    private bool isPatrolling = true;
    private bool isPatrollingRight = true;
    private bool isSearchingLastPos = false;
    public bool isShootingMini = false;
    public bool isShootingLaser = false;
    public bool isShootingExplosion = false;
    private float shootTimeLaser = 0;
    private float shootTimeMini = 0;
    private float shootTimeExplosion = 0;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private int bulletDamage;

    private bool isPlayerHiding = false;
    private float playerHidingTime = 0;
    [SerializeField] private float fireRateMinigun = 0.3f;
    [SerializeField] private int bulletsPerMinigun = 7;

    [SerializeField] private float fireRateLaserBoom = 0.2f;
    [SerializeField] private float laserBoomRange = 0.5f;

    [SerializeField] private Transform miniGun;
    [SerializeField] private Transform laserGun;

    [SerializeField] private LayerMask player;
    [SerializeField] private LayerMask world;

    [SerializeField] private float followOffsetX;
    private float actualFollowOffsetX;
    [SerializeField] private float followOffsetY;
    private float actualFollowOffsetY;

    private Vector2 lastSeenPos;
    private Vector2 lastSeenPosForLaser;
    private Vector2 aimDirection;
    private int facing;
    private bool fightStarted = false;
    private float lastFollowUpdateTime = 0;
    private Vector2 lastFollowUpdatePos;
    private bool isTurning = false;

    // Start is called before the first frame update
    void Start()
    {
        posWithoutOscillation = transform.position;
        target = GameObject.FindGameObjectWithTag("PlayerTrigger").transform;
        oscillationTween = Tween.Custom(startValue: 0, endValue: maxOscillation, settings: settingsTween, onValueChange: val => oscillation = val);
        flyingLaser.HideLaser();
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

        if (canOscillate)
        {
        }
        newPos += oscillation;

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

    private void GoToLastPos()
    {
        Vector2 posToFollowAt = lastSeenPos;

        if (distanceToLeft < 2f)
        {
            lastSeenPos.x = transform.position.x + 2f;
        }
        if (distanceToRight < 2f)
        {
            lastSeenPos.x = transform.position.x - 2f;
        }
        if (distanceToCeiling < 2f)
        {
            lastSeenPos.y = transform.position.y - 2f;
        }
        if (distanceToGround < 3.5f)
        {
            lastSeenPos.y = transform.position.y + 3.5f;
        }

        transform.position = Vector2.SmoothDamp(transform.position, posToFollowAt, ref velocity, 2, movementSettings.moveSpeedx * Time.deltaTime);

        posWithoutOscillation = transform.position;

        if (!isShootingLaser)
        {
            if (flyingLaser && flyingLaser.LaserPointer(lastSeenPosForLaser))
            {
                lastSeenPos = target.position;
            }

        }

        if (Mathf.Abs(transform.position.x - lastSeenPos.x) < 0.5f)
        {
            isSearchingLastPos = false;
            if (flyingLaser) flyingLaser.HideLaser();
            ResetOscillation();

        }
    }


    private void LocatePlayer()
    {

        Vector2 targetPos = target.position;
        direction = targetPos - (Vector2)posWithoutOscillation;
        
        if (target.CompareTag("PlayerTrigger"))
        {
            isPlayerDetected = true;
        }
        else
        {
            if (isPlayerDetected)
            {
                isPlayerDetected = false;
                //lastSeenPos = targetPos;
                if (lastSeenPos.x < transform.position.x)
                {
                   // lastSeenPos.x -= 3;
                }
                else
                {
                    //lastSeenPos.x += 3;

                }
                lastSeenPosForLaser = targetPos;
                isSearchingLastPos = true;

            }
        }
        if (isPlayerDetected)
        {


            if (!isShootingLaser) laserGun.transform.up = -direction;
            miniGun.transform.up = direction;
            if (Time.time > shootTimeLaser && isPlayerDetected)
            {
                shootTimeLaser = Time.time + 1f / fireRateLaserBoom;
                ShootLaser();
            }
            if (Time.time > shootTimeMini && isPlayerDetected)
            {
                    shootTimeMini = Time.time + 1f / fireRateMinigun;
                    ShootMini();
            }
        }
        else
        {
            if (!isShootingLaser && isSearchingLastPos)
            {
                flyingLaser.HideLaser();
            }
        }


    }


    private void ShootLaser()
    {
        isShootingLaser = true;
        if (flyingLaser) flyingLaser.ShootLaser(target.position, laserBoomRange, 10 );
    }

    private IEnumerator MiniShoot()
    {
        for(int i = 0;  i < bulletsPerMinigun; i++)
        {
            GameObject savedBullet = Instantiate(bullet, shootPoint.position, miniGun.rotation);
            var random = new System.Random();
            Vector2 newdir = new Vector2(direction.x * (float)random.Next(85,115)/100f, direction.y * (float)random.Next(85, 115) / 100f);
            newdir.Normalize();
            savedBullet.GetComponent<BulletScript>().damage = bulletDamage  ;
            savedBullet.GetComponent<Rigidbody2D>().AddForce(newdir * bulletSpeed);
            yield return new WaitForSeconds(0.15f);
        }
        yield return new WaitForSeconds(0.3f);
        isShootingMini = false;

    }
    private void ShootMini()
    {
        isShootingMini = true;
        StartCoroutine(MiniShoot());

    
    }

    private void FollowPlayer()
    {
        var random = new System.Random();
        if (lastFollowUpdateTime < Time.time - 1)
        {
            lastFollowUpdateTime = Time.time;
            lastFollowUpdatePos = new Vector2(target.position.x + (float)random.Next(-100,100)/150f, (target.position.y + (float)random.Next(-10, 200) / 100));
        }
        Vector2 posToFollowAt = lastFollowUpdatePos;
        if (!isShootingLaser)
        {

            bool goRight = false;
            if (direction.x > 0) goRight = true;
            RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position, 1, direction, 1000, player);


            if (goRight)
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
                if (distanceToCeiling < 1.5f)
                {
                    posToFollowAt.y = transform.position.y - 5;
                }
                if (distanceToGround < 4f)
                {
                    posToFollowAt.y = transform.position.y + 5;

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
                    posToFollowAt.y = transform.position.y - 5;
                }
                if(distanceToGround < 4f)
                {
                    posToFollowAt.y = transform.position.y + 5;

                }
                transform.position = Vector2.SmoothDamp(transform.position, posToFollowAt, ref velocity, 1, movementSettings.moveSpeedx * Time.deltaTime);
            }
            posWithoutOscillation = transform.position;

            if((facing < 0 && target.position.x < transform.position.x + 1) || (facing > 0 && target.position.x > transform.position.x -1))
            {
                if (flyingLaser && !isTurning && !isShootingLaser) flyingLaser.LaserPointer(target.position);
            }
        }
    }

    private void Attack()
    {
        LocatePlayer();
        FollowPlayer();
    }

    private IEnumerator TurnBack()
    {

        yield return new WaitForSeconds(1.2f);
        transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
        isTurning = false;
    }
    private void TurnAround()
    {

        if (!isShootingLaser)
        {
            transform.position = Vector2.SmoothDamp(transform.position, lastFollowUpdatePos, ref velocity, 1, movementSettings.moveSpeedx * Time.deltaTime);
            if (!isTurning)
            {
                isTurning = true;
                lastSeenPos = target.position;
                aimDirection = lastSeenPos - (Vector2)transform.position;
                StartCoroutine(TurnBack());
            }
            RaycastHit2D raycastHit2D = Physics2D.Raycast(flyingLaser.origin.position, aimDirection, 1000, world);
            if ((facing < 0 && raycastHit2D.point.x < transform.position.x + 1) || (facing > 0 && raycastHit2D.point.x > transform.position.x - 1))
            {
                flyingLaser.LaserPointer(raycastHit2D.point);
                laserGun.up = -aimDirection;
            }
            else
            {
                flyingLaser.HideLaser();
            }
            /* 
            aimDirection.y -= 2 * Time.deltaTime;
            if (facing < 0)
            {
                aimDirection.x -= 5 * Time.deltaTime;
            }
            else
            {
                aimDirection.x += 5 * Time.deltaTime;

            }*/
        }


        shootTimeLaser = Time.time + 1/fireRateLaserBoom/3;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector2.Distance((Vector2)transform.position, target.position) < 25)
        {
            fightStarted = true;
        }
        else
        {
            fightStarted = false;
        }
        if (!fightStarted) return;
        healthManager.healthSlider.GetComponent<CanvasGroup>().alpha = 1;
        if (healthManager.currentHealth <= 0 && healthManager.currentLives <= 1)
        {
            if (flyingLaser) flyingLaser.HideLaser();
            if (flyingLaser) Destroy(flyingLaser);
        }
        if (!GlobalManager.isGamePaused && healthManager.currentHealth >= 0)
        {
           facing = (int)Mathf.Sign(transform.localScale.x);
            if(target.CompareTag("PlayerTrigger"))
            {
                if (facing < 0)
                {
                    if (target.position.x < transform.position.x + 1)
                    {
                        Attack();
                        
                    }
                    else
                    {
                        TurnAround();
                    }
                }
                else
                {
                    if (target.position.x > transform.position.x - 1)
                    {
                        Attack();
                    }
                    else
                    {
                        TurnAround();
                    }
                }
            }
           
            posWithoutOscillationTransform.position = posWithoutOscillation;

        }

    }
}
