using PrimeTween;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FlyingEnemyLaser : MonoBehaviour
{
    [SerializeField] private LineRenderer m_Laser;
    [SerializeField] private Transform origin;
    [SerializeField] private FlyingEnemyEntity entity;
    [SerializeField] private GameObject _light;
    private Vector2 target;
    private Vector2 savedPos;
    private float shootDelay;
    private bool canAim = true;
    Vector3[] positions = new Vector3[2];
    Vector3[] lightning = new Vector3[50];
    [SerializeField] private LayerMask player;
    [SerializeField] private LayerMask world;
    private Tween tween;
    [SerializeField] ParticleSystem particle;

    private void Start()
    {
        positions[1] = origin.position  ;

    }
    private void Update()
    {
        positions[0] = origin.position;
        /*
        if(!entity.isShooting)
        {
        */
            m_Laser.positionCount = 2;
            m_Laser.SetPositions(positions);
        /*
        }
        else
        {
            m_Laser.positionCount = 50;
            m_Laser.SetPositions(lightning);
            Debug.Log(lightning);
            Debug.Log(lightning[10]);
        }
        */

        if (canAim)
        {
        }
    }
    public void LaserPointer(Vector2 end)
    {
        m_Laser.enabled = true;
        Vector2 direction = end - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(origin.position, 0.1f, direction, 1000, player);
        positions[1] = hit.point;

    }

    public static float EaseInElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
    }

    private IEnumerator Laser(Vector2 end)
    {
        Vector2 direction = end - (Vector2)transform.position;

        RaycastHit2D final = Physics2D.CircleCast(origin.position, 0.5f, direction, 1000, world);
        /*
        for (int i = 0; i < lightning.Length; i++)
        {
            float percent = (float)i / (float)lightning.Length;
            //lightning[i] = Vector3.Lerp(origin.position, end, percent);
            lightning[i] = new Vector3(EaseInElastic(origin.position.x, end.x, percent), EaseInElastic(origin.position.y, end.y, percent));

        }
        */
        positions[1] = final.point;
        m_Laser.SetPositions(lightning);
        tween = Tween.Custom(m_Laser.widthMultiplier, 0, 0.3f, ease: Ease.InSine, onValueChange: val => m_Laser.widthMultiplier = val);
        yield return tween.ToYieldInstruction();
        tween = Tween.Custom(m_Laser.widthMultiplier, 15, 0.2f, ease: Ease.InSine, onValueChange: val => m_Laser.widthMultiplier = val);
        yield return tween.ToYieldInstruction();
        RaycastHit2D raycastHit = Physics2D.CircleCast(origin.position, 0.5f, direction, 1000, player);
        particle.transform.position = new Vector2(final.point.x, final.point.y);
        particle.transform.up = -direction;
        particle.Play();
        GameObject savedLight = Instantiate(_light, particle.transform.position, Quaternion.identity);
        var lightValues = savedLight.GetComponent<LaserLightScript>();
        lightValues.intensity = 10;
        lightValues.radius = 5;
        lightValues.color = Color.red;
        lightValues.delay = 0;
        lightValues.duration = 0.5f;
        if (raycastHit.collider != null)
        {
            if (raycastHit.collider.CompareTag("PlayerTrigger") || raycastHit.collider.CompareTag("Player"))
            {
                raycastHit.collider.GetComponent<HealthManager>().TakeDamage(20);
                Debug.Log(raycastHit.collider.GetComponent<HealthManager>().GetHP());

            }
        }

        tween = Tween.Custom(m_Laser.widthMultiplier, 0, 0.05f, ease: Ease.OutSine, onValueChange: val => m_Laser.widthMultiplier = val);
        yield return tween.ToYieldInstruction();
        m_Laser.enabled = false;
        yield return new WaitForSeconds(0.3f); 
        entity.isShooting = false;
        m_Laser.widthMultiplier = 1;
    }
    public void ShootLaser(Vector2 end)
    {
        StartCoroutine(Laser(end));
    }

    public void HideLaser()
    {
        m_Laser.enabled = false;
    }

    private void OnDestroy()
    {
        tween.Stop();
    }
}
