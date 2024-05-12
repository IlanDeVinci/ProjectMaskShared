using PrimeTween;
using System.Collections;
using UnityEngine;
using PrimeTween;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class KamikazeScript : MonoBehaviour
{
    private Transform target;
    private bool isPlayerInRange = false;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask onlyPlayer;

    [SerializeField] private float explosionRange;
    private EnemyAI EnemyAI;
    [SerializeField] float explosionDuration;
    [SerializeField] int explosionDamage;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystem particle;
    private Shader shaderGUItext;
    [SerializeField] private Shader shaderSpritesDefault;
    private Coroutine coroutine;
    private Slider healthBar;
    private Light2D lightboom;
    private HealthManager healthManager;
    private float explosionTimer;
    private bool isOver = false;
    private enum State
    {
        Idle,
        FollowPlayerNormal,
        FollowPlayerAbove,
        Jumping,
        Exploding
    }
    // Start is called before the first frame update
    void Start()
    {
        shaderGUItext = Shader.Find("GUI/Text Shader");
        healthBar = GetComponentInChildren<Slider>();
        lightboom = GetComponentInChildren<Light2D>();
        healthManager = GameObject.FindAnyObjectByType<HealthManager>();
        EnemyAI = GetComponent<EnemyAI>();
        target = GameObject.FindGameObjectWithTag("PlayerTrigger").transform;
        coroutine = StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (healthManager.currentHealth > 0)
        {
            if (isPlayerInRange)
            {
                spriteRenderer.material.shader = shaderGUItext;
                yield return new WaitForSeconds(0.4f);
                spriteRenderer.material.shader = shaderSpritesDefault;
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                spriteRenderer.material.shader = shaderSpritesDefault;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void LocatePlayer()
    {
        var direction = target.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, explosionRange, playerLayer);
        isPlayerInRange = false;
        if (hit && hit.collider != null)
        {
            if (hit.collider.CompareTag("PlayerTrigger"))
            {
                isPlayerInRange = true;
            }
            else
            {
                isPlayerInRange = false;

            }
        }

    }
    // Update is called once per frame
    void Update()
    {
        if(!isOver && !GlobalManager.isGamePaused)
        {
            LocatePlayer();
            if (isPlayerInRange)
            {
                EnemyAI.followEnabled = false;
                explosionTimer += Time.deltaTime;
                
            }
            else
            {

                EnemyAI.followEnabled = true;
                explosionTimer = 0f;

            }
            if (explosionTimer > explosionDuration)
            {
                EnemyAI.followEnabled = false;
                isOver = true;
                StartCoroutine(Explode());
            }
        }

    }

    private IEnumerator Explode()
    {
        Tween tween = Tween.Color(spriteRenderer, Color.red, 0.3f);
        Tween.Scale(transform, startValue:transform.localScale, endValue:transform.localScale*1.3f, 0.3f);
        yield return tween.ToYieldInstruction();
        particle.Play();
        Sequence.Create().Chain(Tween.Custom(lightboom.intensity, 10, 0.8f, onValueChange:val => lightboom.intensity = val)).Chain(Tween.Custom(lightboom.intensity, 0, 0.1f, onValueChange: val => lightboom.intensity = val));    
        var direction = target.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, explosionRange, onlyPlayer);
        if (hit)
        {
            healthManager.TakeDamage(explosionDamage);
        }
        StopCoroutine(coroutine);
        Destroy(healthBar);
        Destroy(spriteRenderer);
        Destroy(gameObject, 1);
    }
}
