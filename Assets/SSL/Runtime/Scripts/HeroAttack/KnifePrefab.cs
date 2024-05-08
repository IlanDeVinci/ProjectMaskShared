using PrimeTween;
using System.Collections;
using UnityEngine;

public class KnifePrefab : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float degats = 1;

    [SerializeField] private float lifetime = 2f;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private BoxCollider2D boxCollider;

    private float lifeTimer;
    private bool hasHitTarget;
    public float knifeSpeed;
    private Tween speedTween;

    private bool isPaused = false;
    private Vector2 velocityBeforePause;
    private float gravityBeforePause;
    private bool isGone = false;
    // Start is called before the first frame update
    private void Awake()
    {
        lifeTimer = 0f;
        hasHitTarget = false;
    }

    private void Start()
    {
        var upgrade = GlobalUpgrades.Instance.Upgrades.Find(x => x.upgradeType == GlobalUpgrades.UpgradeType.KnifeDamage);
        degats = upgrade.upgradesList[upgrade.upgradeLevel].upgradeValue;
        speedTween = Tween.Custom(rb.velocity.x, rb.velocity.x / 2, 2,
            onValueChange: newVal => rb.velocity = new Vector2(newVal, rb.velocity.y));
    }

    private void Update()
    {
        if (!GlobalManager.isGamePaused)
        {
            lifeTimer += Time.deltaTime;
        }
        else
        {
            if (!isPaused)
            {
                StartCoroutine(Pause());
            }
        }

        if(!isGone)
        {
            if (lifeTimer > lifetime && !hasHitTarget)
            {
                isGone = true;
                Tween.Alpha(sprite, 0, 0.5f, Ease.OutSine);
                speedTween.Stop();
                Destroy(gameObject, 1);
            }
        }

    }

    private IEnumerator Pause()
    {
        isPaused = true;
        gravityBeforePause = rb.gravityScale;
        rb.gravityScale = 0;
        velocityBeforePause = rb.velocity;
        if (speedTween.isAlive) speedTween.isPaused = true;
        rb.velocity = Vector2.zero;
        yield return new WaitUntil(() => GlobalManager.isGamePaused == false);
        rb.velocity = velocityBeforePause;
        if (speedTween.isAlive) speedTween.isPaused = false;
        isPaused = false;
        rb.gravityScale = gravityBeforePause;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player") && !collision.collider.CompareTag("PlayerTrigger") && !collision.collider.CompareTag("Player"))
        {
            speedTween.Stop();

            hasHitTarget = true;
            rb.gravityScale = 0;
            //rb.velocity = new Vector2(knifeSpeed / 10, 0);
            Tween.PositionX(rb.transform, rb.position.x + knifeSpeed, 0.2f);
            if (!isGone)
            {
                isGone = true;
                Destroy(boxCollider);
                rb.velocity = Vector2.zero;
                Tween.Alpha(sprite, 0, 0.5f, Ease.OutSine);
                Destroy(gameObject, 1);
            }
        }
      

    }

    // Update is called once per frame
}