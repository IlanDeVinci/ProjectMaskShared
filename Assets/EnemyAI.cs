using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    private Transform target;
    [SerializeField] private float detectionRange;
    [SerializeField] private float updateCd = 0.2f;

    [Header("Physics")]
    [SerializeField] private float speed = 200f;
    [SerializeField] private float nextWaypointDistance = 3f;
    [SerializeField] private float jumpNodeHeightRequirement = 0.8f;
    [SerializeField] private float jumpModifier = 0.3f;

    [Header("Custom Behaviour")]
    [SerializeField] public bool followEnabled = true;
    [SerializeField] private bool jumpEnabled = true;
    [SerializeField] private bool directionEnabled = true;

    private SpriteRenderer spriteRenderer;
    private Path path;
    private int currentWaypoint = 0;
    private float jumptimer = 0;
    private float wallTimer = 0;
    private bool isPaused = false;
    [SerializeField] LayerMask groundLayer;
    private Vector2 velocityBeforePause;
    private float gravityBeforePause;
    [SerializeField] Seeker seeker;
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] Collider2D col;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        target = GameObject.FindGameObjectWithTag("PlayerTrigger").transform;

        InvokeRepeating("UpdatePath", 0f, updateCd);
    }

    private void FixedUpdate()
    {
        if(!GlobalManager.isGamePaused)
        {

            if (TargetInRange() && followEnabled && target.CompareTag("PlayerTrigger"))
            {
                PathFollow();
            }
            jumptimer += Time.deltaTime;
            wallTimer += Time.deltaTime;
        }
        else
        {
            if (!isPaused)
            {
                StartCoroutine(Pause());
            }
        }
    }

    private IEnumerator Pause()
    {
        isPaused = true;
        gravityBeforePause = rb.gravityScale;
        rb.gravityScale = 0;
        velocityBeforePause = rb.velocity;
        rb.velocity = Vector2.zero;
        yield return new WaitUntil(() => GlobalManager.isGamePaused == false);
        rb.velocity = velocityBeforePause;
        isPaused = false;
        rb.gravityScale = gravityBeforePause;
    }

    private void UpdatePath()
    {
        if(followEnabled && TargetInRange() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if(path == null)
        {
            //Debug.Log("gyatt");
            return;
        }
        if(currentWaypoint >= path.vectorPath.Count)
        {
            //Debug.Log("gyatted");

            return;
        }

        RaycastHit2D isGrounded = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        bool isHole = false;
        RaycastHit2D isHoleRight = Physics2D.Raycast(new Vector2((transform.position.x + col.bounds.extents.x + 0.5f),transform.position.y), Vector2.down, col.bounds.extents.y + 0.5f, groundLayer);
        RaycastHit2D isHoleLeft = Physics2D.Raycast(new Vector2((transform.position.x - col.bounds.extents.x - 0.5f), transform.position.y), Vector2.down, col.bounds.extents.y + 0.5f, groundLayer);
        RaycastHit2D isWallRight = Physics2D.Raycast(new Vector2(transform.position.x, (transform.position.y - col.bounds.extents.y)), Vector2.right, col.bounds.extents.x + 2f, groundLayer);
        RaycastHit2D isWallLeft = Physics2D.Raycast(new Vector2(transform.position.x, (transform.position.y - col.bounds.extents.y)), Vector2.left, col.bounds.extents.x + 2f, groundLayer);
        if(isWallLeft || isWallRight)
        {
            wallTimer = 0;
            if(isGrounded && jumptimer > 1.25f)
            {
                isHole=true;
            }
        }
        if (rb.velocity.x > 0 && !isHoleRight) { 
            isHole = true;
        } else if(rb.velocity.x < 0 && !isHoleLeft)
        {
            isHole = true;
        }
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        direction.Normalize();
        Vector2 force = direction * speed * Time.deltaTime;
        float verticalDistance = target.transform.position.y - transform.position.y;
        if(verticalDistance <= 1) {
            verticalDistance = 1;
        }
        if(jumpEnabled && isGrounded && (jumptimer > 1.25f || isHole))
        {
            if((direction.y > jumpNodeHeightRequirement) || isHole)
            {
                //verticalDistance = Mathf.Min(verticalDistance, 10);
                float extraJump = Mathf.Log(Mathf.Abs(verticalDistance));

                extraJump = Mathf.Clamp(extraJump,1, 1.5f);

                rb.AddForce(Vector2.up * speed * jumpModifier * extraJump);
                jumptimer = 0;

            }
        }

        rb.AddForce(force);
        if(!isWallRight && !isWallLeft && wallTimer < 0.5f)
        {

            if (target.position.x > transform.position.x)
            {
                rb.AddForce(Vector2.right * 100);

            }
            else
            {
                rb.AddForce(Vector2.left * 100);
            }

            wallTimer = 1;
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (directionEnabled)
        {
            if(rb.velocity.x > 0.05f)
            {
                spriteRenderer.flipX = true;
            }
            else if(rb.velocity.x < -0.05f)
            {
                spriteRenderer.flipX = false;

            }
        }
    }

    private bool TargetInRange()
    {
        return Vector2.Distance(transform.position, target.transform.position) < detectionRange;
    }

    private void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

}
