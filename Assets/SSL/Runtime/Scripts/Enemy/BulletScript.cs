using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private GameObject circle;
    [SerializeField] private CircleCollider2D circleCollider;
    private bool isPaused = false;
    private Vector2 velocityBeforePause;
    private float gravityBeforePause;
    [SerializeField] ParticleSystem particle;
    private float lifetime = 2;
    private float lifeTimer;
    private bool hasHitTarget = false;
    public int damage = 5;
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
        if(rb != null)
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
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("EnemyBullet") && !collision.CompareTag("CameraTrigger"))
        {
            if (collision.CompareTag("PlayerTrigger"))
            {
                if(!hasHitTarget)
                {
                    collision.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
                    hasHitTarget = true;
                }
            }
            /*
            ContactPoint2D[] points = new ContactPoint2D[1];
            collision.GetContacts(points);
            foreach (ContactPoint2D contactPoint in points)
            {
                Debug.Log(contactPoint.ToString());
            }
            Vector3 normal = points[0].normal;
            Vector3 vel = rb.velocity;
            float angle = Vector3.Angle(vel,-normal);
            */
            particle.transform.up = -transform.up;
            particle.Play();
            Destroy(circle);
            Destroy(circleCollider);
            Destroy(rb);
            Destroy(this.gameObject, 1);

        }

    }
}
