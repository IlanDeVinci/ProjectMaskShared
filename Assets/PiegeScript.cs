using PrimeTween;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class PiegeScript : MonoBehaviour
{
    [SerializeField] private List<Position> positions;
    [SerializeField] private Light2D light2D;

    [SerializeField] private ParticleSystem particle;
    [SerializeField] private int amountOfHits;
    [SerializeField] private int hitDamage;
    private Transform target;
    private HealthManager targetHealth;
    private RaycastHit2D distance;
    private RaycastHit2D hit;
    [SerializeField] private float detectionRadius;
    private int currentPos = 0;
    private bool isGoingRight = true;
    private bool canMove = true;
    private bool isBzzing = false;
    [SerializeField] private LayerMask player;
    private float dmgTimer = 0;

    [Serializable]
    public class Position
    {
        [SerializeField] public Vector2 position;
        [SerializeField] public float speed;

    }

    // Start is called before the first frame update
    void Awake()
    {
        positions[0].position = transform.position;
    }
    private IEnumerator Boom()
    {
        canMove = false;
        isBzzing = true;
        yield return new WaitForSeconds(0.3f);
        Tween first = Tween.Custom(startValue: light2D.intensity, endValue: 25, duration: 0.3f, onValueChange: val => { light2D.intensity = val; });
        Tween tween = Tween.Custom(startValue: light2D.pointLightOuterRadius, endValue: 4, duration: 0.5f, cycles: 2, cycleMode: CycleMode.Yoyo, onValueChange: val =>
        {
            light2D.pointLightOuterRadius = val;
        });
        yield return first.ToYieldInstruction();
        particle.Play();
        for (int i = 0; i < amountOfHits; i++)
        {
            if (hit) targetHealth.TakeDamage(hitDamage);
            yield return new WaitForSeconds(0.05f);
        }
        first = Tween.Custom(startValue: light2D.intensity, endValue: 1, duration: 0.3f, onValueChange: val => { light2D.intensity = val; });

        yield return tween.ToYieldInstruction();

        yield return new WaitForSeconds(0.5f);
        canMove = true;
        isBzzing = false;
    }
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("PlayerTrigger").transform;
        targetHealth = target.GetComponent<HealthManager>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if(dmgTimer > 1f)
        {
            if(collision.CompareTag("PlayerTrigger") || collision.CompareTag("Player"))
            {
                dmgTimer = 0f;

                targetHealth.TakeDamage(5);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(!GlobalManager.isGamePaused)
        {
            dmgTimer += Time.deltaTime;
            var direction = target.position - transform.position;
            distance = Physics2D.Raycast(transform.position, direction, detectionRadius, player);
            hit = Physics2D.Raycast(transform.position, direction, detectionRadius + 1, player);

            if (distance && distance.collider.CompareTag("PlayerTrigger"))
            {
                if (!isBzzing)
                {
                    StartCoroutine(Boom());
                }
            }
            if (canMove)
            {
                transform.position = Vector2.MoveTowards(transform.position, positions[currentPos].position, positions[currentPos].speed * Time.deltaTime);
                if ((Vector2)transform.position == positions[currentPos].position)
                {
                    if (isGoingRight)
                    {
                        if (currentPos < positions.Count - 1)
                        {
                            currentPos++;
                        }
                        else
                        {
                            isGoingRight = false;
                        }
                    }
                    else
                    {
                        if (currentPos > 0)
                        {
                            currentPos--;
                        }
                        else
                        {
                            isGoingRight = true;
                        }
                    }

                }
            }
        }


    }
}
