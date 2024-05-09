using PrimeTween;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FlyingEnemyLaser : MonoBehaviour
{
    [SerializeField] private LineRenderer m_Laser;
    [SerializeField] private Transform origin;
    [SerializeField] private FlyingEnemyEntity entity;
    private Vector2 target;
    private Vector2 savedPos;
    private float shootDelay;
    private bool canAim = true;
    Vector3[] positions = new Vector3[2];
    [SerializeField] private LayerMask player;
    [SerializeField] private LayerMask world;


    private void Start()
    {
        positions[1] = origin.position  ;

    }
    private void Update()
    {
        positions[0] = origin.position;
        m_Laser.SetPositions(positions);

        if (canAim)
        {
        }
    }
    public void LaserPointer(Vector2 end)
    {
        m_Laser.enabled = true;
        positions[1] = end;

    }

    private IEnumerator Laser(Vector2 start, Vector2 end)
    {
        Vector2 direction = end - (Vector2)transform.position;

        RaycastHit2D final = Physics2D.CircleCast(start, 0.5f, direction, 1000, world);
        positions[1] = final.point;
        Tween first = Tween.Custom(m_Laser.widthMultiplier, 0, 0.3f, ease: Ease.InSine, onValueChange: val => m_Laser.widthMultiplier = val);
        yield return first.ToYieldInstruction();
        Tween tween = Tween.Custom(m_Laser.widthMultiplier, 10, 0.3f, ease: Ease.InSine, onValueChange: val => m_Laser.widthMultiplier = val);
        yield return tween.ToYieldInstruction();
        RaycastHit2D raycastHit = Physics2D.CircleCast(start, 0.5f, direction, 1000, player);
        Debug.Log(raycastHit.collider);
        if (raycastHit.collider != null)
        {
            if (raycastHit.collider.CompareTag("PlayerTrigger"))
            {
                raycastHit.collider.GetComponent<HealthManager>().TakeDamage(20);
            }
        }

        Tween tweenBack = Tween.Custom(m_Laser.widthMultiplier, 1, 0.1f, ease: Ease.OutSine, onValueChange: val => m_Laser.widthMultiplier = val);
        yield return tweenBack.ToYieldInstruction();
        m_Laser.enabled = false;
        yield return new WaitForSeconds(0.3f); 
        entity.isShooting = false;
    }
    public void ShootLaser(Vector2 start, Vector2 end)
    {
        StartCoroutine(Laser(start,end));
    }

    public void HideLaser()
    {
        m_Laser.enabled = false;
    }
}
