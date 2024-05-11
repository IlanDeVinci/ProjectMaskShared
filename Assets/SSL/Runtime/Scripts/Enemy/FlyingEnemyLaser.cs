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
    [SerializeField] int damage;

    private void Start()
    {
        positions[1] = origin.position  ;

    }
    private void Update()
    {
        positions[0] = origin.position;

            m_Laser.positionCount = 2;
            m_Laser.SetPositions(positions);


        if (canAim)
        {
        }
    }
    public bool LaserPointer(Vector2 end)
    {
        m_Laser.enabled = true;
        Vector2 direction = end - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(origin.position, 0.1f, direction, 1000, player);
        positions[1] = hit.point;
        if(hit.collider.CompareTag("PlayerTrigger"))
        {
            return true;
        }
        return false;
    }

    public void StopAttack()
    {
        tween.Stop();
        m_Laser.widthMultiplier = 1;
    }


    private IEnumerator Laser(Vector2 end)
    {
        Vector2 direction = end - (Vector2)origin.position;

        RaycastHit2D final = Physics2D.CircleCast(origin.position, 0.3f, direction, 1000, world);
        if(final.collider == null)
        {
            final = Physics2D.CircleCast(origin.position, 0.5f, direction, 1000, player);

        }

        positions[1] = final.point;
        m_Laser.SetPositions(lightning);
        tween = Tween.Custom(m_Laser.widthMultiplier, 0, 0.3f, ease: Ease.InSine, onValueChange: val => m_Laser.widthMultiplier = val);
        yield return tween.ToYieldInstruction();
        tween = Tween.Custom(m_Laser.widthMultiplier, 15, 0.2f, ease: Ease.InSine, onValueChange: val => m_Laser.widthMultiplier = val);
        yield return tween.ToYieldInstruction();
        RaycastHit2D raycastHit = Physics2D.CircleCast(origin.position, 0.3f, direction, 1000, player);
        particle.transform.position = new Vector2(final.point.x, final.point.y);
        particle.transform.up = -direction;
        particle.Play();
        GameObject savedLight = Instantiate(_light, particle.transform.position, Quaternion.identity);
        var lightValues = savedLight.GetComponent<LaserLightScript>();
        lightValues.intensity = 30;
        lightValues.radius = 5;
        lightValues.color = Color.red;
        lightValues.delay = 0;
        lightValues.duration = 0.5f;
        if (raycastHit.collider != null)
        {
            if (raycastHit.collider.CompareTag("PlayerTrigger") || raycastHit.collider.CompareTag("Player"))
            {
                raycastHit.collider.GetComponent<HealthManager>().TakeDamage(damage);

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
