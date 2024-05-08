using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class BulletScript : MonoBehaviour
{

    private bool isPaused = false;
    private Vector2 velocityBeforePause;
    private float gravityBeforePause;

    private float lifetime = 2;
    private float lifeTimer;
    private bool hasHitTarget = false;
    [SerializeField] private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        //Destroy(gameObject, 2);
    }

    // Update is called once per frame
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

        if (lifeTimer > lifetime)
        {
            Destroy(gameObject, 0.1f);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("EnemyBullet") && !collision.CompareTag("CameraTrigger"))
        {
            if (collision.CompareTag("PlayerTrigger"))
            {
                if(!hasHitTarget)
                {
                    collision.gameObject.GetComponent<HealthManager>().TakeDamage(5);
                    hasHitTarget = true;
                }
            }
            Destroy(this.gameObject, 0.01f);

        }

    }
}
