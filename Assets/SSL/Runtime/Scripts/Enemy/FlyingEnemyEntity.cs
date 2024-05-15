using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlyingEnemyEntity : MonoBehaviour
{
    [SerializeField] private FlyingEnemyMovementSettings movementSettings;
    [SerializeField] private FlyingEnemyRaycasts raycasts;
    [SerializeField] private FlyingEnemyLaser flyingLaser;
    [SerializeField] private EnemyHealthManager healthManager;
    [SerializeField] private float detectionRange;
    [SerializeField] private SpriteRenderer detectionImage;

    private bool canMove = true;
    private bool isPlayerDetected = false;
    private float distanceToGround => raycasts.DistanceFromGround(posWithoutOscillationTransform);
    private float distanceToCeiling => raycasts.DistanceFromCeiling(transform);

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
    private bool isSearchingLastPos = false;
    public bool isShooting = false;
    private bool isPlayerHiding = false;
    private float playerHidingTime = 0;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private Transform gun;
    [SerializeField] private LayerMask player;
    [SerializeField] private float followOffsetX;
    [SerializeField] private float followOffsetY;
    private float lastFollowUpdateTime = 0;
    private Vector2 lastFollowUpdatePos;
    private Vector2 lastSeenPos;
    private Vector2 lastSeenPosForLaser;

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
        if(distanceToGround < 3.5f)
        {
            lastSeenPos.y = transform.position.y + 3.5f;
        }

        transform.position = Vector2.SmoothDamp(transform.position, posToFollowAt, ref velocity, 2, movementSettings.moveSpeedx * Time.deltaTime);

        posWithoutOscillation = transform.position;

        if(!isShooting) {
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
        direction.Normalize();
        raycastHit2D = Physics2D.Raycast(posWithoutOscillation, direction, detectionRange, player);
        if (raycastHit2D.collider != null)
        {
            if (raycastHit2D.collider.gameObject.CompareTag("PlayerTrigger"))
            {
                if (!isPlayerDetected)
                {
                    shootTime = Time.time + 1f / fireRate;
                    velocity = Vector2.zero;
                    posWithoutOscillation = transform.position;
                    isPlayerDetected = true;
                    oscillationTween.isPaused = false;
                    isSearchingLastPos = false;

                }
            }
            else
            {
                if (isPlayerDetected)
                {
                    isPlayerDetected = false;

                    lastSeenPos = targetPos;
                    if(lastSeenPos.x < transform.position.x)
                    {
                        lastSeenPos.x -= 3;
                    }
                    else
                    {
                        lastSeenPos.x += 3;

                    }
                    lastSeenPosForLaser = targetPos;
                    isSearchingLastPos = true;

                }
            }
        }
        else
        {
            if (isPlayerDetected)
            {
                isPlayerDetected = false;
                lastSeenPos = targetPos;
                if (lastSeenPos.x < transform.position.x)
                {
                    lastSeenPos.x -= 3;
                }
                else
                {
                    lastSeenPos.x += 3;

                }
                lastSeenPosForLaser = targetPos;
                isSearchingLastPos = true;

            }
        }
        if (isPlayerDetected || isSearchingLastPos)
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
            if (isSearchingLastPos)
            {
                direction = lastSeenPosForLaser - posWithoutOscillation;
            }
            if (!isShooting) gun.transform.up = -direction;
            if (Time.time > shootTime && isPlayerDetected)
            {
                shootTime = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        else
        {
            if (!isShooting && isSearchingLastPos)
            {
                flyingLaser.HideLaser();
            }
        }
    }

    private void Shoot()
    {
        isShooting = true;
        if (flyingLaser) flyingLaser.ShootLaser(target.position);
    }

    private void FollowPlayer()
    {
        if(lastFollowUpdateTime < Time.time - 1)
        {
            lastFollowUpdateTime = Time.time;
            lastFollowUpdatePos = target.position;
        }
        Vector2 posToFollowAt = lastFollowUpdatePos;
        if (!isShooting)
        {
            playerHidingTime += Time.deltaTime;
            if(playerHidingTime > 3)
            {
                isPlayerHiding = false;
            }
            bool goRight = false;
            if(direction.x > 0) goRight = true;
            RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position, 1, direction, 1000, player);

            if (raycastHit2D && raycastHit2D.collider.CompareTag("Ground") || raycastHit2D.collider.CompareTag("Tremplin"))
            {
                isPlayerHiding = true;
                playerHidingTime = Time.time;

            }
            if (isPlayerHiding)
            {
                if (goRight)
                {
                    goRight = false;
                }
                else
                {
                    goRight = true;
                }
            }

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

            if(flyingLaser) flyingLaser.LaserPointer(target.position);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (healthManager.currentHealth <= 0)
        {
            if (flyingLaser) flyingLaser.HideLaser();
            if (flyingLaser) Destroy(flyingLaser);
        }
        if (!GlobalManager.isGamePaused && healthManager.currentHealth >= 0)
        {
            LocatePlayer();
            if (!isPlayerDetected && !isSearchingLastPos)
            {
                if (!isPatrolling)
                {
                    isPatrolling = true;
                    if (direction.x < 0)
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
                else if (isSearchingLastPos)
                {
                    GoToLastPos();
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
            if(isPlayerDetected)
            {
                detectionImage.color = Color.red;
            } else if (isSearchingLastPos)
            {
                detectionImage.color = Color.yellow;
            }
            else
            {
                detectionImage.color = Color.green;
            }
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
