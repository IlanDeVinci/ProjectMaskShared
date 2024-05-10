using PrimeTween;
using System.Collections;
using UnityEngine;

public class KnifePrefab : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int degats = 1;

    [SerializeField] private float lifetime = 2f;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private float angular;

    private float lifeTimer;
    private bool hasHitTarget;
    public float knifeSpeed;
    private Tween speedTween;
    public bool isRight;
    [SerializeField] private TweenSettings<Quaternion> rotationSettings;
    [SerializeField] private TweenSettings<Quaternion> rotationSettingsBack;

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
        degats = (int)upgrade.upgradesList[upgrade.upgradeLevel].upgradeValue;
        var upgrade1 = GlobalUpgrades.Instance.Upgrades.Find(x => x.upgradeType == GlobalUpgrades.UpgradeType.KnifePiercing);
        if(upgrade.upgradesList[upgrade.upgradeLevel].upgradeValue == 1)
        {
            boxCollider.isTrigger = true;
        }
        /*
        speedTween = Tween.Custom(rb.velocity.x, rb.velocity.x / 2, 2,
            onValueChange: newVal => rb.velocity = new Vector2(newVal, rb.velocity.y));
        */
        if (isRight)
        {
            //rb.centerOfMass = new Vector2(10, 0);
            rb.angularVelocity = -angular;
        }
        else
        {
            //rb.centerOfMass = new Vector2(-10, 0);
            rb.angularVelocity = angular;

        }
    }
        private void Update()
    {
        if (!GlobalManager.isGamePaused)
        {
            lifeTimer += Time.deltaTime;
            /*
            if (transform.localScale.x > 0)
            {
                rotationAmount -= 30 * Time.deltaTime;
            }
            else
            {
                rotationAmount += 30 * Time.deltaTime;

            }
            */
            //transform.eulerAngles = Vector3.forward * rotationAmount;
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
                //speedTween.Stop();
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

    private void DoCollision(Collision2D collision)
    {
        Debug.Log(collision.collider.name);
        Debug.Log(collision.collider.tag);

        if (!collision.collider.CompareTag("Player") && !collision.collider.CompareTag("PlayerTrigger") && !collision.collider.CompareTag("Player") && !collision.collider.CompareTag("CameraTriggerTarget"))
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<EnemyHealthManager>().TakeDamage(degats);
            }
            //speedTween.Stop();

            hasHitTarget = true;
            rb.gravityScale = 0;
            //rb.velocity = new Vector2(knifeSpeed / 10, 0);
            if (!isGone)
            {
                Tween.PositionX(rb.transform, rb.position.x + knifeSpeed, 0.2f);
                isGone = true;
                Destroy(boxCollider);
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                Tween.Alpha(sprite, 0, 0.5f, Ease.OutSine);
                Destroy(gameObject, 1);
            }
        }

    }

    private void DoCollisionTrigger(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (!collision.CompareTag("Player") && !collision.CompareTag("PlayerTrigger") && !collision.CompareTag("Player") && !collision.CompareTag("CameraTriggerTarget"))
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<EnemyHealthManager>().TakeDamage(degats);
            }
            //speedTween.Stop();

            hasHitTarget = true;
            rb.gravityScale = 0;
            //rb.velocity = new Vector2(knifeSpeed / 10, 0);
            if (!isGone)
            {
                Tween.PositionX(rb.transform, rb.position.x + knifeSpeed, 0.2f);
                isGone = true;
                Destroy(boxCollider);
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                Tween.Alpha(sprite, 0, 0.5f, Ease.OutSine);
                Destroy(gameObject, 1);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(boxCollider.isTrigger) DoCollisionTrigger(collision);

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        DoCollision(collision);
    }

    // Update is called once per frame
}