using UnityEngine;

public class FlyingEnemyRaycasts : MonoBehaviour
{

    [Header("Detection")]
    [SerializeField] private Transform detectionPoint;

    [SerializeField] private LayerMask _groundLayerMask;

    public float DistanceFromGround(Transform pos)
    {

        RaycastHit2D hitResult = Physics2D.Raycast(
            detectionPoint.position,
            Vector2.down,
            10,
            _groundLayerMask
            );
        if (hitResult.collider == null)
        {
            return 10;
        }
        else
        {
            return hitResult.distance;
        }

    }

    public float DistanceFromLeft()
    {
        RaycastHit2D hitResult = Physics2D.Raycast(
            detectionPoint.position,
            Vector2.left,
            10,
            _groundLayerMask
            );
        if (hitResult.collider == null)
        {
            return 10;
        }
        else
        {
            return hitResult.distance;
        }
    }
    public float DistanceFromRight()
    {
        RaycastHit2D hitResult = Physics2D.Raycast(
            detectionPoint.position,
            Vector2.right,
            10,
            _groundLayerMask
            );
        if (hitResult.collider == null)
        {
            return 10;
        }
        else
        {
            return hitResult.distance;
        }
    }

}

