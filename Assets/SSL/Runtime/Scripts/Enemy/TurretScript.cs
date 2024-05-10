using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    [SerializeField] private float detectionRange;
    private Transform target;
    private bool isPlayerDetected = false;
    private Vector2 direction;
    [SerializeField] private GameObject alarmlight;
    private RaycastHit2D raycastHit2D;
    [SerializeField] private Transform gun;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float fireRate;
    [SerializeField] private Transform shootPoint;
    private float shootTime = 0;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private LayerMask player;
    // Start is called before the first frame update
    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("PlayerTrigger").transform;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!GlobalManager.isGamePaused)
        {
            Vector2 targetPos = target.position;
            direction = targetPos - (Vector2)transform.position;
            direction.Normalize();
            raycastHit2D = Physics2D.Raycast(transform.position, direction, detectionRange, player);
            if (raycastHit2D.collider != null)
            {
                Debug.DrawRay(transform.position, raycastHit2D.collider.transform.position, Color.red);
                if (raycastHit2D.collider.gameObject.CompareTag("PlayerTrigger"))
                {
                    if (!isPlayerDetected)
                    {
                        isPlayerDetected = true;
                        alarmlight.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                }
                else
                {
                    if (isPlayerDetected)
                    {
                        isPlayerDetected = false;
                        alarmlight.GetComponent<SpriteRenderer>().color = Color.green;
                    }
                }
            }
            else
            {
                if (isPlayerDetected)
                {
                    isPlayerDetected = false;
                    alarmlight.GetComponent<SpriteRenderer>().color = Color.green;
                }
            }
            if (isPlayerDetected)
            {
                gun.transform.up = direction;
                if (Time.time > shootTime)
                {
                    shootTime = Time.time + 1f / fireRate;
                    Shoot();
                }
            }
        }
       
    }

    private void Shoot()
    {
        GameObject savedBullet = Instantiate(bullet,shootPoint.position, gun.rotation);
        savedBullet.GetComponent<Rigidbody2D>().AddForce(direction * bulletSpeed);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        //Gizmos.DrawLine(transform.position, target.position);
    }

    /*
    private void OnGUI()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label($"isplayerdetected {isPlayerDetected}");
        //if(raycastHit2D && raycastHit2D.collider != null)
        {
            GUILayout.Label($"Collision = {raycastHit2D.collider}");

        }
        GUILayout.EndVertical();

    }
    */
}
